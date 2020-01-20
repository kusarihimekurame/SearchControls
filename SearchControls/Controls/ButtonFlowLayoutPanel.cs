using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
#if NET45 || NET451 || NET452
using System.Data.SqlClient;
#else
using Microsoft.Data.SqlClient;
#endif
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using System.Reflection;
using System.IO;
using SearchControls.Export;
using System.Threading;
using System.Text.RegularExpressions;
using SearchControls.Interface;

namespace SearchControls.Controls
{
    [ComVisible(true)]
    [ProvideProperty("FlowBreak", typeof(Control))]
    [DefaultProperty("FlowDirection")]
    [Designer("System.Windows.Forms.Design.FlowLayoutPanelDesigner")]
    [Docking(DockingBehavior.Ask)]
    public partial class ButtonFlowLayoutPanel : FlowLayoutPanel
    {
        private Color buttonBackColor;
        [
            DefaultValue(typeof(Color), "ButtonFace"),
            Category("ButtonStyle"),
            Description("按钮的背景色。")
        ]
        public Color ButtonBackColor
        {
            get => buttonBackColor;
            set
            {
                SuspendLayout();
                buttonBackColor = value;
                foreach (Button b in Controls)
                {
                    b.BackColor = value;
                }
                ResumeLayout(false);
            }
        }
        private FlatStyle buttonFlatStyle;
        [
            DefaultValue(typeof(FlatStyle), "Standard"),
            Category("ButtonStyle"),
            Description("确定当用户将鼠标移动到按钮上并单击时该按钮的外观。")
        ]
        public FlatStyle ButtonFlatStyle
        {
            get => buttonFlatStyle;
            set
            {
                SuspendLayout();
                buttonFlatStyle = value;
                foreach (Button b in Controls)
                {
                    b.FlatStyle = value;
                }
                ResumeLayout(false);
            }
        }
        private bool buttonUseVisualStyleBackColor;
        [
            DefaultValue(false),
            Category("ButtonStyle"),
            Description("确定在支持视觉样式的情况下，是否使用视觉样式绘制背景。")
        ]
        public bool ButtonUseVisualStyleBackColor
        {
            get => buttonUseVisualStyleBackColor;
            set
            {
                SuspendLayout();
                buttonUseVisualStyleBackColor = value;
                foreach (Button b in Controls)
                {
                    b.UseVisualStyleBackColor = value;
                }
                ResumeLayout(false);
            }
        }

        [
            Localizable(true),
            Category("ButtonStyle"),
            Description("用于显示按钮中的文本的字体。")
        ]
        public Font ButtonFont
        {
            get => Font;
            set
            {
                SuspendLayout();
                if (!Font.Equals(value)) Font = value;
                foreach (Button b in Controls)
                {
                    b.Font = value;
                }
                ResumeLayout(false);
            }
        }
        private Color buttonForeColor;
        [
            DefaultValue(typeof(Color), "ControlText"),
            Category("ButtonStyle"),
            Description("按钮的前景色，用于显示文本。")
        ]
        public Color ButtonForeColor
        {
            get => buttonForeColor;
            set
            {
                SuspendLayout();
                buttonForeColor = value;
                foreach (Button b in Controls)
                {
                    b.ForeColor = value;
                }
                ResumeLayout(false);
            }
        }
        private Padding buttonMargin;
        [
            DefaultValue(typeof(Padding), "2, 2, 2, 2"),
            Category("ButtonStyle"),
            Description("按钮的前景色，用于显示文本。")
        ]
        public Padding ButtonMargin
        {
            get => buttonMargin;
            set
            {
                SuspendLayout();
                buttonMargin = value;
                foreach (Button b in Controls)
                {
                    b.Margin = value;
                }
                ResumeLayout(false);
            }
        }
        private Size buttonSize;
        [
            DefaultValue(typeof(Size), "50, 30"),
            Category("ButtonStyle"),
            Description("按钮的前景色，用于显示文本。")
        ]
        public Size ButtonSize
        {
            get => buttonSize;
            set
            {
                SuspendLayout();
                buttonSize = value;
                foreach (Button b in Controls)
                {
                    b.Size = value;
                }
                ResumeLayout(false);
            }
        }

        private IButtonFlowLayoutPanelMethod method;
        public IButtonFlowLayoutPanelMethod Method
        {
            get => method;
            set
            {
                if (value != null)
                {
                    value.ButtonFlowLayoutPanel = this;
                }
                method = value;
            }
        }

        private readonly Button[] buttons;
        public ButtonFlowLayoutPanel()
        {
            InitializeComponent();
            ButtonBackColor = SystemColors.ButtonFace;
            ButtonFlatStyle = FlatStyle.Standard;
            ButtonUseVisualStyleBackColor = false;
            ButtonFont = Font;
            ButtonForeColor = SystemColors.ControlText;
            ButtonMargin = new Padding(2);
            ButtonSize = new Size(50, 30);

            buttons = new Button[Controls.Count];
            Controls.CopyTo(buttons, 0);
        }

        protected override void OnControlAdded(ControlEventArgs e)
        {
            base.OnControlAdded(e);
            if (buttons == null) return;
            if (e.Control is Button b)
            {
                SuspendLayout();
                b.BackColor = ButtonBackColor;
                b.FlatStyle = ButtonFlatStyle;
                b.UseVisualStyleBackColor = ButtonUseVisualStyleBackColor;
                b.Font = ButtonFont;
                b.ForeColor = ButtonForeColor;
                b.Margin = ButtonMargin;
                b.Size = ButtonSize;
                ResumeLayout(false);
            }

            for (int i = 0; i < buttons.Count(); i++)
            {
                if (e.Control.Location.Equals(buttons[i].Location))
                {
                    Controls.SetChildIndex(e.Control, i);
                    break;
                }
            }
            for (int i = 0; i < Controls.Count; i++)
            {
                Controls[i].TabIndex = i;
            }
        }

        protected virtual void BtnFirst_Click(object sender, EventArgs e) => Method?.BtnFirst_Click();
        protected virtual void BtnDown_Click(object sender, EventArgs e) => Method?.BtnDown_Click();
        protected virtual void BtnUp_Click(object sender, EventArgs e) => Method?.BtnUp_Click();
        protected virtual void BtnLast_Click(object sender, EventArgs e) => Method?.BtnLast_Click();
        protected virtual void BtnInsert_Click(object sender, EventArgs e) => Method?.BtnInsert_Click();
        protected virtual void BtnDelete_Click(object sender, EventArgs e) => Method?.BtnDelete_Click();
        protected virtual void BtnUpdate_Click(object sender, EventArgs e) => Method?.BtnUpdate_Click();
        protected virtual void BtnFound_Click(object sender, EventArgs e) => Method?.BtnFound_Click();
        protected virtual void BtnExcel_Click(object sender, EventArgs e) => Method?.BtnExcel_Click();
        protected virtual void BtnWord_Click(object sender, EventArgs e) => Method?.BtnWord_Click();
        protected virtual void BtnCancel_Click(object sender, EventArgs e) => Method?.BtnCancel_Click();
        protected virtual void BtnQuit_Click(object sender, EventArgs e) => Method?.BtnQuit_Click();
    }
}
