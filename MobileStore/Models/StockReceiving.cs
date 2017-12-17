using System;
using System.Collections.Generic;
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
        public Guid StockReceivingID { get; set; }

        #region StockReceiving Info
        public DateTime Date { get; set; }
        #endregion

        #region Supplier ForeignKey 
        public int SupplierID { get; set; }
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
