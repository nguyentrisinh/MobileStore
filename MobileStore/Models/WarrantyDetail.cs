using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MobileStore.Models
{
    public class WarrantyDetail
    {
        public int WarrantyDetailID { get; set; }

        public DateTime Date { get; set; }
        public string DefectInfo { get; set; }
        public WarrantyDetailStatus Status { get; set; }

        #region Warranty ForeignKey
        public int WarrantyCardID { get; set; }
        public WarrantyCard WarrantyCard { get; set; }
        #endregion

        public enum WarrantyDetailStatus
        {
            Fixing = 1,
            Fixed = 2,
            Returned = 3
        }
    }
}
