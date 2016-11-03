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
            
            if(context.Courses.Count() == 0)
            {
                #region Core Classes
                Course c1 = new Course
                {
                    CourseName = "Software Systems Engineering",
                    CourseAttributeNumber = 5200,
                    DeptAbbreviation = "CSCI"                   
                };
                context.Courses.Add(c1);

                Course c2 = new Course
                {
                    CourseName = "Software Design",
                    CourseAttributeNumber = 5300,
                    DeptAbbreviation = "CSCI"
                };
                context.Courses.Add(c2);

                Course c3 = new Course
                {
                    CourseName = "Software Project Management",
                    CourseAttributeNumber = 5230,
                    DeptAbbreviation = "CSCI"
                };
                context.Courses.Add(c3);

                //removed research methods course from core
                #endregion

                #region Applied CS Courses
                Course c5 = new Course
                {
                    CourseName = "Distributed Systems",
                    CourseAttributeNumber = 5150,
                    DeptAbbreviation = "CSCI"
                };
                context.Courses.Add(c5);

                Course c6 = new Course
                {
                    CourseName = "Analysis of Algorithms",
                    CourseAttributeNumber = 5620,
                    DeptAbbreviation = "CSCI"
                };
                context.Courses.Add(c6);

                Course c7 = new Course
                {
                    CourseName = "Database Design",
                    CourseAttributeNumber = 5250,
                    DeptAbbreviation = "CSCI"
                };
                context.Courses.Add(c7);

                Course c8 = new Course
                {
                    CourseName = "Software Verification and Validation",
                    CourseAttributeNumber = 5220,
                    DeptAbbreviation = "CSCI"
                };
                context.Courses.Add(c8);
                #endregion

                #region Information Technology
                Course c9 = new Course
                {
                    CourseName = "Advanced Networking",
                    CourseAttributeNumber = 5410,
                    DeptAbbreviation = "CSCI"
                };
                context.Courses.Add(c9);

                Course c10 = new Course
                {
                    CourseName = "E-Commerce Implementation",
                    CourseAttributeNumber = 5710,
                    DeptAbbreviation = "CSCI"
                };
                context.Courses.Add(c10);

                Course c11 = new Course
                {
                    CourseName = "Enterprise Information Systems",
                    CourseAttributeNumber = 5730,
                    DeptAbbreviation = "CSCI"
                };
                context.Courses.Add(c11);

                Course c12 = new Course
                {
                    CourseName = "Networking and Information Security",
                    CourseAttributeNumber = 5460,
                    DeptAbbreviation = "CSCI"
                };
                context.Courses.Add(c12);
                #endregion
            }
            
        }
    }
}
