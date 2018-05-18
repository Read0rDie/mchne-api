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
    public class AvatarController : Controller
    {
        private readonly ApplicationDbContext _appDbContext;
        private readonly UserManager<AppUser> _userManager;
        private readonly IConfiguration _configuration;

        public AvatarController(
            UserManager<AppUser> userManager, 
            ApplicationDbContext appDbContext, 
            IConfiguration configuration)
        {
            _userManager = userManager;
            _appDbContext = appDbContext;
            _configuration = configuration;
        }
        
        [HttpGet]
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

        
    }
        
}