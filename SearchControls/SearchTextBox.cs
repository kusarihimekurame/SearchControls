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
    /// <summary>
    /// 带有模糊查找(DataGridView)的TextBox
    /// </summary>
    /// <example>使用模糊查找文本框的示例
    /// <code>
    ///     class TestForm : Form
    ///     {
    ///         private Search.SearchTextBox searchTextBox1;
    ///         private System.Windows.Forms.TextBox textBox1;
    ///         private System.Data.DataTable dataTable1;
    ///         
    ///         public TestForm()
    ///         {
    ///             searchTextBox1 = new Search.SearchTextBox(); //初始化
    ///             searchTextBox1.DataSource = dataTable1; //绑定数据源
    ///             searchTextBox1.DisplayDataName = "MC";  //需要展示的列名
    ///             searchTextBox1.AutoInputDataName = "DM"; //需要自动输入的列名
    ///         }
    ///     }
    /// </code>
    /// </example>
    [
        DisplayName("SearchTextBox"),
        Description("带有模糊查找(DataGridView)的TextBox"),
        DefaultEvent("GridSelecting"),
        ComplexBindingProperties("DataSource", "DataMember"),
        ToolboxItem(true)
    ]
    public partial class SearchTextBox : TextBox
    {
        private SearchForm SearchForm;
        private Rectangle TopLocation => TopLevelControl.RectangleToScreen(TopLevelControl.ClientRectangle);

        /// <summary>
        /// 多重选择的列
        /// </summary>
        /// <remarks>
        /// 源代码
        /// <code>
        /// new DataGridViewCheckBoxColumn
        /// {
        ///     Name = "Select",
        ///     DataPropertyName = "Select",
        ///     HeaderText = "选择",
        ///     Width = 30,
        ///     FalseValue = false,
        ///     TrueValue = true,
        ///     ValueType = typeof(bool)
        /// };
        /// </code>
        /// </remarks>
        public DataGridViewCheckBoxColumn MultSelectColumn { get; } = new DataGridViewCheckBoxColumn
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
        private bool _IsMultSelect = false;
        /// <summary>
        /// 是否多选
        /// </summary>
        /// <value>
        /// <para><c>IsMultSelect</c>为<c>True </c>时，添加多选框</para>
        /// <para><c>IsMultSelect</c>为<c>False</c>时，只能单选</para>
        /// </value>
        [
            DefaultValue(false),
            Category("Search"),
            Description("是否多选")
        ]
        public bool IsMultSelect
        {
            get => _IsMultSelect;
            set
            {
                _IsMultSelect = value;
                if (_IsMultSelect && !SearchGrid.Columns.Contains(MultSelectColumn))
                {
                    SearchGrid.Columns.Insert(0, MultSelectColumn);
                    if (SearchGrid.DataSource != null) SetMultSelectColumn(true);
                    SearchGrid.Sort(SearchGrid.Columns[MultSelectColumn.Name], ListSortDirection.Descending);
                }
                else if (!_IsMultSelect && SearchGrid.Columns.Contains(MultSelectColumn))
                {
                    SearchGrid.Columns.Remove(MultSelectColumn);
                    if (SearchGrid.DataSource != null) SetMultSelectColumn(false);
                }
            }
        }
        /// <summary>
        /// 多重选择的分隔符
        /// </summary>
        [
            DefaultValue(";"),
            Category("Search"),
            Description("多重选择的分隔符")
        ]
        public string MultSelectSplit { get; set; } = ";";
        /// <summary>
        /// 附属的文本框集合
        /// </summary>
        /// <example>添加附属文本框的方法<see cref="SubSearchTextBoxCollection.Add(TextBox, string, string)"/>
        /// <code>
        ///     class TestForm : Form
        ///     {
        ///         private Search.SearchTextBox searchTextBox1;
        ///         private System.Data.DataTable dataTable1;
        ///         
        ///         public TestForm()
        ///         {
        ///             searchTextBox1 = new Search.SearchTextBox(); //初始化
        ///             searchTextBox1.DataSource = dataTable1; //绑定数据源
        ///             searchTextBox1.DisplayDataName = "DM";  //需要展示的列名
        ///             searchTextBox1.SubSearchTextBoxes.Add(textBox1, "MC", "DM");
        ///         }
        ///     }
        /// </code>
        /// </example>
        [
            Editor(typeof(SubSearchTextBoxCollectionEditor), typeof(UITypeEditor)),
            DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
            MergableProperty(false),
            Category("Search"),
            Description("附属的文本框")
        ]
        public SubSearchTextBoxCollection SubSearchTextBoxes { get; } = new SubSearchTextBoxCollection();
        /// <summary>
        /// 模糊查找的DataGridView表<see cref="DataGridView"/>
        /// </summary>
        [
            EditorBrowsable(EditorBrowsableState.Always),
            DesignerSerializationVisibility(DesignerSerializationVisibility.Visible),
            Category("Search"),
            Description("模糊查找的DataGridView表")
        ]
        public DataGridView SearchGrid => SearchForm.SearchGrid;
        /// <summary>
        /// 模糊查找表格的列<see cref="DataGridViewColumnCollection"/>
        /// </summary>
        [
            DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
            Category("Search"),
            Description("模糊查找表格的列")
        ]
        public DataGridViewColumnCollection Columns => SearchGrid.Columns;
        /// <summary>
        /// 根据文本框的内容返回表格当前的行<see cref="DataGridViewRow"/>
        /// </summary>
        /// <remarks>
        /// 源代码
        /// <code>
        ///     public partial class SearchTextBox : TextBox
        ///     {
        ///         public DataGridViewRow CurrentRow
        ///         {
        ///             get
        ///             {
        ///                 DataGridViewRow dgvr;
        ///                 if (string.IsNullOrWhiteSpace(this.Text)) return SearchGrid.CurrentRow;
        ///                 if (string.IsNullOrWhiteSpace(DisplayDataName)) 
        ///                 {
        ///                     dgvr = SearchGrid.Rows.Cast&lt;DataGridViewRow&gt;()
        ///                             .FirstOrDefault(_dgvr => 
        ///                                 _dgvr.Cells.Cast&lt;DataGridViewCell&gt;().Select(_dgvc=>_dgvc.Value.ToString()).Any(item=>item.Equals(this.Text))
        ///                             );
        ///                 }
        ///                 else
        ///                 {
        ///                     dgvr = SearchGrid.Rows.Cast&lt;DataGridViewRow&gt;().FirstOrDefault(_dgvr => _dgvr.Cells[DisplayDataName].Value.Equals(this.Text));
        ///                 }
        ///                 return dgvr == null ? SearchGrid.CurrentRow : dgvr;
        ///             }
        ///         }
        ///     }
        /// </code>
        /// </remarks>
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
                                _dgvr.Cells.Cast<DataGridViewCell>().Select(_dgvc=>_dgvc.Value.ToString()).Any(item=>item.Equals(Text))
                            );
                }
                else
                {
                    dgvr = SearchGrid.Rows.Cast<DataGridViewRow>().FirstOrDefault(_dgvr => _dgvr.Cells[DisplayDataName].Value.Equals(Text));
                }
                return dgvr == null ? SearchGrid.CurrentRow : dgvr;
            }
        }

        /// <summary>
        /// 是否将表朝上
        /// </summary>
        /// <value>
        /// <para><c>IsUp</c>为true 时，表在文本框的上面</para>
        /// <para><c>IsUp</c>为false时，表在文本框的下面</para>
        /// </value>
        [
            DefaultValue(false),
            Category("Search"),
            Description("是否将表朝上")
        ]
        public bool IsUp { get; set; } = false;
        private string _DisplayDataName;
        /// <summary>
        /// 需要在文本框中展示的对应的列名
        /// </summary>
        /// <remarks>
        /// 当选择完之后需要自动写入文本框内的列名
        /// </remarks>
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
        /// <summary>
        /// 自动输入对应的列名
        /// </summary>
        /// <remarks>
        /// 根据文本框中的内容是否与对应的列中的数据一致，自动选择对应的内容
        /// </remarks>
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
        /// <summary>
        /// 是否进行模糊查找
        /// </summary>
        /// <value>
        /// <para><c>IsTextChanged</c>为<c>True </c>时，自动对所有列中的内容进行模糊查找</para>
        /// <para><c>IsTextChanged</c>为<c>false</c>时，不会进行模糊查找</para>
        /// </value>
        [
            DefaultValue(true),
            Category("Search"),
            Description("是否进行模糊查找")
        ]
        public bool IsTextChanged { get; set; } = true;
        /// <summary>
        /// 是否当输入内容完全正确时，自动触发选择相应内容
        /// </summary>
        /// <seealso cref="AutoInputDataName"/>
        [
            DefaultValue(true),
            Category("Search"),
            Description("当输入内容完全正确时，自动触发GridSelectedEnter(dataGridView,第0行)")
        ]
        public bool IsAutoInput { get; set; } = true;
        /// <summary>
        /// 想要显示的行数
        /// </summary>
        [
            DefaultValue(15),
            Category("Search"),
            Description("想要显示的行数")
        ]
        public int DisplayRowCount { get; set; } = 15;
        /// <summary>
        /// 模糊查找框显示的时候是否自动还原到原来的状态
        /// </summary>
        /// <value>
        /// <para><c>IsAutoReset</c>为<c>True </c>时，列表显示时自动还原到初始状态，显示列表全部内容</para>
        /// <para><c>IsAutoReset</c>为<c>false</c>时，列表显示时是模糊查找以后的状态</para>
        /// </value>
        /// <seealso cref="Reset"/>
        [
            DefaultValue(true),
            Category("Search"),
            Description("模糊查找框显示的时候是否自动还原到原来的状态")
        ]
        public bool IsAutoReset { get; set; } = true;

        private IEnumerable _OriginSource;
        /// <summary>
        /// 设置或者获取SearchGrid的数据源<see cref="DataGridView.DataSource"/>
        /// </summary>
        [
            DefaultValue(null),
            AttributeProvider(typeof(IListSource)),
            Category("Search"),
            Description("设置或者获取SearchGrid的数据源")
        ]
        public object DataSource
        {
            get => ((SearchGrid)SearchGrid).DataSource;
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
                        ((SearchGrid)SearchGrid).DataSource = value;

                        DataTable dt = null;
                        if (((SearchGrid)SearchGrid).DataSource is DataTable _dt) dt = _dt;
                        else if (((SearchGrid)SearchGrid).DataSource is DataView _dv) dt = _dv.Table;
                        else if (((SearchGrid)SearchGrid).DataSource is IEnumerable ie) _OriginSource = ie;
                        else if (((SearchGrid)SearchGrid).DataSource is DataSet _ds)
                        {
                            void AddPinYinColumns(object sender, CollectionChangeEventArgs e)
                            {
                                _ds.Tables.AsParallel().Cast<DataTable>().ToList().ForEach(dt_ =>
                                {
                                    dt_.Columns.Cast<DataColumn>().Where(dc => dc.ColumnName.Contains("PY_")).ToList().ForEach(dc =>
                                    {
                                        DataGridViewTextBoxColumn dgvc = new DataGridViewTextBoxColumn()
                                        {
                                            Name = dc.ColumnName,
                                            DataPropertyName = dc.ColumnName,
                                            Visible = false
                                        };
                                        if (!SearchGrid.Columns.Contains(dgvc.Name)) SearchGrid.Columns.Add(dgvc);
                                    });
                                });
                            }
                            AddPinYinColumns(null, null);
                            _ds.Tables.CollectionChanged += AddPinYinColumns;
                        }

                        if (dt != null && !SearchGrid.AutoGenerateColumns)
                        {
                            dt.Columns.Cast<DataColumn>().Where(dc => dc.ColumnName.Contains("PY_")).ToList().ForEach(dc =>
                            {
                                DataGridViewTextBoxColumn dgvc = new DataGridViewTextBoxColumn()
                                {
                                    Name = dc.ColumnName,
                                    DataPropertyName = dc.ColumnName,
                                    Visible = false
                                };
                                SearchGrid.Columns.Add(dgvc);
                            });
                        }
                        if (IsMultSelect)
                        {
                            SetMultSelectColumn(true);
                            SearchGrid.Sort(SearchGrid.Columns[MultSelectColumn.Name], ListSortDirection.Descending);
                        }
                    }
                };
                action();
            }
        }

        /// <summary>
        /// 获取或设置 System.Windows.Forms.DataGridView 正在为其显示数据的数据源中的列表或表的名称。
        /// </summary>
        [
            DefaultValue(""),
            Editor("System.Windows.Forms.Design.DataMemberListEditor, System.Design, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", typeof(UITypeEditor)),
            Category("Search"),
            Description("获取或设置 System.Windows.Forms.DataGridView 正在为其显示数据的数据源中的列表或表的名称。")
        ]
        public string DataMember
        {
            get => ((SearchGrid)SearchGrid).DataMember;
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
                        ((SearchGrid)SearchGrid).DataMember = value;
                    }
                };
                action();
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

        /// <summary>
        /// 当表格被点击或者按确定时，执行委托方法
        /// </summary>
        /// <example>
        /// 事件执行普通方法
        /// <code>
        /// class TestForm : Form
        /// {
        ///     private Search.SearchTextBox searchTextBox1;
        ///     private System.Windows.Forms.TextBox textBox1;
        ///     private System.Data.DataTable dataTable1;
        ///         
        ///     public TestForm()
        ///     {
        ///         searchTextBox1 = new Search.SearchTextBox(); //初始化
        ///         searchTextBox1.DataSource = dataTable1; //绑定数据源
        ///         searchTextBox1.DisplayDataName = "MC";  //需要展示的列名
        ///         searchTextBox1.AutoInputDataName = "DM"; //需要自动输入的列名
        ///         
        ///         searchTextBox1.GridSelecting += searchTextBox1_GridSelecting;
        ///     }
        ///     
        ///     private void searchTextBox1_GridSelecting(object sender, GridSelectingEventArgs e)
        ///     {
        ///         e.Handled = true;  //已经处理此事件(手动设置选择以后需要处理的步骤)
        ///         if (sender is TextBox tb)
        ///         {
        ///             //tb.Text = e.SearchGrid["MC", e.RowIndex].Value.ToString();
        ///             tb.Text = e.CurrentRow.Cells["MC"].Value.ToString();
        /// 
        ///             textBox1.Focus();
        ///         }
        ///     }
        /// }
        /// </code>
        /// 事件执行的Linq方法
        /// <code>
        /// class TestForm : Form
        /// {
        ///     private Search.SearchTextBox searchTextBox1;
        ///     private System.Windows.Forms.TextBox textBox1;
        ///     private System.Data.DataTable dataTable1;
        ///         
        ///     public TestForm()
        ///     {
        ///         searchTextBox1 = new Search.SearchTextBox(); //初始化
        ///         searchTextBox1.DataSource = dataTable1; //绑定数据源
        ///         searchTextBox1.DisplayDataName = "MC";  //需要展示的列名
        ///         searchTextBox1.AutoInputDataName = "DM"; //需要自动输入的列名
        ///         
        ///         searchTextBox1.GridSelecting += (sender, e) => 
        ///         {
        ///             e.Handled = true;  //已经处理此事件(手动设置选择以后需要处理的步骤)
        ///             if (sender is TextBox tb)
        ///             {
        ///                 //tb.Text = e.SearchGrid["MC", e.RowIndex].Value.ToString();
        ///                 tb.Text = e.CurrentRow.Cells["MC"].Value.ToString();
        /// 
        ///                 textBox1.Focus();
        ///             }
        ///         };
        ///     }
        /// }
        /// </code>
        /// </example>
        /// <seealso cref="GridSelecting"/>
        public delegate void GridSelectingEventHandler(object sender, GridSelectingEventArgs e);
        private GridSelectingEventHandler onGridSelecting;
        /// <summary>
        /// 当选择完某一行时发生（鼠标点击和键盘确定键都会触发）。
        /// </summary>
        /// <example>
        /// 事件执行普通方法
        /// <code>
        /// class TestForm : Form
        /// {
        ///     private Search.SearchTextBox searchTextBox1;
        ///     private System.Windows.Forms.TextBox textBox1;
        ///     private System.Data.DataTable dataTable1;
        ///         
        ///     public TestForm()
        ///     {
        ///         searchTextBox1 = new Search.SearchTextBox(); //初始化
        ///         searchTextBox1.DataSource = dataTable1; //绑定数据源
        ///         searchTextBox1.DisplayDataName = "MC";  //需要展示的列名
        ///         searchTextBox1.AutoInputDataName = "DM"; //需要自动输入的列名
        ///         
        ///         searchTextBox1.GridSelecting += searchTextBox1_GridSelecting;
        ///     }
        ///     
        ///     private void searchTextBox1_GridSelecting(object sender, GridSelectingEventArgs e)
        ///     {
        ///         e.Handled = true;  //已经处理此事件(手动设置选择以后需要处理的步骤)
        ///         if (sender is TextBox tb)
        ///         {
        ///             //tb.Text = e.SearchGrid["MC", e.RowIndex].Value.ToString();
        ///             tb.Text = e.CurrentRow.Cells["MC"].Value.ToString();
        /// 
        ///             textBox1.Focus();
        ///         }
        ///     }
        /// }
        /// </code>
        /// 事件执行的Linq方法
        /// <code>
        /// class TestForm : Form
        /// {
        ///     private Search.SearchTextBox searchTextBox1;
        ///     private System.Windows.Forms.TextBox textBox1;
        ///     private System.Data.DataTable dataTable1;
        ///         
        ///     public TestForm()
        ///     {
        ///         searchTextBox1 = new Search.SearchTextBox(); //初始化
        ///         searchTextBox1.DataSource = dataTable1; //绑定数据源
        ///         searchTextBox1.DisplayDataName = "MC";  //需要展示的列名
        ///         searchTextBox1.AutoInputDataName = "DM"; //需要自动输入的列名
        ///         
        ///         searchTextBox1.GridSelecting += (sender, e) => 
        ///         {
        ///             e.Handled = true;  //已经处理此事件(手动设置选择以后需要处理的步骤)
        ///             if (sender is TextBox tb)
        ///             {
        ///                 //tb.Text = e.SearchGrid["MC", e.RowIndex].Value.ToString();
        ///                 tb.Text = e.CurrentRow.Cells["MC"].Value.ToString();
        /// 
        ///                 textBox1.Focus();
        ///             }
        ///         };
        ///     }
        /// }
        /// </code>
        /// </example>
        [
            Category("Search"),
            Description("当选择完某一行时发生（鼠标点击和键盘确定键都会触发）。")
        ]
        public event GridSelectingEventHandler GridSelecting
        {
            add => onGridSelecting += value;
            remove => onGridSelecting -= value;
        }
        /// <summary>
        /// 引发 Search.SearchTextBox.GridSelecting 事件。
        /// </summary>
        /// <param name="e">包含事件数据的 System.ComponentModel.CollectionChangeEventArgs。</param>
        protected virtual void OnGridSelecting(GridSelectingEventArgs e)
        {
            onGridSelecting?.Invoke(this, e);
        }


        /// <summary>
        /// 当表格调整完位置和高宽后，执行对应的事件
        /// </summary>
        public delegate void SearchGridLocationSizeChangedEventHandler(object sender, SubFormLocationSizeEventArgs e);
        private SearchGridLocationSizeChangedEventHandler onSearchGridLocationSizeChanged;
        /// <summary>
        /// 当表格调整完位置和高宽后
        /// </summary>
        [
            Category("Search"),
            Description("当表格调整完位置和高宽后")
        ]
        public event SearchGridLocationSizeChangedEventHandler SearchGridLocationSizeChanged
        {
            add => onSearchGridLocationSizeChanged += value;
            remove => onSearchGridLocationSizeChanged -= value;
        }
        /// <summary>
        /// 引发 Search.SearchTextBox.SearchGridLocationSizeChanged 事件。
        /// </summary>
        /// <param name="e">包含事件数据的System.EventArgs</param>
        protected virtual void OnSearchGridLocationSizeChanged(SubFormLocationSizeEventArgs e)
        {
            onSearchGridLocationSizeChanged?.Invoke(this, e);
        }

        /// <summary>
        /// SearchTextBox初始化
        /// </summary>
        /// <example>使用模糊查找文本框的示例<see cref="SearchTextBox"/>
        /// <code>
        ///     class TestForm : Form
        ///     {
        ///         private Search.SearchTextBox searchTextBox1;
        ///         private System.Data.DataTable dataTable1;
        ///         
        ///         public TestForm()
        ///         {
        ///             searchTextBox1 = new Search.SearchTextBox(); //初始化
        ///             searchTextBox1.DataSource = dataTable1; //绑定数据源
        ///             searchTextBox1.DisplayDataName = "MC";  //需要展示的列名
        ///             searchTextBox1.AutoInputDataName = "DM"; //需要自动输入的列名
        ///         }
        ///     }
        /// </code>
        /// </example>
        public SearchTextBox()
        {
            InitializeComponent();
            
            TextChanged += TextBox_TextChanged;
            MouseClick += TextBox_MouseClick;
            MouseDoubleClick += TextBox_MouseDoubleClick;
            KeyDown += TextBox_KeyDown;
            Enter += (sender, e) =>
            {
                IsEnter = true;
                Show_event(sender, e);
            };
            Leave += Hide_event2;
            
            SearchForm = new SearchForm();
            SearchForm.VisibleChanged += SearchForm_VisibleChanged;

            SearchGrid.CellValueChanged += SearchGrid_CellValueChanged;
            SearchGrid.CellClick += SearchGrid_CellClick;

            SubSearchTextBoxes.CollectionChanged += SubSearchTextBoxes_CollectionChanged;
        }

        private void SubSearchTextBoxes_CollectionChanged(object sender, CollectionChangeEventArgs e)
        {
            switch (e.Action)
            {
                case CollectionChangeAction.Add:
                    if (e.Element is SubSearchTextBox sstb)
                    {
                        sstb.TextBox.TextChanged += TextBox_TextChanged;
                        sstb.TextBox.MouseClick += TextBox_MouseClick;
                        sstb.TextBox.MouseDoubleClick += TextBox_MouseDoubleClick;
                        sstb.TextBox.KeyDown += TextBox_KeyDown;
                        sstb.TextBox.Enter -= Hide_event;
                        sstb.TextBox.Click -= Hide_event;
                        sstb.TextBox.Click += Show_event;
                        sstb.TextBox.Enter += Show_event;
                        sstb.TextBox.Leave += Hide_event2;
                        sstb.TextBox.KeyPress += TextBox_KeyPress;
                    }
                    else if (e.Element is IEnumerable<SubSearchTextBox> sstbs)
                    {
                        sstbs.ToList().ForEach(_sstb =>
                        {
                            _sstb.TextBox.TextChanged += TextBox_TextChanged;
                            _sstb.TextBox.MouseClick += TextBox_MouseClick;
                            _sstb.TextBox.MouseDoubleClick += TextBox_MouseDoubleClick;
                            _sstb.TextBox.KeyDown += TextBox_KeyDown;
                            _sstb.TextBox.Enter -= Hide_event;
                            _sstb.TextBox.Click -= Hide_event;
                            _sstb.TextBox.Click += Show_event;
                            _sstb.TextBox.Enter += Show_event;
                            _sstb.TextBox.Leave += Hide_event2;
                            _sstb.TextBox.KeyPress += TextBox_KeyPress;
                        });
                    }
                    break;
                case CollectionChangeAction.Remove:
                    if (e.Element is SubSearchTextBox sstbr)
                    {
                        sstbr.TextBox.TextChanged -= TextBox_TextChanged;
                        sstbr.TextBox.MouseClick -= TextBox_MouseClick;
                        sstbr.TextBox.MouseDoubleClick -= TextBox_MouseDoubleClick;
                        sstbr.TextBox.KeyDown -= TextBox_KeyDown;
                        sstbr.TextBox.Enter += Hide_event;
                        sstbr.TextBox.Click += Hide_event;
                        sstbr.TextBox.Click -= Show_event;
                        sstbr.TextBox.Enter -= Show_event;
                        sstbr.TextBox.Leave -= Hide_event2;
                        sstbr.TextBox.KeyPress -= TextBox_KeyPress;
                    }
                    else if (e.Element is IEnumerable<SubSearchTextBox> sstbs)
                    {
                        sstbs.ToList().ForEach(_sstb =>
                        {
                            _sstb.TextBox.TextChanged -= TextBox_TextChanged;
                            _sstb.TextBox.MouseClick -= TextBox_MouseClick;
                            _sstb.TextBox.MouseDoubleClick -= TextBox_MouseDoubleClick;
                            _sstb.TextBox.KeyDown -= TextBox_KeyDown;
                            _sstb.TextBox.Enter += Hide_event;
                            _sstb.TextBox.Click += Hide_event;
                            _sstb.TextBox.Click -= Show_event;
                            _sstb.TextBox.Enter -= Show_event;
                            _sstb.TextBox.Leave -= Hide_event2;
                            _sstb.TextBox.KeyPress -= TextBox_KeyPress;
                        });
                    }
                    break;
                case CollectionChangeAction.Refresh:
                    if (sender is SubSearchTextBoxCollection sstbc)
                    {
                        sstbc.ForEach(_sstb =>
                        {
                            _sstb.TextBox.TextChanged -= TextBox_TextChanged;
                            _sstb.TextBox.MouseClick -= TextBox_MouseClick;
                            _sstb.TextBox.MouseDoubleClick -= TextBox_MouseDoubleClick;
                            _sstb.TextBox.KeyDown -= TextBox_KeyDown;
                            _sstb.TextBox.Enter += Hide_event;
                            _sstb.TextBox.Click += Hide_event;
                            _sstb.TextBox.Click -= Show_event;
                            _sstb.TextBox.Enter -= Show_event;
                            _sstb.TextBox.Leave -= Hide_event2;
                            _sstb.TextBox.KeyPress -= TextBox_KeyPress;
                        });
                    }
                    break;
            }
        }

        private void SearchForm_VisibleChanged(object sender, EventArgs e)
        {
            if (IsAutoReset && SearchForm.Visible)
            {
                Reset();
            }
        }

        /// <summary>
        /// 将模糊查找还原到原来的状态
        /// </summary>
        /// <remarks>
        /// <c>DataTable.DefaultView.RowFilter = "";</c>
        /// </remarks>
        public void Reset()
        {
            DataTable dt = null;
            if (SearchGrid.DataSource is DataTable _dt) dt = _dt;
            else if (SearchGrid.DataSource is DataView _dv) dt = _dv.Table;
            else if (SearchGrid.DataSource is DataSet _ds) dt = _ds.Tables[SearchGrid.DataMember];
            else if (SearchGrid.DataSource is IEnumerable) SearchGrid.DataSource = _OriginSource;
            if (dt != null)
            {
                dt.DefaultView.RowFilter = "";
                ShowSearchGrid();
            }
        }

        /// <summary>
        /// 引发 System.Windows.Forms.Control.CreateControl 事件。
        /// </summary>
        protected override void OnCreateControl()
        {
            base.OnCreateControl();
            if (TopLevelControl != null)
            {
                TopLevelControl.Move += (sender, e) => SetSearchGridLocation();
                TopLevelControl.Enter += Hide_event;
                TopLevelControl.Click += Hide_event;

                if (TopLevelControl is Form f)
                {
                    f.Load += (sender, e) => Click_Add_Visible_event(this);
                }
            }
        }

        private void Show_event(object sender, EventArgs e)
        {
            if (!SearchForm.Visible) SearchForm.Show(TopLevelControl);
        }

        private void Hide_event(object sender, EventArgs e)
        {
            #region 隐藏小表

            if (sender is Control cl)
            {
                if (cl.Text != "退出")
                {
                    if (SearchForm.Visible) SearchForm.Visible = false;
                }
            }

            #endregion
        }

        private void Hide_event2(object sender, EventArgs e)
        {
            #region 隐藏小表

            if (sender is Control cl)
            {
                if (cl.Text != "退出" && SubSearchTextBoxes.Select(sstb => sstb.TextBox).All(tb => !tb.Focused) && !Focused)
                {
                    if (SearchForm.Visible) SearchForm.Visible = false;
                }
            }

            #endregion
        }

        private void Click_Add_Visible_event(Control cl)
        {
            if (cl != null && cl.Parent != null && cl.TopLevelControl != null)
            {
                cl.Parent.Controls.Cast<Control>().Where(c => !c.Equals(this) && SubSearchTextBoxes.Select(sstb => sstb.TextBox).All(tb => !tb.Equals(c))).ToList().ForEach(c =>
                {
                    c.Enter += Hide_event;
                    c.Click += Hide_event;
                });
                if (cl.Parent != TopLevelControl)
                {
                    Click_Add_Visible_event(cl.Parent);
                }
            }
        }

        private bool IsEnter = false;
        private int _SelectionStart;
        private int _SelectionLength;

        /// <summary>
        /// 反转搜索状态
        /// </summary>
        public void ReversalSearchState()
        {
            DataTable dt = null;
            if (SearchGrid.DataSource is DataTable _dt) dt = _dt;
            else if (SearchGrid.DataSource is DataView _dv) dt = _dv.Table;
            else if (SearchGrid.DataSource is DataSet _ds) dt = _ds.Tables[DataMember];

            if (dt != null && (Focused || SubSearchTextBoxes.Any(sstb => sstb.TextBox.Focused)))
            {
                TextBox tb;
                if (Focused) tb = this;
                else tb = SubSearchTextBoxes.First(sstb => sstb.TextBox.Focused).TextBox;
                if (string.IsNullOrEmpty(dt.DefaultView.RowFilter)) TextBox_TextChanged(tb, null);
                else
                {
                    dt.DefaultView.RowFilter = null;
                    ShowSearchGrid();
                }
            }
        }

        private void TextBox_MouseClick(object sender, MouseEventArgs e)
        {
            if (sender is TextBox tb)
            {
                if (e != null)
                {
                    if (IsEnter) IsEnter = false;
                    else
                    {
                        if (SearchForm.Visible) ReversalSearchState();
                        else Show_event(sender, new EventArgs());
                    }
                }

                _SelectionStart = tb.SelectionStart;
                _SelectionLength = tb.SelectionLength;
            }
        }

        private void TextBox_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (sender is TextBox tb)
            {
                ReversalSearchState();

                if (tb.Text.Contains(MultSelectSplit))
                {
                    string[] Texts = tb.Text.Split(new string[] { MultSelectSplit }, StringSplitOptions.None);
                    GetSelectedText(Texts, out int LocationStart, out int Index);
                    tb.Select(LocationStart, Texts[Index].Length);
                }
                else
                {
                    tb.SelectAll();
                }
            }
        }

        /// <summary>
        /// 获得光标所在处的词组信息
        /// </summary>
        /// <param name="Texts">用;分隔的数组</param>
        /// <param name="LocationStart">词组最开始的位置</param>
        /// <param name="Index">光标所在的数组的位置</param>
        public void GetSelectedText(string[] Texts, out int LocationStart, out int Index)
        {
            LocationStart = 0;
            Index = 0;
            for (int i = 0; i < Texts.Count(); i++)
            {
                LocationStart += Texts[i].Length + MultSelectSplit.Length;
                if (LocationStart > _SelectionStart)
                {
                    LocationStart -= Texts[i].Length + MultSelectSplit.Length;
                    Index = i;
                    break;
                }
            }
        }

        internal int[] location(Control c)
        {
            int[] l1 = new int[2];
            if (c != TopLevelControl)
            {
                l1[0] = c.Location.X;
                l1[1] = c.Location.Y;
                if (c is TabPage) l1[0]++;

                if (c.Parent != TopLevelControl)
                {
                    int[] l2 = location(c.Parent);
                    l1[0] += l2[0];
                    l1[1] += l2[1];
                }
            }
            return l1;
        }

        /// <summary>
        /// 显示并设置网格高宽
        /// </summary>
        public void ShowSearchGrid()
        {
            #region 显示并设置网格位置和高宽

            SetSearchGridSize();

            SetSearchGridLocation();

            OnSearchGridLocationSizeChanged(new SubFormLocationSizeEventArgs(SearchForm));

            #endregion
        }

        /// <summary>
        /// 设置网格大小
        /// </summary>
        public void SetSearchGridSize()
        {
            int irowcount = SearchGrid.RowCount;

            SearchForm.Width = ScrollBars.Equals(ScrollBars.Vertical) ? Columns.GetColumnsWidth(DataGridViewElementStates.Visible) + 3 :
                                                             Columns.GetColumnsWidth(DataGridViewElementStates.Visible) + 20;  // 计算有展示的列的总宽

            if (irowcount > 0)
            {
                SearchForm.Visible = true;
                SearchGrid.ClearSelection();
                SearchGrid.Rows[0].Selected = true;     // 选中网格第一行
            }
            else
            {
                SearchForm.Visible = false;
            }

            if (irowcount <= DisplayRowCount)
            {
                if (SearchGrid.ColumnHeadersVisible) SearchForm.Height = SearchGrid.Rows.GetRowsHeight(DataGridViewElementStates.Visible) + 3 + SearchGrid.ColumnHeadersHeight;
                else SearchForm.Height = SearchGrid.Rows.GetRowsHeight(DataGridViewElementStates.Visible) + 3;
                if (!ScrollBars.Equals(ScrollBars.Vertical)) SearchForm.Width -= 17;  // 设置网格总宽(由于没有上下滚动条，所以总宽要减去 17)
            }
            else
            {
                if (SearchGrid.ColumnHeadersVisible) SearchForm.Height = SearchGrid.Rows.Cast<DataGridViewRow>().Take(DisplayRowCount).Sum(dgvr => dgvr.Height) + 3 + SearchGrid.ColumnHeadersHeight;
                else SearchForm.Height = SearchGrid.Rows.Cast<DataGridViewRow>().Take(DisplayRowCount).Sum(dgvr => dgvr.Height) + 3;  // 重新设置网格总高
            }
        }

        /// <summary>
        /// 设置网格位置
        /// </summary>
        public void SetSearchGridLocation()
        {
            int[] l = location(Parent);

            if (IsUp)
            {
                SearchForm.Location = new Point(TopLocation.X + l[0] + Location.X, TopLocation.Y + l[1] + Location.Y - SearchForm.Height);
            }
            else
            {
                SearchForm.Location = new Point(TopLocation.X + l[0] + Location.X, TopLocation.Y + l[1] + Location.Y + Height);
            }
        }

        private void TextBox_TextChanged(object sender, EventArgs e)
        {
            if (SearchGrid.DataSource == null && SearchGrid.RowCount.Equals(0) || !IsTextChanged) return;

            SearchForm.VisibleChanged -= SearchForm_VisibleChanged;

            DataTable dt = null;
            if (SearchGrid.DataSource is DataTable _dt) dt = _dt;
            else if (SearchGrid.DataSource is DataView _dv) dt = _dv.Table;
            else if (SearchGrid.DataSource is DataSet _ds) dt = _ds.Tables[DataMember];

            if (sender is TextBox tb)
            {
                SearchGrid.CellValueChanged -= SearchGrid_CellValueChanged;

                if (string.IsNullOrEmpty(tb.Text))
                {
                    Text = "";
                    SubSearchTextBoxes.ToList().ForEach(sstb => sstb.TextBox.Text = "");
                }

                string[] XZ_Texts = tb.Text.Split(new string[] { MultSelectSplit }, StringSplitOptions.None);
                string[] Texts = tb.Text.Split(new char[] { ' ' }, StringSplitOptions.None);

                #region 多选框

                if (IsMultSelect)
                {
                    SearchGrid.Rows.Cast<DataGridViewRow>().ToList().ForEach(dgvr =>
                    {
                        dgvr.Cells[MultSelectColumn.Name].Value = dgvr.Cells.Cast<DataGridViewCell>().Select(dgvc => dgvc.Value.ToString().Trim()).Any(v => XZ_Texts.Any(t => !string.IsNullOrWhiteSpace(t) && t.Equals(v)));
                    });
                }

                #endregion

                #region 文本框的模糊查找      

                string cWHERE = "";

                TextBox_MouseClick(tb, null);
                GetSelectedText(XZ_Texts, out int LocationStart, out int Index);

                if (!IsMultSelect && tb.Text.Contains(' '))
                {
                    Texts.ToList().ForEach(t =>
                    {
                        string _cWHERE = "";
                        Columns.Cast<DataGridViewColumn>().Where(dgvc => dgvc.Visible || dgvc.Name.Contains("PY_")).ToList().ForEach(dgvc =>
                          {
                              if (!dgvc.Equals(MultSelectColumn) && !dgvc.Name.Equals("NID") && dgvc.ValueType.Equals(typeof(string)))
                                  _cWHERE += SearchGrid.DataSource is IEnumerable ?
                                    "OR " + dgvc.Name + ".Contains(@\"" + t + "\") " :
                                    "OR " + dgvc.Name + " like '%" + t.Replace("'", "''").Replace("[", "[[ ").Replace("]", " ]]").Replace("*", "[*]").Replace("%", "[%]").Replace("[[ ", "[[]").Replace(" ]]", "[]]") + "%' "; //字段模糊查找
                          });
                        cWHERE += "AND (" + _cWHERE.Substring(3) + ") ";
                    });
                }
                else
                {
                    Columns.Cast<DataGridViewColumn>().Where(dgvc => dgvc.Visible || dgvc.Name.Contains("PY_")).ToList().ForEach(dgvc =>
                    {
                        if (!dgvc.Name.Equals(MultSelectColumn) && !dgvc.Name.Equals("NID") && dgvc.ValueType.Equals(typeof(string)))
                            cWHERE += SearchGrid.DataSource is IEnumerable ?
                                "OR " + dgvc.Name + ".Contains(@\"" + XZ_Texts[Index] + "\") " :
                                "OR " + dgvc.Name + " like '%" + XZ_Texts[Index].Replace("'", "''").Replace("[", "[[ ").Replace("]", " ]]").Replace("*", "[*]").Replace("%", "[%]").Replace("[[ ", "[[]").Replace(" ]]", "[]]") + "%' "; //字段模糊查找
                    });
                }

                try
                {
                    if (dt != null)
                    {
                        cWHERE = IsMultSelect ? cWHERE.Substring(3) + "OR Select = true" : cWHERE.Substring(3);
                        if (!dt.DefaultView.RowFilter.Equals(cWHERE))
                            dt.DefaultView.RowFilter = cWHERE;
                    }
                    else
                    {
                        object hzpy = _OriginSource.OfType<object>().First().GetType().GetMethod("HZPY").Invoke(null, new object[] { _OriginSource, XZ_Texts[Index] });
                        DataSource = hzpy ?? _OriginSource.Where(cWHERE.Substring(3));
                    }
                }
                catch { }


                if (tb.Focused)    // 如果有焦点存在，就执行以下代码
                {
                    ShowSearchGrid();  //显示并调整高宽
                }

                #endregion

                if (e != null && IsAutoInput)
                {
                    if (SearchGrid.RowCount.Equals(1) && !string.IsNullOrWhiteSpace(AutoInputDataName) && SearchGrid.Rows[0].Cells[AutoInputDataName].Value.ToString().Equals(tb.Text, StringComparison.OrdinalIgnoreCase))
                        TextBox_KeyDown(this, new KeyEventArgs(Keys.Enter));
                }

                SearchForm.VisibleChanged += SearchForm_VisibleChanged;
                SearchGrid.CellValueChanged += SearchGrid_CellValueChanged;
            }
        }

        private void SearchGrid_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (sender is DataGridView dgv)
            {
                if (dgv.Columns[e.ColumnIndex].Name.Equals(MultSelectColumn.Name) && e.RowIndex >= 0 && !Convert.ToBoolean(dgv.Rows[e.RowIndex].Cells[e.ColumnIndex].Value))
                {
                    string _DisplayDataName = string.Empty;
                    TextBox tb = null;
                    SubSearchTextBox sstb = SubSearchTextBoxes.FirstOrDefault(_sstb => _sstb.TextBox.Focused);
                    if (Focused)
                    {
                        tb = this;
                        _DisplayDataName = DisplayDataName;
                    }
                    else if (sstb != null)
                    {
                        tb = sstb.TextBox;
                        _DisplayDataName = sstb.DisplayDataName;
                    }

                    if (string.IsNullOrEmpty(_DisplayDataName))
                    {
                        string[] Texts = tb.Text.Contains(MultSelectSplit) ? tb.Text.Split(new string[] { MultSelectSplit }, StringSplitOptions.RemoveEmptyEntries) : (new string[] { tb.Text.Trim() });
                        DataRow dr = (dgv.Rows[e.RowIndex].DataBoundItem as DataRowView).Row;
                        string OldText = dr.ItemArray.Where(s => s is string).Cast<string>().SingleOrDefault(s => Texts.Count(t => t.Trim().Equals(s)).Equals(1) && !string.IsNullOrWhiteSpace(s)).Trim();
                        tb.Text = tb.Text.Replace(OldText + MultSelectSplit, "");
                    }
                    else
                    {
                        tb.Text = tb.Text.Replace(dgv.Rows[e.RowIndex].Cells[DisplayDataName].Value.ToString().Trim() + MultSelectSplit, "");
                    }
                    tb.Select(tb.TextLength, 0);
                }
            }
        }

        private void SearchGrid_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex > -1 && SearchGrid.RowCount > 0)
            {
                if(IsMultSelect && SearchGrid.Columns[e.ColumnIndex].Name.Equals(MultSelectColumn.Name))
                    TextBox_KeyDown(this, new KeyEventArgs(Keys.Space));
                else
                    TextBox_KeyDown(this, new KeyEventArgs(Keys.Enter));
            }
        }

        private void TextBox_KeyDown(object sender, KeyEventArgs e)
        {
            #region 键盘事件

            int index = 0;
            if (SearchGrid.RowCount > 0) index = SearchGrid.SelectedRows[0].Index;   // 获取选择的行数
            SearchForm.VisibleChanged -= SearchForm_VisibleChanged;

            switch (e.KeyCode)
            {
                case Keys.Up:
                    e.Handled = true;   //禁止文本框的光标移动
                    if (SearchGrid.RowCount > 0 && index > 0)
                    {
                        SearchGrid.ClearSelection();
                        SearchGrid.Rows[index - 1].Selected = true;   // 上移
                        if (index == SearchGrid.FirstDisplayedScrollingRowIndex)
                            SearchGrid.FirstDisplayedScrollingRowIndex--;   // 滚动条自动移动
                    }
                    break;
                case Keys.Down:
                    e.Handled = true;   //禁止文本框的光标移动
                    if (SearchGrid.RowCount > 0 && index >= 0 && index < SearchGrid.RowCount - 1)
                    {
                        SearchGrid.ClearSelection();
                        SearchGrid.Rows[index + 1].Selected = true;   //下移
                        if (index == SearchGrid.FirstDisplayedScrollingRowIndex + DisplayRowCount - 1)
                            SearchGrid.FirstDisplayedScrollingRowIndex++;   // 滚动条自动移动
                    }
                    break;
                case Keys.Enter:
                    if (SearchGrid.RowCount > 0)
                    {
                        TextChanged -= TextBox_TextChanged;
                        SubSearchTextBoxes.ForEach(_sstb =>
                        {
                            _sstb.TextBox.TextChanged -= TextBox_TextChanged;
                        });

                        TextBox tb = null;
                        SubSearchTextBox sstb = SubSearchTextBoxes.FirstOrDefault(_sstb => _sstb.TextBox.Focused);
                        if (Focused) tb = this;
                        else if (sstb != null) tb = sstb.TextBox;

                        GridSelectingEventArgs row = new GridSelectingEventArgs(SearchGrid, index, e);

                        OnGridSelecting(row);
                        if (!row.Handled)
                        {
                            if (IsMultSelect)
                            {
                                if(Convert.ToBoolean(SearchGrid.CurrentRow.Cells[MultSelectColumn.Name].Value))
                                {
                                    Text = Text.Substring(0, Text.Length - 1);
                                    SubSearchTextBoxes.Where(_sstb => !string.IsNullOrWhiteSpace(_sstb.DisplayDataName)).ToList().ForEach(_sstb =>
                                    {
                                        _sstb.TextBox.Text = _sstb.TextBox.Text.Substring(0, Text.Length - 1);
                                    });
                                }
                                else
                                {
                                    if (!string.IsNullOrWhiteSpace(DisplayDataName))
                                    {
                                        Text = Text + SearchGrid[DisplayDataName, index].Value.ToString().Trim();
                                    }
                                    SubSearchTextBoxes.Where(_sstb => !string.IsNullOrWhiteSpace(_sstb.DisplayDataName)).ToList().ForEach(_sstb =>
                                    {
                                        _sstb.TextBox.Text = _sstb.TextBox.Text + SearchGrid[_sstb.DisplayDataName, index].Value.ToString().Trim();
                                    });
                                }
                            }
                            else
                            {
                                if (!string.IsNullOrWhiteSpace(DisplayDataName))
                                {
                                    Text = SearchGrid[DisplayDataName, index].Value.ToString().Trim();
                                }
                                SubSearchTextBoxes.Where(_sstb => !string.IsNullOrWhiteSpace(_sstb.DisplayDataName)).ToList().ForEach(_sstb =>
                                  {
                                      _sstb.TextBox.Text = SearchGrid[_sstb.DisplayDataName, index].Value.ToString().Trim();
                                  });
                            }
                            if (tb.Focused) SendKeys.Send("{TAB}");
                        }
                        
                        TextChanged += TextBox_TextChanged;
                        SubSearchTextBoxes.ForEach(_sstb =>
                        {
                            _sstb.TextBox.TextChanged += TextBox_TextChanged;
                        });

                        SearchForm.Visible = false;
                    }
                    else
                    {
                        SendKeys.Send("{TAB}");  //没有表格时，触发Tab按键，移动焦点到下一个控件
                    }
                    break;
                case Keys.Space:
                    if (SearchGrid.Columns.Contains(MultSelectColumn.Name))
                    {
                        if (SearchGrid.RowCount > 0)
                        {
                            TextChanged -= TextBox_TextChanged;
                            SubSearchTextBoxes.ForEach(_sstb =>
                            {
                                _sstb.TextBox.TextChanged -= TextBox_TextChanged;
                            });

                            if (Convert.ToBoolean(SearchGrid.Rows[index].Cells[MultSelectColumn.Name].Value))
                            {
                                DataTable dt = null;
                                if (SearchGrid.DataSource is DataTable _dt) dt = _dt;
                                else if (SearchGrid.DataSource is DataView _dv) dt = _dv.Table;
                                else if (SearchGrid.DataSource is DataSet _ds) dt = _ds.Tables[SearchGrid.DataMember];
                                if (dt == null || string.IsNullOrEmpty(dt.DefaultView.Sort))
                                {
                                    SearchGrid.Rows[index].Cells[MultSelectColumn.Name].Value = false;
                                }
                                else
                                {
                                    if (string.IsNullOrEmpty(DisplayDataName))
                                    {
                                        object displayData = SearchGrid.Rows[index].Cells[1].Value;
                                        DataView dv = dt.DefaultView;
                                        string sort = dv.Sort;
                                        dv.Sort = "";
                                        SearchGrid.Rows.Cast<DataGridViewRow>().First(dgvr => dgvr.Cells[1].Value.Equals(displayData)).Cells[MultSelectColumn.Name].Value = false;
                                        dv.Sort = sort;
                                    }
                                    else
                                    {
                                        object displayData = SearchGrid.Rows[index].Cells[DisplayDataName].Value;
                                        DataView dv = dt.DefaultView;
                                        string sort = dv.Sort;
                                        dv.Sort = "";
                                        SearchGrid.Rows.Cast<DataGridViewRow>().First(dgvr => dgvr.Cells[DisplayDataName].Value.Equals(displayData)).Cells[MultSelectColumn.Name].Value = false;
                                        dv.Sort = sort;
                                    }
                                }
                            }
                            else
                            {
                                TextBox tb = null;
                                SubSearchTextBox sstb = SubSearchTextBoxes.FirstOrDefault(_sstb => _sstb.TextBox.Focused);
                                if (Focused) tb = this;
                                else if (sstb != null) tb = sstb.TextBox;

                                GridSelectingEventArgs row = new GridSelectingEventArgs(SearchGrid, index, e);

                                OnGridSelecting(row);
                                if (!row.Handled)
                                {
                                    TextBox_MouseDoubleClick(tb, null);
                                    if (!string.IsNullOrWhiteSpace(DisplayDataName))
                                    {
                                        SelectedText = SearchGrid[DisplayDataName, index].Value.ToString().Trim();
                                    }
                                    SubSearchTextBoxes.Where(_sstb => !string.IsNullOrWhiteSpace(_sstb.DisplayDataName)).ToList().ForEach(_sstb =>
                                    {
                                        _sstb.TextBox.SelectedText = SearchGrid[_sstb.DisplayDataName, index].Value.ToString().Trim();
                                    });
                                }

                                if (!string.IsNullOrWhiteSpace(Text) && !Text[TextLength - 1].Equals(MultSelectSplit[MultSelectSplit.Length - 1]))
                                {
                                    Text += MultSelectSplit;
                                    SelectionLength = 0;
                                    SelectionStart = TextLength;
                                }
                                SubSearchTextBoxes.Where(_sstb => !string.IsNullOrWhiteSpace(_sstb.TextBox.Text) && !_sstb.TextBox.Text.Substring(_sstb.TextBox.TextLength - 1, 1).Equals(MultSelectSplit)).ToList().ForEach(_sstb =>
                                   {
                                       _sstb.TextBox.Text = SearchGrid[_sstb.DisplayDataName, index].Value.ToString().Trim();
                                   });
                            }

                            TextChanged += TextBox_TextChanged;
                            SubSearchTextBoxes.ForEach(_sstb =>
                            {
                                _sstb.TextBox.TextChanged += TextBox_TextChanged;
                            });

                            TextBox_TextChanged(this, null);
                        }
                    }
                    break;
            }

            SearchForm.VisibleChanged += SearchForm_VisibleChanged;

            #endregion
        }

        private void TextBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (_IsMultSelect && e.KeyChar.Equals(' '))
            {
                e.Handled = true;
            }
        }
    }
}
