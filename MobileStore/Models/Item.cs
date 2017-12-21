using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace MobileStore.Models
{
    public enum ItemStatus
    {
        InStock = 1,
        Sold = 2,
        Returned = 3,
    }

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
        public virtual ModelFromSupplier ModelFromSupplier { get; set; }
        #endregion

        #region Model foreign key
        public int ModelID { get; set; }
        public virtual Model Model { get; set; }
        #endregion

        #region Navigator OrderDetail
        public virtual OrderDetail OrderDetail { get; set; }
        #endregion

        #region Navigator NewItem
        public virtual ReturnItem NewItem { get; set; }
        #endregion

        #region Navigator OldItem
        public virtual ReturnItem OldItem { get; set; }
        #endregion

        #region Navigator WarrantyCard
        public IList<WarrantyCard> WarrantyCards{ get; set; }
        #endregion


    }
}
