using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MobileStore.Models
{
    public class ReturnItem : BaseModel
    {

        [DisplayName("Mã phiếu đổi trả")]
        public int ReturnItemID { get; set; }

        [DisplayName("Ngày đổi trả")]

        public DateTime ReturnDate { get; set; }

        [DisplayName("Lỗi")]
        public string DefectInfo { get; set; }

        #region NewItem Foreign Key

        [DisplayName("Sản phẩm đền bù")]
        public int NewItemID { get; set; }

        [DisplayName("sản phẩm đền bù")]
        public virtual Item NewItem { get; set; }
        #endregion

        #region OldItem Foreign Key

        [DisplayName("Sản phẩm bị trả lại")]
        public int OldItemID { get; set; }

        [DisplayName("Sản phẩm bị trả lại")]
        public virtual Item OldItem { get; set; }
        #endregion 


    }
}
