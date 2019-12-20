using SearchControls.Interface;
using SearchControls.SearchGridForm;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics.Contracts;
using System.Drawing;
using System.Drawing.Design;
using System.Linq;
using System.Security.Permissions;
using System.Text;
using System.Windows.Forms;

namespace SearchControls
{
    /// <include file='Include_Tag.xml' path='Tab/Members/Member[@Name="SearchDataGridView"]/*'/>
    [
        DisplayName("SearchDataGridView"),
        Description("带有模糊查找(DataGridView)的DataGridView"),
        DefaultEvent("GridSelected"),
        ComplexBindingProperties("DataSource", "DataMember"),
        ToolboxItem(true)
    ]
    public partial class SearchDataGridView : DataGridView, IGrid, IDataText
    {
        private bool _IsTextChanged;
        bool IDataText.IsTextChanged { get => _IsTextChanged; set => _IsTextChanged = value; }
        private bool _IsAutoInput;
        bool IDataText.IsAutoInput { get => _IsAutoInput; set => _IsAutoInput = value; }
        private string _AutoInputDataName;
        string IDataText.AutoInputDataName { get => _AutoInputDataName; set => _AutoInputDataName = value; }
        private string _DisplayDataName;
        string IDataText.DisplayDataName { get => _DisplayDataName; set => _DisplayDataName = value; }

        bool IGrid.IsUp { get; set; }
        
        Rectangle IGrid.Bounds => CurrentCell.OwningColumn is DataGridViewSearchTextBoxColumn stbc ?
                                    stbc.IsMain ?
                                        RectangleToScreen(GetCellDisplayRectangle(CurrentCell.ColumnIndex, CurrentCell.RowIndex, false)) :
                                        RectangleToScreen(GetCellDisplayRectangle(Columns[stbc.MainColumnName].Index, CurrentRow.Index, false))
                                    : Rectangle.Empty;
        private int _DisplayRowCount;
        int IGrid.DisplayRowCount { get => _DisplayRowCount; set => _DisplayRowCount = value; }
        /// <include file='Include_Tag.xml' path='Tab/Members/Member[@Name="IsAutoReset"]/*'/>
        [
            DefaultValue(true),
            Category("Search"),
            Description("模糊查找框显示的时候是否自动还原到原来的状态")
        ]
        public bool IsAutoReset { get; set; }
        TextBox IDataText.textBox => EditingControl as TextBox;

        private SearchForm SearchForm;
        private SearchGrid _SearchGrid => SearchForm.SearchGrid;
        /// <include file='Include_Tag.xml' path='Tab/Members/Member[@Name="SearchGrid"]/*'/>
        [
            EditorBrowsable(EditorBrowsableState.Always),
            DesignerSerializationVisibility(DesignerSerializationVisibility.Visible),
            Category("Search"),
            Description("模糊查找的DataGridView表")
        ]
        public DataGridView SearchGrid => SearchForm.SearchGrid;

        private DataSet _OriginSource;
        private string _DataMember;

        /// <include file='Include_Tag.xml' path='Tab/Members/Member[@Name="DataMember"]/*'/>
        [
            DefaultValue(""),
            Editor("System.Windows.Forms.Design.DataMemberListEditor, System.Design, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", typeof(UITypeEditor)),
            Category("Search"),
            Description("获取或设置 System.Windows.Forms.DataGridView 正在为其显示数据的数据源中的列表或表的名称。")
        ]
        public new string DataMember
        {
            get => _OriginSource == null ? _DataMember : base.DataMember;
            set
            {
                if (_OriginSource != null)
                {
                    _DataMember = value;
                    base.DataSource = _OriginSource.Tables[value];
                    OnDataMemberChanged(EventArgs.Empty);
                }
                else base.DataMember = value;
            }
        }

        /// <include file='Include_Tag.xml' path='Tab/Members/Member[@Name="DataSource"]/*'/>
        [
            DefaultValue(null),
            AttributeProvider(typeof(IListSource)),
            Category("Search"),
            Description("设置或者获取SearchGrid的数据源")
        ]
        public new object DataSource
        {
            get => _OriginSource ?? base.DataSource;
            set
            {
                if (value is DataSet _ds)
                {
                    _OriginSource = _ds;
                    if (string.IsNullOrEmpty(DataMember)) base.DataSource = _ds.Tables[DataMember];
                }
                else
                {
                    base.DataSource = value;
                    _OriginSource = null;
                }
            }
        }

