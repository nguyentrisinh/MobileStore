using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace MobileStore.Models
{
    public class Order : BaseModel
    {

        [DisplayName("Mã hóa đơn mua hàng")]
        public int OrderID { get; set; }

        #region Order Specifications

        [DisplayName("Tổng tiền")]
        public double Total { get; set; }

        [DisplayName("Ngày mua")]
        public DateTime Date { get; set; }

        [DisplayName("Đã in?")]
        public bool IsPrinted { get; set; }
        #endregion

        //#region ApplicationUser ForeignKey (Staff in system)
        //public string ApplicationUserID { get; set; }
        //public virtual ApplicationUser ApplicationUser { get; set; }
        //#endregion

        #region Customer ForeignKey 

        [DisplayName("Khách mua hàng")]
        public int CustomerID { get; set; }

        [DisplayName("Khách mua hàng")]
        public virtual Customer Customer { get; set; }
        #endregion

        #region Navigator OrderDetails

        [DisplayName("Danh sách chi tiết hóa đơn")]
        public IList<OrderDetail> OrderDetails { get; set; }
        #endregion
    }
}
