using System.Collections.Generic;
using SukkarFamily.Models;

namespace SukkarFamily.ViewModel
{
    public class HomeIndexViewModel
    {
        public IEnumerable<News> News { get; set; }
        public FamilyStatisticsViewModel FamilyStatistics { get; set; }
    }
}