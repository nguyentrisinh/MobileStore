using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MobileStore.Models.RevenueAnalyticViewModels
{
    public class RevenueListViewModel
    {
        public int Month { get; set; }
        public int Year { get; set; }
        public double Total { get; set; }
        public double Revenue { get; set; }
    }

    public class RevenueAnalyticViewModel
    {
        public IList<RevenueListViewModel> RevenueListViewModels { get; set; }
    }
}
