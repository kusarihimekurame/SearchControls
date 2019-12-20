using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Windows.Forms;

namespace SearchControls
{
    /// <summary>
    /// 附属的文本框的类
    /// </summary>
    [DesignTimeVisible(false)]
    public class SubSearchTextBox : Component
    {
        /// <summary>
        /// 附属的文本框
        /// </summary>
        [
            Category("Item"),
            Description("文本框"),
            DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)
        ]
        public TextBox TextBox { get; set; }
        private string _DisplayDataName;
        /// <summary>
        /// 要展示的列名
        /// </summary>
        [
            Category("Item"),
            Description("需要在文本框中展示的对应的列名"),
            DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)
        ]
        public string DisplayDataName
        {
            get => _DisplayDataName;
            set
            {
                _DisplayDataName = value;
                if (string.IsNullOrWhiteSpace(_AutoInputDataName)) _AutoInputDataName = value;
            }
        }
        private string _AutoInputDataName;
        /// <summary>
        /// 自动输入对应的列名(根据文本框中的内容是否与对应的列中的数displayData据一致，自动输入DisplayDataName列的内容)
        /// </summary>
        [
            Category("Item"),
            Description("自动输入对应的列名(根据文本框中的内容是否与对应的列中的数据一致，自动输入DisplayDataName列的内容)"),
            DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)
        ]
        public string AutoInputDataName
        {
            get => _AutoInputDataName;
            set
            {
                if (string.IsNullOrWhiteSpace(value)) _AutoInputDataName = DisplayDataName;
                else _AutoInputDataName = value;
            }
        }
        /// <summary>
        /// 类的初始化
        /// </summary>
        /// <param name="textBox">附属的文本框</param>
        /// <param name="displayDataName">要展示的列名</param>
        /// <param name="autoInputDataName">自动输入的列名</param>
        public SubSearchTextBox(TextBox textBox, string displayDataName, string autoInputDataName = null)
        {
            TextBox = textBox;
            DisplayDataName = displayDataName;
            AutoInputDataName = autoInputDataName;
        }

        /// <summary>
        /// 类的初始化
        /// </summary>
        public SubSearchTextBox() { }
    }
}
