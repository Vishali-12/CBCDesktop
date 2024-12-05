namespace AutoStartApplication
{
    partial class Form1
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.syncbtn = new System.Windows.Forms.Button();
            this.dateTimePicker1 = new System.Windows.Forms.DateTimePicker();
            this.btnSync = new System.Windows.Forms.Button();
            this.btnHistory = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // syncbtn
            // 
            this.syncbtn.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.syncbtn.Location = new System.Drawing.Point(150, 223);
            this.syncbtn.Name = "syncbtn";
            this.syncbtn.Size = new System.Drawing.Size(150, 32);
            this.syncbtn.TabIndex = 0;
            this.syncbtn.Text = "Sync\r\n";
            this.syncbtn.UseVisualStyleBackColor = true;
            this.syncbtn.Click += new System.EventHandler(this.syncbtn_Click);
            // 
            // dateTimePicker1
            // 
            this.dateTimePicker1.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.dateTimePicker1.Location = new System.Drawing.Point(128, 149);
            this.dateTimePicker1.Name = "dateTimePicker1";
            this.dateTimePicker1.Size = new System.Drawing.Size(202, 24);
            this.dateTimePicker1.TabIndex = 1;
            this.dateTimePicker1.TabStop = false;
            // 
            // btnSync
            // 
            this.btnSync.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnSync.Location = new System.Drawing.Point(91, 52);
            this.btnSync.Name = "btnSync";
            this.btnSync.Size = new System.Drawing.Size(136, 37);
            this.btnSync.TabIndex = 2;
            this.btnSync.Text = "Sync";
            this.btnSync.UseVisualStyleBackColor = true;
            this.btnSync.Click += new System.EventHandler(this.btnSync_Click);
            // 
            // btnHistory
            // 
            this.btnHistory.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnHistory.Location = new System.Drawing.Point(226, 52);
            this.btnHistory.Name = "btnHistory";
            this.btnHistory.Size = new System.Drawing.Size(135, 37);
            this.btnHistory.TabIndex = 3;
            this.btnHistory.Text = "History";
            this.btnHistory.UseVisualStyleBackColor = true;
            this.btnHistory.Click += new System.EventHandler(this.btnHistory_Click);
            // 
            // Form1
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.ClientSize = new System.Drawing.Size(443, 345);
            this.Controls.Add(this.btnHistory);
            this.Controls.Add(this.btnSync);
            this.Controls.Add(this.dateTimePicker1);
            this.Controls.Add(this.syncbtn);
            this.MaximizeBox = false;
            this.Name = "Form1";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Form1";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button syncbtn;
        private System.Windows.Forms.Button btnSync;
        private System.Windows.Forms.Button btnHistory;
        private System.Windows.Forms.DateTimePicker dateTimePicker1;
    }
}

