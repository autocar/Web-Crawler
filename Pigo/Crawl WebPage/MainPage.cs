using System;

namespace Pigo.Crawl_WebPage
{
    //PageToCrawl
    public class MainPage
    {
        public MainPage(Uri uri)
        {
            if (uri == null)
                throw new ArgumentNullException("uri");

            Uri = uri;
        }

        /// <summary>
        /// The uri of the page
        /// </summary>
        public Uri Uri { get; set; }

        /// <summary>
        /// The parent uri ID of the page
        /// </summary>
        public int ParentUriID { get; set; }

        public bool isCrawled { get; set; }    

        public override string ToString()
        {
            return Uri.AbsoluteUri;
        }
    }
}
