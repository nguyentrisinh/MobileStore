using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace MobileStore.Models
{
    public class OrderDetail
    {

        [DisplayName("Mã chi tiết hóa đơn")]
        public int OrderDetailID { get; set; }

        [DisplayName("Giá bán")]

        public double PriceSold { get; set; }

        #region Item ForeignKey

        [DisplayName("Sản phẩm")]
        public int ItemID { get; set; }

        [DisplayName("Sản phẩm")]
        public virtual Item Item { get; set; }
        #endregion

        #region Order ForeignKey
        public int OrderID { get; set; }
        public virtual Order Order { get; set; }
        #endregion
    }
}
