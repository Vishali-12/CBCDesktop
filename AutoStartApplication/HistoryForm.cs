using System;
using System.Data;
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
            BindData();
        }

        public void BindData()
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("ID", typeof(int));
            dt.Columns.Add("Name", typeof(string));
            dt.Columns.Add("Age", typeof(int));

            // Add some rows
            dt.Rows.Add(1, "John", 25);
            dt.Rows.Add(2, "Alice", 30);
            dt.Rows.Add(3, "Bob", 28);
            dataGridView1.DataSource=dt;
            dataGridView1.ReadOnly = true;  
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
