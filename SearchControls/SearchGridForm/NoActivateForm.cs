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
            SuspendLayout();
            // 
            // NoActivateForm
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            AutoScaleMode = AutoScaleMode.Dpi;
            ClientSize = new System.Drawing.Size(331, 370);
            FormBorderStyle = FormBorderStyle.None;
            Margin = new Padding(4, 4, 4, 4);
            Name = "NoActivateForm";
            ShowInTaskbar = false;
            ResumeLayout(false);
        }
    }
}
