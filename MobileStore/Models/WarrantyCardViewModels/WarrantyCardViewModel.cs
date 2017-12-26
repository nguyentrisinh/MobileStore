using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MobileStore.Models.WarrantyCardViewModels
{
    public class WarrantyCardViewModel
    {
        //Sử dụng cho Detail của WarrantyCards controller
        public WarrantyCard WarrantyCard { get; set; }
        public WarrantyDetail WarrantyDetail { get; set; }
        public IEnumerable<WarrantyDetail> WarrantyDetails { get; set; }
        public bool CanReturn { get; set; }
        public WarrantyCardStatus WarrantyCardStatus { get; set; }
        public ReturnItem ReturnItem { get; set; }
        public IEnumerable<Item> Items { get; set; }
    }

    public enum WarrantyCardStatus
    {
       Returned = 1,
       Waiting = 2,
        Expired = 3,
        CanWarrant = 4
    }
}
