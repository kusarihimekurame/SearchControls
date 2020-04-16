using SearchControls.Interface;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;

namespace SearchControls.SearchGridForm
{
    /// <summary>
    /// 模糊查找表的窗口
    /// </summary>
    public partial class SearchForm : Form
    {
        private IGrid _grid;
        /// <include file='Include_Tag.xml' path='Tab/Members/Member[@Name="IGrid"]/*'/>
        internal IGrid Grid
        {
            get => _grid;
            set
            {
                if (value != null)
                {
                    _grid = value;
                    if (_grid is Control c)
                    {
                        c.Leave += Hide_event2;
                        c.HandleCreated += (sender, e) =>
                        {
                            if (c.TopLevelControl != null)
                            {
                                c.TopLevelControl.SizeChanged += (_sender, _e) => SetSearchGridLocation();
                                c.TopLevelControl.Move += (_sender, _e) => SetSearchGridLocation();
                                c.TopLevelControl.Enter += Hide_event;
                                c.TopLevelControl.Click += Hide_event;
                                c.TopLevelControl.VisibleChanged += (_sender, _e) =>
                                {
                                    if (!c.TopLevelControl.Visible) Visible = false;
                                };

                                if (c.TopLevelControl is Form f)
                                {
                                    if (f.Visible) Click_Add_Visible_event(c);
                                    else f.Load += (_sender, _e) => Click_Add_Visible_event(c);
                                }
                            }
                        };
                    }
                    if (_grid is SearchTextBox stb)
                    {
                        stb.SubSearchTextBoxes.CollectionChanged += SubSearchTextBoxes_CollectionChanged;
                    }
                }
            }
        }

        private SubSearchTextBoxCollection SubSearchTextBoxes => _grid is ISubSearchTextBoxes sstb ? sstb.SubSearchTextBoxes : null;

        /// <summary>
        /// <para>模糊查找表的窗口</para>
        /// <para><c>SearchForm = new SearchFrom(this);</c></para>
        /// </summary>
        /// <param name="control">需要附加的控件(包含接口IGrid、IDataText)</param>
        public SearchForm(Control control) : base()
        {
            InitializeComponent();
            if (control is IGrid grid) Grid = grid;
            if (control is IDataText dataText) SearchGrid.DataText = dataText;
            if (control is IMultiSelect multiSelect) SearchGrid.MultiSelect = multiSelect;
        }

        /// <summary>
        /// 显示窗口时不将其激活
        /// </summary>
        protected override bool ShowWithoutActivation => true;

        private const int WM_MOUSEACTIVATE = 0x21;
        private const int MA_NOACTIVATE = 3;
        /// <summary>
        /// 处理 Windows 消息。
        /// </summary>
        /// <param name="m">一个 Windows 消息对象。</param>
        [DebuggerStepThrough]
        protected override void WndProc(ref Message m)
        {
            if (m.Msg.Equals(WM_MOUSEACTIVATE))
            {
                m.Result = new IntPtr(MA_NOACTIVATE);
                return;
            }
            base.WndProc(ref m);
        }

        private bool _IsAutoReset = true;
        /// <summary>
        /// 引发 System.Windows.Forms.Control.VisibleChanged 事件。
        /// </summary>
        /// <param name="e">包含事件数据的 System.EventArgs。</param>
        protected override void OnVisibleChanged(EventArgs e)
        {
            if (SearchGrid.DataSource == null) Visible = false;
            if (Visible)
            {
                if (SearchGrid.DataText.IsAutoReset && _IsAutoReset) SearchGrid.DataText.Reset();
                _grid.ShowSearchGrid();
            }
            base.OnVisibleChanged(e);
        }

        /// <include file='Include_Tag.xml' path='Tab/Members/Member[@Name="ShowSearchGrid"]/*'/>
        public virtual void ShowSearchGrid()
        {
            #region 显示并设置网格位置和高宽

            _grid.SetSearchGridSize();

            _grid.SetSearchGridLocation();

            _grid.OnSearchGridLocationSizeChanged(new SearchFormLocationSizeEventArgs(this));

            #endregion
        }

        /// <include file='Include_Tag.xml' path='Tab/Members/Member[@Name="SetSearchGridSize"]/*'/>
        public virtual void SetSearchGridSize()
        {
            if (SearchGrid.RowCount > 0)
            {
                if (!Visible)
                {
                    _IsAutoReset = false;
                    Visible = true;
                    _IsAutoReset = true;
                    return;
                }
                SearchGrid.ClearSelection();
                SearchGrid.Rows[SearchGrid.FirstDisplayedCell?.RowIndex ?? 0].Selected = true;
            }
            else
            {
                Visible = false;
            }

            Width = SearchGrid.RowCount <= _grid.DisplayRowCount
                                            ? SearchGrid.Columns.GetColumnsWidth(DataGridViewElementStates.Visible) + 3
                                            : SearchGrid.Columns.GetColumnsWidth(DataGridViewElementStates.Visible) + 20;  // 计算有展示的列的总宽

            if (SearchGrid.ColumnHeadersVisible) Height = SearchGrid.Rows.Cast<DataGridViewRow>().Take(_grid.DisplayRowCount).Sum(dgvr => dgvr.Height) + 3 + SearchGrid.ColumnHeadersHeight;
            else Height = SearchGrid.Rows.Cast<DataGridViewRow>().Take(_grid.DisplayRowCount).Sum(dgvr => dgvr.Height) + 3;  // 重新设置网格总高
        }

