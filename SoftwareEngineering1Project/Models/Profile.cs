///////////////////////////////////////////////////////////////////////////////////////////////////
//
//	File Name:         Profile.cs
//	Description:       This class represents a Profile.
//
//	Author:            Mackenzie Eagan, eaganm@etsu.edu
//
///////////////////////////////////////////////////////////////////////////////////////////////////
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
        [Display(Name = "Email/Username")]
        public string UserEmail { get; set; }
        [Display(Name = "First Name")]
        public string FirstName { get; set; }
        [Display(Name = "Last Name")]
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