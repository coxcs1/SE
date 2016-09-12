using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using SoftwareEngineering1Project.Models;

namespace SoftwareEngineering1Project.ViewModels
{
    [NotMapped]
    public class UserProfile
    { 
        [Required]
        [EmailAddress]
        [Display(Name = "Email/Username")]
        [Key]
        public string Email { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm Password")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }

        [Required]
        [Display(Name = "Role")]
        public string Role { get; set; }

        [Required]
        [Display(Name ="First Name")]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 1)]
        public string FirstName { get; set; }

        [Required]
        [Display(Name = "Last Name")]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 1)]
        public string LastName { get; set; }

        public UserProfile()
        {
            Email = "";
            FirstName = "";
            LastName = "";
            Role = "X";
        }

        public UserProfile(string email, string first, string last)
        {
            Email = email;
            FirstName = first;
            LastName = last;
        }

        public UserProfile(Profile profile)
        {
            Email = profile.UserEmail;
            FirstName = profile.FirstName;
            LastName = profile.LastName;
        }

    }
}