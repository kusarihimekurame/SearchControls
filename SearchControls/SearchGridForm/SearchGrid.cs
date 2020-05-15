using SearchControls.Interface;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Drawing;
using System.Drawing.Design;
using System.Linq;
using System.Linq.Dynamic;
using System.Reflection;
using System.Text;
using System.Windows.Forms;

namespace SearchControls.SearchGridForm
{
    /// <summary>
    /// 模糊查找的表格
    /// </summary>
    [ToolboxItem(false)]
    public partial class SearchGrid : DataGridView
    {
        private IGrid grid => ((SearchForm)TopLevelControl).Grid;
        private IDataText _IDataText;
        /// <include file='Include_Tag.xml' path='Tab/Members/Member[@Name="IDataText"]/*'/>
        internal IDataText DataText 
        {
            get => _IDataText;
            set
            {
                _IDataText = value;
                if (value is ISubSearchTextBoxes sstb)
                {
                    sstb.SubSearchTextBoxes.CollectionChanged += SubSearchTextBoxes_CollectionChanged;
                }
                if (TopLevelControl is SearchForm sf) sf.AddEvent();
                if (value.TextBox != null) Add_TextBoxEvent();
            }
        }
        /// <include file='Include_Tag.xml' path='Tab/Members/Member[@Name="IMultiSelect"]/*'/>
        internal new IMultiSelect MultiSelect { get; set; }
        private object _OriginSource;
        private SubSearchTextBoxCollection SubSearchTextBoxes => DataText is ISubSearchTextBoxes sstb ? sstb.SubSearchTextBoxes : null;

        /// <summary>
        /// 获取或设置一个值，该值指示是否显示列标题行。
        /// </summary>
        /// <value>
        /// 如果显示列标题，为 true；否则为 false。 默认值为 true。
        /// </value>
        public new bool ColumnHeadersVisible
        {
            get => base.ColumnHeadersVisible;
            set
            {
                base.ColumnHeadersVisible = value;
                if (TopLevelControl != null)
                {
                    if (value)
                    {
                        TopLevelControl.Height += ColumnHeadersHeight;
                    }
                    else
                    {
                        TopLevelControl.Height -= ColumnHeadersHeight;
                    }
                }
            }
        }

        /// <include file='Include_Tag.xml' path='Tab/Members/Member[@Name="DataMember"]/*'/>
        [
            DefaultValue(""),
            Editor("System.Windows.Forms.Design.DataMemberListEditor, System.Design, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", typeof(UITypeEditor)),
            Category("Search"),
            Description("获取或设置 System.Windows.Forms.DataGridView 正在为其显示数据的数据源中的列表或表的名称。")
        ]
        public new string DataMember
        {
            get => base.DataMember;
            set
            {
                if (AutoGenerateColumns)
                {
                    Columns.Cast<DataGridViewColumn>().Where(dgvc => dgvc.Name.Contains("PY_")).ToList().ForEach(dgvc => dgvc.Visible = false);
                }
                base.DataMember = value;
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
                if (value is IEnumerable)
                {
                    _OriginSource = value;
                    base.DataSource = value;
                }
                else
                {
                    _OriginSource = null;
                    base.DataSource = value;
                }

                if (AutoGenerateColumns)
                {
                    Columns.Cast<DataGridViewColumn>().Where(dgvc => dgvc.Name.Contains("PY_")).ToList().ForEach(dgvc => dgvc.Visible = false);
                }
            }
        }

        /// <summary>
        /// 模糊查找的表格
        /// </summary>
        public SearchGrid()
        {
            InitializeComponent();
            base.MultiSelect = false;
            ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
        }

        /// <summary>
        /// 添加文本框事件
        /// </summary>
        public void Add_TextBoxEvent()
        {
            DataText.TextBox.KeyPress += TextBox_KeyPress;
            DataText.TextBox.TextChanged += TextBox_TextChanged;
            DataText.TextBox.MouseClick += TextBox_MouseClick;
            DataText.TextBox.MouseDoubleClick += TextBox_MouseDoubleClick;
            DataText.TextBox.KeyDown += TextBox_KeyDown;
        }

        /// <summary>
        /// 移除文本框事件
        /// </summary>
        public void Remove_TextBoxEvent()
        {
            PropertyInfo propertyInfo = typeof(Control).GetProperty("Events", BindingFlags.Instance | BindingFlags.NonPublic);
            EventHandlerList eventHandlerList = (EventHandlerList)propertyInfo.GetValue(DataText.TextBox);
            FieldInfo fieldInfo = typeof(Control).GetField("EventKeyDown", BindingFlags.Static | BindingFlags.NonPublic);
            Delegate d = eventHandlerList[fieldInfo.GetValue(null)];
            if (d != null)
            {
                for (int i = 0; i < d.GetInvocationList().Count(); i++)
                {
                    DataText.TextBox.KeyPress -= TextBox_KeyPress;
                    DataText.TextBox.TextChanged -= TextBox_TextChanged;
                    DataText.TextBox.MouseClick -= TextBox_MouseClick;
                    DataText.TextBox.MouseDoubleClick -= TextBox_MouseDoubleClick;
                    DataText.TextBox.KeyDown -= TextBox_KeyDown;
                }
            }
        }

