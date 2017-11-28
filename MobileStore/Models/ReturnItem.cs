using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MobileStore.Models
{
    public class ReturnItem
    {
        public int ReturnItemID { get; set; }

        public DateTime ReturnDate { get; set; }
        public string DefectInfo { get; set; }

        #region NewItem Foreign Key
        public int NewItemID { get; set; }
        public virtual Item NewItem { get; set; }
        #endregion

        #region OldItem Foreign Key
        public int OldItemID { get; set; }
        public virtual Item OldItem { get; set; }
        #endregion 


    }
}
