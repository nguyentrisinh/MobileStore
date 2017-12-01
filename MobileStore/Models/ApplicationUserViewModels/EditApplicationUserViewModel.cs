using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace MobileStore.Models.ApplicationUserViewModels
{
    public class EditApplicationUserViewModel
    {
        #region ApplicationUser Identity Attribute
        [Required]
        public string Id { get; set; }
        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }
        #endregion

        #region ApplicationUser info
        [Display(Name = "First Name")]
        public string FirstName { get; set; }
        [Display(Name = "Last Name")]
        public string LastName { get; set; }
        [DataType(DataType.Date)]
        public DateTime Birthday { get; set; }
        public string Phone { get; set; }
        public string Address { get; set; }

        public UserRole Role { get; set; }
        #endregion
    }
}
