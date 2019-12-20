using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace SearchControls.Interface
{
    /// <include file='Include_Tag.xml' path='Tab/Members/Member[@Name="IGrid"]/*'/>
    public interface IGrid
    {
        /// <include file='Include_Tag.xml' path='Tab/Members/Member[@Name="DisplayRowCount"]/*'/>
        [
            DefaultValue(15),
            Category("Search"),
            Description("想要显示的行数")
        ]
        int DisplayRowCount { get; set; }
        /// <include file='Include_Tag.xml' path='Tab/Members/Member[@Name="IsUp"]/*'/>
        [
            DefaultValue(false),
            Category("Search"),
            Description("是否将表朝上")
        ]
        bool IsUp { get; set; }
        /// <summary>
        /// 需要跟踪的控件的边框Rectangle
        /// </summary>
        Rectangle Bounds { get; }
        /// <include file='Include_Tag.xml' path='Tab/Members/Member[@Name="ShowSearchGrid"]/*'/>
        void ShowSearchGrid();
        /// <include file='Include_Tag.xml' path='Tab/Members/Member[@Name="SetSearchGridSize"]/*'/>
        void SetSearchGridSize();
        /// <include file='Include_Tag.xml' path='Tab/Members/Member[@Name="SetSearchGridLocation"]/*'/>
        void SetSearchGridLocation();
        /// <include file='Include_Tag.xml' path='Tab/Members/Member[@Name="SearchGridLocationSizeChanged"]/*'/>
        [
            Category("Search"),
            Description("当表格调整完位置和高宽后")
        ]
        event SearchGridLocationSizeChangedEventHandler SearchGridLocationSizeChanged;
        /// <include file='Include_Tag.xml' path='Tab/Members/Member[@Name="OnSearchGridLocationSizeChanged"]/*'/>
        void OnSearchGridLocationSizeChanged(SearchFormLocationSizeEventArgs e);
    }
}
