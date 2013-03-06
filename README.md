Web-Crawler
===========


0.  Web Crawler Implementation

1.	Download a given webpage from a website
2.	Using appropriate techniques, extract the hyperlinks found on the downloaded page
3.	Store the links in a database
4.	Fetch new links from the database and display in a UI
5.  Continue to crawl the new links found
Notes:
1.	Use multithreading and event handling where it is feasible
2.	The application must compile and run in Visual Studio 2010 or 2012 (must include the data store added to the       project, as well as all necessary libraries and resources)
3.	As a guideline, you should spend maximum 8 hours in total to develop the application


1. Code contains following modules:
1.	Downloader
    a.	This module will download the web page and extract the links from the page using the HtmlAgilityPack DLL.
2.	Crawl WebPage
    a.	This module has information about crawl page
3.	Components 
    a.	Multithreaded component handles the multiple threads to handle the crawling
    b.	Queue component will feed the links to the multiple threads for crawling
4.	Database
    a.	Create one table called ‘crawl’ you can find the create script at ‘Pigo\Database\Create_Table.sql’

Note: Please change the app settings as below:
<appSettings>
    <add key="MaximumThreads" value="10"/>
    <add key="ConnectionString" value="Data Source=localhost;Initial Catalog=Crawler;User       ID=sa;Password=Password!"/>
</appSettings>

Change ConnectionString according to SQL server setup.
Weakness:
  1.	Validations are not properly handled in the code. i.e. validation about web page content and crawling validations
  2.	Store Procedures are not used, Indexing for table is not done
  3.	HTML Parser is not written by own.
Improvement Areas:
  1.	Write own efficient HTML Parser
  2.	Crawl high rank web pages ahead of normal pages
  3.	Write an Algorithm for Re-Visit crawling Policy
  4.	Write Reinforcement Machine learning algorithm for focused crawling using some pre training data.
  5.	 Write URL caching techniques for web crawling.
 

Crawling strategy:
 
Downloader is implemented using Multithreading and Queue techniques. To extract links from the given Page I have used ‘HtmlAgilityPack’ DLL (‘Pigo\Library\HtmlAgilityPack.dll’) 

For saving Links to database I have used SQL Server 2008. Create Database with the name of ‘Crawler’ and use Create table script from the ‘Pigo\Database\Create_Table.sql’

I have put check in the application so that it can’t crawl the same page again. 

GUI will continuously display the links that are queued up for crawling. 


