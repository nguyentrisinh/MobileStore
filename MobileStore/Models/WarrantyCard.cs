using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace MobileStore.Models
{
    public class WarrantyCard : BaseModel
    {
        public int WarrantyCardID { get; set; }

        #region WarrantyCard Info
        public Guid TransactionCode { get; set; }
        public int NumberOfWarranty { get; set; }
        [Display(Name = "Start date")]
        [DataType(DataType.Date)]
        public DateTime StartDate { get; set; }
        [Display(Name = "End date")]
        [DataType(DataType.Date)]
        public DateTime EndDate { get; set; }
        public bool IsPrinted { get; set; }
        public bool IsDisabled { get; set; } // Trường này để check nếu sản phẩm này đã được đổi trả thì thẻ bào hành này không còn giá trị nữa
        #endregion

        #region Item ForeignKey
        public int ItemID { get; set; }
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
