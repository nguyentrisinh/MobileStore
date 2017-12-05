﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace MobileStore.Models
{
    public enum UserRole
    {
        Admin = 1,
        Sale = 2,
        Technical = 3,
        WarehouseManager = 4,
        Accountant = 5,
    }
    // Add profile data for application users by adding properties to the ApplicationUser class
    public class ApplicationUser : IdentityUser
    {
        [Display(Name = "First Name")]
        public string FirstName { get; set; }
        [Display(Name = "Last Name")]
        public string LastName { get; set; }
        [DataType(DataType.Date)]
        public DateTime Birthday { get; set; }
        public string Phone { get; set; }
        public string Address { get; set; }

        public UserRole Role { get; set; }

        public IList<Order> Orders { get; set; }

    }
}
