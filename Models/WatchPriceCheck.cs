using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WatchingWatches.Models
{
    public class WatchPriceCheck
    {
        public int Id { get; set; }
        public string Url { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public DateTime When { get; set; }
    }
}
