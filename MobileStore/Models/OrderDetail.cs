using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MobileStore.Models
{
    public class OrderDetail
    {
        public int OrderDetailID { get; set; }

        public double PriceSold { get; set; }

        #region Item ForeignKey
        public int ItemID { get; set; }
        public virtual Item Item { get; set; }
        #endregion

        #region Order ForeignKey
        public int OrderID { get; set; }
        public virtual Order Order { get; set; }
        #endregion
    }
}
