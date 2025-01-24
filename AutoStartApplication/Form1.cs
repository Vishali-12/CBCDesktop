using AutoStartApplication.APIs;
using AutoStartApplication.Common;
using System;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Timers;

namespace AutoStartApplication
{
    public partial class Form1 : Form
    {
        private NotifyIcon notifyIcon;
        private readonly CheckInternetConnection checkInternetConnection;
        private System.Timers.Timer autoSyncTimer; // Add a timer for periodic sync

        public Form1()
        {
            InitializeComponent();
            checkInternetConnection = new CheckInternetConnection();
            this.Load += new System.EventHandler(this.Form1_Load);
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            ConfigureNotifyIcon();
            ConfigureAutoSyncTimer(); // Configure the auto-sync timer
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
            autoSyncTimer.Start();
        }


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
            HistoryForm historyForm = new HistoryForm();
            historyForm.Show();
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
                if (dateTimePicker1.Value <= DateTime.Now)
                {
                    var data = await syncData.GetData(fromDateTime, toDateTime);
                    if (data != "")
                    {
                        this.UseWaitCursor = false;
                        AutoClosingMessageBox.Show(data, 3);
                        var response = await syncData.AddEmployeesInBiometric();
                        if (response != "")
                        {
                            MessageBox.Show(response);
                        }

                    }
                }
                else
                {
                    this.UseWaitCursor = false;
                    MessageBox.Show("No records found for the selected date.");
                }
            }
            else
            {
                this.UseWaitCursor = false;
                MessageBox.Show("Please Check Your Internet Connetion and try again.");
            }

        }
        private void ConfigureAutoSyncTimer()
        {
            autoSyncTimer = new System.Timers.Timer(300000);// Set interval to 10 minutes (600,000 ms)
            autoSyncTimer = new System.Timers.Timer(300000);// Set interval to 10 minutes (600,000 ms)
            autoSyncTimer.Elapsed += AutoSyncTimer_Elapsed;
            autoSyncTimer.AutoReset = true; // Ensure the timer restarts after each interval
        }
        private async void AutoSyncTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            if (checkInternetConnection.IsConnectedToInternet())
            {
                SyncData syncData = new SyncData();


                string fromDateTime = DateTime.Now.ToString("yyyy-MM-dd");
                string toDateTime = DateTime.Today.AddDays(1).ToString("yyyy-MM-dd");

                //string fromDateTime = DateTime.Now.AddMinutes(-10).ToString("yyyy-MM-dd HH:mm:ss");
                //string toDateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                try
                {
                    var data = await syncData.GetData(fromDateTime, toDateTime);
                    if (!string.IsNullOrEmpty(data))
                    {
                        AutoClosingMessageBox.Show($"Auto-sync completed: {data}", 3);
                        var response = await syncData.AddEmployeesInBiometric();
                        if (!string.IsNullOrEmpty(response))
                        {
                            MessageBox.Show(response);
                        }
                    }
                }
                catch (Exception ex)
                {
                    // Handle any exceptions that occur during the sync process
                    MessageBox.Show($"Error during auto-sync: {ex.Message}");
                }
            }
            else
            {
                MessageBox.Show("Auto-sync failed: No internet connection.");
            }
        }
     
    }

}
public class AutoClosingMessageBox : Form
{
    private Label messageLabel;

    public AutoClosingMessageBox(string message, int durationInSeconds)
    {
        // Initialize the form
        this.Text = "Sync Status";
        this.Size = new Size(300, 150);
        this.StartPosition = FormStartPosition.CenterScreen;
        this.MaximizeBox = false;
        this.MinimizeBox = false;

        // Add a label to display the message
        messageLabel = new Label
        {
            Text = message,
            AutoSize = false,
            TextAlign = ContentAlignment.MiddleCenter,
            Dock = DockStyle.Fill,
            Font = new Font("Arial", 10, FontStyle.Regular),
        };
        this.Controls.Add(messageLabel);

        // Set a timer to close the form after the specified duration
        Task.Delay(durationInSeconds * 1000).ContinueWith(_ => this.Invoke((Action)this.Close));
    }

    public static void Show(string message, int durationInSeconds)
    {
        // Show the auto-closing message box
        var messageBox = new AutoClosingMessageBox(message, durationInSeconds);
        messageBox.ShowDialog();
    }
}