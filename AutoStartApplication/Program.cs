using AutoStartApplication.APIs;
using AutoStartApplication.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AutoStartApplication
{
    internal static class Program
    {
        public static CheckInternetConnection checkInternetConnection;
        private static Mutex mutex = null;
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            checkInternetConnection = new CheckInternetConnection();
            const string appName = "AutoStartApplication";
            bool isNewInstance;

            // Create a mutex with a unique name
            mutex = new Mutex(true, appName, out isNewInstance);

            // Check if this is a new instance of the application
            if (!isNewInstance)
            {
                return; // Exit the application
            }

            try
            {
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);

                // Perform the sync logic before showing the form
                RunSyncOperationAsync().GetAwaiter().GetResult();

                // Start the main form
                Application.Run(new Form1());
            }
            finally
            {
                // Release the mutex
                if (mutex != null)
                {
                    mutex.ReleaseMutex();
                    mutex = null;
                }
            }

            #region without any condition code
            //Application.EnableVisualStyles();
            //Application.SetCompatibleTextRenderingDefault(false);

            //// Perform the sync logic before showing the form
            //RunSyncOperationAsync().GetAwaiter().GetResult();

            //// Start the application with the main form
            //Application.Run(new Form1()); 
            #endregion
        }

        private static async Task RunSyncOperationAsync()
        {
            if (checkInternetConnection.IsConnectedToInternet())
            {
                SyncData syncData = new SyncData();

                DateTime yesterdayDate = DateTime.Today.AddDays(-1);
                string fromDateTime = yesterdayDate.ToString("yyyy-MM-dd");
                string toDateTime = DateTime.Now.ToString("yyyy-MM-dd");

                var data = await syncData.GetData(fromDateTime, toDateTime);

                if (!string.IsNullOrEmpty(data))
                {
                    MessageBox.Show(data, "Sync Status", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            else
            {
                MessageBox.Show("Please Check Your Internet Connection", "No Internet", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
    }
}
