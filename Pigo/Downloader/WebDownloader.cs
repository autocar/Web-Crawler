using System;
using System.Collections.Generic;
using Pigo.Components;
using Pigo.Crawl_WebPage;
using System.Net;
using System.IO;
using HtmlAgilityPack;
using System.Windows.Forms;
using Pigo.Database;
using System.Threading;
using System.Configuration;

namespace Pigo.Downloader
{
    public class WebDownloader
    {
        #region variables

        bool _crawlFinished = false;
        MultiThreader _threader;
        DownloadQueue _queue;        
        CrawlDAO _crawlDAO = null;
        Form1 myFormControl1;
        
        #endregion

        #region C'tor

        public WebDownloader(Form1 form)
        {
            _threader = new MultiThreader(Convert.ToInt32(ConfigurationManager.AppSettings["MaximumThreads"]));
            _queue = new DownloadQueue();
            _crawlDAO = new CrawlDAO();
            myFormControl1 = form;
        }

        #endregion

        #region Methods

        #region Crawling

        public void CrawlMainPage(Uri uri)
        {
            if (uri == null)
                throw new ArgumentNullException("uri");
                        
            _crawlFinished = false;
            
            _crawlDAO.insertLinksToDB(uri.ToString(), 0, 0);

            _queue.Add(new MainPage(uri) { ParentUriID = 1 });

            CrawlSite();            
        }

        private void CrawlSite()
        {
            while (!_crawlFinished)
            {
                if (_queue.Count > 0)
                {
                    _threader.Action(() => CrawlPage(_queue.GetNext()));
                }
                else if (!_threader.HasRunningThreads())
                {
                    _crawlFinished = true;
                }
                else
                {                    
                    System.Threading.Thread.Sleep(2500);
                }
            }

            Cursor.Current = Cursors.Default;
            MessageBox.Show("Crawling completed !!!");
        }
        
        private void CrawlPage(MainPage MainPage)
        {
            if (MainPage == null)
                return;

            CrawledPage crawledPage = DownloadPage(MainPage.Uri);
          
            crawledPage.ParentUriID = MainPage.ParentUriID;
           
            IEnumerable<Uri> crawledPageLinks = GetLinks(crawledPage.Uri, crawledPage.RawContent);
                        
            foreach (Uri uri in crawledPageLinks)
            {
                if (!_crawlDAO.isURLCrawled(uri.ToString().ToLower()))
                    _crawlDAO.insertLinksToDB(uri.ToString().ToLower(), 0, crawledPage.ParentUriID);   
            }
            
            _crawlDAO.updateURLToCrawled(crawledPage.Uri.ToString());

            CrawlContinuesPage();
        }

        private void CrawlContinuesPage()
        {
            List<MainPage> pageList = _crawlDAO.getLinksFromDB();
            foreach (var item in pageList)
            {                
                Thread.Sleep(400);
                myFormControl1.Invoke(myFormControl1.myDelegate,new Object[] {item.Uri.ToString() + "\n"});

                _queue.Add(item);
            }           
        }

        #endregion

        #region HTML Parser

        /// <summary>
        /// Make an http web request to the url and download its content based on the param func decision
        /// </summary>
        public CrawledPage DownloadPage(Uri uri)
        {
            if (uri == null)
                throw new ArgumentNullException("uri");

            CrawledPage crawledPage = new CrawledPage(uri);

            HttpWebRequest request = null;
            HttpWebResponse response = null;
            try
            {
                request = (HttpWebRequest)WebRequest.Create(uri);
                request.AllowAutoRedirect = true;
                request.MaximumAutomaticRedirections = 5;                
                request.Accept = "*/*";
                response = (HttpWebResponse)request.GetResponse();

                crawledPage.HttpWebRequest = request;

                if (response != null)
                {
                    crawledPage.HttpWebResponse = response;

                    string rawHtml = GetRawHtml(response, uri);
                    if (!string.IsNullOrWhiteSpace(rawHtml))
                        crawledPage.RawContent = rawHtml;

                    response.Close();
                }
            }
            catch (WebException e)
            {
                crawledPage.WebException = e;

                if (e.Response != null)
                    response = (HttpWebResponse)e.Response;
                
            }            

            return crawledPage;
        }

        /// <summary>
        /// Parses html to extract anchor and area tag href values
        /// </summary>
        public IEnumerable<Uri> GetLinks(Uri pageUri, string pageHtml)
        {
            if (pageUri == null)
                throw new ArgumentNullException("pageUri");

            if (pageHtml == null)
                throw new ArgumentNullException("pageHtml");

            HtmlAgilityPack.HtmlDocument htmlDoc = new HtmlAgilityPack.HtmlDocument();
            htmlDoc.LoadHtml(pageHtml);

            HtmlNodeCollection aTags = htmlDoc.DocumentNode.SelectNodes("//a[@href]");
            HtmlNodeCollection areaTags = htmlDoc.DocumentNode.SelectNodes("//area[@href]");

            Uri uriToUse = pageUri;

            //If html base tag exists use it instead of page uri for relative links
            string baseHref = GetBaseTagHref(htmlDoc);
            if (!string.IsNullOrEmpty(baseHref))
            {
                try
                {
                    uriToUse = new Uri(baseHref);
                }
                catch { }
            }

            List<Uri> hyperlinks = ExtractHref(aTags, uriToUse);
            hyperlinks.AddRange(ExtractHref(areaTags, uriToUse));

            return hyperlinks;
        }

        private List<Uri> ExtractHref(HtmlNodeCollection nodes, Uri page)
        {
            List<Uri> uris = new List<Uri>();

            if (nodes == null)
                return uris;

            string hrefValue = "";
            foreach (HtmlNode node in nodes)
            {
                hrefValue = node.Attributes["href"].Value.ToLower();

                try
                {
                    Uri newUri = new Uri(page, hrefValue.Split('#')[0]);
                    if (!uris.Contains(newUri))
                        uris.Add(newUri);
                }
                catch (Exception e)
                {
                    MessageBox.Show("Could not parse link", e.Message);
                }
            }

            return uris;
        }

        private string GetBaseTagHref(HtmlAgilityPack.HtmlDocument doc)
        {
            string hrefValue = "";
            HtmlNode node = doc.DocumentNode.SelectSingleNode("//base");

            //Must use node.InnerHtml instead of node.InnerText since "aaa<br />bbb" will be returned as "aaabbb"
            if (node != null)
                hrefValue = node.GetAttributeValue("href", "").Trim();

            return hrefValue;
        }

        protected virtual string GetRawHtml(HttpWebResponse response, Uri requestUri)
        {
            string rawHtml = "";
            try
            {
                using (StreamReader sr = new StreamReader(response.GetResponseStream()))
                {
                    rawHtml = sr.ReadToEnd();
                    sr.Close();
                }
            }
            catch (Exception e)
            {
                
            }

            return rawHtml;
        }

        #endregion

        #endregion

    }
}
