using AutoStartApplication.APIs;
using AutoStartApplication.Common;
using AutoStartApplication.Models;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
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
                // Set up global exception handlers
                Application.ThreadException += Application_ThreadException;
                AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;

                Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);

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
                DateTime yesterdayDate;
                SyncData syncData = new SyncData();
                var history = await syncData.GetAttendanceLogHistory();
                var unsyncedDates = history.Where(x=> x.status == "No").ToList();

                foreach (var rec in unsyncedDates)
                {
                    var toDate = DateTime.Parse(rec.date).AddDays(1).ToString("yyyy-MM-dd") ;
                    var result = await syncData.GetData(rec.date, toDate);
                }

                 yesterdayDate = DateTime.Today.AddDays(-1);
            
                string fromDateTime = yesterdayDate.ToString("yyyy-MM-dd");
                string toDateTime = DateTime.Now.ToString("yyyy-MM-dd");

                var data = await syncData.GetData(fromDateTime, toDateTime);

                if (!string.IsNullOrEmpty(data))
                {
                    AutoClosingMessageBox.Show(data, 3);
                    var response = await syncData.AddEmployeesInBiometric();
                    if (response != "")
                    {
                        MessageBox.Show(response);
                    }
                    //MessageBox.Show(data, "Sync Status", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            else
            {
                MessageBox.Show("Please Check Your Internet Connection", "No Internet", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private async static void Application_ThreadException(object sender, System.Threading.ThreadExceptionEventArgs e)
        {
            // Handle UI thread exceptions
            var exception = e.Exception;

            await LogExceptionToAirtable(new ExcetionViewModel
            {
                Message = string.IsNullOrEmpty(exception.Message) ? exception.InnerException?.Message : exception.Message,
                OccuredOn = DateTime.Now,
                InnerException = exception?.InnerException?.Message,
                StackTrace = exception?.StackTrace
            });
        }

        private async static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {

            // Handle non-UI thread exceptions
            Exception exception = e.ExceptionObject as Exception;
            //var _exceptionMiddleware = new ExceptionMiddleware();

            await LogExceptionToAirtable(new ExcetionViewModel
            {
                Message = string.IsNullOrEmpty(exception.Message) ? exception.InnerException.Message : exception.Message,
                InnerException = exception?.InnerException?.Message,
                OccuredOn = DateTime.Now,
                StackTrace = exception?.StackTrace
            });


        }


        private static async Task LogExceptionToAirtable(ExcetionViewModel excetionViewModel)
        {
            try
            {
                using (var httpClient = new HttpClient())
                {
                    httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", "patDCtokqJzRG1WGb.b4f7f8bb7e922e1a7138549c22470f6e90d0fc126aedae281e76f8731d22e063");

                    var model = new
                    {
                        fields = excetionViewModel
                    };

                    var data = new StringContent(JsonConvert.SerializeObject(model), Encoding.UTF8, "application/json");

                    var httpResponse = httpClient.PostAsync("https://api.airtable.com/v0/appg954VRZyhntmCb/tbl72RY4aWyt9KDvY", data).Result;

                    var data1 = httpResponse.Content.ReadAsStringAsync();
                    MessageBox.Show(excetionViewModel.Message);
                }
            }
            catch (Exception)
            {

                throw;
            }

        }
    }
}
