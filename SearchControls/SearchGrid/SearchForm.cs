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

namespace SearchControls
{
    internal partial class SearchForm : Form
    {
        internal IGrid grid { get; set; }
        public SearchForm()
        {
            InitializeComponent();
        }

        protected override bool ShowWithoutActivation => true;

        private const int WM_MOUSEACTIVATE = 0x21;
        private const int MA_NOACTIVATE = 3;
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

        protected override void OnVisibleChanged(EventArgs e)
        {
            if (SearchGrid.DataSource == null) Visible = false;
            if (Visible)
            {
                if (grid.IsAutoReset && grid.isAutoReset) grid.Reset();
                ShowSearchGrid();
            }
            base.OnVisibleChanged(e);
        }

        public void ShowSearchGrid()
        {
            #region 显示并设置网格位置和高宽

            SetSearchGridSize();

            SetSearchGridLocation();

            grid.OnSearchGridLocationSizeChanged(new SubFormLocationSizeEventArgs(this));

            #endregion
        }

        public void SetSearchGridSize()
        {
            int irowcount = SearchGrid.RowCount;

            Width = grid.textBox.ScrollBars.Equals(ScrollBars.Vertical) ? SearchGrid.Columns.GetColumnsWidth(DataGridViewElementStates.Visible) + 3 :
                                                                          SearchGrid.Columns.GetColumnsWidth(DataGridViewElementStates.Visible) + 20;  // 计算有展示的列的总宽

            if (irowcount > 0)
            {
                if (!Visible)
                {
                    Visible = true;
                    return;
                }
                SearchGrid.ClearSelection();
                SearchGrid.Rows[0].Selected = true;     // 选中网格第一行
            }
            else
            {
                Visible = false;
            }

            if (irowcount <= grid.DisplayRowCount)
            {
                if (SearchGrid.ColumnHeadersVisible) Height = SearchGrid.Rows.GetRowsHeight(DataGridViewElementStates.Visible) + 3 + SearchGrid.ColumnHeadersHeight;
                else Height = SearchGrid.Rows.GetRowsHeight(DataGridViewElementStates.Visible) + 3;
                if (!grid.textBox.ScrollBars.Equals(ScrollBars.Vertical)) Width -= 17;  // 设置网格总宽(由于没有上下滚动条，所以总宽要减去 17)
            }
            else
            {
                if (SearchGrid.ColumnHeadersVisible) Height = SearchGrid.Rows.Cast<DataGridViewRow>().Take(grid.DisplayRowCount).Sum(dgvr => dgvr.Height) + 3 + SearchGrid.ColumnHeadersHeight;
                else Height = SearchGrid.Rows.Cast<DataGridViewRow>().Take(grid.DisplayRowCount).Sum(dgvr => dgvr.Height) + 3;  // 重新设置网格总高
            }
        }

        public void SetSearchGridLocation()
        {
            Rectangle TextBoxBounds = grid.textBox.Parent.RectangleToScreen(grid.textBox.Bounds);

            if (grid.IsUp)
            {
                Location = new Point(TextBoxBounds.X, TextBoxBounds.Y - Height);
            }
            else
            {
                Location = new Point(TextBoxBounds.X, TextBoxBounds.Y + TextBoxBounds.Height);
            }
        }
    }
}