        /// <summary>
        /// 引发 System.Windows.Forms.DataGridView.Scroll 事件。
        /// </summary>
        /// <param name="e">包含事件数据的 System.Windows.Forms.ScrollEventArgs。</param>
        protected override void OnScroll(ScrollEventArgs e)
        {
            base.OnScroll(e);
            if (e.ScrollOrientation.Equals(ScrollOrientation.VerticalScroll))
            {
                int index = Rows.Cast<DataGridViewRow>().FirstOrDefault(dgvr => dgvr.Selected)?.Index ?? 0;
                if (index < FirstDisplayedScrollingRowIndex || index > FirstDisplayedScrollingRowIndex + DisplayedRowCount(false) - 1)
                {
                    switch(e.Type)
                    {
                        case ScrollEventType.SmallIncrement:
                        case ScrollEventType.LargeIncrement:
                            Rows[FirstDisplayedScrollingRowIndex].Selected = true;
                            break;
                        case ScrollEventType.SmallDecrement:
                        case ScrollEventType.LargeDecrement:
                            Rows[FirstDisplayedScrollingRowIndex + DisplayedRowCount(false) - 1].Selected = true;
                            break;
                    }
                }
            }
        }

        private const int WM_MOUSEACTIVATE = 0x21;
        private const int WM_LBUTTONDOWN = 0x201;
        private const int MA_NOACTIVATE = 3;
        /// <summary>
        /// 处理窗口消息。
        /// </summary>
        /// <param name="m">通过引用传递的 System.Windows.Forms.Message，表示要处理的窗口消息。</param>
        [DebuggerStepThrough]
        protected override void WndProc(ref Message m)
        {
            if (m.Msg.Equals(WM_LBUTTONDOWN))
            {
                //重写左键方法，避免触发焦点
                WmMouseDown(ref m, MouseButtons.Left, 1);
                m.Result = new IntPtr(MA_NOACTIVATE);
                return;
            }
            base.WndProc(ref m);
        }

