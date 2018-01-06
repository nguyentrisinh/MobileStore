using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace MobileStore.Models
{
    public class Brand
    {

        [DisplayName("Mã thương hiệu")]
        public int BrandID { get; set; }

        #region Brand Information
        [Display(Name = "Tên thương hiệu")]
        public string Name { get; set; }
        [Display(Name = "Quốc gia")]
        public string Country { get; set; }
        [Display(Name = "Mô tả")]
        public string Description { get; set; }
        #endregion

        #region Navigator Model
        public IList<Model> Models { get; set; }
        #endregion
    }
}
