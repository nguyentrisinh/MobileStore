using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace MobileStore.Models.ApplicationUserViewModels
{
    //public enum UserRole
    //{
    //    Admin = 1,
    //    Sale = 2,
    //    Technical = 3,
    //    WarehouseManager = 4,
    //    Accountant = 5,
    //}
    public class CreateApplicationUserViewModel
    {
        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }

        [Display(Name = "First Name")]
        public string FirstName { get; set; }
        [Display(Name = "Last Name")]
        public string LastName { get; set; }
        [DataType(DataType.Date)]
        public DateTime Birthday { get; set; }
        public string Phone { get; set; }
        public string Address { get; set; }

        public UserRole Role { get; set; }
    }
}
