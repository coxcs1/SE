﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SoftwareEngineering1Project.Models
{
    public class Student
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
        /// Gets or sets the first name.
        /// </summary>
        /// <value>
        /// The first name.
        /// </value>
        [Required]
        public string FirstName { get; set; }
        /// <summary>
        /// Gets or sets the last name.
        /// </summary>
        /// <value>
        /// The last name.
        /// </value>
        [Required]
        public string LastName { get; set; }
        /// <summary>
        /// Gets or sets the enter date.
        /// </summary>
        /// <value>
        /// The enter date.
        /// </value>
        [Required]
        public DateTime EnterDate { get; set; }
        /// <summary>
        /// Gets or sets the concentration.
        /// </summary>
        /// <value>
        /// The concentration.
        /// </value>
        [Required]
        public string Concentration { get; set; }
        /// <summary>
        /// Gets or sets the notes.
        /// </summary>
        /// <value>
        /// The notes.
        /// </value>
        [Required]
        public string Notes { get; set; }

        /// <summary>
        /// Gets or sets the tests.
        /// </summary>
        /// <value>
        /// The tests they take. (Hopefully only one for their sake)
        /// </value>
        public virtual ICollection<Test> Tests { get; set; }
    }
}