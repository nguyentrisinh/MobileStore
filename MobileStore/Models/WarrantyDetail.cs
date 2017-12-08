using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace MobileStore.Models
{
    public enum WarrantyDetailStatus
    {
        Fixing = 1,
        Fixed = 2,
        Returned = 3
    }

    public class WarrantyDetail : BaseModel
    {
        public int WarrantyDetailID { get; set; }

        public DateTime Date { get; set; }
        public string DefectInfo { get; set; }
        public WarrantyDetailStatus Status { get; set; }

        #region Warranty ForeignKey
        public int WarrantyCardID { get; set; }
        public virtual WarrantyCard WarrantyCard { get; set; }
        #endregion

    }
}
