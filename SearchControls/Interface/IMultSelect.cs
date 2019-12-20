using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Windows.Forms;

namespace SearchControls.Interface
{
    /// <include file='Include_Tag.xml' path='Tab/Members/Member[@Name="IMultiSelect"]/*'/>
    public interface IMultiSelect
    {
        /// <include file='Include_Tag.xml' path='Tab/Members/Member[@Name="MultSelectColumn"]/*'/>
        [
            Category("Search"),
            Description("多重选择的列")
        ]
        DataGridViewCheckBoxColumn MultiSelectColumn { get; }
        /// <include file='Include_Tag.xml' path='Tab/Members/Member[@Name="IsMultSelect"]/*'/>
        [
            DefaultValue(false),
            Category("Search"),
            Description("是否多选")
        ]
        bool IsMultiSelect { get; set; }
        /// <include file='Include_Tag.xml' path='Tab/Members/Member[@Name="MultSelectSplit"]/*'/>
        [
            DefaultValue(";"),
            Category("Search"),
            Description("多重选择的分隔符")
        ]
        string MultiSelectSplit { get; set; }
    }
}
