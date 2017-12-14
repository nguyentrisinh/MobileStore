using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace MobileStore.Models
{
    public enum SupplierStatus
    {
        Deactive = 0,
        Active = 1,
    }

    public class Supplier
    {
        [DisplayName("Mã số")]
        //[Display(Name = "Last Name")]
        public int SupplierID { get; set; }

        #region SupplierInfo
        [DisplayName("Tên nhà cung cấp")]
        public string Name { get; set; }
        [DisplayName("Địa chỉ")]
        public string Address { get; set; }
        [DisplayName("SĐT")]
        public string Phone { get; set; }
        [DisplayName("Email")]
        public string Email { get; set; }
        [DisplayName("Mã vùng")]
        public string Code { get; set; }
        [DisplayName("Tình trạng")]
        public SupplierStatus Status { get; set; }
        #endregion

        #region PicInfo (pic = person in charge)
        [DisplayName("Tên người đại diện")]
        public string PicName { get; set; }
        [DisplayName("SĐT Người đại diện")]
        public string PicPhone { get; set; }
        [DisplayName("Email người đại diện")]
        public string PicEmail { get; set; }
        #endregion

        #region Navigator StockReceiving
        public IList<StockReceiving> StockReceivings { get; set; }
        #endregion
       

    }
    
}
