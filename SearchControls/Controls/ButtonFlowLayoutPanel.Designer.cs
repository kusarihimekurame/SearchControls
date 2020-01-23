using System.ComponentModel;

namespace SearchControls
{
    partial class ButtonFlowLayoutPanel
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

        #region 组件设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要修改
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.BtnCancel = new System.Windows.Forms.Button();
            this.BtnDelete = new System.Windows.Forms.Button();
            this.BtnUpdate = new System.Windows.Forms.Button();
            this.BtnInsert = new System.Windows.Forms.Button();
            this.BtnWord = new System.Windows.Forms.Button();
            this.BtnExcel = new System.Windows.Forms.Button();
            this.BtnLast = new System.Windows.Forms.Button();
            this.BtnUp = new System.Windows.Forms.Button();
            this.BtnDown = new System.Windows.Forms.Button();
            this.BtnFirst = new System.Windows.Forms.Button();
            this.BtnFound = new System.Windows.Forms.Button();
            this.BtnQuit = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // BtnCancel
            // 
            this.BtnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.BtnCancel.Location = new System.Drawing.Point(110, 36);
            this.BtnCancel.Name = "BtnCancel";
            this.BtnCancel.TabIndex = 6;
            this.BtnCancel.Text = "撤销";
            this.BtnCancel.UseVisualStyleBackColor = false;
            this.BtnCancel.Click += new System.EventHandler(this.BtnCancel_Click);
            // 
            // BtnDelete
            // 
            this.BtnDelete.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.BtnDelete.Enabled = false;
            this.BtnDelete.Location = new System.Drawing.Point(56, 36);
            this.BtnDelete.Name = "BtnDelete";
            this.BtnDelete.TabIndex = 5;
            this.BtnDelete.Text = "删除";
            this.BtnDelete.UseVisualStyleBackColor = false;
            this.BtnDelete.Click += new System.EventHandler(this.BtnDelete_Click);
            // 
            // BtnUpdate
            // 
            this.BtnUpdate.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.BtnUpdate.Location = new System.Drawing.Point(164, 36);
            this.BtnUpdate.Name = "BtnUpdate";
            this.BtnUpdate.TabIndex = 7;
            this.BtnUpdate.Text = "提交";
            this.BtnUpdate.UseVisualStyleBackColor = false;
            this.BtnUpdate.Click += new System.EventHandler(this.BtnUpdate_Click);
            // 
            // BtnInsert
            // 
            this.BtnInsert.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.BtnInsert.Enabled = false;
            this.BtnInsert.Location = new System.Drawing.Point(2, 36);
            this.BtnInsert.Name = "BtnInsert";
            this.BtnInsert.TabIndex = 4;
            this.BtnInsert.Text = "添加";
            this.BtnInsert.UseVisualStyleBackColor = false;
            this.BtnInsert.Click += new System.EventHandler(this.BtnInsert_Click);
            // 
            // BtnWord
            // 
            this.BtnWord.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.BtnWord.Location = new System.Drawing.Point(56, 70);
            this.BtnWord.Name = "BtnWord";
            this.BtnWord.TabIndex = 9;
            this.BtnWord.Text = "WORD";
            this.BtnWord.UseVisualStyleBackColor = false;
            this.BtnWord.Click += new System.EventHandler(this.BtnWord_Click);
            // 
            // BtnExcel
            // 
            this.BtnExcel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.BtnExcel.Location = new System.Drawing.Point(2, 70);
            this.BtnExcel.Name = "BtnExcel";
            this.BtnExcel.TabIndex = 8;
            this.BtnExcel.Text = "EXCEL";
            this.BtnExcel.UseVisualStyleBackColor = false;
            this.BtnExcel.Click += new System.EventHandler(this.BtnExcel_Click);
            // 
            // BtnLast
            // 
            this.BtnLast.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.BtnLast.Enabled = false;
            this.BtnLast.Location = new System.Drawing.Point(164, 2);
            this.BtnLast.Name = "BtnLast";
            this.BtnLast.TabIndex = 3;
            this.BtnLast.Text = "最后";
            this.BtnLast.UseVisualStyleBackColor = false;
            this.BtnLast.Click += new System.EventHandler(this.BtnLast_Click);
            // 
            // BtnUp
            // 
            this.BtnUp.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.BtnUp.Enabled = false;
            this.BtnUp.Location = new System.Drawing.Point(110, 2);
            this.BtnUp.Name = "BtnUp";
            this.BtnUp.TabIndex = 2;
            this.BtnUp.Text = "向上";
            this.BtnUp.UseVisualStyleBackColor = false;
            this.BtnUp.Click += new System.EventHandler(this.BtnUp_Click);
            // 
            // BtnDown
            // 
            this.BtnDown.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.BtnDown.Enabled = false;
            this.BtnDown.Location = new System.Drawing.Point(56, 2);
            this.BtnDown.Name = "BtnDown";
            this.BtnDown.TabIndex = 1;
            this.BtnDown.Text = "向下";
            this.BtnDown.UseVisualStyleBackColor = false;
            this.BtnDown.Click += new System.EventHandler(this.BtnDown_Click);
            // 
            // BtnFirst
            // 
            this.BtnFirst.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.BtnFirst.Enabled = false;
            this.BtnFirst.Location = new System.Drawing.Point(2, 2);
            this.BtnFirst.Name = "BtnFirst";
            this.BtnFirst.TabIndex = 0;
            this.BtnFirst.Text = "第一";
            this.BtnFirst.UseVisualStyleBackColor = false;
            this.BtnFirst.Click += new System.EventHandler(this.BtnFirst_Click);
            // 
            // BtnFound
            // 
            this.BtnFound.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.BtnFound.Enabled = false;
            this.BtnFound.Location = new System.Drawing.Point(110, 70);
            this.BtnFound.Name = "BtnFound";
            this.BtnFound.TabIndex = 10;
            this.BtnFound.Text = "查找";
            this.BtnFound.UseVisualStyleBackColor = false;
            this.BtnFound.Click += new System.EventHandler(this.BtnFound_Click);
            // 
            // BtnQuit
            // 
            this.BtnQuit.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.BtnQuit.Location = new System.Drawing.Point(164, 70);
            this.BtnQuit.Name = "BtnQuit";
            this.BtnQuit.TabIndex = 11;
            this.BtnQuit.Text = "退出";
            this.BtnQuit.UseVisualStyleBackColor = false;
            this.BtnQuit.Click += new System.EventHandler(this.BtnQuit_Click);
            // 
            // ButtonFlowLayoutPanel
            // 
            this.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.Controls.Add(this.BtnFirst);
            this.Controls.Add(this.BtnDown);
            this.Controls.Add(this.BtnUp);
            this.Controls.Add(this.BtnLast);
            this.Controls.Add(this.BtnInsert);
            this.Controls.Add(this.BtnDelete);
            this.Controls.Add(this.BtnCancel);
            this.Controls.Add(this.BtnUpdate);
            this.Controls.Add(this.BtnExcel);
            this.Controls.Add(this.BtnWord);
            this.Controls.Add(this.BtnFound);
            this.Controls.Add(this.BtnQuit);
            this.Location = new System.Drawing.Point(1, 240);
            this.Size = new System.Drawing.Size(216, 102);
            this.ResumeLayout(false);
        }

        #endregion

        /// <summary>
        /// 撤销按钮
        /// </summary>
        public System.Windows.Forms.Button BtnCancel;
        /// <summary>
        /// 删除按钮
        /// </summary>
        public System.Windows.Forms.Button BtnDelete;
        /// <summary>
        /// 提交按钮
        /// </summary>
        public System.Windows.Forms.Button BtnUpdate;
        /// <summary>
        /// 添加按钮
        /// </summary>
        public System.Windows.Forms.Button BtnInsert;
        /// <summary>
        /// Word按钮
        /// </summary>
        public System.Windows.Forms.Button BtnWord;
        /// <summary>
        /// Excel按钮
        /// </summary>
        public System.Windows.Forms.Button BtnExcel;
        /// <summary>
        /// 最后按钮
        /// </summary>
        public System.Windows.Forms.Button BtnLast;
        /// <summary>
        /// 向上按钮
        /// </summary>
        public System.Windows.Forms.Button BtnUp;
        /// <summary>
        /// 向下按钮
        /// </summary>
        public System.Windows.Forms.Button BtnDown;
        /// <summary>
        /// 第一按钮
        /// </summary>
        public System.Windows.Forms.Button BtnFirst;
        /// <summary>
        /// 查找按钮
        /// </summary>
        public System.Windows.Forms.Button BtnFound;
        /// <summary>
        /// 退出按钮
        /// </summary>
        public System.Windows.Forms.Button BtnQuit;
    }
}
