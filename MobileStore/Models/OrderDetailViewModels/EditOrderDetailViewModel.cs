using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MobileStore.Models.OrderDetailViewModels
{
    public class EditOrderDetailViewModel
    {
        public OrderDetail OrderDetail { get; set; }
        public WarrantyCard WarrantyCard { get; set; }
        //Danh sách Item dùng để đổi cho đơn hàng hiện tại
        // Phải là những sản phẩm còn trong InStock
        public IEnumerable<Item> Items { get; set; }
       // Danh sách những đơn hàng có thể đổi sản phẩm sang
       // Phải là những đơn hàng chưa in
        public IEnumerable<Order> Orders { get; set; }
        public bool CanReturn { get; set; }
    }
}
