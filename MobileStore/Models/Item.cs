using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MobileStore.Models
{
    public class Item
    {
        public int ItemID { get; set; }

        #region Item Specification
        public string Name { get; set; }
        public string IMEI { get; set; }
        public string SerializerNumber { get; set; }
        public string Note { get; set; }
        public ItemStatus Status { get; set; }
        #endregion

        #region ModelFromSupplier foreign key
        public int ModelFromSupplierID { get; set; }
        public ModelFromSupplier ModelFromSupplier { get; set; }
        #endregion

        #region Model foreign key
        public int ModelID { get; set; }
        public Model Model { get; set; }
        #endregion

        public enum ItemStatus
        {

        }
    }
}
