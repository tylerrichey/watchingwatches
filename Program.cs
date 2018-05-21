using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace WatchingWatches
{
    public static class Program
    {
        private static Timer _timer = new Timer(TimerTask, null, TimeSpan.FromMilliseconds(0), TimeSpan.FromDays(1));
        public static void Main(string[] args)
        {
            BuildWebHost(args).Run();
        }

        private static void TimerTask(Object stateInfo = null) => CheckWatches.UpdatePrices();

        public static IWebHost BuildWebHost(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseConfiguration(new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("appsettings.json", optional: false)
                    .Build())
                .UseStartup<Startup>()
                .Build();
    }
}
