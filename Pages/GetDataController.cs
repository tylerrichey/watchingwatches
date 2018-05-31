using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace WatchingWatches.Pages
{
    [Produces("application/json")]
    [Route("/GetData")]
    public class GetDataController : Controller
    {
        public IActionResult Get()
        {
            CheckWatches.UpdatePrices();
            return RedirectToPage("/Index");
        }
    }
}