using SearchControls.SearchGridForm;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Windows.Forms;

namespace SearchControls.Interface
{
    /// <include file='Include_Tag.xml' path='Tab/Members/Member[@Name="IDataText"]/*'/>
    public interface IDataText
    {
#if NETCOREAPP3_0 || NETCOREAPP3_1
        /// <summary>
        /// <para>实现默认接口用</para>
        /// <para><c>SearchGrid IDataText.SearchGrid => SearchForm.SearchGrid;</c></para>
        /// </summary>
        SearchGrid SearchGrid { get; }
#endif
        /// <summary>
        /// 对应的文本框
        /// </summary>
        TextBox TextBox { get; }
        /// <include file='Include_Tag.xml' path='Tab/Members/Member[@Name="IsTextChanged"]/*'/>
        [
            DefaultValue(true),
            Category("Search"),
            Description("是否进行模糊查找")
        ]
        bool IsTextChanged { get; set; }
        /// <include file='Include_Tag.xml' path='Tab/Members/Member[@Name="TextChangedColumnNames"]/*'/>
        [
            DefaultValue(null),
            Category("Search"),
            Description("需要进行模糊查找的列名，为空时默认为全部的需要显示的列")
        ]
        string[] TextChangedColumnNames { get; set; }
        /// <include file='Include_Tag.xml' path='Tab/Members/Member[@Name="IsAutoInput"]/*'/>
        [
            DefaultValue(true),
            Category("Search"),
            Description("当输入内容完全正确时，自动触发GridSelectedEnter(dataGridView,第0行)")
        ]
        bool IsAutoInput { get; set; }
        /// <include file='Include_Tag.xml' path='Tab/Members/Member[@Name="AutoInputDataName"]/*'/>
        [
            Category("Search"),
            Description("自动输入对应的列名(根据文本框中的内容是否与对应的列中的数据一致，自动输入DisplayDataName列的内容)")
        ]
        string AutoInputDataName { get; set; }
        /// <include file='Include_Tag.xml' path='Tab/Members/Member[@Name="DisplayDataName"]/*'/>
        [
            Category("Search"),
            Description("需要在文本框中展示的对应的列名")
        ]
        string DisplayDataName { get; set; }
        /// <include file='Include_Tag.xml' path='Tab/Members/Member[@Name="IsAutoReset"]/*'/>
        [
            DefaultValue(true),
            Category("Search"),
            Description("模糊查找框显示的时候是否自动还原到原来的状态")
        ]
        bool IsAutoReset { get; set; }
        /// <include file='Include_Tag.xml' path='Tab/Members/Member[@Name="Reset"]/*'/>
#if NETCOREAPP3_0 || NETCOREAPP3_1
        void Reset() => SearchGrid.Reset();
#else
        void Reset();
#endif
        /// <include file='Include_Tag.xml' path='Tab/Members/Member[@Name="ReversalSearchState"]/*'/>
#if NETCOREAPP3_0 || NETCOREAPP3_1
        void ReversalSearchState() => SearchGrid.ReversalSearchState();
#else
        void ReversalSearchState();
#endif
        /// <include file='Include_Tag.xml' path='Tab/Members/Member[@Name="GridSelecting"]/*'/>
        [
            Category("Search"),
            Description("当选择完某一行时发生（鼠标点击和键盘确定键都会触发）。")
        ]
        event GridSelectingEventHandler GridSelecting;
        /// <include file='Include_Tag.xml' path='Tab/Members/Member[@Name="GridSelected"]/*'/>
        [
            Category("Search"),
            Description("当选择完某一行时发生（鼠标点击和键盘确定键都会触发）。")
        ]
        event GridSelectedEventHandler GridSelected;
        /// <include file='Include_Tag.xml' path='Tab/Members/Member[@Name="OnGridSelecting"]/*'/>
        void OnGridSelecting(GridSelectingEventArgs e);
        /// <include file='Include_Tag.xml' path='Tab/Members/Member[@Name="OnGridSelected"]/*'/>
        void OnGridSelected(GridSelectedEventArgs e);
    }
}
