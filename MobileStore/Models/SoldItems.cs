using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace MobileStore.Models
{
    // This model is used for display static of solded items
    // Not use for store data
    public class SoldItems
    {
        [DisplayName("ModelFromSupplierID")] // for test
        public int ModelFromSupplierID { get; set; }
        [DisplayName("Tên sản phẩm")]
        public string Name { get; set; }
        [DisplayName("SL đã bán")]
        public int NumberSold { get; set; }
        [DisplayName("Đơn giá mua")]
        public double PriceBought { get; set; }
        [DisplayName("Đơn giá bán")]
        public double PriceSold { get; set; }
        [DisplayName("Đơn giá bán thực")]
        public double ActualPriceSold { get; set; }
        [DisplayName("Chênh lệch")]
        public double DiffInPrice { get; set; }
        [DisplayName("Lời")]
        public double Revenue { get; set; }
        [DisplayName("Doanh thu")]
        public double TotalRevenue { get; set; }
        [DisplayName("Nhà cung cấp")]
        public string SupplierName { get; set; }
    }
}
