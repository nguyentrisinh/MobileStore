using System;
using System.Collections.Generic;
using System.ComponentModel;
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
        [DisplayName("Mã chi tiết phiếu bảo hành")]
        public int WarrantyDetailID { get; set; }
        [DisplayName("Ngày mua hàng")]
        public DateTime Date { get; set; } // Ngày khách hàng đến yêu cầu bảo hành
        [DisplayName("Ngày dự kiến")]
        public DateTime ExpectedDate { get; set; } // Ngày dự kiến sẽ bảo hành xong sản phẩm
        [DisplayName("Ngày sửa xong")]
        public DateTime? WarrantyDate { get; set; } // Ngày sản phẩm được bảo hành xong
        [DisplayName("Ngày trả khách")]
        public DateTime? ReturnedDate { get; set; } // Ngày khách hàng lấy sản phẩm về
        [DisplayName("Lỗi")]
        public string DefectInfo { get; set; }

        [DisplayName("Đã xuất phiếu hẹn")]
        public bool IsPrinted { get; set; }

        [DisplayName("Trạng thái")]
        public WarrantyDetailStatus Status { get; set; }

        #region Warranty ForeignKey

        [DisplayName("Nằm trong phiếu bảo hành")]
        public int WarrantyCardID { get; set; }

        [DisplayName("Nằm trong phiếu bảo hành")]
        public virtual WarrantyCard WarrantyCard { get; set; }
        #endregion

    }
}
