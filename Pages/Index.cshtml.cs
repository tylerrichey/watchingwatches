using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WatchingWatches.Models;

namespace WatchingWatches.Pages
{
    public class IndexModel : PageModel
    {
        public List<WatchPriceDto> WatchPriceDtos { get; internal set; }

        public void OnGet(bool old = false)
        {
            var prices = new List<WatchPriceDto>();
            var data = CheckWatches.GetPriceSummary()
                .Where(d => (!old && CheckWatches.Urls.Contains(d.Url)) || old);
            foreach (var w in data)
            {
                var next = data.Where(d => d.Url == w.Url && d.When < w.When)
                    .OrderByDescending(d => d.When)
                    .FirstOrDefault();
                prices.Add(new WatchPriceDto
                {
                    Watch = w,
                    PriceReduced = next != null && w.Price < next.Price
                });
            }
            WatchPriceDtos = prices;
        }
    }

    public class WatchPriceDto
    {
        public WatchPriceCheck Watch { get; set; }
        public bool PriceReduced { get; set; }
    }
}
