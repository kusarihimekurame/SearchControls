﻿namespace Example
{
    partial class searchTextBox
    {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要修改
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.searchTextBox1 = new SearchControls.SearchTextBox();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // searchTextBox1
            // 
            this.searchTextBox1.AutoInputDataName = null;
            this.searchTextBox1.DataMember = null;
            this.searchTextBox1.DisplayDataName = null;
            this.searchTextBox1.Location = new System.Drawing.Point(14, 17);
            this.searchTextBox1.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.searchTextBox1.Name = "searchTextBox1";
            this.searchTextBox1.Size = new System.Drawing.Size(116, 23);
            this.searchTextBox1.TabIndex = 0;
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(138, 17);
            this.textBox1.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.textBox1.Name = "textBox1";
            this.textBox1.ReadOnly = true;
            this.textBox1.Size = new System.Drawing.Size(116, 23);
            this.textBox1.TabIndex = 1;
            // 
            // searchTextBox
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 17F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(281, 75);
            this.Controls.Add(this.textBox1);
            this.Controls.Add(this.searchTextBox1);
            this.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.Name = "searchTextBox";
            this.Text = "SearchTextBox";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private SearchControls.SearchTextBox searchTextBox1;
        private System.Windows.Forms.TextBox textBox1;
    }
}

