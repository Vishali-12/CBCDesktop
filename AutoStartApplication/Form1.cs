using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
            // Add the application to startup
            AddToStartup();
            MessageBox.Show("Application has started!");
        }

        private void AddToStartup()
        {
            try
            {
                string appName = "AutoStartApp"; // Name of the application in the registry
                string exePath = Application.ExecutablePath; // Full path to the executable

                // Open the registry key for the current user's startup programs
                RegistryKey registryKey = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);

                // Add or update the registry entry
                registryKey.SetValue(appName, exePath);

                MessageBox.Show("Application is set to run at startup.");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to set startup: " + ex.Message);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Button click!");
        }
    }
}
