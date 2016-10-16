///////////////////////////////////////////////////////////////////////////////////////////////////
//
//	File Name:         Test.cs
//	Description:       This class represents a Test given to a Graduate Student.
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
    public class Test
    {
        /// <summary>
        /// Gets or sets the identifier.
        /// </summary>
        /// <value>
        /// The identifier.
        /// </value>
        [Key]
        public int ID { get; set; }
        [ForeignKey("Student")]
        public int StudentID { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="Test"/> is passed.
        /// </summary>
        /// <value>
        ///   <c>true</c> if passed; otherwise, <c>false</c>.
        /// </value>
        [Required]
        public bool Passed { get; set; }
        /// <summary>
        /// Gets or sets the date taken.
        /// </summary>
        /// <value>
        /// The date taken.
        /// </value>
        [Required]
        public DateTime DateTaken { get; set; }

        /// <summary>
        /// Gets or sets the questions.
        /// </summary>
        /// <value>
        /// The questions on the test.
        /// </value>
        public virtual List<TestQuestion> TestQuestions { get; set; }
        /// <summary>
        /// Gets or sets the student.
        /// </summary>
        /// <value>
        /// The student taking the test.
        /// </value>
        public virtual Student Student { get; set; }
    }
}