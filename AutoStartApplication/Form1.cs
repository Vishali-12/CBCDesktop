
using Microsoft.Win32;
using System;
using System.Windows.Forms;

namespace AutoStartApplication
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            this.Load += new System.EventHandler(this.Form1_Load);
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
