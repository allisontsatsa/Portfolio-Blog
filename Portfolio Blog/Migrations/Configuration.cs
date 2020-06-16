namespace Portfolio_Blog.Migrations
{
    using Microsoft.AspNet.Identity;
    using Microsoft.AspNet.Identity.EntityFramework;
    using Portfolio_Blog.Models;
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<Portfolio_Blog.Models.ApplicationDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = true;
        }

        protected override void Seed(Portfolio_Blog.Models.ApplicationDbContext context)
        {
            var roleManager = new RoleManager<IdentityRole>(
                new RoleStore<IdentityRole>(context));

            if (!context.Roles.Any(r => r.Name == "Admin"))
            {
                roleManager.Create(new IdentityRole { Name = "Admin"});
            }

            if (!context.Roles.Any(r => r.Name == "Mod"))
            {
                roleManager.Create(new IdentityRole { Name = "Mod" });
            }

            var userStore = new UserStore<ApplicationUser>(context);
            var userManager = new UserManager<ApplicationUser>(userStore);

            if (!context.Users.Any(u => u.Email == "allison_tsatsa@hotmail.com"))
            {
                var user = new ApplicationUser
                {
                    UserName = "allison_tsatsa@hotmail.com",
                    Email = "allison_tsatsa@hotmail.com",
                    FirstName = "Allison",
                    LastName = "Tsatsa",
                    DisplayName = "Big Al"
                };

                userManager.Create(user, "ABC456");

                userManager.AddToRoles(user.Id, "Admin");

            }
            if (!context.Users.Any(u => u.Email == "arussell@coderfoundry.com"))
            {
                var user = new ApplicationUser
                {
                    UserName = "arussell@coderfoundry.com",
                    Email = "arussell@coderfoundry.com",
                    FirstName = "Drew",
                    LastName = "Russell",
                    DisplayName = "Prof"
                };

                userManager.Create(user, "123456");

                userManager.AddToRoles(user.Id, "Mod");
            }

            //  This method will be called after migrating to the latest version.

            //  You can use the DbSet<T>.AddOrUpdate() helper extension method 
            //  to avoid creating duplicate seed data.
        }
    }
}
