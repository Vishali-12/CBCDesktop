using AutoStartApplication.APIs;
using System;
using System.Windows.Forms;

namespace AutoStartApplication
{
    public partial class HistoryForm : Form
    {
        
        public HistoryForm()
        {
            InitializeComponent();
            this.Load += new System.EventHandler(this.HistoryForm_Load);
        }

        private void HistoryForm_Load(object sender, EventArgs e)
        {
            SyncData syncData = new SyncData();
            var data = syncData.GetAttendanceLogHistory();
            dataGridView1.DataSource = data.Result;
            dataGridView1.ReadOnly = true;
            dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            // Set the font for the header to Bold and Font size to 12
            dataGridView1.ColumnHeadersDefaultCellStyle.Font = new System.Drawing.Font("Arial", 12, System.Drawing.FontStyle.Bold);
            dataGridView1.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            // Set the font for the cells to size 11 and center-align the text
            dataGridView1.DefaultCellStyle.Font = new System.Drawing.Font("Arial", 11);
            dataGridView1.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            foreach (DataGridViewColumn column in dataGridView1.Columns)
            {
                column.HeaderText = column.HeaderText.ToUpper();
            }
          
        }


        private void btnBack_Click(object sender, EventArgs e)
        {
            this.Hide();
            // Create and show Form2
            Form1 form1 = new Form1();
            form1.Show();
        }
    }
}
