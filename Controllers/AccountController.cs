using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using AutoMapper;
using mchne_api.Data;
using mchne_api.Models;
using mchne_api.ViewModels;
using mchne_api.Helpers;
using mchne_api.Auth;
using System;
using Newtonsoft.Json;
using Microsoft.Extensions.Options;
using System.Net.Http;
using System.Net.Http.Headers;

namespace mchne_api.Controllers
{
    [Route("api/[controller]/[action]")]
    public class AccountController : Controller
    {
        private readonly SignInManager<AppUser> _signInManager;
        private readonly ApplicationDbContext _appDbContext;
        private readonly UserManager<AppUser> _userManager;
        private readonly IConfiguration _configuration;
        private readonly IMapper _mapper;
        private readonly IJwtFactory _jwtFactory;
        private readonly JwtIssuerOptions _jwtOptions;

        public AccountController(
            UserManager<AppUser> userManager, 
            SignInManager<AppUser> signInManager, 
            IMapper mapper, 
            ApplicationDbContext appDbContext, 
            IConfiguration configuration, 
            IOptions<JwtIssuerOptions> jwtOptions, 
            IJwtFactory jwtFactory)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _mapper = mapper;
            _appDbContext = appDbContext;
            _configuration = configuration;
            _jwtFactory = jwtFactory;
            _jwtOptions = jwtOptions.Value;
        }

        [HttpPost]
        public async Task<IActionResult> Login([FromBody] LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var identity = await GetClaimsIdentity(model.Email, model.Password);

            if (identity == null)
            {
                return BadRequest(Errors.AddErrorToModelState("login_failure", "Invalid username or password.", ModelState));
            }
                
            var jwt = await Tokens.GenerateJwt(identity, _jwtFactory, model.Email, _jwtOptions, new JsonSerializerSettings { Formatting = Formatting.Indented });

            return new OkObjectResult(jwt);            
        }

        [HttpPost]
        public async Task<IActionResult> Register([FromBody]RegistrationViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var userIdentity = _mapper.Map<AppUser>(model);

            var result = await _userManager.CreateAsync(userIdentity, model.Password);

            if (!result.Succeeded) return new BadRequestObjectResult(Errors.AddErrorsToModelState(result, ModelState));

            await _appDbContext.Users.AddAsync(new User { IdentityId = userIdentity.Id});
            await _userManager.AddToRoleAsync(userIdentity,  "User");
            await _appDbContext.SaveChangesAsync();

            
            return new OkObjectResult("Account created");
        }

        [HttpGet]
        public async Task<IActionResult> GetAvatar(string email)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var identity =  await _userManager.FindByEmailAsync(email);
            if (identity == null)
            {
                return BadRequest(Errors.AddErrorToModelState("user_search_failure", "Could not find user in database.", ModelState));
            }                      
            var user = _appDbContext.Users.First(u => u.Identity.Email.Equals(email));
            return new OkObjectResult(user);            
        }

        public async Task<IActionResult> ChangeAvatar(string email, string imageUrl)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var identity =  await _userManager.FindByEmailAsync(email);
            if (identity == null)
            {
                return BadRequest(Errors.AddErrorToModelState("user_search_failure", "Could not find user in database.", ModelState));
            }                      
            var user = _appDbContext.Users.First(u => u.Identity.Equals(identity));
            user.AvatarUrl = imageUrl;
            await _appDbContext.SaveChangesAsync();            
            return new OkObjectResult(user.AvatarUrl);            
        }  


        public async Task<IActionResult> AllAvatars()
        {
            using(var client = new HttpClient()){
                var url = new Uri(_configuration.GetSection("CloudAccess").GetSection("api_url").Value);                

                client.DefaultRequestHeaders.Authorization = 
                    new AuthenticationHeaderValue(
                        "Basic", 
                        Convert.ToBase64String(
                            System.Text.ASCIIEncoding.ASCII.GetBytes(
                                string.Format("{0}:{1}", 
                                _configuration.GetSection("CloudAccess").GetSection("api_key").Value, 
                                _configuration.GetSection("CloudAccess").GetSection("api_secret").Value))));


                var response = await client.GetAsync(url);
                response.EnsureSuccessStatusCode();
                string json;
                using (var content = response.Content)
                {
                    json = await content.ReadAsStringAsync();
                }
                return new OkObjectResult(json);                
            }
            
        }

        public async Task<IActionResult> UserName(string email)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var identity =  await _userManager.FindByEmailAsync(email);
            if (identity == null)
            {
                return BadRequest(Errors.AddErrorToModelState("user_search_failure", "Could not find user in database.", ModelState));
            }                      
            var user = _appDbContext.Users.First(u => u.Identity.Equals(identity));                       
            return new OkObjectResult(user.Identity.alias);            
        }

        public async Task<IActionResult> EditUserName(string email, string username)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var identity =  await _userManager.FindByEmailAsync(email);
            if (identity == null)
            {
                return BadRequest(Errors.AddErrorToModelState("user_search_failure", "Could not find user in database.", ModelState));
            }                      
            var user = _appDbContext.Users.First(u => u.Identity.Equals(identity));
            user.Identity.alias = username;
            await _appDbContext.SaveChangesAsync();            
            return new OkObjectResult(user.Identity.alias);            
        }             

        private async Task<ClaimsIdentity> GetClaimsIdentity(string userName, string password)
        {
            if (string.IsNullOrEmpty(userName) || string.IsNullOrEmpty(password))
                return await Task.FromResult<ClaimsIdentity>(null);

            // get the user to verifty
            var userToVerify = await _userManager.FindByNameAsync(userName);

            if (userToVerify == null) return await Task.FromResult<ClaimsIdentity>(null);

            // check the credentials
            if (await _userManager.CheckPasswordAsync(userToVerify, password))
            {
                return await Task.FromResult(_jwtFactory.GenerateClaimsIdentity(userName, userToVerify.Id));
            }

            // Credentials are invalid, or account doesn't exist
            return await Task.FromResult<ClaimsIdentity>(null);
        }       
        
    }
        
}