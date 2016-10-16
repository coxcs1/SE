///////////////////////////////////////////////////////////////////////////////////////////////////
//
//	File Name:         Course.cs
//	Description:       This class represents a Question given on a test.
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
    public class TestQuestion
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
        /// Gets or sets the test identifier.
        /// </summary>
        /// <value>
        /// The test identifier.
        /// </value>
        [ForeignKey("Test")]
        public int TestID { get; set; }
        /// <summary>
        /// Gets or sets the question identifier.
        /// </summary>
        /// <value>
        /// The question identifier.
        /// </value>
        [ForeignKey("Question")]
        public int QuestionID { get; set; }
        /// <summary>
        /// Gets or sets the question score.
        /// </summary>
        /// <value>
        /// The question score.
        /// </value>
        public int QuestionScore { get; set; }
        /// <summary>
        /// Gets or sets the question.
        /// </summary>
        /// <value>
        /// The question.
        /// </value>
        public virtual Question Question { get; set; }
        /// <summary>
        /// Gets or sets the test.
        /// </summary>
        /// <value>
        /// The test.
        /// </value>
        public virtual Test Test { get; set; }
    }
}