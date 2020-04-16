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

        /// <summary>
        /// 要展示的列名
        /// </summary>
        [
            Category("Item"),
            Description("需要在文本框中展示的对应的列名"),
            DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)
        ]
        public string DisplayDataName { get; set; }

        /// <summary>
        /// 自动输入对应的列名(根据文本框中的内容是否与对应的列中的数displayData据一致，自动输入DisplayDataName列的内容)
        /// </summary>
        [
            Category("Item"),
            Description("自动输入对应的列名(根据文本框中的内容是否与对应的列中的数据一致，自动输入DisplayDataName列的内容)"),
            DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)
        ]
        public string AutoInputDataName { get; set; }
        /// <summary>
        /// 模糊查找的表是否移到副表位置
        /// </summary>
        [
            DefaultValue(false),
            Category("Item"),
            Description("模糊查找的表是否移到副表位置"),
            DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)
        ]
        public bool IsMoveGrid { get; set; } = false;
        /// <summary>
        /// 类的初始化
        /// </summary>
        /// <param name="textBox">附属的文本框</param>
        /// <param name="displayDataName">要展示的列名</param>
        /// <param name="autoInputDataName">自动输入的列名</param>
        /// <param name="isMoveGrid">模糊查找的表是否移到副表位置</param>
        public SubSearchTextBox(TextBox textBox, string displayDataName, string autoInputDataName = null, bool isMoveGrid = false)
        {
            TextBox = textBox;
            DisplayDataName = displayDataName;
            AutoInputDataName = autoInputDataName;
            IsMoveGrid = isMoveGrid;
        }

        /// <summary>
        /// 类的初始化
        /// </summary>
        public SubSearchTextBox() { }
    }
}
