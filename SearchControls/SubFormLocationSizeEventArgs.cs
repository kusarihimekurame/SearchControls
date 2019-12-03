using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Search
{
    /// <summary>
    /// 子窗口的位置和大小发生变动时所需的参数
    /// </summary>
    public class SubFormLocationSizeEventArgs : EventArgs
    {
        /// <summary>
        /// 子窗口
        /// </summary>
        public Form SubForm { get; }
        /// <summary>
        /// 初始化 SubFormLocationSizeEventArgs 类的新实例
        /// </summary>
        /// <param name="subForm">子窗口</param>
        public SubFormLocationSizeEventArgs(Form subForm)
        {
            SubForm = subForm;
        }
    }
}