        private bool IsMouseDown = false;
        /// <summary>
        /// 引发 System.Windows.Forms.Control.MouseDown 事件。
        /// </summary>
        /// <param name="e">包含事件数据的 System.Windows.Forms.MouseEventArgs。</param>
        [DebuggerStepThrough]
        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);
            IsMouseDown = true;
        }

        /// <summary>
        /// 引发 System.Windows.Forms.Control.MouseUp 事件。
        /// </summary>
        /// <param name="e">包含事件数据的 System.Windows.Forms.MouseEventArgs。</param>
        [DebuggerStepThrough]
        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);
            if (IsMouseDown) IsMouseDown = false;
            else return;
            if (e.Button.Equals(MouseButtons.Left))
            {
                OnClick(e);
                OnMouseClick(e);
            }
        }

        /// <summary>
        /// 引发 System.Windows.Forms.Control.MouseClick 事件。
        /// </summary>
        /// <param name="e">包含事件数据的 System.Windows.Forms.MouseEventArgs。</param>
        protected override void OnMouseClick(MouseEventArgs e)
        {
            HitTestInfo hti = HitTest(e.X, e.Y);

            DataGridViewCellMouseEventArgs dgvcme = null;
            if (!hti.Type.Equals(DataGridViewHitTestType.None) &&
                !hti.Type.Equals(DataGridViewHitTestType.HorizontalScrollBar) &&
                !hti.Type.Equals(DataGridViewHitTestType.VerticalScrollBar))
            {
                int mouseX = e.X - hti.ColumnX;
                dgvcme = new DataGridViewCellMouseEventArgs(hti.ColumnIndex, hti.RowIndex, mouseX, e.Y - hti.RowY, e);
                if (e.Button.Equals(MouseButtons.Left))
                {
                    OnCellClick(new DataGridViewCellEventArgs(hti.ColumnIndex, hti.RowIndex));
                }
                base.OnMouseClick(e);
                if (dgvcme.ColumnIndex < Columns.Count && dgvcme.RowIndex < Rows.Count)
                {
                    OnCellMouseClick(dgvcme);
                }
            }
            else
            {
                base.OnMouseClick(e);
            }

            switch (hti.Type)
            {
                case DataGridViewHitTestType.ColumnHeader:
                    {
                        Debug.Assert(dgvcme != null);
                        if (dgvcme.ColumnIndex < Columns.Count && dgvcme.RowIndex < Rows.Count)
                        {
                            OnColumnHeaderMouseClick(dgvcme);
                        }
                        break;
                    }

                case DataGridViewHitTestType.RowHeader:
                    {
                        Debug.Assert(dgvcme != null);
                        if (dgvcme.ColumnIndex < Columns.Count && dgvcme.RowIndex < Rows.Count)
                        {
                            OnRowHeaderMouseClick(dgvcme);
                        }
                        break;
                    }
            }
        }

        [DebuggerStepThrough]
        private void WmMouseDown(ref Message m, MouseButtons button, int clicks)
        {
            // If this is a "real" mouse event (not just WM_LBUTTONDOWN, etc) then
            // we need to see if something happens during processing of
            // user code that changed the state of the buttons (i.e. bringing up
            // a dialog) to keep the control in a consistent state...
            //
            MouseButtons realState = MouseButtons;

            // If the UserMouse style is set, the control does its own processing
            // of mouse messages
            //
            if (!GetStyle(ControlStyles.UserMouse))
            {
                DefWndProc(ref m);
                // we might had re-entered the message loop and processed a WM_CLOSE message
                if (IsDisposed)
                {
                    return;
                }
            }

            if (realState != MouseButtons)
            {
                return;
            }

            // control should be enabled when this method is entered, but may have become
            // disabled during its lifetime (e.g. through a Click or Focus listener)
            if (Enabled)
            {
                MouseEventArgs e = new MouseEventArgs(button, clicks, (short)(unchecked((int)(long)m.LParam) & 0xFFFF), (short)((unchecked((int)(long)m.LParam) >> 16) & 0xffff), 0);
                OnMouseDown(e);
            }
        }

        private void ToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            ColumnHeadersVisible = !ColumnHeadersVisible;
        }

        /// <include file='Include_Tag.xml' path='Tab/Members/Member[@Name="Reset"]/*'/>
        public virtual void Reset()
        {
            DataView dv = null;
            if (DataSource is DataTable _dt) dv = _dt.DefaultView;
            else if (DataSource is DataView _dv) dv = _dv;
            else if (DataSource is DataSet _ds) dv = (BindingContext[base.DataSource, base.DataMember] as CurrencyManager).List as DataView;
            else if (DataSource is IEnumerable) base.DataSource = _OriginSource;
            else if (DataSource is BindingSource bs)
            {
                bs.Filter = "";
            }
            if (dv != null)
            {
                dv.RowFilter = "";
            }
        }

        internal bool IsEnter = false;
        private int _SelectionStart;
        private int _SelectionLength;

        /// <include file='Include_Tag.xml' path='Tab/Members/Member[@Name="ReversalSearchState"]/*'/>
        public virtual void ReversalSearchState()
        {
            DataView dv = null;
            if (DataSource is DataTable _dt) dv = _dt.DefaultView;
            else if (DataSource is DataView _dv) dv = _dv;
            else if (DataSource is DataSet _ds) dv = (BindingContext[base.DataSource, base.DataMember] as CurrencyManager).List as DataView;

            if (DataText.TextBox.Focused || (SubSearchTextBoxes != null && SubSearchTextBoxes.Any(sstb => sstb.TextBox.Focused)))
            {
                TextBox tb;
                if (DataText.TextBox.Focused) tb = DataText.TextBox;
                else tb = SubSearchTextBoxes.First(sstb => sstb.TextBox.Focused).TextBox;
                if (dv != null)
                {
                    if (string.IsNullOrEmpty(dv.RowFilter))
                    {
                        TextBox_TextChanged(tb, null);
                    }
                    else
                    {
                        dv.RowFilter = null;
                        grid.ShowSearchGrid();
                    }
                }
                else if(DataSource is BindingSource bs)
                {
                    if (string.IsNullOrEmpty(bs.Filter))
                    {
                        TextBox_TextChanged(tb, null);
                    }
                    else
                    {
                        bs.Filter = null;
                        grid.ShowSearchGrid();
                    }
                }
            }
        }

        /// <include file='Include_Tag.xml' path='Tab/Members/Member[@Name="GetSelectedText"]/*'/>
        public void GetSelectedText(string[] Texts, out int LocationStart, out int Index)
        {
            LocationStart = 0;
            Index = 0;
            for (int i = 0; i < Texts.Count(); i++)
            {
                LocationStart += Texts[i].Length + MultiSelect.MultiSelectSplit.Length;
                if (LocationStart > _SelectionStart)
                {
                    LocationStart -= Texts[i].Length + MultiSelect.MultiSelectSplit.Length;
                    Index = i;
                    break;
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
                    else if (TopLevelControl.Visible) DataText.ReversalSearchState();
                    else if (!TopLevelControl.Visible) (TopLevelControl as Form).Show((DataText as Control).TopLevelControl);
                }

                _SelectionStart = tb.SelectionStart;
                _SelectionLength = tb.SelectionLength;
            }
        }

        private void TextBox_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (sender is TextBox tb)
            {
                if (e != null) DataText.ReversalSearchState();

                if (MultiSelect != null && tb.Text.Contains(MultiSelect.MultiSelectSplit))
                {
                    string[] Texts = tb.Text.Split(new string[] { MultiSelect.MultiSelectSplit }, StringSplitOptions.None);
                    GetSelectedText(Texts, out int LocationStart, out int Index);
                    tb.Select(LocationStart, Texts[Index].Length);
                }
                else
                {
                    tb.SelectAll();
                }
            }
        }

        private void TextBox_TextChanged(object sender, EventArgs e)
        {
            if (DataSource == null && RowCount.Equals(0) || (DataSource is DataSet && string.IsNullOrEmpty(DataMember)) || !DataText.IsTextChanged) return;

            DataTable dt = null;
            if (DataSource is DataTable _dt) dt = _dt;
            else if (DataSource is DataView _dv) dt = _dv.Table;
            else if (DataSource is DataSet _ds) dt = _ds.Tables[DataMember];

            if (sender is TextBox tb)
            {
                isCellValueChanged = false;

                if (string.IsNullOrEmpty(tb.Text))
                {
                    if (SubSearchTextBoxes != null) SubSearchTextBoxes.ToList().ForEach(sstb => sstb.TextBox.Text = "");
                    DataText.TextBox.Text = "";
                }

                string[] XZ_Texts = MultiSelect == null ? null : tb.Text.Split(new string[] { MultiSelect.MultiSelectSplit }, StringSplitOptions.None);
                string[] Texts = tb.Text.Split(new char[] { ' ' }, StringSplitOptions.None);

                #region 多选框

                if (MultiSelect != null && MultiSelect.IsMultiSelect)
                {
                    Rows.Cast<DataGridViewRow>().ToList().ForEach(dgvr =>
                    {
                        dgvr.Cells[MultiSelect.MultiSelectColumn.Name].Value = dgvr.Cells.Cast<DataGridViewCell>().Select(dgvc => dgvc.Value.ToString().Trim()).Any(v => XZ_Texts.Any(t => !string.IsNullOrWhiteSpace(t) && t.Equals(v)));
                    });
                }

                #endregion

                #region 文本框的模糊查找      

                string cWHERE = "";

                TextBox_MouseClick(tb, null);

                if (MultiSelect == null || !MultiSelect.IsMultiSelect)
                {

                    Texts.ToList().ForEach(t =>
                    {
                        if (dt != null)
                        {
                            cWHERE = Method.CreateFilter(dt, tb.Text);
                        }
                        else
                        {
                            string _cWHERE = "";
                            Columns.Cast<DataGridViewColumn>().Where(dgvc => dgvc.Visible || dgvc.Name.Contains("PY_")).ToList().ForEach(dgvc =>
                            {
                                if (!dgvc.DataPropertyName.Equals("NID") && (dgvc.ValueType == null || dgvc.ValueType.Equals(typeof(string))))
                                    _cWHERE +=
                                      //"OR " + dgvc.DataPropertyName + ".IndexOf(\"" + t + "\", @0) >= 0 " :
                                      "OR " + dgvc.Name + ".Contains(\"" + t + "\") "; //字段模糊查找
                            });
                            cWHERE += string.IsNullOrEmpty(_cWHERE) ? string.Empty : "AND (" + _cWHERE.Substring(3) + ") ";
                        }
                    });
                }
                else
                {
                    GetSelectedText(XZ_Texts, out int LocationStart, out int Index);

                    if (dt != null)
                    {
                        cWHERE = Method.CreateFilter(dt, XZ_Texts[Index]);
                    }
                    else
                    {
                        Columns.Cast<DataGridViewColumn>().Where(dgvc => dgvc.Visible || dgvc.Name.Contains("PY_")).ToList().ForEach(dgvc =>
                        {
                            if (!dgvc.Name.Equals(MultiSelect.MultiSelectColumn) && !dgvc.Name.Equals("NID") && (dgvc.ValueType == null || dgvc.ValueType.Equals(typeof(string))))
                                cWHERE += "OR " + dgvc.Name + ".Contains(\"" + XZ_Texts[Index] + "\") "; //字段模糊查找
                        });
                    }
                }

                try
                {
                    if (dt != null)
                    {
                        cWHERE = MultiSelect != null && MultiSelect.IsMultiSelect ? cWHERE + (string.IsNullOrEmpty(cWHERE) ? string.Empty : "OR Select = true") : cWHERE;
                        DataView dv = base.DataSource is DataSet ? (BindingContext[base.DataSource, base.DataMember] as CurrencyManager).List as DataView : dt.DefaultView;
                        if (!dv.RowFilter.Equals(cWHERE)) dv.RowFilter = cWHERE;
                    }
                    else if (DataSource is BindingSource bs)
                    {
                        if (!bs.Filter.Equals(cWHERE)) bs.Filter = cWHERE;
                    }
                    else if (_OriginSource is IEnumerable os)
                    {
                        base.DataSource = os.Where(cWHERE.Substring(3));
                    }
                }
                catch { }


                if (tb.Focused)    // 如果有焦点存在，就执行以下代码
                {
                    grid.ShowSearchGrid();  //显示并调整高宽
                }

                if (MultiSelect != null && MultiSelect.IsMultiSelect)
                {
                    Sort(MultiSelect.MultiSelectColumn, ListSortDirection.Descending);
                    DataGridViewRow noSelectRow = Rows.Cast<DataGridViewRow>().FirstOrDefault(dgvr => !(bool)dgvr.Cells[MultiSelect.MultiSelectColumn.Name].Value);
                    if (noSelectRow != null) CurrentCell = noSelectRow.Cells[0];
                }

                #endregion

                if (e != null && DataText.IsAutoInput && (MultiSelect == null || MultiSelect != null && !MultiSelect.IsMultiSelect))
                {
                    if (RowCount.Equals(1) && !string.IsNullOrWhiteSpace(DataText.AutoInputDataName) && Rows[0].Cells.Cast<DataGridViewCell>().First(dgvc => dgvc.OwningColumn.Name.Equals(DataText.AutoInputDataName) || dgvc.OwningColumn.DataPropertyName.Equals(DataText.AutoInputDataName)).Value.ToString().Equals(tb.Text, StringComparison.OrdinalIgnoreCase))
                        TextBox_KeyDown(this, new KeyEventArgs(Keys.Enter));
                }

                isCellValueChanged = true;
            }
        }

        private bool isCellValueChanged = true;
        /// <summary>
        /// 引发 System.Windows.Forms.DataGridView.CellValueChanged 事件。
        /// </summary>
        /// <param name="e">包含事件数据的 System.Windows.Forms.DataGridViewCellEventArgs。</param>
        protected override void OnCellValueChanged(DataGridViewCellEventArgs e)
        {
            if (isCellValueChanged)
            {
                base.OnCellValueChanged(e);
                if (Columns[e.ColumnIndex].Name.Equals(MultiSelect.MultiSelectColumn.Name) && e.RowIndex >= 0 && !Convert.ToBoolean(Rows[e.RowIndex].Cells[e.ColumnIndex].Value))
                {
                    string _DisplayDataName = string.Empty;
                    TextBox tb;
                    SubSearchTextBox sstb = SubSearchTextBoxes?.FirstOrDefault(_sstb => _sstb.TextBox.Focused);
                    if (DataText.TextBox.Focused)
                    {
                        tb = DataText.TextBox;
                        _DisplayDataName = DataText.DisplayDataName;
                    }
                    else if (sstb != null)
                    {
                        tb = sstb.TextBox;
                        _DisplayDataName = sstb.DisplayDataName;
                    }
                    else
                    {
                        tb = DataText.TextBox;
                        _DisplayDataName = DataText.DisplayDataName;
                    }

                    if (string.IsNullOrEmpty(_DisplayDataName))
                    {
                        string[] Texts = tb.Text.Contains(MultiSelect.MultiSelectSplit) ? tb.Text.Split(new string[] { MultiSelect.MultiSelectSplit }, StringSplitOptions.RemoveEmptyEntries) : (new string[] { tb.Text.Trim() });
                        DataRow dr = (Rows[e.RowIndex].DataBoundItem as DataRowView).Row;
                        string OldText = dr.ItemArray.Where(s => s is string).Cast<string>().SingleOrDefault(s => Texts.Count(t => t.Trim().Equals(s)).Equals(1) && !string.IsNullOrWhiteSpace(s)).Trim();
                        tb.Text = tb.Text.Replace(OldText + MultiSelect.MultiSelectSplit, "");
                    }
                    else
                    {
                        tb.Text = Columns.Contains(DataText.DisplayDataName)
                            ? tb.Text.Replace(this[DataText.DisplayDataName, e.RowIndex].Value.ToString().Trim() + MultiSelect.MultiSelectSplit, "")
                            : tb.Text.Replace(Rows[e.RowIndex].Cells.Cast<DataGridViewCell>().First(dgvc => dgvc.OwningColumn.DataPropertyName.Equals(DataText.DisplayDataName)).Value.ToString().Trim() + MultiSelect.MultiSelectSplit, "");
                    }
                    tb.Select(tb.TextLength, 0);
                }
            }
            if (Columns[e.ColumnIndex].Name.Equals(MultiSelect.MultiSelectColumn.Name)) Sort(MultiSelect.MultiSelectColumn, ListSortDirection.Descending);
        }

        /// <summary>
        /// 引发 System.Windows.Forms.DataGridView.CellClick 事件。
        /// </summary>
        /// <param name="e">包含事件数据的 System.Windows.Forms.DataGridViewCellEventArgs。</param>
        protected override void OnCellClick(DataGridViewCellEventArgs e)
        {
            base.OnCellClick(e);
            if (e.RowIndex > -1 && RowCount > 0)
            {
                if (MultiSelect != null && MultiSelect.IsMultiSelect && Columns[e.ColumnIndex].Name.Equals(MultiSelect.MultiSelectColumn.Name))
                    TextBox_KeyDown(this, new KeyEventArgs(Keys.Space));
                else
                    TextBox_KeyDown(this, new KeyEventArgs(Keys.Enter));
            }
        }

        /// <summary>
        /// 键盘事件
        /// </summary>
        /// <param name="sender">事件源。</param>
        /// <param name="e">包含事件数据的 System.Windows.Forms.KeyEventArgs。</param>
        public void TextBox_KeyDown(object sender, KeyEventArgs e)
        {
            #region 键盘事件

            int index = 0;
            if (RowCount > 0 && SelectedRows.Count > 0) index = SelectedRows[0].Index;   // 获取选择的行数

            switch (e.KeyCode)
            {
                case Keys.Up:
                    e.Handled = true;   //禁止文本框的光标移动
                    if (RowCount > 0 && index > 0)
                    {
                        ClearSelection();
                        Rows[index - 1].Selected = true;   // 上移
                        if (index.Equals(FirstDisplayedScrollingRowIndex))
                            FirstDisplayedScrollingRowIndex--;   // 滚动条自动移动
                    }
                    break;
                case Keys.Down:
                    e.Handled = true;   //禁止文本框的光标移动
                    if (RowCount > 0 && index >= 0 && index < RowCount - 1)
                    {
                        ClearSelection();
                        Rows[index + 1].Selected = true;   //下移
                        if (index.Equals(FirstDisplayedScrollingRowIndex + grid.DisplayRowCount - 1))
                            FirstDisplayedScrollingRowIndex++;   // 滚动条自动移动
                    }
                    break;
                case Keys.Enter:
                    if (RowCount > 0)
                    {
                        DataText.TextBox.TextChanged -= TextBox_TextChanged;
                        SubSearchTextBoxes?.ForEach(_sstb =>
                        {
                            _sstb.TextBox.TextChanged -= TextBox_TextChanged;
                        });

                        TextBox tb = null;
                        SubSearchTextBox sstb = SubSearchTextBoxes?.FirstOrDefault(_sstb => _sstb.TextBox.Focused);
                        if (DataText.TextBox.Focused) tb = DataText.TextBox;
                        else if (sstb != null) tb = sstb.TextBox;

                        GridSelectingEventArgs row = new GridSelectingEventArgs(this, index, e);
                        GridSelectedEventArgs rowed = new GridSelectedEventArgs(this, index, e);

                        DataText.OnGridSelecting(row);
                        if (!row.Handled)
                        {
                            if (MultiSelect != null && MultiSelect.IsMultiSelect)
                            {
                                if (Rows.Cast<DataGridViewRow>().All(dgvr => !(bool)dgvr.Cells[MultiSelect.MultiSelectColumn.Name].Value))
                                {
                                    TextBox_KeyDown(sender, new KeyEventArgs(Keys.Space));
                                }
                                //if (Convert.ToBoolean(CurrentRow.Cells[MultiSelect.MultiSelectColumn.Name].Value))
                                //{
                                //    DataText.TextBox.Text = DataText.TextBox.Text.Substring(0, DataText.TextBox.Text.Length - 1);
                                //    SubSearchTextBoxes?.Where(_sstb => !string.IsNullOrWhiteSpace(_sstb.DisplayDataName)).ToList().ForEach(_sstb =>
                                //    {
                                //        _sstb.TextBox.Text = _sstb.TextBox.Text.Substring(0, Text.Length - 1);
                                //    });
                                //}
                                //else
                                //{
                                //    if (!string.IsNullOrWhiteSpace(DataText.DisplayDataName))
                                //    {
                                //        DataText.TextBox.Text = Columns.Contains(DataText.DisplayDataName)
                                //            ? DataText.TextBox.Text + this[DataText.DisplayDataName, index].Value.ToString().Trim()
                                //            : DataText.TextBox.Text + Rows[index].Cells.Cast<DataGridViewCell>().First(dgvc => dgvc.OwningColumn.DataPropertyName.Equals(DataText.DisplayDataName)).Value.ToString().Trim();
                                //        SubSearchTextBoxes?.Where(_sstb => !string.IsNullOrWhiteSpace(_sstb.DisplayDataName)).ToList().ForEach(_sstb =>
                                //        {
                                //            _sstb.TextBox.Text = Columns.Contains(_sstb.DisplayDataName)
                                //                ? _sstb.TextBox.Text + this[_sstb.DisplayDataName, index].Value.ToString().Trim()
                                //                : _sstb.TextBox.Text + Rows[index].Cells.Cast<DataGridViewCell>().First(dgvc => dgvc.OwningColumn.DataPropertyName.Equals(_sstb.DisplayDataName)).Value.ToString().Trim();
                                //        });
                                //    }
                                //}
                            }
                            else
                            {
                                if (!string.IsNullOrWhiteSpace(DataText.DisplayDataName))
                                {
                                    if (!Columns.Contains(DataText.DisplayDataName) && !Rows[index].Cells.Cast<DataGridViewCell>().Any(dgvc => dgvc.OwningColumn.DataPropertyName.Equals(DataText.DisplayDataName))) 
                                        throw new Exception($"没有找到列名'{DataText.DisplayDataName}',请检查DisplayDataName属性的值是否正确。");
                                    DataText.TextBox.Text = Columns.Contains(DataText.DisplayDataName)
                                        ? this[DataText.DisplayDataName, index].Value.ToString().Trim()
                                        : Rows[index].Cells.Cast<DataGridViewCell>().First(dgvc => dgvc.OwningColumn.DataPropertyName.Equals(DataText.DisplayDataName)).Value.ToString().Trim();
                                }
                                SubSearchTextBoxes?.Where(_sstb => !string.IsNullOrWhiteSpace(_sstb.DisplayDataName)).ToList().ForEach(_sstb =>
                                {
                                    if (!Columns.Contains(_sstb.DisplayDataName) && !Rows[index].Cells.Cast<DataGridViewCell>().Any(dgvc => dgvc.OwningColumn.DataPropertyName.Equals(_sstb.DisplayDataName)))
                                        throw new Exception($"没有找到列名'{_sstb.DisplayDataName}',请检查子文本框的DisplayDataName属性的值是否正确。");
                                    _sstb.TextBox.Text = Columns.Contains(_sstb.DisplayDataName)
                                        ? this[_sstb.DisplayDataName, index].Value.ToString().Trim()
                                        : Rows[index].Cells.Cast<DataGridViewCell>().First(dgvc => dgvc.OwningColumn.DataPropertyName.Equals(_sstb.DisplayDataName)).Value.ToString().Trim();
                                });
                            }

                            TopLevelControl.Visible = false;
                            DataText.OnGridSelected(rowed);
                            if (tb != null && tb.Focused) SendKeys.Send("{TAB}");
                        }
                        else
                        {
                            DataText.OnGridSelected(rowed);
                        }

                        if (DataText.TextBox != null) DataText.TextBox.TextChanged += TextBox_TextChanged;
                        SubSearchTextBoxes?.ForEach(_sstb =>
                        {
                            _sstb.TextBox.TextChanged += TextBox_TextChanged;
                        });
                    }
                    else
                    {
                        SendKeys.Send("{TAB}");  //没有表格时，触发Tab按键，移动焦点到下一个控件
                    }
                    break;
                case Keys.Space:
                    if (MultiSelect != null && Columns.Contains(MultiSelect.MultiSelectColumn.Name))
                    {
                        if (RowCount > 0)
                        {
                            GridSelectingEventArgs row = new GridSelectingEventArgs(this, index, e);
                            GridSelectedEventArgs rowed = new GridSelectedEventArgs(this, index, e);

                            DataText.TextBox.TextChanged -= TextBox_TextChanged;
                            SubSearchTextBoxes.ForEach(_sstb =>
                            {
                                _sstb.TextBox.TextChanged -= TextBox_TextChanged;
                            });

                            DataText.OnGridSelecting(row);

                            if (Convert.ToBoolean(Rows[index].Cells[MultiSelect.MultiSelectColumn.Name].Value))
                            {
                                DataView dv = null;
                                if (DataSource is DataTable _dt) dv = _dt.DefaultView;
                                else if (DataSource is DataView _dv) dv = _dv;
                                else if (DataSource is DataSet _ds) dv = (BindingContext[base.DataSource, base.DataMember] as CurrencyManager).List as DataView;

                                if (!e.Handled)
                                {
                                    if ((dv == null && !(DataSource is BindingSource)) || string.IsNullOrEmpty(dv.Sort))
                                    {
                                        Rows[index].Cells[MultiSelect.MultiSelectColumn.Name].Value = false;
                                    }
                                    else
                                    {
                                        if (string.IsNullOrEmpty(DataText.DisplayDataName))
                                        {
                                            object displayData = Rows[index].Cells[1].Value;
                                            if (DataSource is BindingSource bs)
                                            {
                                                string sort = bs.Sort;
                                                bs.Sort = "";
                                                Rows.Cast<DataGridViewRow>().First(dgvr => dgvr.Cells[1].Value.Equals(displayData)).Cells[MultiSelect.MultiSelectColumn.Name].Value = false;
                                                bs.Sort = sort;
                                            }
                                            else
                                            {
                                                string sort = dv.Sort;
                                                dv.Sort = "";
                                                Rows.Cast<DataGridViewRow>().First(dgvr => dgvr.Cells[1].Value.Equals(displayData)).Cells[MultiSelect.MultiSelectColumn.Name].Value = false;
                                                dv.Sort = sort;
                                            }
                                        }
                                        else
                                        {
                                            object displayData = Columns.Contains(DataText.DisplayDataName) ? this[DataText.DisplayDataName, index].Value : Rows[index].Cells.Cast<DataGridViewCell>().First(dgvc => dgvc.OwningColumn.DataPropertyName.Equals(DataText.DisplayDataName));
                                            if (DataSource is BindingSource bs)
                                            {
                                                string sort = bs.Sort;
                                                bs.Sort = "";
                                                Rows.Cast<DataGridViewRow>().First(dgvr => dgvr.Cells.Cast<DataGridViewCell>().First(dgvc => dgvc.OwningColumn.Name.Equals(DataText.DisplayDataName) || dgvc.OwningColumn.DataPropertyName.Equals(DataText.DisplayDataName)).Value.Equals(displayData)).Cells[MultiSelect.MultiSelectColumn.Name].Value = false;
                                                bs.Sort = sort;
                                            }
                                            else
                                            {
                                                string sort = dv.Sort;
                                                dv.Sort = "";
                                                Rows.Cast<DataGridViewRow>().First(dgvr => dgvr.Cells.Cast<DataGridViewCell>().First(dgvc => dgvc.OwningColumn.Name.Equals(DataText.DisplayDataName) || dgvc.OwningColumn.DataPropertyName.Equals(DataText.DisplayDataName)).Value.Equals(displayData)).Cells[MultiSelect.MultiSelectColumn.Name].Value = false;
                                                dv.Sort = sort;
                                            }
                                        }
                                    }
                                }
                            }
                            else
                            {
                                TextBox tb = null;
                                SubSearchTextBox sstb = SubSearchTextBoxes.FirstOrDefault(_sstb => _sstb.TextBox.Focused);
                                if (DataText.TextBox.Focused) tb = DataText.TextBox;
                                else if (sstb != null) tb = sstb.TextBox;

                                if (!row.Handled)
                                {
                                    TextBox_MouseDoubleClick(tb, null);
                                    if (!string.IsNullOrWhiteSpace(DataText.DisplayDataName))
                                    {
                                        DataText.TextBox.SelectedText = Columns.Contains(DataText.DisplayDataName)
                                            ? this[DataText.DisplayDataName, index].Value.ToString().Trim()
                                            : Rows[index].Cells.Cast<DataGridViewCell>().First(dgvc => dgvc.OwningColumn.DataPropertyName.Equals(DataText.DisplayDataName)).Value.ToString().Trim();
                                    }
                                    SubSearchTextBoxes.Where(_sstb => !string.IsNullOrWhiteSpace(_sstb.DisplayDataName)).ToList().ForEach(_sstb =>
                                    {
                                        _sstb.TextBox.Text = Columns.Contains(_sstb.DisplayDataName)
                                            ? this[_sstb.DisplayDataName, index].Value.ToString().Trim()
                                            : Rows[index].Cells.Cast<DataGridViewCell>().First(dgvc => dgvc.OwningColumn.DataPropertyName.Equals(_sstb.DisplayDataName)).Value.ToString().Trim();
                                    });
                                }

                                if (!string.IsNullOrWhiteSpace(DataText.TextBox.Text) && !DataText.TextBox.Text[DataText.TextBox.TextLength - 1].Equals(MultiSelect.MultiSelectSplit[MultiSelect.MultiSelectSplit.Length - 1]))
                                {
                                    DataText.TextBox.Text += MultiSelect.MultiSelectSplit;
                                    DataText.TextBox.SelectionLength = 0;
                                    DataText.TextBox.SelectionStart = DataText.TextBox.TextLength;
                                }
                                SubSearchTextBoxes.Where(_sstb => !string.IsNullOrWhiteSpace(_sstb.TextBox.Text) && !_sstb.TextBox.Text.Substring(_sstb.TextBox.TextLength - 1, 1).Equals(MultiSelect.MultiSelectSplit)).ToList().ForEach(_sstb =>
                                {
                                    _sstb.TextBox.Text += MultiSelect.MultiSelectSplit;
                                    _sstb.TextBox.SelectionLength = 0;
                                    _sstb.TextBox.SelectionStart = _sstb.TextBox.TextLength;
                                });
                            }

                            DataText.TextBox.TextChanged += TextBox_TextChanged;
                            SubSearchTextBoxes.ForEach(_sstb =>
                            {
                                _sstb.TextBox.TextChanged += TextBox_TextChanged;
                            });

                            TextBox_TextChanged(DataText.TextBox, null);
                            DataText.OnGridSelected(rowed);
                        }
                    }
                    break;
            }

            #endregion
        }

        private void TextBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (MultiSelect != null && MultiSelect.IsMultiSelect && e.KeyChar.Equals(' '))
            {
                e.Handled = true;
            }
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
                            _sstb.TextBox.KeyPress -= TextBox_KeyPress;
                        });
                    }
                    break;
            }
        }

        /// <summary>
        /// <para>设置当前处于活动状态的单元格。</para>
        /// <para>防止线程访问（线程中操作数据源DaTaTable时会触发）</para>
        /// </summary>
        /// <param name="columnIndex">包含单元格的列的索引。</param>
        /// <param name="rowIndex">包含该单元格的行的索引。</param>
        /// <param name="setAnchorCellAddress">如果将新的当前单元格用作使用 Shift 键选择的后续多个单元格的定位单元格，则为 true；否则为 false。</param>
        /// <param name="validateCurrentCell">如果要验证旧的当前单元格中的值并在验证失败时取消更改，则为 true；否则为 false。</param>
        /// <param name="throughMouseClick">如果当前的单元格是通过单击鼠标设置的，则为 true；否则为 false。</param>
        /// <returns>如果当前单元格设置成功，则为 true；否则为 false。</returns>
        protected override bool SetCurrentCellAddressCore(int columnIndex, int rowIndex, bool setAnchorCellAddress, bool validateCurrentCell, bool throughMouseClick)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new MethodInvoker(() => SetCurrentCellAddressCore(columnIndex, rowIndex, setAnchorCellAddress, validateCurrentCell, throughMouseClick)));
                return false;
            }
            else
            {
                try
                {
                    return base.SetCurrentCellAddressCore(columnIndex, rowIndex, setAnchorCellAddress, validateCurrentCell, throughMouseClick);
                }
                catch
                {
                    return false;
                }
            }
        }

        /// <summary>
        /// 引发 System.Windows.Forms.DataGridView.DataError 事件。
        /// </summary>
        /// <param name="displayErrorDialogIfNoHandler">如果要在没有 System.Windows.Forms.DataGridView.DataError 事件的处理程序的情况下显示错误对话框，则为 true。</param>
        /// <param name="e">包含事件数据的 System.Windows.Forms.DataGridViewDataErrorEventArgs。</param>
        protected override void OnDataError(bool displayErrorDialogIfNoHandler, DataGridViewDataErrorEventArgs e)
        {
            if (e.Exception.Message.Equals("给定关键字不在字典中。"))
                displayErrorDialogIfNoHandler = false;
            base.OnDataError(displayErrorDialogIfNoHandler, e);
        }
    }
}
