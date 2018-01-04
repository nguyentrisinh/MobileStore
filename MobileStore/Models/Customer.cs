using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace MobileStore.Models
{
    public enum GenderType
    {
        Male = 1,
        Female = 2,
    }
    public class Customer
    {

        [DisplayName("Mã khách hàng")]
        public int CustomerID { get; set; }

        #region Customer Information

        [DisplayName("Tên khách hàng")]
        public string Name { get; set; }

        [DisplayName("SĐT")]
        public string Phone { get; set; }

        [DisplayName("Địa chỉ")]
        public string Address { get; set; }

        [DisplayName("Giới tính")]
        public GenderType Gender { get; set; }
        [DataType(DataType.Date)]

        [DisplayName("Ngày sinh")]
        public DateTime Birthday { get; set; }

        [DisplayName("Quốc gia")]
        public string Country { get; set; }
        #endregion

        #region Navigator Order
        public IList<Order> Orders { get; set; }
        #endregion

    }
}
