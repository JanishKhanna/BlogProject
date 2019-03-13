namespace BlogProject.Migrations
{
    using BlogProject.Models;
    using BlogProject.Models.Domain;
    using Microsoft.AspNet.Identity;
    using Microsoft.AspNet.Identity.EntityFramework;
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<BlogProject.Models.ApplicationDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = true;
        }

        protected override void Seed(BlogProject.Models.ApplicationDbContext context)
        {
            //  This method will be called after migrating to the latest version.

            //  You can use the DbSet<T>.AddOrUpdate() helper extension method 
            //  to avoid creating duplicate seed data.
            var newPost = new Post()
            {
                Title = "myTestPost",
                Body = "Mybody",
                DateCreated = DateTime.Now,
                DateUpdated = DateTime.Now,
                Published = true                
            };
            context.AllPosts.AddOrUpdate(p => p.Title, newPost);
            context.SaveChanges();

            var roleManager =
                new RoleManager<IdentityRole>(
                    new RoleStore<IdentityRole>(context));

            //UserManager, used to manage users
            var userManager =
                new UserManager<ApplicationUser>(
                        new UserStore<ApplicationUser>(context));


            if (!context.Roles.Any(p => p.Name == "Admin"))
            {
                var adminRole = new IdentityRole("Admin");
                roleManager.Create(adminRole);
            }

            //Creating the adminuser
            ApplicationUser adminUser;

            if (!context.Users.Any(
                p => p.UserName == "admin@blog.com"))
            {
                adminUser = new ApplicationUser();
                adminUser.UserName = "admin@blog.com";
                adminUser.Email = "admin@blog.com";

                userManager.Create(adminUser, "Password-1");
            }
            else
            {
                adminUser = context
                    .Users
                    .First(p => p.UserName == "admin@blog.com");
            }

            //Make sure the user is on the admin role
            if (!userManager.IsInRole(adminUser.Id, "Admin"))
            {
                userManager.AddToRole(adminUser.Id, "Admin");
            }


            if (!context.Roles.Any(p => p.Name == "moderator"))
            {
                var moderatorRole = new IdentityRole("moderator");
                roleManager.Create(moderatorRole);
            }

            //Creating the moderatorUser
            ApplicationUser moderatorUser;

            if (!context.Users.Any(
                p => p.UserName == "moderator@blog.com"))
            {
                moderatorUser = new ApplicationUser();
                moderatorUser.UserName = "moderator@blog.com";
                moderatorUser.Email = "moderator@blog.com";

                userManager.Create(moderatorUser, "Password-1");
            }
            else
            {
                moderatorUser = context
                    .Users
                    .First(p => p.UserName == "moderator@blog.com");
            }

            //Make sure the user is on the admin role
            if (!userManager.IsInRole(moderatorUser.Id, "moderator"))
            {
                userManager.AddToRole(moderatorUser.Id, "moderator");
            }

        }
    }
}
