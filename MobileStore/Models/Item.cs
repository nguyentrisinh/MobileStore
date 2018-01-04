using System;
using System.Collections.Generic;
using System.ComponentModel;
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
        [DisplayName("ID")]
        public int ItemID { get; set; }

        #region Item Specification
        [DisplayName("Tên sản phẩm")]
        public string Name { get; set; }
        [DisplayName("IMEI")]
        public string IMEI { get; set; }
        [DisplayName("Số Seri")]
        public string SerializerNumber { get; set; }

        [DisplayName("Ghi chú")]
        public string Note { get; set; }

        [DisplayName("Tình trạng")]
        public ItemStatus Status { get; set; }
        #endregion

        #region ModelFromSupplier foreign key

        [DisplayName("Đợt hàng")]
        public int ModelFromSupplierID { get; set; }

        [DisplayName("Đợt hàng")]
        public virtual ModelFromSupplier ModelFromSupplier { get; set; }
        #endregion

        #region Model foreign key

        [DisplayName("Mẫu mã")]
        public int ModelID { get; set; }

        [DisplayName("Mẫu mã")]
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