        /// <include file='Include_Tag.xml' path='Tab/Members/Member[@Name="GridSelecting"]/*'/>
        [
            Category("Search"),
            Description("当选择完某一行时发生（鼠标点击和键盘确定键都会触发）。")
        ]
        public event GridSelectingEventHandler GridSelecting;
        /// <include file='Include_Tag.xml' path='Tab/Members/Member[@Name="OnGridSelecting"]/*'/>
        public virtual void OnGridSelecting(GridSelectingEventArgs e)
        {
            GridSelecting?.Invoke(this, e);
            if (!e.Handled)
            {
                e.Handled = true;

                DataGridViewSearchTextBoxColumn currentColumn = CurrentCell.OwningColumn as DataGridViewSearchTextBoxColumn;
                DataGridViewSearchTextBoxColumn MainColumn = currentColumn.IsMain ? currentColumn : Columns.Cast<DataGridViewColumn>().First(dgvc => dgvc.Name.Equals(currentColumn.MainColumnName) || dgvc.DataPropertyName.Equals(currentColumn.MainColumnName)) as DataGridViewSearchTextBoxColumn;
                Columns.Cast<DataGridViewColumn>().Where(dgvc => dgvc is DataGridViewSearchTextBoxColumn).Cast<DataGridViewSearchTextBoxColumn>().Where(stbc => !string.IsNullOrWhiteSpace(stbc.DisplayDataName) && (stbc.Equals(MainColumn) || !stbc.IsMain && (stbc.MainColumnName.Equals(MainColumn.Name) || stbc.MainColumnName.Equals(MainColumn.DataPropertyName)))).ToList().ForEach(stbc =>
                 {
                     string text = SearchGrid.Columns.Contains(stbc.DisplayDataName)
                         ? SearchGrid[stbc.DisplayDataName, e.RowIndex].Value.ToString().Trim()
                         : e.CurrentRow.Cells.Cast<DataGridViewCell>().First(dgvc => dgvc.OwningColumn.DataPropertyName.Equals(stbc.DisplayDataName)).Value.ToString().Trim();
                     if (stbc.Equals(currentColumn))
                     {
                         if (!(EditingControl as TextBox).Text.Equals(text)) (EditingControl as TextBox).Text = text;
                     }
                     else
                     {
                         if (!CurrentRow.Cells[stbc.Name].Value.Equals(text)) CurrentRow.Cells[stbc.Name].Value = text;
                     }
                 });
            }
        }

        /// <include file='Include_Tag.xml' path='Tab/Members/Member[@Name="GridSelected"]/*'/>
        [
            Category("Search"),
            Description("当选择完某一行时发生（鼠标点击和键盘确定键都会触发）。")
        ]
        public event GridSelectedEventHandler GridSelected;
        /// <include file='Include_Tag.xml' path='Tab/Members/Member[@Name="OnGridSelected"]/*'/>
        public virtual void OnGridSelected(GridSelectedEventArgs e)
        {
            EndEdit();
            //int row = CurrentRow.Index + 1;
            //if (row > RowCount - 1)      // 如果row大于总行数
            //{
            //    row = 0;
            //}
            //CurrentCell = this[CurrentCell.ColumnIndex, row];
            GridSelected?.Invoke(this, e);
        }

        /// <include file='Include_Tag.xml' path='Tab/Members/Member[@Name="SearchGridLocationSizeChanged"]/*'/>
        [
            Category("Search"),
            Description("当表格调整完位置和高宽后")
        ]
        public event SearchGridLocationSizeChangedEventHandler SearchGridLocationSizeChanged;
        /// <include file='Include_Tag.xml' path='Tab/Members/Member[@Name="OnSearchGridLocationSizeChanged"]/*'/>
        public virtual void OnSearchGridLocationSizeChanged(SearchFormLocationSizeEventArgs e)
        {
            SearchGridLocationSizeChanged?.Invoke(this, e);
        }

        /// <include file='Include_Tag.xml' path='Tab/Members/Member[@Name="SearchDataGridView"]/*'/>
        /// <summary>
        /// SearchDataGridView初始化
        /// </summary>
        public SearchDataGridView()
        {
            InitializeComponent();

            SearchForm = new SearchForm();
            SearchGrid.RowsDefaultCellStyle.BackColor = Color.Bisque;
            SearchForm.Grid = this;

            _SearchGrid.DataText = this;

            Columns.CollectionChanged += Columns_CollectionChanged;
        }

        private void Columns_CollectionChanged(object sender, CollectionChangeEventArgs e)
        {
            if (e.Action.Equals(CollectionChangeAction.Add) && e.Element is DataGridViewSearchTextBoxColumn stbc)
            {
                if (!stbc.IsMain)
                {
                    string ErrorMessage = null;
                    if (string.IsNullOrEmpty(stbc.MainColumnName))
                    {
                        ErrorMessage = "不是搜索主列的列必须要填写主列的列名(MainColumnName)";
                    }
                    else if (!Columns.Contains(stbc.MainColumnName))
                    {
                        ErrorMessage = "根据MainColumnName无法找到主列";
                    }
                    else if (!(Columns[stbc.MainColumnName] is DataGridViewSearchTextBoxColumn))
                    {
                        ErrorMessage = "主列必须是搜索列类型(DataGridViewSearchTextBoxColumn)";
                    }
                    if (!string.IsNullOrEmpty(ErrorMessage)) throw new Exception(ErrorMessage);
                }
                if (string.IsNullOrEmpty(stbc.AutoInputDataName)) stbc.AutoInputDataName = stbc.DisplayDataName;
            }
        }

