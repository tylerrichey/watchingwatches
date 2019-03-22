using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WatchingWatches
{
    public class SoldOutException : Exception
    {
        public SoldOutException() : base()
        {
        }

        public SoldOutException(string message) : base(message)
        {
        }

        public SoldOutException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
