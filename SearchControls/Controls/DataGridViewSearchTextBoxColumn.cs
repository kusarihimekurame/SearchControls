using SearchControls.Design;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Design;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace SearchControls
{
    /// <summary>
    /// 承载的集合的 SearchControls.DataGridViewSearchTextBoxCell 单元格。
    /// </summary>
    [
        ToolboxBitmap(typeof(DataGridViewTextBoxColumn), "DataGridViewTextBoxColumn.bmp")
    ]
    public class DataGridViewSearchTextBoxColumn : DataGridViewColumn
    {
        private const int DATAGRIDVIEWTEXTBOXCOLUMN_maxInputLength = 32767;

        /// <summary>
        /// 将 System.Windows.Forms.DataGridViewTextBoxColumn 类的新实例初始化为默认状态。
        /// </summary>
        public DataGridViewSearchTextBoxColumn() : base(new DataGridViewSearchTextBoxCell())
        {
            SortMode = DataGridViewColumnSortMode.Automatic;
        }

        /// <summary>
        /// 获取或设置用于模仿单元格外观的模板。
        /// </summary>
        /// <value>
        /// 一个 System.Windows.Forms.DataGridViewCell，列中的所有其他单元格都以它为模型。
        /// </value>
        /// <exception cref="InvalidCastException">
        /// 所设置的类型与类型 System.Windows.Forms.DataGridViewSearchTextBoxCell 不兼容。
        /// </exception>
        [
            Browsable(false),
            DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)
        ]
        public override DataGridViewCell CellTemplate
        {
            get => base.CellTemplate;
            set
            {
                if (value != null && !(value is DataGridViewSearchTextBoxCell))
                {
                    throw new InvalidCastException("所设置的类型与类型 SearchControls.DataGridViewSearchTextBoxCell 不兼容。");
                }
                base.CellTemplate = value;
            }
        }

        /// <summary>
        /// 获取或设置可以在文本框中输入的最大字符数。
        /// </summary>
        /// <value>
        /// 可以在文本框中输入的最大字符数，默认值是 32767。
        /// </value>
        /// <exception cref="InvalidOperationException">
        /// System.Windows.Forms.DataGridViewSearchTextBoxColumn.CellTemplate 属性的值为 null。
        /// </exception>
        [
            DefaultValue(DATAGRIDVIEWTEXTBOXCOLUMN_maxInputLength)
        ]
        public int MaxInputLength
        {
            get
            {
                if (TextBoxCellTemplate == null)
                {
                    throw new InvalidOperationException("SearchControls.DataGridViewSearchTextBoxColumn.CellTemplate 属性的值为 null。");
                }
                return TextBoxCellTemplate.MaxInputLength;
            }
            set
            {
                if (MaxInputLength != value)
                {
                    TextBoxCellTemplate.MaxInputLength = value;
                    if (DataGridView != null)
                    {
                        DataGridViewRowCollection dataGridViewRows = DataGridView.Rows;
                        int rowCount = dataGridViewRows.Count;
                        for (int rowIndex = 0; rowIndex < rowCount; rowIndex++)
                        {
                            DataGridViewRow dataGridViewRow = dataGridViewRows.SharedRow(rowIndex);
                            DataGridViewSearchTextBoxCell dataGridViewCell = dataGridViewRow.Cells[Index] as DataGridViewSearchTextBoxCell;
                            if (dataGridViewCell != null)
                            {
                                dataGridViewCell.MaxInputLength = value;
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 获取或设置列的排序模式。
        /// </summary>
        /// <value>
        /// System.Windows.Forms.DataGridViewColumnSortMode，指定根据列中单元格的值对行进行排序的条件。
        /// </value>
        [
            DefaultValue(DataGridViewColumnSortMode.Automatic)
        ]
        public new DataGridViewColumnSortMode SortMode
        {
            get => base.SortMode;
            set => base.SortMode = value;
        }

        private DataGridViewTextBoxCell TextBoxCellTemplate => (DataGridViewTextBoxCell)CellTemplate;

        /// <summary>
        /// 获取一个描述该列的字符串。
        /// </summary>
        /// <returns>
        /// 用于描述列的 System.String。
        /// </returns>
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder(64);
            sb.Append("DataGridViewSearchTextBoxColumn { Name=");
            sb.Append(Name);
            sb.Append(", Index=");
            sb.Append(Index.ToString(CultureInfo.CurrentCulture));
            sb.Append(" }");
            return sb.ToString();
        }

        /// <summary>
        /// 创建此区段的一个完全相同的副本。
        /// </summary>
        /// <returns>一个 System.Object，表示克隆的 System.Windows.Forms.DataGridViewBand。</returns>
        public override object Clone()
        {
            DataGridViewSearchTextBoxColumn stbc = (DataGridViewSearchTextBoxColumn)base.Clone();
            stbc.SearchDataSource = SearchDataSource;
            stbc.SearchDataMember = SearchDataMember;
            stbc.SearchColumns = SearchColumns;
            stbc.IsMain = IsMain;
            stbc.MainColumnName = MainColumnName;
            stbc.DisplayDataName = DisplayDataName;
            stbc.AutoInputDataName = AutoInputDataName;
            stbc.DisplayRowCount = DisplayRowCount;
            stbc.IsTextChanged = IsTextChanged;
            stbc.IsAutoInput = IsAutoInput;
            return stbc;
        }

        /// <include file='Include_Tag.xml' path='Tab/Members/Member[@Name="DataSource"]/*'/>
        /// <value>值为null时，自动获取DataGridView的DataSource</value>
        [
            DefaultValue(null),
            AttributeProvider(typeof(IListSource)),
            Category("Search.Main"),
            Description("设置或者获取SearchGrid的数据源")
        ]
        public object SearchDataSource { get; set; }

        /// <include file='Include_Tag.xml' path='Tab/Members/Member[@Name="DataMember"]/*'/>
        [
            DefaultValue(""),
            Editor("System.Windows.Forms.Design.DataMemberListEditor, System.Design, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", typeof(UITypeEditor)),
            Category("Search.Main"),
            Description("获取或设置 System.Windows.Forms.DataGridView 正在为其显示数据的数据源中的列表或表的名称。")
        ]
        public string SearchDataMember { get; set; }

        /// <summary>
        /// 模糊查找表格的列
        /// </summary>
        [
            Editor(typeof(HelpCollectionEditor), typeof(UITypeEditor)),
            DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
            MergableProperty(false),
            Category("Search.Main"),
            Description("模糊查找表格的列")
        ]
        public List<DataGridViewColumn> SearchColumns { get; set; } = new List<DataGridViewColumn>();

        /// <summary>
        /// 是否是主列
        /// </summary>
        /// <value>
        /// <para>值为true 时,表格根据当前单元格的位置显示</para>
        /// <para>需要填写属性<see cref="SearchDataSource"/>、<see cref="SearchDataMember"/>、<see cref="SearchColumns"/></para>
        /// <para>值为false时,表格根据当前行的主列<see cref="MainColumnName"/>的位置显示</para>
        /// <para>需要填写属性<see cref="MainColumnName"/></para>
        /// </value>
        [
            DefaultValue(true),
            Category("Search"),
            Description("是否是主列\r\n值为true 时,表格根据当前单元格的位置显示\r\n需要填写Search.Main类中的属性\r\n值为false时,表格根据当前行的主列MainColumnName的位置显示\r\n需要填写Search.Sub类中的属性")
        ]
        public bool IsMain { get; set; } = true;

        /// <summary>
        /// 主列名
        /// </summary>
        /// <remarks>
        /// 根据主列名搜索当前行所在的列的位置
        /// </remarks>
        [
            Category("Search.Sub"),
            Description("主列名\r\n根据主列名搜索当前行所在的列的位置")
        ]
        public string MainColumnName { get; set; }

        /// <include file='Include_Tag.xml' path='Tab/Members/Member[@Name="DisplayRowCount"]/*'/>
        [
            DefaultValue(15),
            Category("Search"),
            Description("想要显示的行数")
        ]
        public int DisplayRowCount { get; set; } = 15;

        /// <include file='Include_Tag.xml' path='Tab/Members/Member[@Name="IsTextChanged"]/*'/>
        [
            DefaultValue(true),
            Category("Search"),
            Description("是否进行模糊查找")
        ]
        public bool IsTextChanged { get; set; } = true;

        /// <include file='Include_Tag.xml' path='Tab/Members/Member[@Name="IsAutoInput"]/*'/>
        [
            DefaultValue(true),
            Category("Search"),
            Description("当输入内容完全正确时，自动触发GridSelectedEnter(dataGridView,第0行)")
        ]
        public bool IsAutoInput { get; set; } = true;

        /// <include file='Include_Tag.xml' path='Tab/Members/Member[@Name="AutoInputDataName"]/*'/>
        [
            Category("Search"),
            Description("自动输入对应的列名(根据文本框中的内容是否与对应的列中的数据一致，自动输入DisplayDataName列的内容)")
        ]
        public string AutoInputDataName { get; set; }
        /// <include file='Include_Tag.xml' path='Tab/Members/Member[@Name="DisplayDataName"]/*'/>
        [
            Category("Search"),
            Description("需要在文本框中展示的对应的列名")
        ]
        public string DisplayDataName { get; set; }
    }
}
