namespace Example
{
    partial class searchDataGridView
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            dataGridView1 = new System.Windows.Forms.DataGridView();
            textBox1 = new System.Windows.Forms.TextBox();
            label1 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)dataGridView1).BeginInit();
            SuspendLayout();
            // 
            // dataGridView1
            // 
            dataGridView1.AllowUserToResizeRows = false;
            dataGridViewCellStyle1.BackColor = System.Drawing.Color.White;
            dataGridView1.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle1;
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridView1.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle2;
            dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridView1.Dock = System.Windows.Forms.DockStyle.Right;
            dataGridView1.Location = new System.Drawing.Point(350, 0);
            dataGridView1.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            dataGridView1.MultiSelect = false;
            dataGridView1.Name = "dataGridView1";
            dataGridView1.RowHeadersVisible = false;
            dataGridView1.RowHeadersWidth = 62;
            dataGridViewCellStyle3.BackColor = System.Drawing.Color.PowderBlue;
            dataGridView1.RowsDefaultCellStyle = dataGridViewCellStyle3;
            dataGridView1.RowTemplate.Height = 23;
            dataGridView1.Size = new System.Drawing.Size(741, 438);
            dataGridView1.TabIndex = 0;
            // 
            // textBox1
            // 
            textBox1.Location = new System.Drawing.Point(92, 370);
            textBox1.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            textBox1.Name = "textBox1";
            textBox1.Size = new System.Drawing.Size(244, 30);
            textBox1.TabIndex = 1;
            textBox1.TextChanged += TextBox1_TextChanged;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new System.Drawing.Point(28, 364);
            label1.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            label1.Name = "label1";
            label1.Size = new System.Drawing.Size(46, 48);
            label1.TabIndex = 2;
            label1.Text = "模糊\r\n查找";
            // 
            // searchDataGridView
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(144F, 144F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            ClientSize = new System.Drawing.Size(1091, 438);
            Controls.Add(label1);
            Controls.Add(textBox1);
            Controls.Add(dataGridView1);
            Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            Name = "searchDataGridView";
            Text = "SearchDataGridView";
            ((System.ComponentModel.ISupportInitialize)dataGridView1).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.Label label1;
    }
}