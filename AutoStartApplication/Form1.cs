using AutoStartApplication.APIs;
using AutoStartApplication.Common;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace AutoStartApplication
{
    public partial class Form1 : Form
    {
        private NotifyIcon notifyIcon;
        private readonly CheckInternetConnection checkInternetConnection;

        public Form1()
        {
            InitializeComponent();
            checkInternetConnection = new CheckInternetConnection();    
            this.Load += new System.EventHandler(this.Form1_Load);
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            ConfigureNotifyIcon();
        }

        private void ConfigureNotifyIcon()
        { // Create and configure a NotifyIcon
            notifyIcon = new NotifyIcon()
            {
                Icon = SystemIcons.Information,
                Text = "Sample NotifyIcon App",
                Visible = true
            };
            ContextMenuStrip contextMenu = new ContextMenuStrip();
            ToolStripMenuItem exitMenuItem = new ToolStripMenuItem("Exit");
            exitMenuItem.Click += ExitMenuItem_Click;
            contextMenu.Items.Add(exitMenuItem);
            notifyIcon.ContextMenuStrip = contextMenu;
            notifyIcon.BalloonTipTitle = "Welcome";
            notifyIcon.BalloonTipText = "This application is running in the system tray.";
            notifyIcon.ShowBalloonTip(3000);
        }

        private void ExitMenuItem_Click(object sender, EventArgs e)
        {
            // Exit the application
            notifyIcon.Visible = false; // Hide the NotifyIcon before exiting
            Application.Exit();
        }

        private async void Form1_Load(object sender, EventArgs e)
        {
            //this.UseWaitCursor = true;
            ////AddToStartup();
            //if (checkInternetConnection.IsConnectedToInternet())
            //{
            //    SyncData syncData = new SyncData();
            //    DateTime yesterdayDate = DateTime.Today.AddDays(-1);
            //    string fromDateTime = yesterdayDate.ToString("yyyy-MM-dd");
            //    string toDateTime = DateTime.Now.ToString("yyyy-MM-dd");
            //    var data = await syncData.GetData(fromDateTime, toDateTime);
            //    if (data != "")
            //    {
            //        this.UseWaitCursor = false;
            //        MessageBox.Show(data);
            //    }
            //}
            //else
            //{
            //    this.UseWaitCursor = false;
            //    MessageBox.Show("Please Check Your Internet connetion");
            //}
        }

        #region AddToStartup code which is not in Use.
        //private void AddToStartup()
        //{
        //    try
        //    {
        //        string appName = "AutoStartApp";
        //        string exePath = Application.ExecutablePath;

        //        RegistryKey registryKey = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);

        //        registryKey.SetValue(appName, exePath);

        //    }
        //    catch (Exception ex)
        //    {
        //        MessageBox.Show("Failed to set startup: " + ex.Message);
        //    }
        //} 
        #endregion

        private void btnSync_Click(object sender, EventArgs e)
        {
           
        }

        /// <summary>
        /// History Button Click :- Open new screen and display the sync data history, including the date and status.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnHistory_Click(object sender, EventArgs e)
        {
            this.UseWaitCursor = true;
            HistoryForm historyForm = new HistoryForm();
            historyForm.Show();
            this.UseWaitCursor = false;
            this.Hide();
        }

        /// <summary>
        /// Sync Button Click :- Manually sync data by fetching it from one API and posting it to another API.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void syncbtn_Click(object sender, EventArgs e)
        {
            this.UseWaitCursor = true;
            if (checkInternetConnection.IsConnectedToInternet())
            {
                SyncData syncData = new SyncData();
                string fromDateTime = dateTimePicker1.Value.ToString("yyyy-MM-dd");
                string toDateTime = dateTimePicker1.Value.AddDays(1).ToString("yyyy-MM-dd");
                var data = await syncData.GetData(fromDateTime, toDateTime);
                if (data !="")
                {
                    this.UseWaitCursor = false;
                    MessageBox.Show(data);
                }
            }
            else
            {
                this.UseWaitCursor = false;
                MessageBox.Show("Please Check Your Internet Connetion and try again.");
            }

        }

    }
}
