using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Pigo.Crawl_WebPage;

namespace Pigo.Components
{
    public interface IDownloadQueue
    {
        /// <summary>
        /// Count of remaining items that are currently scheduled
        /// </summary>
        int Count { get; }

        /// <summary>
        /// Schedules the param to be crawled
        /// </summary>
        void Add(MainPage page);

        /// <summary>
        /// Gets the next page to crawl
        /// </summary>
        MainPage GetNext();
    }
}
