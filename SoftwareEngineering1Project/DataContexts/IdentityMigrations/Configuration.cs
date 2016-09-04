namespace SoftwareEngineering1Project.DataContexts.IdentityMigrations
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;
    using Microsoft.AspNet.Identity;
    using SoftwareEngineering1Project.Models;
    using Microsoft.AspNet.Identity.EntityFramework;

    internal sealed class Configuration : DbMigrationsConfiguration<SoftwareEngineering1Project.DataContexts.IdentityDb>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
            MigrationsDirectory = @"DataContexts\IdentityMigrations";
        }

        protected override void Seed(SoftwareEngineering1Project.DataContexts.IdentityDb context)
        {
            RoleStore<IdentityRole> roleStore = new RoleStore<IdentityRole>(context);
            RoleManager<IdentityRole> roleManager = new RoleManager<IdentityRole>(roleStore);

            UserStore<ApplicationUser> userStore = new UserStore<ApplicationUser>(context);
            UserManager<ApplicationUser> userManager = new UserManager<ApplicationUser>(userStore);

            //will only populate the user one time
            if (!context.Users.Any(u => u.UserName == "admin@user.com"))
            {
                ApplicationUser user = new ApplicationUser
                {
                    Email = "admin@user.com",
                    EmailConfirmed = true,
                    UserName = "admin@user.com"
                };

                userManager.Create(user, "Pass123!");
                roleManager.Create(new IdentityRole { Name = "sysadmin" });
                userManager.AddToRole(user.Id, "sysadmin");
            }
            if (!context.Roles.Any(r => r.Name == "professor"))
            {
                roleManager.Create(new IdentityRole { Name = "professor" });
            }

        }
    }
}
