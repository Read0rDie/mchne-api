using mchne_api.Models;
using mchne_api.ViewModels;
using System;
using System.Linq;
using Microsoft.AspNetCore.Identity;
using AutoMapper;
using Microsoft.Extensions.Configuration;


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
            new Novel{
                Title="Tempest of the Stellar War",
                CoverImage="https://cdn.novelupdates.com/images/2016/04/Tempest-of-the-Stellar.jpg",
                Synopsis=   "In a distant future, the empires of mankind span the galaxy, and glorious Earth has devolved into a peripheral backwater." + "\r\n" + "\r\n" +
                            "In Shanjing city in the Asian region, Wang Zheng’s dreams of becoming a mech pilot are crushed when his college entrance exam genetic score turns out a pathetic twenty eight, barely above an animal." + "\r\n" + "\r\n" +
                            "To make things worse, people get the impression he attempted suicide after being rejected by the campus beauty." + "\r\n" + "\r\n" +
                            "Then the closest thing to a family he has, the old man in the book store across the road, goes missing, leaving him only a mysterious birthday present.",
                Genres= new string[5]
                },
            new Novel{
                Title="Galactic Dark Net",
                CoverImage="https://cdn.novelupdates.com/images/2017/03/Galactic-Dark-Net.jpg",
                Synopsis=   "When the last prodigy level esper on Earth disappeared, Earth was in deep trouble of becoming another species' colony. The ordinary Han, with his intelligence and hardworking character, was able to make a fortune after “accidentally” stepping into the world of dark net, later purchasing an esper power crystal that brought him the ultimate power that changed the fate of the universe." + "\r\n" + "\r\n" +
                            "Dark net is a subset of the Deep Web that is not only not indexed by traditional search engines, but that also requires special tools like specific proxy or authentication to gain access. Dark net is not restricted by any law or morals, so the dark net market has everything that is prohibited by the law. Drugs, slaves, firearms, uranium, bioweapons, rare animals, human testing, assassination, and the list goes on. During the year of 2075 on Earth, Han Lang logged into the largest hyperspace dark net market, and our story begins.",
                Genres= new string[4]
                },
            new Novel{
                Title="Rebirth of the Thief Who Roamed The World",
                CoverImage="https://cdn.novelupdates.com/images/2015/08/chongshengzhi.jpg",
                Synopsis=   "The world’s largest VRMMO, Conviction, was almost like a second world for humanity. It had integrated itself into the real world’s economy, with both corporations and individuals seeking their fortunes through the game." + "\r\n" + "\r\n" +
                            "In this game, Nie Yan prided himself in his Level 180 Thief. He could barely be considered among the top experts in the game. Though, that was the only thing he could take pride in. He was penniless and unable to advance in life; a situation he was forced into by the enemy of his father. If it weren’t for the little money he made by selling off items in Conviction, he would’ve barely been able to eat. In the end, he chose to settle his matters once and for all. He assassinated his father’s enemy. He lay dying shortly after being shot in the pursuit." + "\r\n" + "\r\n" +
                            "However, that wasn’t the end of his story. Instead, he awoke moments later to find that he had reincarnated into his past self. Armed with his experience and knowledge of future events, he sets out to live his life anew.",
                Genres= new string[5]
                }            
            };

            novels[0].Genres[0] = "Science Fiction";
            novels[0].Genres[1] = "School Life";
            novels[0].Genres[2] = "Martial Arts";
            novels[0].Genres[3] = "Comedy";
            novels[0].Genres[4] = "Romance";

            novels[1].Genres[0] = "Science Fiction";
            novels[1].Genres[1] = "Romance";
            novels[1].Genres[2] = "Martial Arts";
            novels[1].Genres[3] = "Comedy";            

            novels[2].Genres[0] = "Science Fiction";
            novels[2].Genres[1] = "School Life";
            novels[2].Genres[2] = "Martial Arts";
            novels[2].Genres[3] = "Comedy";
            novels[2].Genres[4] = "Romance";
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

        public static void InitializeSuperUser(
            ApplicationDbContext context, 
            UserManager<AppUser> userManager, 
            RoleManager<IdentityRole> roleManager, 
            IMapper mapper, 
            IConfiguration config)
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

            if (userManager.FindByEmailAsync(config.GetSection("Admin").GetSection("email").Value).Result == null)
            {
                RegistrationViewModel model = new RegistrationViewModel{
                    Email = config.GetSection("Admin").GetSection("email").Value,
                    Password = config.GetSection("Admin").GetSection("password").Value,
                    Username = config.GetSection("Admin").GetSection("username").Value};
                var userIdentity = mapper.Map<AppUser>(model);
                IdentityResult result = userManager.CreateAsync(userIdentity, model.Password).Result;
                if (result.Succeeded)
                {
                    context.Users.AddAsync(new User { 
                        IdentityId = userIdentity.Id, 
                        AvatarUrl = config.GetSection("Default").GetSection("avatar_url").Value}).Wait();
                    userManager.AddToRoleAsync(userIdentity,  "Admin").Wait();
                    context.SaveChangesAsync();
                }
            }

        }
    }
}