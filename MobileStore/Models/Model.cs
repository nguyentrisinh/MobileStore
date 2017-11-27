using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MobileStore.Models
{
    public class Model
    {
        public int ModelID { get; set; }

        #region Model Specifications
        public string Name { get; set; }
        public string Color { get; set; }
        public string Description { get; set; }
        public string Specification { get; set; }

        public ModelType Type { get; set; }
        #endregion

        #region Brand Foreignkey 
        public int BrandID { get; set; }
        public virtual Brand Brand { get; set; }
        #endregion

        #region Navigator Items
        public IList<Item> Items { get; set; }
        #endregion

        #region Navigator ModelFromSuppliers
        public IList<ModelFromSupplier> ModelFromSuppliers { get; set; }
        #endregion

        public enum ModelType
        {
            Device = 1,
            Accessory = 2,
        }
    }
}
