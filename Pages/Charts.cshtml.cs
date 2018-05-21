using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace WatchingWatches.Pages
{
    public class ChartsModel : PageModel
    {
        public IEnumerable<string> Labels { get; internal set; }
        public IEnumerable<decimal> Amounts { get; internal set; }
        public string Name { get; internal set; }

        public void OnGet(string url, long fromDateTicks)
        {
            var prices = CheckWatches.GetForWatch(WebUtility.UrlDecode(url), new DateTime(fromDateTicks)).Take(30);
            Labels = prices.Select(w => w.When.Month + "/" + w.When.Day);
            Amounts = prices.Select(w => w.Price);
            Name = prices.First().Name;
        }
    }
}