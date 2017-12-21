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
        public bool CanWarrant { get; set; }
        public ReturnItem ReturnItem { get; set; }
        public IEnumerable<Item> Items { get; set; }
    }
}
