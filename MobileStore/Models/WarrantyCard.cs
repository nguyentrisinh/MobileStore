using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace MobileStore.Models
{
    public class WarrantyCard
    {
        public int WarrantyCardID { get; set; }

        public int NumberOfWarranty { get; set; }
        [Display(Name = "Start date")]
        [DataType(DataType.Date)]
        public DateTime StartDate { get; set; }
        [Display(Name = "End date")]
        [DataType(DataType.Date)]
        public DateTime EndDate { get; set; }
        public int Period { get; set; } // By month

        #region Item ForeignKey
        public int ItemID { get; set; }
        public virtual Item Item { get; set; }
        #endregion

        #region Navigator for WarrantyDetail
        public IList<WarrantyDetail> WarrantyDetails { get; set; }
        #endregion

    }
}
