using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace SoftwareEngineering1Project.Models
{
    public class Profile
    {
        [Key]
        public int Id { get; set; }
   
        public string UserEmail { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        /// <summary>
        /// Gets or sets the courses.
        /// </summary>
        /// <value>
        /// The courses a profile(Professor) teaches.
        /// </value>
        public virtual ICollection<Course> Courses { get; set; }
    }
}