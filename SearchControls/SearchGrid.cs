using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Search
{
    internal partial class SearchGrid : DataGridView
    {
        private DataSet _OriginSource;
        private string _DataMember;
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

        public new object DataSource
        {
            get => _OriginSource == null ? base.DataSource : _OriginSource;
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

        public SearchGrid()
        {
            InitializeComponent();
        }
        [DebuggerStepThrough]
        protected override void OnPaint(PaintEventArgs pe)
        {
            base.OnPaint(pe);
        }

        protected override void OnScroll(ScrollEventArgs e)
        {
            base.OnScroll(e);
            if (e.ScrollOrientation.Equals(ScrollOrientation.VerticalScroll))
            {
                int index = Rows.Cast<DataGridViewRow>().First(dgvr => dgvr.Selected).Index;
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
        [DebuggerStepThrough]
        protected override void WndProc(ref Message m)
        {
            if (m.Msg == WM_LBUTTONDOWN)
            {
                //重写左键方法，避免触发焦点
                WmMouseDown(ref m, MouseButtons.Left, 1);
                m.Result = new IntPtr(MA_NOACTIVATE);
                return;
            }
            base.WndProc(ref m);
        }

        [DebuggerStepThrough]
        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);
            if (e.Button.Equals(MouseButtons.Left))
            {
                OnClick(e);
                OnMouseClick(e);
            }
        }

        protected override void OnMouseClick(MouseEventArgs e)
        {
            HitTestInfo hti = HitTest(e.X, e.Y);

            DataGridViewCellMouseEventArgs dgvcme = null;
            if (hti.Type != DataGridViewHitTestType.None &&
                hti.Type != DataGridViewHitTestType.HorizontalScrollBar &&
                hti.Type != DataGridViewHitTestType.VerticalScrollBar)
            {
                int mouseX = e.X - hti.ColumnX;
                dgvcme = new DataGridViewCellMouseEventArgs(hti.ColumnIndex, hti.RowIndex, mouseX, e.Y - hti.RowY, e);
                if (e.Button == MouseButtons.Left)
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
            if (ColumnHeadersVisible)
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
