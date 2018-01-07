using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace MobileStore.Models
{
    public enum ModelType
    {
        Device = 1,
        Accessory = 2,
    }

    public class Model
    {

        [DisplayName("Mã mẫu mã")]
        public int ModelID { get; set; }

        #region Model Specifications

        [DisplayName("Tên mẫu mã")]
        public string Name { get; set; }

        [DisplayName("Màu sắc")]
        public string Color { get; set; }

        [DisplayName("Mô tả")]
        public string Description { get; set; }

        [DisplayName("Đặc điểm")]
        public string Specification { get; set; }

        [DisplayName("Ảnh mẫu mã")]
        public string PictureOneUrl { get; set; }

        [DisplayName("Ảnh mẫu mã")]
        public string PictureTwoUrl { get; set; }

        [DisplayName("Ảnh mẫu mã")]
        public string PictureThreeUrl { get; set; }

        [DisplayName("Loại mẫu mã")]

        public ModelType Type { get; set; }
        #endregion

        #region Brand Foreignkey 

        [DisplayName("Thương hiệu")]
        public int BrandID { get; set; }

        [DisplayName("Thương hiệu")]
        public virtual Brand Brand { get; set; }
        #endregion

        #region Navigator Items
        public IList<Item> Items { get; set; }
        #endregion

        #region Navigator ModelFromSuppliers
        public IList<ModelFromSupplier> ModelFromSuppliers { get; set; }
        #endregion

    }
}
