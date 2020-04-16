using SearchControls.SearchGridForm;
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
        /// <summary>
        /// <para>实现默认接口用</para>
        /// <para><c>SearchForm IGrid.SearchForm => SearchForm;</c></para>
        /// </summary>
        SearchForm SearchForm { get; }
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
        /// <include file='Include_Tag.xml' path='Tab/Members/Member[@Name="IsLeft"]/*'/>
        [
            DefaultValue(false),
            Category("Search"),
            Description("是否将表朝左")
        ]
        bool IsLeft { get; set; }
        /// <summary>
        /// 需要跟踪的控件的边框Rectangle
        /// </summary>
        Rectangle Bounds { get; }
        /// <include file='Include_Tag.xml' path='Tab/Members/Member[@Name="ShowSearchGrid"]/*'/>
#if NETCOREAPP3_0 || NETCOREAPP3_1
        void ShowSearchGrid() => SearchForm.ShowSearchGrid();
#else
        void ShowSearchGrid();
#endif
        /// <include file='Include_Tag.xml' path='Tab/Members/Member[@Name="SetSearchGridSize"]/*'/>
#if NETCOREAPP3_0 || NETCOREAPP3_1
        void SetSearchGridSize() => SearchForm.SetSearchGridSize();
#else
        void SetSearchGridSize();
#endif
        /// <include file='Include_Tag.xml' path='Tab/Members/Member[@Name="SetSearchGridLocation"]/*'/>
#if NETCOREAPP3_0 || NETCOREAPP3_1
        void SetSearchGridLocation() => SearchForm.SetSearchGridLocation();
#else
        void SetSearchGridLocation();
#endif
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
