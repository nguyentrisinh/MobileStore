using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MobileStore.Models.StockReceivingViewModels
{
    public class StockReceivingViewModel
    {
        public StockReceiving StockReceiving { get; set; }
        public ModelFromSupplier ModelFromSupplier { get; set; }
        public IEnumerable<ModelFromSupplier> ModelFromSuppliers { get; set; }

        public IEnumerable<Supplier> Suppliers { get; set; }
        public IEnumerable<Model> Models { get; set; }

    }
}
