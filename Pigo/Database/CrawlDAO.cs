using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Windows.Forms;
using Pigo.Crawl_WebPage;
using System.Configuration;

namespace Pigo.Database
{
    public class CrawlDAO
    {
        //string connetionString = "Data Source=localhost;Initial Catalog=Crawler;User ID=sa;Password=Password!";
        string connetionString = ConfigurationManager.AppSettings["ConnectionString"];

        #region Save Links to Database

        public void insertLinksToDB(string url, int isCrawled, int ParentUrlID)
        {
            const string query = "insert into crawl (URL, ParentUrlID, isCrawled) Values (@url, @ParentUrlID, @isCrawled)";

            try
            {
                using (SqlConnection conn = new SqlConnection(connetionString))
                {
                    conn.Open();
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.Add(new SqlParameter("@url", url));
                        cmd.Parameters.Add(new SqlParameter("@isCrawled", isCrawled));
                        cmd.Parameters.Add(new SqlParameter("@ParentUrlID", ParentUrlID));
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Can not open connection ! ");
            }
        }

        #endregion

        #region Retrieve Links from the Database

        public List<MainPage> getLinksFromDB()
        {
            List<MainPage> pageList = new List<MainPage>();       
            SqlDataReader reader;
            string sql = "select * from crawl with (nolock) where isCrawled = 'false'";          

            try
            {
                using (SqlConnection conn = new SqlConnection(connetionString))
                {
                    conn.Open();
                    using (SqlCommand command = new SqlCommand(sql, conn))
                    {
                        reader = command.ExecuteReader();

                        while (reader.Read())
                        {
                            MainPage _page = new MainPage(new Uri( reader["URL"].ToString()));
                            _page.ParentUriID = Convert.ToInt32(reader["UrlID"].ToString());

                            pageList.Add(_page);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Can not open connection ! ");
            }

            return pageList;
        }

        public bool isURLCrawled(string url)
        {
            const string query = "select COUNT(*) as Url_Count from crawl where URL = @url";
            
            try
            {
                using (SqlConnection conn = new SqlConnection(connetionString))
                {
                    conn.Open();
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.Add(new SqlParameter("@url", url));
                        SqlDataReader reader = cmd.ExecuteReader();

                        if (reader.Read() == true)
                        {
                            if (Convert.ToInt32(reader["Url_Count"].ToString()) > 0)
                                return true;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Can not open connection ! ");
            }

            return false;
        }


        public void updateURLToCrawled(string url)
        {
            const string query = "update crawl set isCrawled = 1 where URL = @url";

            try
            {
                using (SqlConnection conn = new SqlConnection(connetionString))
                {
                    conn.Open();
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.Add(new SqlParameter("@url", url));                       
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Can not open connection ! ");
            }
        }

        #endregion

    }
}
