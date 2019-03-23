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
        private const string _dbFile = "Filename=db/data.db";
        private const string urlFile = "db/watches.txt";
        public static readonly IEnumerable<string> Urls = File.ReadAllLines(urlFile);

        public static string CleanHtmlText(this string value) => value.Replace("\n", "").Replace("$", "").Trim();

        private static WatchPriceCheck PriceCheck(string url)
        {
            var name = string.Empty;
            var price = string.Empty;
            var web = new HtmlWeb();
            var data = web.Load(url);

            if (url.Contains("crownandcaliber", StringComparison.OrdinalIgnoreCase))
            {
                name = data.DocumentNode
                    .Descendants("h1")
                    .FirstOrDefault()?
                    .InnerText;
                var watchPrice = data.DocumentNode
                    .Descendants("span")
                    .FirstOrDefault(d => d.Id == "ProductPrice-product-template");
                if (watchPrice == null)
                {
                    throw new SoldOutException("Could not find price, probably sold out.");
                }
                price = watchPrice.Attributes
                    .FirstOrDefault(w => w.Name == "content")?
                    .Value;
            }
            else if (url.Contains("chrono24", StringComparison.OrdinalIgnoreCase))
            {
                name = data.DocumentNode
                    .Descendants("h1")
                    .FirstOrDefault()?
                    .InnerText
                    .CleanHtmlText();
                var watchPrice = data.DocumentNode
                    .Descendants("span")
                    .ToArray()[29];
                if (watchPrice == null)
                {
                    throw new SoldOutException("Could not find price, probably sold out.");
                }
                price = watchPrice
                    .InnerText
                    .CleanHtmlText();
            }

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
            var urls = Urls.ToList();
            using (var db = new LiteDatabase(_dbFile))
            {
                var prices = db.GetCollection<WatchPriceCheck>("watchPriceCheck");
                foreach (var u in urls)
                {
                    try
                    {
                        var current = prices.Find(p => p.Url.Equals(u))
                            .OrderByDescending(p => p.When)
                            .FirstOrDefault();
                        if (current == null || current.When.ToShortDateString() != DateTime.Now.ToShortDateString())
                        {
                            var update = PriceCheck(u);
                            prices.Insert(update);
                            if (current != null && current.Price != update.Price)
                            {
                                Console.WriteLine($"{update.Name}\tPrice change: {current.Price} => {update.Price}");
                            }
                        }
                    }
                    catch (SoldOutException)
                    {
                        urls.RemoveAll(x => x == u);
                        File.WriteAllLines(urlFile, urls);
                        throw;
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
                    .OrderByDescending(p => p.When);
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
