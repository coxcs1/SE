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
            : base("DefaultConnection")
            //: base("AzureConnection") - Do Not Uncomment
        {
        }

        public static ApplicationDb Create()
        {
            return new ApplicationDb();
        }

        public DbSet<Profile> Profiles { get; set; }        
    }
}