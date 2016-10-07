using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;
using Microsoft.AspNet.Identity.EntityFramework;
using SoftwareEngineering1Project.Models;

namespace SoftwareEngineering1Project.DataContexts
{
    public class ApplicationDb : DbContext
    {
        public ApplicationDb()          
            //: base("DefaultConnection")
            : base("AzureConnection")
        {
        }

        public static ApplicationDb Create()
        {
            return new ApplicationDb();
        }

        public DbSet<Profile> Profiles { get; set; }
        public DbSet<Course> Courses { get; set; }
        public DbSet<Test> Tests { get; set; }
        public DbSet<Question> Questions { get; set; }
        public DbSet<TestQuestion> TestQuestions { get; set; }
        public DbSet<Student> Students { get; set; }
        public DbSet<Teacher> Teachers { get; set; }
    }
}