using System;
using System.Collections.Generic;
using Pigo.Crawl_WebPage;

namespace Pigo.Components
{
    public class DownloadQueue
    {
        Queue<MainPage> _pagesToCrawl = new Queue<MainPage>();
        Object locker = new Object();

        /// <summary>
        /// Count of remaining items that are currently scheduled
        /// </summary>
        public int Count
        {
            get
            {
                lock (locker)
                {
                    return _pagesToCrawl.Count;
                }
            }
        }

        /// <summary>
        /// Schedules the param to be crawled in a FIFO fashion
        /// </summary>
        public void Add(MainPage page)
        {
            if (page == null)
                throw new ArgumentNullException("page");

            lock (locker)
            {
                _pagesToCrawl.Enqueue(page);
            }
        }

        /// <summary>
        /// Gets the next page to crawl
        /// </summary>
        public MainPage GetNext()
        {
            MainPage nextItem = null;
            lock (locker)
            {
                if (_pagesToCrawl.Count > 0)
                    nextItem = _pagesToCrawl.Dequeue();
            }

            return nextItem;
        }
    }
}
