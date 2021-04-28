using SearchControls.Interface;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using System.Text;
using System.Windows.Forms;

namespace SearchControls.SearchGridForm
{
    /// <summary>
    /// 模糊查找表的窗口
    /// </summary>
    public partial class SearchForm : NoActivateForm
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
                                c.TopLevelControl.SizeChanged += (_sender, _e) =>
                                {
                                    SetSearchGridLocation();
                                    _grid.OnSearchGridLocationSizeChanged(new SearchFormLocationSizeEventArgs(this));
                                };
                                c.TopLevelControl.Move += (_sender, _e) =>
                                {
                                    SetSearchGridLocation();
                                    _grid.OnSearchGridLocationSizeChanged(new SearchFormLocationSizeEventArgs(this));
                                };
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
                    if (_grid is ISubSearchTextBoxes stb)
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
                    Show((_grid as Control).TopLevelControl);
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

        //private Rectangle bounds;
        /// <include file='Include_Tag.xml' path='Tab/Members/Member[@Name="SetSearchGridLocation"]/*'/>
        public virtual void SetSearchGridLocation()
        {
            if (!Bounds.Equals(Rectangle.Empty))
            {
                int Height = SearchGrid.Rows.Cast<DataGridViewRow>().Take(_grid.DisplayRowCount).Sum(dgvr => dgvr.Height) + 3;

                int x, y;
                //Rectangle _bounds = bounds.IsEmpty ? _grid.GetBounds() : bounds;
                Rectangle _bounds = _grid.GetBounds();

                y = _grid.IsUp || _bounds.Top - Screen.FromRectangle(_bounds).Bounds.Top + _bounds.Height + Height > Screen.FromControl(this).Bounds.Height
                    ? _bounds.Top - Height - (SearchGrid.ColumnHeadersVisible ? SearchGrid.ColumnHeadersHeight : 0)
                    : _bounds.Top + _bounds.Height;

                x = _grid.IsLeft.HasValue && _grid.IsLeft.Value || !_grid.IsLeft.HasValue && _bounds.Left - Screen.FromRectangle(_bounds).Bounds.Left + SearchGrid.Width > Screen.FromControl(this).Bounds.Width
                    ? _bounds.Left + _bounds.Width - SearchGrid.Width
                    : _bounds.Left;

                Location = new Point(x, y);
            }
        }

        private void TextBox_Enter(object sender, EventArgs e)
        {
            SearchGrid.IsEnter = true;
            if (!Visible && Grid.IsEnterShow)
            {
                //bounds = sender is TextBox tb && SubSearchTextBoxes.Any(sstb => sstb.TextBox.Equals(tb) && sstb.IsMoveGrid) ? (_grid as Control).Parent.RectangleToScreen(tb.Bounds) : _grid.Bounds;
                Show((_grid as Control).TopLevelControl);
#if NETCOREAPP3_0_OR_GREATER
                object window = typeof(Control).GetField("_window", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(this);
#else
                object window = typeof(Control).GetField("window", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(this);
#endif

                SetWindowPos(
                    new HandleRef(window, Handle), new HandleRef(null, (IntPtr)0), 0, 0, 0, 0,
                    0x0002 | 0x0001 | 0x0010
                );
                
                //BringToFront();
                //(_grid as Control).TopLevelControl.Focus();
            }
        }

        [DllImport("User32", ExactSpelling = true, CharSet = CharSet.Auto)]
        [ResourceExposure(ResourceScope.None)]
        private static extern bool SetWindowPos(HandleRef hWnd, HandleRef hWndInsertAfter,
                                               int x, int y, int cx, int cy, int flags);

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
#if NET40
                    EventHandlerList eventHandlerList = c.GetType().GetProperty("Events", BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance).GetValue(c, null) as EventHandlerList;
#else
                    EventHandlerList eventHandlerList = c.GetType().GetProperty("Events", BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance).GetValue(c) as EventHandlerList;
#endif
                    var dels = new[] 
                    { 
                        new 
                        { 
                            eventInfo = c.GetType().GetEvents().FirstOrDefault(ei => ei.Name == "Click"),
#if NETCOREAPP3_0_OR_GREATER
                            del = eventHandlerList[typeof(Control).GetField("s_clickEvent", BindingFlags.NonPublic | BindingFlags.Static).GetValue(c)]
#else
                            del = eventHandlerList[typeof(Control).GetField("EventClick", BindingFlags.NonPublic | BindingFlags.Static).GetValue(c)] 
#endif

                        },
                        new 
                        { 
                            eventInfo = c.GetType().GetEvents().FirstOrDefault(ei => ei.Name == "Enter"),
#if NETCOREAPP3_0_OR_GREATER
                            del = eventHandlerList[typeof(Control).GetField("s_enterEvent", BindingFlags.NonPublic | BindingFlags.Static).GetValue(c)]
#else
                            del = eventHandlerList[typeof(Control).GetField("EventEnter", BindingFlags.NonPublic | BindingFlags.Static).GetValue(c)] 
#endif
                        }
                    };
                    
                    c.Enter += Hide_event;
                    c.Click += Hide_event;
                    
                    foreach (var del in from del in dels.Where(_del => _del.del != null)
                                        from Delegate h in del.del.GetInvocationList()
                                        select new { del.eventInfo, h })
                    {
                        del.eventInfo.RemoveEventHandler(c, del.h);
                        del.eventInfo.AddEventHandler(c, del.h);
                    }

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
