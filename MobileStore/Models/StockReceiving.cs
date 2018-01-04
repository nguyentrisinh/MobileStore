using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.SqlTypes;

namespace MobileStore.Models
{
    public class StockReceiving : BaseModel
    {
        [Key]

        [DisplayName("Mã nhập hàng")]
        public Guid StockReceivingID { get; set; }

        #region StockReceiving Info

        [DisplayName("Ngày nhập hàng")]
        public DateTime Date { get; set; }
        #endregion

        #region Supplier ForeignKey 

        [DisplayName("Nhà cung cấp")]
        public int SupplierID { get; set; }

        [DisplayName("Nhà cung cấp")]
        public virtual Supplier Supplier { get; set; }
        #endregion

        #region ModelFromSupplier Navigator
        public IList<ModelFromSupplier> ModelFromSuppliers { get; set; }
        #endregion

        public StockReceiving()
        {
            this.StockReceivingID = Guid.NewGuid();
        }
    }
}
