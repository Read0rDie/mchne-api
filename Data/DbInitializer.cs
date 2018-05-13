using mchne_api.Models;
using mchne_api.ViewModels;
using System;
using System.Linq;
using Microsoft.AspNetCore.Identity;
using AutoMapper;


namespace mchne_api.Data
{
    public static class DbInitializer
    {
        public static void Initialize(MchneContext context)
        {
            context.Database.EnsureCreated();

            // Look for any students.
            if (context.Novels.Any())
            {
                return;   // DB has been seeded
            }

            var novels = new Novel[]
            {
            new Novel{Title="Tempest of the Stellar War",CoverImage="https://cdn.novelupdates.com/images/2016/04/Tempest-of-the-Stellar.jpg"},
            new Novel{Title="Galactic Dark Net",CoverImage="https://cdn.novelupdates.com/images/2017/03/Galactic-Dark-Net.jpg"},
            new Novel{Title="Rebirth of the Thief Who Roamed The World",CoverImage="https://cdn.novelupdates.com/images/2015/08/chongshengzhi.jpg"}            
            };
            foreach (Novel n in novels)
            {
                context.Novels.Add(n);
            }
            context.SaveChanges();

            var chapters = new Chapter[]
            {
            new Chapter{ChapterNumber=1,ContentUrl="Chemistry",NovelID=1},
            new Chapter{ChapterNumber=2,ContentUrl="Microeconomics",NovelID=1},
            new Chapter{ChapterNumber=1,ContentUrl="Chemistry",NovelID=2},
            new Chapter{ChapterNumber=2,ContentUrl="Microeconomics",NovelID=2}            
            };
            foreach (Chapter c in chapters)
            {
                context.Chapters.Add(c);
            }
            context.SaveChanges();            
        }

        public static void InitializeSuperUser(ApplicationDbContext context, UserManager<AppUser> userManager, RoleManager<IdentityRole> roleManager, IMapper mapper)
        {            
            if (!roleManager.RoleExistsAsync("Admin").Result)
            {
                IdentityRole role = new IdentityRole();
                role.Name = "Admin";                
                IdentityResult roleResult = roleManager.
                CreateAsync(role).Result;
            }

            if (!roleManager.RoleExistsAsync("Editor").Result)
            {
                IdentityRole role = new IdentityRole();
                role.Name = "Editor";                
                IdentityResult roleResult = roleManager.
                CreateAsync(role).Result;
            }

            if (!roleManager.RoleExistsAsync("User").Result)
            {
                IdentityRole role = new IdentityRole();
                role.Name = "User";                
                IdentityResult roleResult = roleManager.
                CreateAsync(role).Result;
            }

            if (userManager.FindByEmailAsync("adydd11@gmail.com").Result == null)
            {
                RegistrationViewModel model = new RegistrationViewModel{Email="adydd11@gmail.com",Password="Nirvash1!",Username="anubis56"};
                var userIdentity = mapper.Map<AppUser>(model);
                IdentityResult result = userManager.CreateAsync(userIdentity, model.Password).Result;
                if (result.Succeeded)
                {
                    context.Users.AddAsync(new User { IdentityId = userIdentity.Id}).Wait();
                    userManager.AddToRoleAsync(userIdentity,  "Admin").Wait();
                    context.SaveChangesAsync();
                }
            }

        }
    }
}