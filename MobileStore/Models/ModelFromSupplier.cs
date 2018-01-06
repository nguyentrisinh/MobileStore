using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace MobileStore.Models
{
    public class ModelFromSupplier
    {
        [DisplayName("Mã chi tiết đợt hàng")]
        public int ModelFromSupplierID { get; set; }

        #region Info of each form
        [DisplayName("Số lượng")]
        public int Quantity { get; set; }
        [Required]
        [DisplayName("Giá mua")]
        public double PriceBought { get; set; }
        [Required]
        [DisplayName("Giá bán")]
        public double PriceSold { get; set; }
        [DataType(DataType.Date)]
        [DisplayName("Ngày nhập")]
        //[DisplayFormat(DataFormatString = "{dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime Date { get; set; }
        [Required]

        [DisplayName("Hạn bảo hành (tháng)")]
        public int Period { get; set; } // Count by month
        #endregion

        //#region Supplier Foreign Key
        //public int SupplierID { get; set; }
        //public virtual Supplier Supplier { get; set; }
        //#endregion

        #region StockReceiving ForeignKey

        [DisplayName("Mã nhập hàng")]
        public Guid StockReceivingID { get; set; }

        [DisplayName("Mã nhập hàng")]
        public virtual StockReceiving StockReceiving { get; set; }
        #endregion

        #region Model Foreign Key
        [DisplayName("Mẫu mã sản phẩm")]
        public int ModelID { get; set; }

        [DisplayName("Mẫu mã sản phẩm")]
        public virtual Model Model { get; set; }
        #endregion

        

        #region Navigators for Items
        public IList<Item> Items { get; set; }

        #endregion

        public string GetDate()
        {
            return Date.ToString("dd/MM/yyyy");
        }
    }
}
