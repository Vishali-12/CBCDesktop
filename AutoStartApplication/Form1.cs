
using AutoStartApplication.APIs;
using Microsoft.Win32;
using System;
using System.Drawing;
using System.Net.NetworkInformation;
using System.Windows.Forms;

namespace AutoStartApplication
{
    public partial class Form1 : Form
    {
        private NotifyIcon notifyIcon;

        public Form1()
        {
            InitializeComponent();
            this.Load += new System.EventHandler(this.Form1_Load);
            this.Text = "Sample Windows Form";
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

        private void Form1_Load(object sender, EventArgs e)
        {
            AddToStartup();
            if (IsConnectedToInternet())
            {
                SyncData syncData = new SyncData();
                DateTime yesterdayDate = DateTime.Today.AddDays(-1);
                string fromDateTime = yesterdayDate.ToString("yyyy-MM-dd");
                string toDateTime = DateTime.Now.ToString("yyyy-MM-dd");
                var data = syncData.GetData(fromDateTime, toDateTime);
            }
            else
            {
                MessageBox.Show("Please Check Your Internet connetion");
            }
        }

        private void AddToStartup()
        {
            try
            {
                string appName = "AutoStartApp";
                string exePath = Application.ExecutablePath;

                RegistryKey registryKey = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);

                registryKey.SetValue(appName, exePath);

            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to set startup: " + ex.Message);
            }
        }

        private void btnSync_Click(object sender, EventArgs e)
        {

            MessageBox.Show("Home page opened");
        }

        /// <summary>
        /// History Button Click :- Open new screen and display the sync data history, including the date and status.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnHistory_Click(object sender, EventArgs e)
        {
            this.Hide();
            HistoryForm historyForm = new HistoryForm();
            historyForm.Show();

        }

        /// <summary>
        /// Sync Button Click :- Manually sync data by fetching it from one API and posting it to another API.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void syncbtn_Click(object sender, EventArgs e)
        {
            if (IsConnectedToInternet())
            {
                SyncData syncData = new SyncData();
                string fromDateTime = dateTimePicker1.Value.ToString("yyyy-MM-dd");
                string toDateTime = dateTimePicker1.Value.AddDays(1).ToString("yyyy-MM-dd");
                var data = syncData.GetData(fromDateTime, toDateTime);
                if (data != null)
                {
                    MessageBox.Show("Data Synced Successfully.");
                }
            }
            else
            {
                MessageBox.Show("Please Check Your Internet Connetion and try again.");
            }

        }

        /// <summary>
        /// Check Internet Connection
        /// </summary>
        /// <returns></returns>
        public bool IsConnectedToInternet()
        {
            string host = "www.google.com"; // Use a valid hostname
            int timeout = 3000; // Timeout in milliseconds
            Ping p = new Ping();
            try
            {
                PingReply reply = p.Send(host, timeout);
                if (reply.Status == IPStatus.Success)
                {
                    return true;
                }
            }
            catch
            {
                // Log exception if needed
            }
            return false;
        }

    }
}
