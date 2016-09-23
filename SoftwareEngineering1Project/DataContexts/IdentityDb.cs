using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.Identity.EntityFramework;
using SoftwareEngineering1Project.Models;

namespace SoftwareEngineering1Project.DataContexts
{
    public class IdentityDb : IdentityDbContext<ApplicationUser>
    {
        public IdentityDb()          
            : base("DefaultConnection", throwIfV1Schema: false)
            //: base("AzureConnection", throwIfV1Schema: false) 
        {
        }

        public static IdentityDb Create()
        {
            return new IdentityDb();
        }
    }
}