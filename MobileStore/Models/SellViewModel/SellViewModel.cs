using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace MobileStore.Models.SellViewModel
{
    public class SellViewModel
    {
        [DisplayName("Hóa đơn mua hàng")]
        public Order Order { get; set; }

        [DisplayName("Chi tiết hóa đơn mua hàng")]
        public OrderDetail OrderDetail { get; set; }

        [DisplayName("Chi tiết hóa đơn mua hàng")]

        public IEnumerable<OrderDetail> OrderDetails { get; set; }

        [DisplayName("Sản phẩm còn trong kho")]

        public IEnumerable<Item> NewItems { get; set; }

        [DisplayName("Khách hàng")]

        public IEnumerable<Customer> Customers { get; set; }

        [DisplayName("Mẫu mã")]

        public IEnumerable<Model> Models { get; set; }

    }
}
