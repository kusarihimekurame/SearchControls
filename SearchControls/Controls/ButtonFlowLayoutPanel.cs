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

namespace SearchControls
{
    /// <summary>
    /// 按钮群
    /// </summary>
    [
        DisplayName("ButtonFlowLayoutPanel"),
        Description("按钮群"),
        ComVisible(true),
        ProvideProperty("FlowBreak", typeof(Control)),
        ProvideProperty("FlowIndex", typeof(Control)),
        DefaultProperty("FlowDirection"),
        Designer("System.Windows.Forms.Design.FlowLayoutPanelDesigner"),
        Docking(DockingBehavior.Ask)
    ]
    public partial class ButtonFlowLayoutPanel : FlowLayoutPanel
    {
        private Color buttonBackColor;
        /// <summary>
        /// 按钮的背景色。
        /// </summary>
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
        /// <summary>
        /// 确定当用户将鼠标移动到按钮上并单击时该按钮的外观。
        /// </summary>
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
        /// <summary>
        /// 确定在支持视觉样式的情况下，是否使用视觉样式绘制背景。
        /// </summary>
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
        /// <summary>
        /// 用于显示按钮中的文本的字体。
        /// </summary>
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
        /// <summary>
        /// 按钮的前景色，用于显示文本。
        /// </summary>
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
        /// <summary>
        /// 指定此控件与另一控件的边距之间的距离。
        /// </summary>
        [
            DefaultValue(typeof(Padding), "2, 2, 2, 2"),
            Category("ButtonStyle"),
            Description("指定此控件与另一控件的边距之间的距离。")
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
        /// <summary>
        /// 控件的大小(以像素为单位)。
        /// </summary>
        [
            DefaultValue(typeof(Size), "50, 30"),
            Category("ButtonStyle"),
            Description("控件的大小(以像素为单位)。")
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
        /// <summary>
        /// 自定义按钮群的方法用的接口
        /// </summary>
        [Description("自定义按钮群的方法用的接口")]
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
        /// <summary>
        /// 初始化
        /// </summary>
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

        /// <summary>
        /// 引发 System.Windows.Forms.Control.ControlAdded 事件。
        /// </summary>
        /// <param name="e">包含事件数据的 System.Windows.Forms.ControlEventArgs。</param>
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

            for (int i = 0; i < Controls.Count; i++)
            {
                Controls[i].TabIndex = i;
            }
        }

        /// <summary>
        /// 父控件中的位置
        /// </summary>
        /// <param name="control">子控件</param>
        /// <returns>父控件中的位置</returns>
        [
            DisplayName("FlowIndex"),
            Description("父控件中的位置")
        ]
        public int GetFlowIndex(Control control) => Controls.GetChildIndex(control);
        /// <summary>
        /// 父控件中的位置
        /// </summary>
        /// <param name="control">子控件</param>
        /// <param name="index">设置父控件中的位置</param>
        [
            DisplayName("FlowIndex"),
            Description("父控件中的位置")
        ]
        public void SetFlowIndex(Control control, int index)
        {
            if (Controls.Contains(control)) Controls.SetChildIndex(control, index);
            ControlAdded += (sender, e) =>
              {
                  if (e.Control.Equals(control))
                  {
                      Controls.SetChildIndex(control, index);
                  }
              };
        }

        /// <summary>
        /// 点击第一按钮
        /// </summary>
        /// <param name="sender">事件源。</param>
        /// <param name="e">不包含事件数据的对象。</param>
        protected virtual void BtnFirst_Click(object sender, EventArgs e) => Method?.BtnFirst_Click();
        /// <summary>
        /// 点击向下按钮
        /// </summary>
        /// <param name="sender">事件源。</param>
        /// <param name="e">不包含事件数据的对象。</param>
        protected virtual void BtnDown_Click(object sender, EventArgs e) => Method?.BtnDown_Click();
        /// <summary>
        /// 点击向上按钮
        /// </summary>
        /// <param name="sender">事件源。</param>
        /// <param name="e">不包含事件数据的对象。</param>
        protected virtual void BtnUp_Click(object sender, EventArgs e) => Method?.BtnUp_Click();
        /// <summary>
        /// 点击最后按钮
        /// </summary>
        /// <param name="sender">事件源。</param>
        /// <param name="e">不包含事件数据的对象。</param>
        protected virtual void BtnLast_Click(object sender, EventArgs e) => Method?.BtnLast_Click();
        /// <summary>
        /// 点击添加按钮
        /// </summary>
        /// <param name="sender">事件源。</param>
        /// <param name="e">不包含事件数据的对象。</param>
        protected virtual void BtnInsert_Click(object sender, EventArgs e) => Method?.BtnInsert_Click();
        /// <summary>
        /// 点击删除按钮
        /// </summary>
        /// <param name="sender">事件源。</param>
        /// <param name="e">不包含事件数据的对象。</param>
        protected virtual void BtnDelete_Click(object sender, EventArgs e) => Method?.BtnDelete_Click();
        /// <summary>
        /// 点击提交按钮
        /// </summary>
        /// <param name="sender">事件源。</param>
        /// <param name="e">不包含事件数据的对象。</param>
        protected virtual void BtnUpdate_Click(object sender, EventArgs e) => Method?.BtnUpdate_Click();
        /// <summary>
        /// 点击查找按钮
        /// </summary>
        /// <param name="sender">事件源。</param>
        /// <param name="e">不包含事件数据的对象。</param>
        protected virtual void BtnFound_Click(object sender, EventArgs e) => Method?.BtnFound_Click();
        /// <summary>
        /// 点击Excel按钮
        /// </summary>
        /// <param name="sender">事件源。</param>
        /// <param name="e">不包含事件数据的对象。</param>
        protected virtual void BtnExcel_Click(object sender, EventArgs e) => Method?.BtnExcel_Click();
        /// <summary>
        /// 点击Word按钮
        /// </summary>
        /// <param name="sender">事件源。</param>
        /// <param name="e">不包含事件数据的对象。</param>
        protected virtual void BtnWord_Click(object sender, EventArgs e) => Method?.BtnWord_Click();
        /// <summary>
        /// 点击撤销按钮
        /// </summary>
        /// <param name="sender">事件源。</param>
        /// <param name="e">不包含事件数据的对象。</param>
        protected virtual void BtnCancel_Click(object sender, EventArgs e) => Method?.BtnCancel_Click();
        /// <summary>
        /// 点击退出按钮
        /// </summary>
        /// <param name="sender">事件源。</param>
        /// <param name="e">不包含事件数据的对象。</param>
        protected virtual void BtnQuit_Click(object sender, EventArgs e) => Method?.BtnQuit_Click();
    }
}
