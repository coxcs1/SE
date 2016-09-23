namespace SoftwareEngineering1Project.DataContexts.ApplicationMigrations
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;
    using SoftwareEngineering1Project.Models;

    internal sealed class Configuration : DbMigrationsConfiguration<SoftwareEngineering1Project.DataContexts.ApplicationDb>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
            MigrationsDirectory = @"DataContexts\ApplicationMigrations";
        }

        protected override void Seed(SoftwareEngineering1Project.DataContexts.ApplicationDb context)
        {
            //  This method will be called after migrating to the latest version.

            //  You can use the DbSet<T>.AddOrUpdate() helper extension method 
            //  to avoid creating duplicate seed data. E.g.
            //
            //    context.People.AddOrUpdate(
            //      p => p.FullName,
            //      new Person { FullName = "Andrew Peters" },
            //      new Person { FullName = "Brice Lambson" },
            //      new Person { FullName = "Rowan Miller" }
            //    );
            //
            if (!context.Profiles.Any(u => u.UserEmail == "admin@user.com"))
            {
                Profile newProfile = new Profile
                {
                    FirstName = "Sysadmin",
                    LastName = "Sysadmin",
                    UserEmail = "admin@user.com"
                };
                context.Profiles.Add(newProfile);
            }
        }
    }
}
