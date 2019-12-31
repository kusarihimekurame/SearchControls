using SearchControls.Design;
using SearchControls.Interface;
using SearchControls.SearchGridForm;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Design;
using System.Linq;
using System.Linq.Dynamic;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace SearchControls
{
    /// <include file='Include_Tag.xml' path='Tab/Members/Member[@Name="SearchTextBox"]/*'/>
    [
        DisplayName("SearchTextBox"),
        Description("带有模糊查找(DataGridView)的TextBox"),
        DefaultEvent("GridSelected"),
        ComplexBindingProperties("DataSource", "DataMember"),
        ToolboxItem(true)
    ]
    public partial class SearchTextBox : TextBox, IGrid, IDataText, IMultiSelect, ISubSearchTextBoxes
    {
        Rectangle IGrid.Bounds => Parent.RectangleToScreen(Bounds);
        TextBox IDataText.textBox => this;

        SearchForm IGrid.SearchForm => SearchForm;
        private readonly SearchForm SearchForm;

        /// <include file='Include_Tag.xml' path='Tab/Members/Member[@Name="MultSelectColumn"]/*'/>
        [
            Category("Search"),
            Description("多重选择的列")
        ]
        public DataGridViewCheckBoxColumn MultiSelectColumn { get; } = new DataGridViewCheckBoxColumn
        {
            Name = "Select",
            DataPropertyName = "Select",
            HeaderText = "选择",
            Width = 30,
            FalseValue = false,
            TrueValue = true,
            ValueType = typeof(bool),
            SortMode = DataGridViewColumnSortMode.Automatic
        };
        private bool _IsMultiSelect = false;
        /// <include file='Include_Tag.xml' path='Tab/Members/Member[@Name="IsMultSelect"]/*'/>
        [
            DefaultValue(false),
            Category("Search"),
            Description("是否多选")
        ]
        public bool IsMultiSelect
        {
            get => _IsMultiSelect;
            set
            {
                _IsMultiSelect = value;
                if (_IsMultiSelect && !SearchGrid.Columns.Contains(MultiSelectColumn))
                {
                    SearchGrid.Columns.Insert(0, MultiSelectColumn);
                    if (SearchGrid.DataSource != null) SetMultSelectColumn(true);
                    SearchGrid.Sort(SearchGrid.Columns[MultiSelectColumn.Name], ListSortDirection.Descending);
                }
                else if (!_IsMultiSelect && SearchGrid.Columns.Contains(MultiSelectColumn))
                {
                    SearchGrid.Columns.Remove(MultiSelectColumn);
                    if (SearchGrid.DataSource != null) SetMultSelectColumn(false);
                }
            }
        }

        /// <include file='Include_Tag.xml' path='Tab/Members/Member[@Name="IsDisplayColumnHeaders"]/*'/>
        [
            DefaultValue(false),
            Category("Search"),
            Description("是否展示列标题")
        ]
        public bool IsDisplayColumnHeaders
        {
            get => _SearchGrid.ColumnHeadersVisible;
            set => _SearchGrid.ColumnHeadersVisible = value;
        }

        /// <include file='Include_Tag.xml' path='Tab/Members/Member[@Name="IsDisplayRowHeaders"]/*'/>
        [
            DefaultValue(false),
            Category("Search"),
            Description("是否展示行标题")
        ]
        public bool IsDisplayRowHeaders
        {
            get => _SearchGrid.RowHeadersVisible;
            set => _SearchGrid.RowHeadersVisible = value;
        }

        /// <include file='Include_Tag.xml' path='Tab/Members/Member[@Name="MultSelectSplit"]/*'/>
        [
            DefaultValue(";"),
            Category("Search"),
            Description("多重选择的分隔符")
        ]
        public string MultiSelectSplit { get; set; } = ";";
        /// <include file='Include_Tag.xml' path='Tab/Members/Member[@Name="SubSearchTextBoxes"]/*'/>
        [
            Editor(typeof(HelpCollectionEditor), typeof(UITypeEditor)),
            DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
            MergableProperty(false),
            Category("Search"),
            Description("附属的文本框")
        ]
        public SubSearchTextBoxCollection SubSearchTextBoxes { get; } = new SubSearchTextBoxCollection();
#if NETCOREAPP3_0 || NETCOREAPP3_1
        SearchGrid IDataText.SearchGrid => SearchForm.SearchGrid;
#endif
        private SearchGrid _SearchGrid => SearchForm.SearchGrid;
        /// <include file='Include_Tag.xml' path='Tab/Members/Member[@Name="SearchGrid"]/*'/>
        [
            EditorBrowsable(EditorBrowsableState.Always),
            DesignerSerializationVisibility(DesignerSerializationVisibility.Visible),
            Category("Search"),
            Description("模糊查找的DataGridView表")
        ]
        public DataGridView SearchGrid => SearchForm.SearchGrid;
        /// <include file='Include_Tag.xml' path='Tab/Members/Member[@Name="Columns"]/*'/>
        [
            DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
            Category("Search"),
            Description("模糊查找表格的列")
        ]
        public DataGridViewColumnCollection Columns => SearchGrid.Columns;
        /// <include file='Include_Tag.xml' path='Tab/Members/Member[@Name="CurrentRow"]/*'/>
        [
            Browsable(false)
        ]
        public DataGridViewRow CurrentRow
        {
            get
            {
                DataGridViewRow dgvr;
                if (string.IsNullOrWhiteSpace(Text)) return SearchGrid.CurrentRow;
                if (string.IsNullOrWhiteSpace(DisplayDataName))
                {
                    dgvr = SearchGrid.Rows.Cast<DataGridViewRow>()
                            .FirstOrDefault(_dgvr =>
                                _dgvr.Cells.Cast<DataGridViewCell>().Select(_dgvc => _dgvc.Value.ToString()).Any(item => item.Equals(Text))
                            );
                }
                else
                {
                    dgvr = SearchGrid.Rows.Cast<DataGridViewRow>().FirstOrDefault(_dgvr => _dgvr.Cells[DisplayDataName].Value.Equals(Text));
                }
                return dgvr ?? SearchGrid.CurrentRow;
            }
        }

        /// <include file='Include_Tag.xml' path='Tab/Members/Member[@Name="IsUp"]/*'/>
        [
            DefaultValue(false),
            Category("Search"),
            Description("是否将表朝上")
        ]
        public bool IsUp { get; set; } = false;
        private string _DisplayDataName;
        /// <include file='Include_Tag.xml' path='Tab/Members/Member[@Name="DisplayDataName"]/*'/>
        [
            Category("Search"),
            Description("需要在文本框中展示的对应的列名")
        ]
        public string DisplayDataName
        {
            get => _DisplayDataName;
            set
            {
                if (string.IsNullOrWhiteSpace(_AutoInputDataName)) _AutoInputDataName = value;
                _DisplayDataName = value;
            }
        }
        private string _AutoInputDataName;
        /// <include file='Include_Tag.xml' path='Tab/Members/Member[@Name="AutoInputDataName"]/*'/>
        [
            Category("Search"),
            Description("自动输入对应的列名(根据文本框中的内容是否与对应的列中的数据一致，自动输入DisplayDataName列的内容)")
        ]
        public string AutoInputDataName
        {
            get => _AutoInputDataName;
            set
            {
                if (string.IsNullOrWhiteSpace(value)) _AutoInputDataName = DisplayDataName;
                else _AutoInputDataName = value;
            }
        }
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
        /// <include file='Include_Tag.xml' path='Tab/Members/Member[@Name="DisplayRowCount"]/*'/>
        [
            DefaultValue(15),
            Category("Search"),
            Description("想要显示的行数")
        ]
        public int DisplayRowCount { get; set; } = 15;
        /// <include file='Include_Tag.xml' path='Tab/Members/Member[@Name="IsAutoReset"]/*'/>
        [
            DefaultValue(true),
            Category("Search"),
            Description("模糊查找框显示的时候是否自动还原到原来的状态")
        ]
        public bool IsAutoReset { get; set; } = true;

        /// <include file='Include_Tag.xml' path='Tab/Members/Member[@Name="DataSource"]/*'/>
        [
            DefaultValue(null),
            AttributeProvider(typeof(IListSource)),
            Category("Search"),
            Description("设置或者获取SearchGrid的数据源")
        ]
        public object DataSource
        {
            get => _SearchGrid.DataSource;
            set
            {
                Action action = null;
                action = () =>
                {
                    if (InvokeRequired)
                    {
                        Invoke(action);
                    }
                    else
                    {
                        _SearchGrid.DataSource = value;

                        if (IsMultiSelect)
                        {
                            SetMultSelectColumn(true);
                            SearchGrid.Sort(SearchGrid.Columns[MultiSelectColumn.Name], ListSortDirection.Descending);
                        }
                    }
                };
                action();
            }
        }

        private string dataMember;
        /// <include file='Include_Tag.xml' path='Tab/Members/Member[@Name="DataMember"]/*'/>
        [
            DefaultValue(""),
            Editor("System.Windows.Forms.Design.DataMemberListEditor, System.Design, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", typeof(UITypeEditor)),
            Category("Search"),
            Description("获取或设置 System.Windows.Forms.DataGridView 正在为其显示数据的数据源中的列表或表的名称。")
        ]
        public string DataMember
        {
            get => dataMember;
            set
            {
                dataMember = value;
                if (DataSource is DataSet ds && ds.Tables.Contains(value))
                {
                    Action action = null;
                    action = () =>
                    {
                        if (InvokeRequired)
                        {
                            Invoke(action);
                        }
                        else
                        {
                            _SearchGrid.DataMember = value;
                        }
                    };
                    action();
                }
            }
        }

        private void SetMultSelectColumn(bool IsAdd)
        {
            DataTable dt = null;
            if (SearchGrid.DataSource is DataTable _dt) dt = _dt;
            else if (SearchGrid.DataSource is DataView _dv) dt = _dv.Table;
            else if (SearchGrid.DataSource is DataSet _ds) dt = _ds.Tables[SearchGrid.DataMember];

            if (dt != null)
            {
                if (IsAdd)
                {
                    if (!dt.Columns.Contains("Select"))
                    {
                        DataColumn dc = new DataColumn("Select", typeof(bool)) { DefaultValue = false };
                        dt.Columns.Add(dc);
                    }
                }
                else
                {
                    if (dt.Columns.Contains("Select"))
                    {
                        dt.Columns.Remove("Select");
                    }
                }
            }
            else if (SearchGrid.DataSource is IEnumerable e)
            {

            }
        }

        /// <include file='Include_Tag.xml' path='Tab/Members/Member[@Name="GridSelecting"]/*'/>
        [
            Category("Search"),
            Description("当选择完某一行时发生（鼠标点击和键盘确定键都会触发）。")
        ]
        public event GridSelectingEventHandler GridSelecting;
        /// <include file='Include_Tag.xml' path='Tab/Members/Member[@Name="OnGridSelecting"]/*'/>
        public virtual void OnGridSelecting(GridSelectingEventArgs e) => GridSelecting?.Invoke(this, e);

        /// <include file='Include_Tag.xml' path='Tab/Members/Member[@Name="GridSelected"]/*'/>
        [
            Category("Search"),
            Description("当选择完某一行时发生（鼠标点击和键盘确定键都会触发）。")
        ]
        public event GridSelectedEventHandler GridSelected;
        /// <include file='Include_Tag.xml' path='Tab/Members/Member[@Name="OnGridSelected"]/*'/>
        public virtual void OnGridSelected(GridSelectedEventArgs e) => GridSelected?.Invoke(this, e);

        /// <include file='Include_Tag.xml' path='Tab/Members/Member[@Name="SearchGridLocationSizeChanged"]/*'/>
        [
            Category("Search"),
            Description("当表格调整完位置和高宽后")
        ]
        public event SearchGridLocationSizeChangedEventHandler SearchGridLocationSizeChanged;
        /// <include file='Include_Tag.xml' path='Tab/Members/Member[@Name="OnSearchGridLocationSizeChanged"]/*'/>
        public virtual void OnSearchGridLocationSizeChanged(SearchFormLocationSizeEventArgs e) => SearchGridLocationSizeChanged?.Invoke(this, e);

        /// <include file='Include_Tag.xml' path='Tab/Members/Member[@Name="SearchTextBox"]/*'/>
        /// <summary>
        /// SearchTextBox初始化
        /// </summary>
        public SearchTextBox()
        {
            InitializeComponent();

            SearchForm = new SearchForm(this);
        }

        /// <include file='Include_Tag.xml' path='Tab/Members/Member[@Name="Reset"]/*'/>
        public virtual void Reset() => _SearchGrid.Reset();

        /// <include file='Include_Tag.xml' path='Tab/Members/Member[@Name="ReversalSearchState"]/*'/>
        public virtual void ReversalSearchState() => _SearchGrid.ReversalSearchState();

        /// <include file='Include_Tag.xml' path='Tab/Members/Member[@Name="ShowSearchGrid"]/*'/>
        public virtual void ShowSearchGrid() => SearchForm.ShowSearchGrid();

        /// <include file='Include_Tag.xml' path='Tab/Members/Member[@Name="SetSearchGridSize"]/*'/>
        public virtual void SetSearchGridSize() => SearchForm.SetSearchGridSize();

        /// <include file='Include_Tag.xml' path='Tab/Members/Member[@Name="SetSearchGridLocation"]/*'/>
        public virtual void SetSearchGridLocation() => SearchForm.SetSearchGridLocation();
    }
}
