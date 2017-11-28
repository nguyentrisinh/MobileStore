using System;
using System.Collections.Generic;
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
        public int CustomerID { get; set; }

        #region Customer Information
        public string Name { get; set; }
        public string Phone { get; set; }
        public string Address { get; set; }
        public GenderType Gender { get; set; }
        [DataType(DataType.Date)]
        public DateTime Birthday { get; set; }
        public string Country { get; set; }
        #endregion

        #region Navigator Order
        public IList<Order> Orders { get; set; }
        #endregion

    }
}
