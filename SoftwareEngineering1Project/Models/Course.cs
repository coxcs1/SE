///////////////////////////////////////////////////////////////////////////////////////////////////
//
//	File Name:         Course.cs
//	Description:       This class represents a Course.
//
//	Author:            Dana Jarred Light, lightdj@etsu.edu
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
    public class Course
    {
        /// <summary>
        /// Gets or sets the identifier.
        /// </summary>
        /// <value>
        /// The identifier.
        /// </value>
        [Key]
        public int ID { get; set; }
        /// <summary>
        /// Gets or sets the name of the course.
        /// </summary>
        /// <value>
        /// The name of the course.
        /// </value>
        [Required]
        [StringLength(50)]
        [Display(Name ="Course Name")]
        public string CourseName { get; set; }

        [Required]
        [StringLength(4)]
        [RegularExpression(@"([A-Z]{4}|[A-Z]{3})", ErrorMessage = "Must be 3 or 4 letters Eg. CSCI")]
        [Display(Name = "Department Abbreviation")]
        public string DeptAbbreviation { get; set; }

        /// <summary>
        /// Gets or sets the course attribute number.
        /// </summary>
        /// <value>
        /// The course attribute number.
        /// </value>
        [Required]
        [RegularExpression(@"^\d{4}$", ErrorMessage = "Must enter a 4 digit course number Eg.(1100)")]
        [Display(Name ="Course Number")]
        public int CourseAttributeNumber { get; set; }
        /// <summary>
        /// Gets or sets the core course boolean indentifier.
        /// </summary>
        /// <value>
        /// The core course boolean identifier.
        /// </value>
        public Boolean Core { get; set; }
        /// <summary>
        /// Gets or sets the archived course boolean indentifier.
        /// </summary>
        /// <value>
        /// The archived course boolean identifier.
        /// </value>
        public Boolean Archived { get; set; }
        /// <summary>
        /// Gets or sets the profiles.
        /// </summary>
        /// <value>
        /// The teachers teaching the course.
        /// </value>
        public virtual ICollection<Profile> Profiles { get; set; }

        /// <summary>
        /// Gets or sets the sections of this course.
        /// </summary>
        /// <value>
        /// The sections of this course.
        /// </value>
        public virtual ICollection<Section> Sections { get; set; }
    }
}