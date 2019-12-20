using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace SearchControls
{
    /// <summary>
    /// 搜索窗口的位置和大小发生变动时所需的参数
    /// </summary>
    public class SearchFormLocationSizeEventArgs : EventArgs
    {
        /// <summary>
        /// 搜索窗口
        /// </summary>
        public Form SearchForm { get; }
        /// <summary>
        /// 初始化 SubFormLocationSizeEventArgs 类的新实例
        /// </summary>
        /// <param name="searchForm">搜索窗口</param>
        public SearchFormLocationSizeEventArgs(Form searchForm)
        {
            SearchForm = searchForm;
        }
    }
}
