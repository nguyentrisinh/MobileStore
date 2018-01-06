using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace MobileStore.Models
{
    public class WarrantyCard : BaseModel
    {

        [DisplayName("Mã phiếu đổi trả")]
        public int WarrantyCardID { get; set; }

        #region WarrantyCard Info
        public Guid TransactionCode { get; set; }

        [DisplayName("Số lần bảo hành")]
        public int NumberOfWarranty { get; set; }
        [Display(Name = "Ngày bắt đầu bảo hành")]
        [DataType(DataType.Date)]
        public DateTime StartDate { get; set; }
        [Display(Name = "Ngày kết thúc bảo hành")]
        [DataType(DataType.Date)]
        public DateTime EndDate { get; set; }

        [DisplayName("Đã xuất phiếu đổi trả?")]
        public bool IsPrinted { get; set; }

        [DisplayName("Còn hiệu lực bảo hành?")]
        public bool IsDisabled { get; set; } // Trường này để check nếu sản phẩm này đã được đổi trả thì thẻ bào hành này không còn giá trị nữa
        #endregion

        #region Item ForeignKey

        [DisplayName("Sản phẩm bảo hành")]
        public int ItemID { get; set; }

        [DisplayName("Sản phẩm bảo hành")]
        public virtual Item Item { get; set; }
        #endregion

        #region Navigator for WarrantyDetail
        public IList<WarrantyDetail> WarrantyDetails { get; set; }
        #endregion

        #region Contructors of WarrantyCard
        public WarrantyCard()
        {
            this.TransactionCode = Guid.NewGuid();
        }
        #endregion

        #region method

        public bool CanWarrant()
        {
            return DateTime.Now >= StartDate && DateTime.Now < EndDate;
        }

        #endregion
    }
}
