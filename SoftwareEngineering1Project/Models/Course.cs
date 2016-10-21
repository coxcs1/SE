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
        /// <summary>
        /// Gets or sets the course CRN.
        /// </summary>
        /// <value>
        /// The course CRN.
        /// </value>
        [Required]
        [RegularExpression(@"^\d{5}$", ErrorMessage = "Must enter a 5 digit CRN, Eg.(12345)")]
        [Display(Name ="CRN")]
        public int CourseCRN { get; set; }
        /// <summary>
        /// Gets or sets the course attribute number.
        /// </summary>
        /// <value>
        /// The course attribute number.
        /// </value>
        [Required]
        [RegularExpression(@"^\d{4}$", ErrorMessage = "Must enter a 4 digit course number Eg.(1100)")]
        [Display(Name ="Attribute Number")]
        public int CourseAttributeNumber { get; set; }

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