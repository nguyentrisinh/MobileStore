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
    public class ModelFromSupplier : BaseModel
    {
        [DisplayName("ID")]
        public int ModelFromSupplierID { get; set; }

        #region Info of each form
        [DisplayName("Số lượng")]
        public int Quantity { get; set; }

        [DisplayName("Giá mua")]
        public double PriceBought { get; set; }
        [DisplayName("Giá bán")]
        public double PriceSold { get; set; }
        [DataType(DataType.Date)]
        [DisplayName("Ngày nhập")]
        //[DisplayFormat(DataFormatString = "{dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime Date { get; set; }
        #endregion

        //#region Supplier Foreign Key
        //public int SupplierID { get; set; }
        //public virtual Supplier Supplier { get; set; }
        //#endregion

        #region StockReceiving ForeignKey
        public Guid StockReceivingID { get; set; }
        public virtual StockReceiving StockReceiving { get; set; }
        #endregion

        #region Model Foreign Key
        [DisplayName("Loại sản phẩm")]
        public int ModelID { get; set; }
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
