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

        public DateTime Date { get; set; } // Ngày khách hàng đến yêu cầu bảo hành
        public DateTime ExpectedDate { get; set; } // Ngày dự kiến sẽ bảo hành xong sản phẩm
        public DateTime WarrantyDate { get; set; } // Ngày sản phẩm được bảo hành xong
        public DateTime ReturnedDate { get; set; } // Ngày khách hàng lấy sản phẩm về
        public string DefectInfo { get; set; }
        public bool IsPrinted { get; set; }

        public WarrantyDetailStatus Status { get; set; }

        #region Warranty ForeignKey
        public int WarrantyCardID { get; set; }
        public virtual WarrantyCard WarrantyCard { get; set; }
        #endregion

    }
}
