using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SoftwareEngineering1Project.Models
{
    public class Teacher
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
        /// Gets or sets the name.
        /// </summary>
        /// <value>
        /// The name.
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
        /// Gets or sets the e number.
        /// </summary>
        /// <value>
        /// The e number.
        /// </value>
        [Required]
        [RegularExpression(@"^E\d{8}$", ErrorMessage = "Must enter a valid ENumber Eg. (E01234567)")]
        public string ENumber { get; set; }
        /// <summary>
        /// An enumeration of titles
        /// </summary>
        public enum Titles
        {
            [Display(Name = "Dr.")]
            Dr,
            [Display(Name = "Mr.")]
            Mr,
            [Display(Name = "Mrs.")]
            Mrs,
            [Display(Name = "Ms.")]
            Ms
        }
        /// <summary>
        /// Gets or sets the title.
        /// </summary>
        /// <value>
        /// The title.
        /// </value>
        [Required]
        public Titles Title { get; set; }
        /// <summary>
        /// Gets or sets the status.
        /// </summary>
        /// <value>
        /// The status.
        /// </value>
        [Required]
        public string Status { get; set; }

        /// <summary>
        /// Gets the full name.
        /// </summary>
        /// <returns>String</returns>
        public string GetFullName()
        {
            return FirstName + " " + LastName;
        }
    }
}