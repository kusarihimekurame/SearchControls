using SearchControls.Classes;
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
using System.Linq.Dynamic.Core;
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
        [System.Runtime.InteropServices.DllImport("User32.dll")]
        private static extern void keybd_event(byte bVk, byte bScan, int dwFlags, int dwExtraInfo);

        private IGrid _grid;
        internal IGrid grid
        {
            get => TopLevelControl is SearchForm sf ? sf.Grid : _grid;
            set => _grid = value;
        }
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
        private BindingSource BindingSource = new();
        //private object _OriginSource;
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
                    if (grid.IsUp)
                    {
                        if (value)
                        {
                            TopLevelControl.Location = new Point(TopLevelControl.Location.X, TopLevelControl.Location.Y - ColumnHeadersHeight);
                        }
                        else
                        {
                            TopLevelControl.Location = new Point(TopLevelControl.Location.X, TopLevelControl.Location.Y + ColumnHeadersHeight);
                        }
                    }

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

        /// <summary>
        /// 模糊查找的表格
        /// </summary>
        public SearchGrid()
        {
            InitializeComponent();
            SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.AllPaintingInWmPaint, true);
            base.MultiSelect = false;
            ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
        }
        /// <summary>
        /// 引发 <see cref="DataGridView.DataSourceChanged"/> 事件。
        /// </summary>
        /// <param name="e">包含事件数据的 <see cref="EventArgs"/>。</param>
        protected override void OnDataSourceChanged(EventArgs e)
        {
            base.OnDataSourceChanged(e);
            if (DataSource is BindingSource bs) BindingSource = bs;
            else if (DataSource is IList && DataSource is not IBindingListView)
            {
                var type = DataSource.GetType().GetGenericArguments()[0];
                BindingSource.DataSource = typeof(Extension).GetMethods(BindingFlags.Static | BindingFlags.Public).Last().MakeGenericMethod(type).Invoke(null, new object[] { DataSource });
                DataSource = BindingSource;
            }
            else
            {
                BindingSource.DataSource = DataSource;
                DataSource = BindingSource;
            }

            if (AutoGenerateColumns)
            {
                Columns.Cast<DataGridViewColumn>().Where(dgvc => dgvc.Name.Contains("PY_")).ToList().ForEach(dgvc => dgvc.Visible = false);
            }
        }
        /// <summary>
        /// 引发 <see cref="DataGridView.DataMemberChanged"/> 事件。
        /// </summary>
        /// <param name="e">包含事件数据的 <see cref="EventArgs"/>。</param>
        protected override void OnDataMemberChanged(EventArgs e)
        {
            base.OnDataMemberChanged(e);
            BindingSource.DataMember = DataMember;
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
#if NET40
            EventHandlerList eventHandlerList = (EventHandlerList)propertyInfo.GetValue(DataText.TextBox, null);
#else
            EventHandlerList eventHandlerList = (EventHandlerList)propertyInfo.GetValue(DataText.TextBox);
#endif
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
                    switch (e.Type)
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
            if (!hti.Type.Equals(DataGridViewHitTestType.None)
                && !hti.Type.Equals(DataGridViewHitTestType.HorizontalScrollBar)
                && !hti.Type.Equals(DataGridViewHitTestType.VerticalScrollBar)
            )
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
            if (TopLevelControl is SearchForm sf)
            {
                sf.ShowSearchGrid();
            }
        }
        
        /// <include file='Include_Tag.xml' path='Tab/Members/Member[@Name="Reset"]/*'/>
        public virtual void Reset()
        {
            BindingSource.Filter = string.Empty;
        }

        internal bool IsEnter = false;
        private int _SelectionStart;
        private int _SelectionLength;

        /// <include file='Include_Tag.xml' path='Tab/Members/Member[@Name="ReversalSearchState"]/*'/>
        public virtual void ReversalSearchState()
        {
            if (DataText.TextBox.Focused || (SubSearchTextBoxes != null && SubSearchTextBoxes.Any(sstb => sstb.TextBox.Focused)))
            {
                TextBox tb;
                if (DataText.TextBox.Focused) tb = DataText.TextBox;
                else tb = SubSearchTextBoxes.First(sstb => sstb.TextBox.Focused).TextBox;

                if (string.IsNullOrEmpty(BindingSource.Filter))
                {
                    TextBox_TextChanged(tb, null);
                }
                else
                {
                    BindingSource.Filter = null;
                    grid.ShowSearchGrid();
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
                if (e != null && !tb.ReadOnly)
                {
                    if (IsEnter) IsEnter = false;
                    else if (TopLevelControl.Visible) DataText.ReversalSearchState();
                    else if (!TopLevelControl.Visible && grid.IsEnterShow) (TopLevelControl as Form).Show((DataText as Control).TopLevelControl);
                }

                _SelectionStart = tb.SelectionStart;
                _SelectionLength = tb.SelectionLength;
            }
        }

        private void TextBox_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (sender is TextBox tb)
            {
                if (e != null && !tb.ReadOnly) DataText.ReversalSearchState();

                if (MultiSelect != null && tb.Text.Contains(MultiSelect.MultiSelectSplit))
                {
#if NET472_OR_GREATER || NETCOREAPP3_0_OR_GREATER
                    string[] Texts = string.IsNullOrEmpty(MultiSelect.MultiSelectSplit) ? tb.Text.Select(_t => _t.ToString()).Append(string.Empty).ToArray() : tb.Text.Split(new string[] { MultiSelect.MultiSelectSplit }, StringSplitOptions.None);
#else
                    List<string> _text = tb.Text.Select(_t => _t.ToString()).ToList();
                    _text.Add(string.Empty);
                    string[] Texts = string.IsNullOrEmpty(MultiSelect.MultiSelectSplit) ? _text.ToArray() : tb.Text.Split(new string[] { MultiSelect.MultiSelectSplit }, StringSplitOptions.None);
#endif
                    if (Texts.Length.Equals(0)) Texts = new string[] { string.Empty };
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
            if (DataSource == null && RowCount.Equals(0)
                || (BindingSource.DataSource is DataSet && string.IsNullOrEmpty(BindingSource.DataMember))
                || !DataText.IsTextChanged
            ) return;

            DataTable dt = null;
            if (BindingSource.DataSource is DataTable _dt) dt = _dt;
            else if (BindingSource.DataSource is DataView _dv) dt = _dv.Table;
            else if (BindingSource.DataSource is DataSet _ds) dt = _ds.Tables[BindingSource.DataMember];

            string CreateFilter(DataTable dataTable, string text, string[] displayColumnNames)
                => dataTable == null
                    ? (string)BindingSource.DataSource.GetType().GetMethod("CreateFilter", BindingFlags.Static | BindingFlags.Public).Invoke(null, new object[] { text, displayColumnNames })
                    : Method.CreateFilter(dt, text, displayColumnNames);

            if (sender is TextBox tb)
            {
                isCellValueChanged = false;

                if (string.IsNullOrEmpty(tb.Text))
                {
                    if (SubSearchTextBoxes != null) SubSearchTextBoxes.ToList().ForEach(sstb => sstb.TextBox.Text = string.Empty);
                    DataText.TextBox.Text = string.Empty;
                }

#if NET472_OR_GREATER || NETCOREAPP3_0_OR_GREATER
                string[] XZ_Texts = MultiSelect == null
                    ? null
                    : string.IsNullOrEmpty(MultiSelect.MultiSelectSplit)
                        ? tb.Text.Select(_t => _t.ToString()).Append(string.Empty).ToArray()
                        : tb.Text.Split(new string[] { MultiSelect.MultiSelectSplit }, StringSplitOptions.None);
#else
                List<string> _text = tb.Text.Select(_t => _t.ToString()).ToList();
                _text.Add(string.Empty);
                string[] XZ_Texts = MultiSelect == null ? null : string.IsNullOrEmpty(MultiSelect.MultiSelectSplit) ? _text.ToArray() : tb.Text.Split(new string[] { MultiSelect.MultiSelectSplit }, StringSplitOptions.None);
#endif
                if (XZ_Texts == null || XZ_Texts.Length.Equals(0)) XZ_Texts = new string[] { string.Empty };
                string[] Texts = tb.Text.Split(new char[] { ' ' }, StringSplitOptions.None);

                #region 多选框

                if (MultiSelect != null && MultiSelect.IsMultiSelect)
                {
                    Rows.Cast<DataGridViewRow>().ToList().ForEach(dgvr =>
                    {
                        dgvr.Cells[MultiSelect.MultiSelectColumn.Name].Value =
                            dgvr.Cells.Cast<DataGridViewCell>()
                            .Select(dgvc => dgvc.Value.ToString().Trim())
                            .Any(v => XZ_Texts.Any(t => !string.IsNullOrWhiteSpace(t) && t.Equals(v)));
                    });
                }

                #endregion

                #region 文本框的模糊查找      

                string cWHERE = "";

                TextBox_MouseClick(tb, null);

                string[] displayColumnNames = null;
                if (dt != null)
                {
                    displayColumnNames = DataText.TextChangedColumnNames != null && DataText.TextChangedColumnNames.Count() > 0
                        ? DataText.TextChangedColumnNames
                        : Columns.Cast<DataGridViewColumn>()
                            .Where(dgvc => dgvc.Visible || dgvc.DataPropertyName.Contains("PY_"))
                            .Select(dgvc => dgvc.DataPropertyName)
                            .Concat(dt.Columns.Cast<DataColumn>().Where(dc => dc.ColumnName.Contains("PY_")).Select(dc => dc.ColumnName))
                            .Distinct().ToArray();
                }

                if (MultiSelect == null || !MultiSelect.IsMultiSelect)
                {

                    Texts.ToList().ForEach(t => cWHERE = CreateFilter(dt, tb.Text, displayColumnNames));
                }
                else
                {
                    GetSelectedText(XZ_Texts, out int LocationStart, out int Index);

                    cWHERE = string.IsNullOrEmpty(MultiSelect.MultiSelectSplit) ? CreateFilter(dt, XZ_Texts[_SelectionStart], displayColumnNames) : CreateFilter(dt, XZ_Texts[Index], displayColumnNames);
                }

                try
                {
                    if (string.IsNullOrWhiteSpace(BindingSource.Filter) || !BindingSource.Filter.Equals(cWHERE)) BindingSource.Filter = cWHERE;
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

                if (e != null && DataText.IsAutoInput && (MultiSelect == null || MultiSelect != null && !MultiSelect.IsMultiSelect) && !string.IsNullOrWhiteSpace(DataText.AutoInputDataName))
                {
                    if (RowCount.Equals(1)
                        && Rows[0].Cells.Cast<DataGridViewCell>().First(dgvc =>
                               dgvc.OwningColumn.Name.Equals(DataText.AutoInputDataName, StringComparison.OrdinalIgnoreCase)
                               || dgvc.OwningColumn.DataPropertyName.Equals(DataText.AutoInputDataName, StringComparison.OrdinalIgnoreCase)
                           ).Value.ToString().Trim().Equals(tb.Text.Trim(), StringComparison.OrdinalIgnoreCase)
                    )
                    {
                        TextBox_KeyDown(this, new KeyEventArgs(Keys.Enter));
                    }
                    else if (!tb.Focused)
                    {
                        using (DataGridViewRow selectRow =
                            Rows.Cast<DataGridViewRow>().FirstOrDefault(dgvr =>
                                dgvr.Cells.Cast<DataGridViewCell>().First(dgvc =>
                                    dgvc.OwningColumn.Name.Equals(DataText.AutoInputDataName, StringComparison.OrdinalIgnoreCase)
                                    || dgvc.OwningColumn.DataPropertyName.Equals(DataText.AutoInputDataName, StringComparison.OrdinalIgnoreCase)
                                ).Value.ToString().Trim().Equals(tb.Text.Trim(), StringComparison.OrdinalIgnoreCase)))
                        {
                            if (selectRow != null)
                            {
                                CurrentCell = selectRow.Cells[0];
                                TextBox_KeyDown(this, new KeyEventArgs(Keys.Enter));
                            }
                        }
                    }
                }

                isCellValueChanged = true;
            }
        }

        private bool isCellValueChanged = true;
        /// <summary>
        /// 引发 <see cref="DataGridView.CellValueChanged"/> 事件。
        /// </summary>
        /// <param name="e">包含事件数据的 <see cref="DataGridViewCellEventArgs"/>。</param>
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
                        string[] Texts =
                            string.IsNullOrEmpty(MultiSelect.MultiSelectSplit)
                            ? tb.Text.Select(_t => _t.ToString()).ToArray()
                            : tb.Text.Contains(MultiSelect.MultiSelectSplit)
                                ? tb.Text.Split(new string[] { MultiSelect.MultiSelectSplit }, StringSplitOptions.RemoveEmptyEntries)
                                : (new string[] { tb.Text.Trim() });

                        DataRow dr = (Rows[e.RowIndex].DataBoundItem as DataRowView)?.Row;
                        if (dr != null)
                        {
                            string OldText = dr.ItemArray.Where(s => s is string).Cast<string>().SingleOrDefault(s => Texts.Count(t => t.Trim().Equals(s)).Equals(1) && !string.IsNullOrWhiteSpace(s)).Trim();
                            tb.Text = tb.Text.Replace(OldText + MultiSelect.MultiSelectSplit, string.Empty);
                        }
                    }
                    else
                    {
                        tb.Text = Columns.Contains(DataText.DisplayDataName)
                            ? tb.Text.Replace(this[DataText.DisplayDataName, e.RowIndex].Value.ToString().Trim() + MultiSelect.MultiSelectSplit, "")
                            : tb.Text.Replace(Rows[e.RowIndex].Cells.Cast<DataGridViewCell>()
                                .First(dgvc => dgvc.OwningColumn.DataPropertyName.Equals(DataText.DisplayDataName, StringComparison.OrdinalIgnoreCase))
                                .Value.ToString().Trim() + MultiSelect.MultiSelectSplit, "");
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
                    //TextBox_KeyDown(this, new KeyEventArgs(Keys.Enter));
                    //SendKeys.SendWait("{Enter}");
                    keybd_event((byte)Keys.Enter, 0, 0, 0);
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
                                string text;
                                if (!string.IsNullOrWhiteSpace(DataText.DisplayDataName))
                                {
                                    if (!Columns.Contains(DataText.DisplayDataName) && !Rows[index].Cells.Cast<DataGridViewCell>().Any(dgvc => dgvc.OwningColumn.DataPropertyName.Equals(DataText.DisplayDataName, StringComparison.OrdinalIgnoreCase)))
                                        throw new Exception($"没有找到列名'{DataText.DisplayDataName}',请检查DisplayDataName属性的值是否正确。");
                                    text = Columns.Contains(DataText.DisplayDataName)
                                        ? this[DataText.DisplayDataName, index].Value.ToString().Trim()
                                        : Rows[index].Cells.Cast<DataGridViewCell>().First(dgvc => dgvc.OwningColumn.DataPropertyName.Equals(DataText.DisplayDataName, StringComparison.OrdinalIgnoreCase)).Value.ToString().Trim();
                                    if (text != DataText.TextBox.Text) DataText.TextBox.Text = text;
                                }
                                SubSearchTextBoxes?.Where(_sstb => !string.IsNullOrWhiteSpace(_sstb.DisplayDataName)).ToList().ForEach(_sstb =>
                                {
                                    if (!Columns.Contains(_sstb.DisplayDataName) && !Rows[index].Cells.Cast<DataGridViewCell>().Any(dgvc => dgvc.OwningColumn.DataPropertyName.Equals(_sstb.DisplayDataName, StringComparison.OrdinalIgnoreCase)))
                                        throw new Exception($"没有找到列名'{_sstb.DisplayDataName}',请检查子文本框的DisplayDataName属性的值是否正确。");
                                    text = Columns.Contains(_sstb.DisplayDataName)
                                        ? this[_sstb.DisplayDataName, index].Value.ToString().Trim()
                                        : Rows[index].Cells.Cast<DataGridViewCell>().First(dgvc => dgvc.OwningColumn.DataPropertyName.Equals(_sstb.DisplayDataName, StringComparison.OrdinalIgnoreCase)).Value.ToString().Trim();
                                    if (text != _sstb.TextBox.Text) _sstb.TextBox.Text = text;
                                });
                            }

                            TopLevelControl.Visible = false;
                            DataText.OnGridSelected(rowed);
                            if (tb != null && DataText.IsAutoMoveFocus && tb.Focused)
                                //SendKeys.SendWait("{TAB}");
                                keybd_event((byte)Keys.Tab, 0, 0, 0);
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
                        if (DataText.IsAutoMoveFocus)
                            //SendKeys.SendWait("{TAB}");  //没有表格时，触发Tab按键，移动焦点到下一个控件
                            keybd_event((byte)Keys.Tab, 0, 0, 0);
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
                                if (!e.Handled)
                                {
                                    if (string.IsNullOrEmpty(BindingSource.Sort))
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
                                                string sort = BindingSource.Sort;
                                                BindingSource.Sort = "";
                                                Rows.Cast<DataGridViewRow>().First(dgvr => dgvr.Cells[1].Value.Equals(displayData)).Cells[MultiSelect.MultiSelectColumn.Name].Value = false;
                                                BindingSource.Sort = sort;
                                            }
                                        }
                                        else
                                        {
                                            object displayData = Columns.Contains(DataText.DisplayDataName) ? this[DataText.DisplayDataName, index].Value : Rows[index].Cells.Cast<DataGridViewCell>().First(dgvc => dgvc.OwningColumn.DataPropertyName.Equals(DataText.DisplayDataName));

                                            string sort = BindingSource.Sort;
                                            BindingSource.Sort = string.Empty;

                                            Rows.Cast<DataGridViewRow>().First(dgvr =>
                                                dgvr.Cells.Cast<DataGridViewCell>().First(dgvc =>
                                                    dgvc.OwningColumn.Name.Equals(DataText.DisplayDataName, StringComparison.OrdinalIgnoreCase)
                                                    || dgvc.OwningColumn.DataPropertyName.Equals(DataText.DisplayDataName, StringComparison.OrdinalIgnoreCase)
                                                ).Value.Equals(displayData)
                                            ).Cells[MultiSelect.MultiSelectColumn.Name].Value = false;

                                            BindingSource.Sort = sort;
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
                                    string text;
                                    if (!string.IsNullOrWhiteSpace(DataText.DisplayDataName))
                                    {
                                        text = Columns.Contains(DataText.DisplayDataName)
                                            ? this[DataText.DisplayDataName, index].Value.ToString().Trim()
                                            : Rows[index].Cells.Cast<DataGridViewCell>().First(dgvc => dgvc.OwningColumn.DataPropertyName.Equals(DataText.DisplayDataName, StringComparison.OrdinalIgnoreCase)).Value.ToString().Trim();
                                        if (text != DataText.TextBox.SelectedText) DataText.TextBox.SelectedText = text;
                                    }
                                    SubSearchTextBoxes.Where(_sstb => !string.IsNullOrWhiteSpace(_sstb.DisplayDataName)).ToList().ForEach(_sstb =>
                                    {
                                        text = Columns.Contains(_sstb.DisplayDataName)
                                            ? this[_sstb.DisplayDataName, index].Value.ToString().Trim()
                                            : Rows[index].Cells.Cast<DataGridViewCell>().First(dgvc => dgvc.OwningColumn.DataPropertyName.Equals(_sstb.DisplayDataName, StringComparison.OrdinalIgnoreCase)).Value.ToString().Trim();
                                        if (text != _sstb.TextBox.Text) _sstb.TextBox.Text = text;
                                    });
                                }

                                if (!string.IsNullOrWhiteSpace(DataText.TextBox.Text) && !string.IsNullOrEmpty(MultiSelect.MultiSelectSplit) && !DataText.TextBox.Text[DataText.TextBox.TextLength - 1].Equals(MultiSelect.MultiSelectSplit[MultiSelect.MultiSelectSplit.Length - 1]))
                                {
                                    DataText.TextBox.Text += MultiSelect.MultiSelectSplit;
                                    DataText.TextBox.SelectionLength = 0;
                                    DataText.TextBox.SelectionStart = DataText.TextBox.TextLength;
                                }
                                SubSearchTextBoxes.Where(_sstb => !string.IsNullOrEmpty(MultiSelect.MultiSelectSplit) && !string.IsNullOrWhiteSpace(_sstb.TextBox.Text) && !_sstb.TextBox.Text.Substring(_sstb.TextBox.TextLength - 1, 1).Equals(MultiSelect.MultiSelectSplit)).ToList().ForEach(_sstb =>
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

        /// <summary>
        /// 修复关闭窗口后会触发CreateHandle()问题
        /// </summary>
        protected override void CreateHandle()
        {
            if (IsDisposed) return;
            base.CreateHandle();
        }
    }
}
