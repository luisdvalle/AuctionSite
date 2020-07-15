using System;
using System.Threading.Tasks;

namespace AuctionApp.Common.Util
{
    public class AsyncLazy<T> : Lazy<Task<T>>
    {
        public AsyncLazy(Func<Task<T>> taskFactory) : base(() => Task.Run(taskFactory)) { }
    }
}
