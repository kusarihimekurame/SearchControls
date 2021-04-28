using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SearchControls.SearchGridForm
{
    /// <summary>
    /// 无焦点窗口
    /// </summary>
    public class NoActivateForm : Form
    {
        /// <summary>
        /// 显示窗口时不将其激活
        /// </summary>
        protected override bool ShowWithoutActivation => true;
        private const int WS_EX_NOACTIVATE = 0x08000000;
        private const int WM_MOUSEACTIVATE = 0x21;
        private const int MA_NOACTIVATE = 3;

        /// <summary>
        /// 初始化
        /// </summary>
        public NoActivateForm()
        {
            InitializeComponent();

            SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.AllPaintingInWmPaint, true);
            SetStyle(ControlStyles.Selectable, false);
        }

        /// <summary>
        /// 获取创建控件句柄时所需要的创建参数。
        /// </summary>
        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams cp = base.CreateParams;
                cp.ExStyle |= WS_EX_NOACTIVATE;
                return cp;
            }
        }

        /// <summary>
        /// 处理 Windows 消息。
        /// </summary>
        /// <param name="m">一个 Windows 消息对象。</param>
        [DebuggerStepThrough]
        protected override void WndProc(ref Message m)
        {
            if (m.Msg.Equals(WM_MOUSEACTIVATE))
            {
                m.Result = new IntPtr(MA_NOACTIVATE);
                return;
            }
            base.WndProc(ref m);
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();
            // 
            // NoActivateForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(284, 261);
            this.ShowInTaskbar = false;
            this.FormBorderStyle = FormBorderStyle.None;
            this.Name = "NoActivateForm";
            this.ResumeLayout(false);
        }
    }
}
