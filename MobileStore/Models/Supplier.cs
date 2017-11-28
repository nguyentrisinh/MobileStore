using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace MobileStore.Models
{
    public class Supplier
    {
        public int SupplierID { get; set; }

        #region SupplierInfo
        public string Name { get; set; }
        public string Address { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string Code { get; set; }
        public SupplierStatus Status { get; set; }
        #endregion

        #region PicInfo (pic = person in charge)
        public string PicName { get; set; }
        public string PicPhone { get; set; }
        public string PicEmail { get; set; }
        #endregion

        #region Navigator ModelFromSuppliers
        public IList<ModelFromSupplier> ModelFromSuppliers { get; set; }
        #endregion

        public enum SupplierStatus
        {
            Deactive = 0,
            Active = 1,
        }
    }
}
