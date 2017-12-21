﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace MobileStore.Models
{
    public class Order : BaseModel
    {
        public int OrderID { get; set; }

        #region Order Specifications
        public double Total { get; set; }
        public DateTime Date { get; set; }
        public bool IsPrinted { get; set; }
        #endregion

        //#region ApplicationUser ForeignKey (Staff in system)
        //public string ApplicationUserID { get; set; }
        //public virtual ApplicationUser ApplicationUser { get; set; }
        //#endregion

        #region Customer ForeignKey 
        public int CustomerID { get; set; }
        public virtual Customer Customer { get; set; }
        #endregion

        #region Navigator OrderDetails
        public IList<OrderDetail> OrderDetails { get; set; }
        #endregion
    }
}
