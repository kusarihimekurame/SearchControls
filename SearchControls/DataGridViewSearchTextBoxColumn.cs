using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Search
{
    /// <summary>
    /// 
    /// </summary>
    internal class DataGridViewSearchTextBoxColumn : DataGridViewTextBoxColumn
    {
        /// <summary>
        /// 
        /// </summary>
        public string BindingItemDataPropertyName
        {
            get;
            set;
        }
        /// <summary>
        /// 
        /// </summary>
        public int BindingItemColumnIndex
        {
            get;
            set;
        }
        /// <summary>
        /// 
        /// </summary>
        public object ItemSource
        {
            get;
            set;
        }
    }
}
