namespace SearchControls.Controls
{
    partial class GridStatusStrip
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
            this.ToolRecordCount = new System.Windows.Forms.ToolStripStatusLabel();
            this.ToolChangeRecordCount = new System.Windows.Forms.ToolStripStatusLabel();
            this.ToolUpdateStatus = new System.Windows.Forms.ToolStripStatusLabel();
            this.ToolSpace = new System.Windows.Forms.ToolStripStatusLabel();
            this.ToolStripProgressBar = new System.Windows.Forms.ToolStripProgressBar();
            this.ToolProgressBarStatus = new System.Windows.Forms.ToolStripStatusLabel();
            this.SuspendLayout();
            // 
            // ToolRecordCount
            // 
            this.ToolRecordCount.Margin = new System.Windows.Forms.Padding(2, 3, 2, 2);
            this.ToolRecordCount.Name = "ToolRecordCount";
            this.ToolRecordCount.Size = new System.Drawing.Size(122, 17);
            this.ToolRecordCount.Text = "总记录数：000000条";
            // 
            // ToolChangeRecordCount
            // 
            this.ToolChangeRecordCount.Margin = new System.Windows.Forms.Padding(2, 3, 2, 2);
            this.ToolChangeRecordCount.Name = "ToolChangeRecordCount";
            this.ToolChangeRecordCount.Size = new System.Drawing.Size(122, 17);
            this.ToolChangeRecordCount.Text = "更改了000000条记录";
            // 
            // ToolUpdateStatus
            // 
            this.ToolUpdateStatus.Name = "ToolUpdateStatus";
            this.ToolUpdateStatus.Size = new System.Drawing.Size(44, 17);
            this.ToolUpdateStatus.Text = "未提交";
            // 
            // ToolSpace
            // 
            this.ToolSpace.Margin = new System.Windows.Forms.Padding(2, 3, 2, 2);
            this.ToolSpace.Name = "ToolSpace";
            this.ToolSpace.Size = new System.Drawing.Size(340, 17);
            this.ToolSpace.Spring = true;
            // 
            // ToolStripProgressBar
            // 
            this.ToolStripProgressBar.Margin = new System.Windows.Forms.Padding(2, 3, 0, 3);
            this.ToolStripProgressBar.Name = "ToolStripProgressBar";
            this.ToolStripProgressBar.Size = new System.Drawing.Size(100, 16);
            // 
            // ToolProgressBarStatus
            // 
            this.ToolProgressBarStatus.Margin = new System.Windows.Forms.Padding(0, 3, 2, 2);
            this.ToolProgressBarStatus.Name = "ToolProgressBarStatus";
            this.ToolProgressBarStatus.Size = new System.Drawing.Size(68, 17);
            this.ToolProgressBarStatus.Text = "进度条信息";
            // 
            // GridStatusStrip
            // 
            this.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.ToolRecordCount,
            this.ToolChangeRecordCount,
            this.ToolUpdateStatus,
            this.ToolSpace,
            this.ToolStripProgressBar,
            this.ToolProgressBarStatus});
            this.Location = new System.Drawing.Point(0, 342);
            this.Size = new System.Drawing.Size(858, 22);
            this.ResumeLayout(false);
        }

        #endregion

        /// <summary>
        /// 状态栏中的总记录数
        /// </summary>
        public System.Windows.Forms.ToolStripStatusLabel ToolRecordCount;
        /// <summary>
        /// 状态栏中的变更记录数
        /// </summary>
        public System.Windows.Forms.ToolStripStatusLabel ToolChangeRecordCount;
        /// <summary>
        /// 状态栏中的更新状态
        /// </summary>
        public System.Windows.Forms.ToolStripStatusLabel ToolUpdateStatus;
        /// <summary>
        /// 状态栏中的空格，为了左右数据分开
        /// </summary>
        public System.Windows.Forms.ToolStripStatusLabel ToolSpace;
        /// <summary>
        /// 状态栏右边的进度条
        /// </summary>
        public System.Windows.Forms.ToolStripProgressBar ToolStripProgressBar;
        /// <summary>
        /// 状态栏右边的进度条的状态
        /// </summary>
        public System.Windows.Forms.ToolStripStatusLabel ToolProgressBarStatus;
    }
}
