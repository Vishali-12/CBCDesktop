
using Microsoft.Win32;
using System;
using System.Drawing;
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
        private void Button_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Hello, world!", "Message");
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
            //MessageBox.Show("Application has started!");
        }

        private void AddToStartup()
        {
            try
            {
                string appName = "AutoStartApp";
                string exePath = Application.ExecutablePath; 

                RegistryKey registryKey = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);

                registryKey.SetValue(appName, exePath);

                //MessageBox.Show("Application is set to run at startup.");
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

        private void btnHistory_Click(object sender, EventArgs e)
        {
            this.Hide();
            HistoryForm historyForm = new HistoryForm();
            historyForm.Show();

        }

        private void syncbtn_Click(object sender, EventArgs e)
        {
            MessageBox.Show("sync functionality");
        }
    }
}
