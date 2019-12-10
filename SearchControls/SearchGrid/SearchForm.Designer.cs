namespace SearchControls
{
    partial class SearchForm
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle5 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle6 = new System.Windows.Forms.DataGridViewCellStyle();
            this.SearchGrid = new SearchControls.SearchGrid();
            ((System.ComponentModel.ISupportInitialize)(this.SearchGrid)).BeginInit();
            this.SuspendLayout();
            // 
            // SearchGrid
            // 
            this.SearchGrid.AllowUserToAddRows = false;
            this.SearchGrid.AllowUserToDeleteRows = false;
            this.SearchGrid.AllowUserToOrderColumns = true;
            this.SearchGrid.AllowUserToResizeColumns = false;
            this.SearchGrid.AllowUserToResizeRows = false;
            dataGridViewCellStyle5.BackColor = System.Drawing.Color.White;
            this.SearchGrid.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle5;
            this.SearchGrid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.SearchGrid.ColumnHeadersVisible = false;
            this.SearchGrid.Dock = System.Windows.Forms.DockStyle.Fill;
            this.SearchGrid.Location = new System.Drawing.Point(0, 0);
            this.SearchGrid.MultiSelect = false;
            this.SearchGrid.Name = "SearchGrid";
            this.SearchGrid.ReadOnly = true;
            this.SearchGrid.RowHeadersVisible = false;
            dataGridViewCellStyle6.BackColor = System.Drawing.Color.PowderBlue;
            this.SearchGrid.RowsDefaultCellStyle = dataGridViewCellStyle6;
            this.SearchGrid.RowTemplate.Height = 23;
            this.SearchGrid.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.SearchGrid.Size = new System.Drawing.Size(284, 261);
            this.SearchGrid.TabIndex = 0;
            // 
            // SearchForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(284, 261);
            this.Controls.Add(this.SearchGrid);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "SearchForm";
            this.ShowInTaskbar = false;
            this.Text = "SearchForm";
            ((System.ComponentModel.ISupportInitialize)(this.SearchGrid)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        /// <summary>
        /// 模糊查找的小表
        /// </summary>
        internal SearchControls.SearchGrid SearchGrid;
    }
}