        private Rectangle bounds;
        /// <include file='Include_Tag.xml' path='Tab/Members/Member[@Name="SetSearchGridLocation"]/*'/>
        public virtual void SetSearchGridLocation()
        {
            if (!Bounds.Equals(Rectangle.Empty))
            {
                int Height = SearchGrid.Rows.Cast<DataGridViewRow>().Take(_grid.DisplayRowCount).Sum(dgvr => dgvr.Height) + 3;

                int x, y;
                Rectangle _bounds = bounds.IsEmpty ? _grid.Bounds : bounds;

                y = _grid.IsUp || _bounds.Y + _bounds.Height + Height > Screen.FromControl(this).Bounds.Height
                    ? _bounds.Y - Height
                    : _bounds.Y + _bounds.Height;

                x = _grid.IsLeft || _bounds.X + SearchGrid.Width > Screen.FromControl(this).Bounds.Width
                    ? _bounds.X + _bounds.Width - SearchGrid.Width
                    : _bounds.X;

                Location = new Point(x, y);
            }
        }

        private void TextBox_Enter(object sender, EventArgs e)
        {
            SearchGrid.IsEnter = true;
            if (!Visible)
            {
                //bounds = sender is TextBox tb && SubSearchTextBoxes.Any(sstb => sstb.TextBox.Equals(tb) && sstb.IsMoveGrid) ? (_grid as Control).Parent.RectangleToScreen(tb.Bounds) : _grid.Bounds;
                Show((_grid as Control).TopLevelControl);
            }
        }

        private void Hide_event(object sender, EventArgs e)
        {
            #region 隐藏小表

            if (sender is Control cl)
            {
                if (cl.Text != "退出")
                {
                    if (Visible) Visible = false;
                }
            }

            #endregion
        }

        private void Hide_event2(object sender, EventArgs e)
        {
            #region 隐藏小表

            if (sender is Control cl)
            {
                if (cl.Text != "退出" && (SubSearchTextBoxes == null || SubSearchTextBoxes.Select(sstb => sstb.TextBox).All(tb => !tb.Focused)) && !Focused)
                {
                    if (Visible) Visible = false;
                }
            }

            #endregion
        }

        private void Click_Add_Visible_event(Control cl)
        {
            if (cl != null && cl.Parent != null && cl.TopLevelControl != null)
            {
                SubSearchTextBoxCollection stbc = new SubSearchTextBoxCollection();
                if (_grid is SearchTextBox stb) stbc = stb.SubSearchTextBoxes;
                cl.Parent.Controls.Cast<Control>().Where(c => !c.Equals(_grid) && stbc.Select(sstb => sstb.TextBox).All(tb => !tb.Equals(c))).ToList().ForEach(c =>
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

        private void SubSearchTextBoxes_CollectionChanged(object sender, CollectionChangeEventArgs e)
        {
            switch (e.Action)
            {
                case CollectionChangeAction.Add:
                    if (e.Element is SubSearchTextBox sstb)
                    {
                        sstb.TextBox.Enter -= Hide_event;
                        sstb.TextBox.Click -= Hide_event;
                        //sstb.TextBox.Click += Show_event;
                        sstb.TextBox.Enter += TextBox_Enter;
                        sstb.TextBox.Leave += Hide_event2;
                    }
                    else if (e.Element is IEnumerable<SubSearchTextBox> sstbs)
                    {
                        sstbs.ToList().ForEach(_sstb =>
                        {
                            _sstb.TextBox.Enter -= Hide_event;
                            _sstb.TextBox.Click -= Hide_event;
                            //_sstb.TextBox.Click += Show_event;
                            _sstb.TextBox.Enter += TextBox_Enter;
                            _sstb.TextBox.Leave += Hide_event2;
                        });
                    }
                    break;
                case CollectionChangeAction.Remove:
                    if (e.Element is SubSearchTextBox sstbr)
                    {
                        sstbr.TextBox.Enter += Hide_event;
                        sstbr.TextBox.Click += Hide_event;
                        //sstbr.TextBox.Click -= Show_event;
                        sstbr.TextBox.Enter -= TextBox_Enter;
                        sstbr.TextBox.Leave -= Hide_event2;
                    }
                    else if (e.Element is IEnumerable<SubSearchTextBox> sstbs)
                    {
                        sstbs.ToList().ForEach(_sstb =>
                        {
                            _sstb.TextBox.Enter += Hide_event;
                            _sstb.TextBox.Click += Hide_event;
                            //_sstb.TextBox.Click -= Show_event;
                            _sstb.TextBox.Enter -= TextBox_Enter;
                            _sstb.TextBox.Leave -= Hide_event2;
                        });
                    }
                    break;
                case CollectionChangeAction.Refresh:
                    if (sender is SubSearchTextBoxCollection sstbc)
                    {
                        sstbc.ForEach(_sstb =>
                        {
                            _sstb.TextBox.Enter += Hide_event;
                            _sstb.TextBox.Click += Hide_event;
                            //_sstb.TextBox.Click -= Show_event;
                            _sstb.TextBox.Enter -= TextBox_Enter;
                            _sstb.TextBox.Leave -= Hide_event2;
                        });
                    }
                    break;
            }
        }

        internal void AddEvent()
        {
            if (SearchGrid.DataText.TextBox != null)
            {
                SearchGrid.DataText.TextBox.Enter += TextBox_Enter;
            }
        }
    }
}