        /// <summary>
        /// 引发 System.Windows.Forms.DataGridView.EditingControlShowing 事件。
        /// </summary>
        /// <param name="e">一个 System.Windows.Forms.DataGridViewEditingControlShowingEventArgs，包含有关编辑控件的信息。</param>
        /// <remarks>启动模糊查找表</remarks>
        protected override void OnEditingControlShowing(DataGridViewEditingControlShowingEventArgs e)
        {
            base.OnEditingControlShowing(e);

            if (CurrentCell.OwningColumn is DataGridViewSearchTextBoxColumn stbc)
            {
                DataGridViewSearchTextBoxColumn MainColumn = null;
                if (!stbc.IsMain) MainColumn = Columns[stbc.MainColumnName] as DataGridViewSearchTextBoxColumn;

                if (stbc.SearchColumns != null && stbc.SearchColumns.Count > 0)
                {
                    _SearchGrid.AutoGenerateColumns = false;
                    _SearchGrid.Columns.Clear();
                    _SearchGrid.Columns.AddRange(stbc.SearchColumns.ToArray());
                }
                else if (!stbc.IsMain)
                {
                    _SearchGrid.AutoGenerateColumns = false;
                    _SearchGrid.Columns.Clear();
                    _SearchGrid.Columns.AddRange(MainColumn.SearchColumns.ToArray());
                }
                else _SearchGrid.AutoGenerateColumns = true;

                _SearchGrid.DataSource = (stbc.IsMain ? stbc.SearchDataSource : MainColumn.SearchDataSource) ?? DataSource;
                _SearchGrid.DataMember = stbc.IsMain ? stbc.SearchDataMember : MainColumn.SearchDataMember;

                _IsAutoInput = stbc.IsAutoInput;
                _IsTextChanged = stbc.IsTextChanged;
                _DisplayRowCount = stbc.DisplayRowCount;
                _AutoInputDataName = stbc.AutoInputDataName;
                _DisplayDataName = stbc.DisplayDataName;
                SearchForm.Show(TopLevelControl);
            }
        }

        /// <summary>
        /// 引发 System.Windows.Forms.DataGridView.Scroll 事件。
        /// </summary>
        /// <param name="e">包含事件数据的 System.Windows.Forms.ScrollEventArgs。</param>
        protected override void OnScroll(ScrollEventArgs e)
        {
            base.OnScroll(e);
            if (EditingControl is TextBox && CurrentCell.Displayed)
            {
                ShowSearchGrid();
            }
            else
            {
                SearchForm.Visible = false;
            }
        }

        /// <summary>
        /// 引发 System.Windows.Forms.DataGridView.CellEndEdit 事件。
        /// </summary>
        /// <param name="e">包含事件数据的 System.Windows.Forms.DataGridViewCellEventArgs。</param>
        /// <remarks>隐藏模糊查找表</remarks>
        protected override void OnCellEndEdit(DataGridViewCellEventArgs e)
        {
            SearchForm.Visible = false;
            base.OnCellEndEdit(e);
        }

        /// <summary>
        /// 处理命令键。
        /// </summary>
        /// <param name="msg">通过引用传递的 System.Windows.Forms.Message，表示要处理的窗口消息。</param>
        /// <param name="keyData">System.Windows.Forms.Keys 值之一，表示要处理的键。</param>
        /// <returns>如果字符已由控件处理，则为 true；否则为 false。</returns>
        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (IsCurrentCellInEditMode)
            {
                switch (keyData)
                {
                    case Keys.Up:
                    case Keys.Down:
                    case Keys.Enter:
                        _SearchGrid.TextBox_KeyDown(EditingControl, new KeyEventArgs(keyData));
                        return true;
                }
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }

        /// <include file='Include_Tag.xml' path='Tab/Members/Member[@Name="Reset"]/*'/>
        public virtual void Reset() => _SearchGrid.Reset();
        /// <include file='Include_Tag.xml' path='Tab/Members/Member[@Name="ShowSearchGrid"]/*'/>
        public virtual void ShowSearchGrid() => SearchForm.ShowSearchGrid();
        /// <include file='Include_Tag.xml' path='Tab/Members/Member[@Name="SetSearchGridSize"]/*'/>
        public virtual void SetSearchGridSize() => SearchForm.SetSearchGridSize();
        /// <include file='Include_Tag.xml' path='Tab/Members/Member[@Name="SetSearchGridLocation"]/*'/>
        public virtual void SetSearchGridLocation() => SearchForm.SetSearchGridLocation();
        /// <include file='Include_Tag.xml' path='Tab/Members/Member[@Name="ReversalSearchState"]/*'/>
        public virtual void ReversalSearchState() => _SearchGrid.ReversalSearchState();
    }
}
