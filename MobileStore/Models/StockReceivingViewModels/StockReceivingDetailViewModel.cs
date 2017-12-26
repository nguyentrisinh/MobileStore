using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MobileStore.Models.StockReceivingViewModels
{
    public class StockReceivingDetailViewModel
    {
        public ModelFromSupplier ModelFromSupplier { get; set; }
        public Item Item { get; set; }
        public IEnumerable<Item> Items { get; set; }
    }
}
