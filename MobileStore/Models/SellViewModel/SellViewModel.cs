using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MobileStore.Models.SellViewModel
{
    public class SellViewModel
    {
        public Order Order { get; set; }
        public OrderDetail OrderDetail { get; set; }


        public IEnumerable<OrderDetail> OrderDetails { get; set; }

        public IEnumerable NewItems { get; set; }

        public IEnumerable<Customer> Customers { get; set; }

    }
}
