using System;
using System.Windows.Forms;
using Pigo.Downloader;
using System.Threading;

namespace Pigo
{
    public partial class Form1 : Form
    {
        #region variables

        public delegate void AddURLItem(String myString);

        public AddURLItem myDelegate;

        private Thread myThread;

        #endregion

        #region C'tor
        public Form1()
        {
            InitializeComponent();

            myDelegate = new AddURLItem(AddURLMethod);
        }

        #endregion

        #region Methods

        public void AddURLMethod(String myString)
        {
            rtbResult.AppendText(myString + "\n");
        }

        private void ThreadFunction()
        {
            WebDownloader myThreadClassObject = new WebDownloader(this);
            myThreadClassObject.CrawlMainPage(new Uri(tbWebsite.Text));           
        }

        #endregion

        #region Events

        private void button1_Click(object sender, EventArgs e)
        {
            Cursor.Current = Cursors.WaitCursor;
            button1.Enabled = false;

            myThread = new Thread(new ThreadStart(ThreadFunction));
            myThread.Start();
        }

        #endregion
    }
}
