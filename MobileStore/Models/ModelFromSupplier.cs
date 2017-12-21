using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace MobileStore.Models
{
    public class ModelFromSupplier
    {
        public int ModelFromSupplierID { get; set; }

        #region Info of each form
        public int Quantity { get; set; }
        public double PriceBought { get; set; }
        public double PriceSold { get; set; }
        public DateTime Date { get; set; }
        public int Period { get; set; } // Count by month
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
        public int ModelID { get; set; }
        public virtual Model Model { get; set; }
        #endregion

        #region Navigators for Items
        public IList<Item> Items { get; set; }

        #endregion
    }
}
