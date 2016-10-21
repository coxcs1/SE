///////////////////////////////////////////////////////////////////////////////////////////////////
//
//	File Name:         Question.cs
//	Description:       This class represents a Question on the Graduate Oral Exit Exam.
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
    public class Question
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
        /// Gets or sets the section identifier.
        /// </summary>
        /// <value>
        /// The section identifier.
        /// </value>
        [ForeignKey("Section")]
        public int SectionID { get; set; }
        /// <summary>
        /// Gets or sets the profile identifier.
        /// </summary>
        /// <value>
        /// The profile identifier.
        /// </value>
        [ForeignKey("Profile")]
        public int ProfileID { get; set; }
        /// <summary>
        /// Gets or sets the text.
        /// </summary>
        /// <value>
        /// The text of a question.
        /// </value>
        [Required]
        public string Text { get; set; }
        /// <summary>
        /// Gets or sets the answer.
        /// </summary>
        /// <value>
        /// The answer to the question.
        /// </value>
        [Required]
        public string Answer { get; set; }
        /// <summary>
        /// Gets or sets the course.
        /// </summary>
        /// <value>
        /// The course the question belongs to.
        /// </value>
        public virtual Section Section { get; set; }
        /// <summary>
        /// Gets or sets the profile.
        /// </summary>
        /// <value>
        /// The professor who created the question.
        /// </value>
        public virtual Profile Profile { get; set; }
    }
}