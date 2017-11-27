using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MobileStore.Models
{
    public class Brand
    {
        public int BrandID { get; set; }

        #region Brand Information
        public string Name { get; set; }
        public string Country { get; set; }
        public string Description { get; set; }
        #endregion

        #region Navigator Model
        public IList<Model> Models { get; set; }
        #endregion
    }
}
