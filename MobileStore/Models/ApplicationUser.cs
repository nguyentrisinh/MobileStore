using System;
using System.Collections.Generic;
using System.ComponentModel;
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
        [Display(Name = "Họ nhân viên")]
        public string FirstName { get; set; }
        [Display(Name = "Tên nhân viên")]
        public string LastName { get; set; }
        [DataType(DataType.Date)]
        
        [DisplayName("Mẫu mã")]
        public DateTime Birthday { get; set; }

        [DisplayName("SĐT")]
        public string Phone { get; set; }

        [DisplayName("Địa chỉ")]
        public string Address { get; set; }

        [DisplayName("Avatar")]
        public string AvatarUrl { get; set; }

        [DisplayName("Chức vụ")]

        public UserRole Role { get; set; }

        #region Navigator Orders
        public IList<Order> Orders { get; set; }
        #endregion

        #region Navigator StockReceiving
        public IList<StockReceiving> StoceReceivings { get; set; }
        #endregion

        #region Navigator ReturnItems
        public IList<ReturnItem> ReturnItems { get; set; }
        #endregion

        #region Navigator WarrantyCard 
        public IList<WarrantyCard> WarrantyCards { get; set; }
        #endregion

        #region Navigator WarrantyDetail
        public IList<WarrantyDetail> WarrantyDetails { get; set; }
        #endregion

    }
}
