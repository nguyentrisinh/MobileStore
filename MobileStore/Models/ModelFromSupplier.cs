using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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
        #endregion

        #region Supplier Foreign Key
        public int SupplierID { get; set; }
        public Supplier Sipplier { get; set; }
        #endregion

        #region Model Foreign Key
        public int ModelID { get; set; }
        public Model Model { get; set; }
        #endregion

        #region Navigators for Items
        public ICollection<Item> Items { get; set; }

        #endregion
    }
}
