using SearchControls.Design;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing.Design;
using System.Text;
using System.Windows.Forms;

namespace SearchControls.Interface
{
    /// <summary>
    /// 附属的文本框集合的接口
    /// </summary>
    public interface ISubSearchTextBoxes
    {
        /// <include file='Include_Tag.xml' path='Tab/Members/Member[@Name="SubSearchTextBoxes"]/*'/>
        [
            Editor(typeof(HelpCollectionEditor), typeof(UITypeEditor)),
            DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
            MergableProperty(false),
            Category("Search"),
            Description("附属的文本框")
        ]
        SubSearchTextBoxCollection SubSearchTextBoxes { get; }
    }
}
