using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using HtmlAgilityPack;
using LiteDB;
using WatchingWatches.Models;

namespace WatchingWatches
{
    public static class CheckWatches
    {
        private const string _dbFile = @"Filename=db/data.db";
        private static IEnumerable<string> _urls = File.ReadAllLines(@"db/watches.txt");

        private static WatchPriceCheck PriceCheck(string url)
        {
            var web = new HtmlWeb();
            var data = web.Load(url);
            var name = data.DocumentNode
                .Descendants("h1")
                .FirstOrDefault()
                .InnerText;
            var watchPrice = data.DocumentNode
                .Descendants("span")
                .FirstOrDefault(d => d.Id == "ProductPrice-product-template");
            if (watchPrice == null)
            {
                throw new ApplicationException("Could not find price, probably sold out.");
            }
            var price = watchPrice.Attributes
                .FirstOrDefault(w => w.Name == "content")
                .Value;
            return new WatchPriceCheck
            {
                Name = name,
                Url = url,
                Price = Convert.ToDecimal(price),
                When = DateTime.Now
            };
        }

        public static void UpdatePrices()
        {

            Console.WriteLine("Updating prices...");
            using (var db = new LiteDatabase(_dbFile))
            {
                var prices = db.GetCollection<WatchPriceCheck>("watchPriceCheck");
                foreach (var u in _urls)
                {
                    try
                    {
                        var current = prices.Find(p => p.Url.Equals(u))
                            .OrderByDescending(p => p.When)
                            .FirstOrDefault();
                        var update = PriceCheck(u);
                        prices.Insert(update);
                        if (current != null && current.Price != update.Price)
                        {
                            Console.WriteLine($"{update.Name}\tPrice change: {current.Price} => {update.Price}");
                        }
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("Exception on " + u + ": " + e.Message);
                    }
                }
            }
        }

        public static IEnumerable<WatchPriceCheck> GetPriceSummary()
        {
            using (var db = new LiteDatabase(_dbFile))
            {
                var prices = db.GetCollection<WatchPriceCheck>("watchPriceCheck");
                return prices.FindAll()
                    .OrderByDescending(p => p.When)
                    .Take(30);
            }
        }

        public static IEnumerable<WatchPriceCheck> GetForWatch(string url, DateTime from)
        {
            using (var db = new LiteDatabase(_dbFile))
            {
                var prices = db.GetCollection<WatchPriceCheck>("watchPriceCheck");
                return prices.FindAll()
                    .Where(w => w.Url == url && w.When <= from)
                    .OrderBy(w => w.When);
            }
        }

        public static string ConsoleListPrices()
        {
            using (var db = new LiteDatabase(_dbFile))
            {
                var prices = db.GetCollection<WatchPriceCheck>("watchPriceCheck");
                var data = GetPriceSummary()
                    .OrderBy(p => p.When)
                    .Select(p => $"{p.Price.ToString("C")}\t{p.When.ToShortDateString()}\t{p.Name}");
                return string.Join(Environment.NewLine, data);
            }
        }
    }
}
