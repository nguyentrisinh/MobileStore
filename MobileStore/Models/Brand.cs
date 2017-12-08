using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace MobileStore.Models
{
    public class Brand
    {
        public int BrandID { get; set; }

        #region Brand Information
        [Display(Name = "Brand")]
        public string Name { get; set; }
        [Display(Name = "Country")]
        public string Country { get; set; }
        [Display(Name = "Description")]
        public string Description { get; set; }
        #endregion

        #region Navigator Model
        public IList<Model> Models { get; set; }
        #endregion
    }
}
