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
            if (m.Msg == WM_MOUSEACTIVATE)
            {
                m.Result = new IntPtr(MA_NOACTIVATE);
                return;
            }
            base.WndProc(ref m);
        }

        protected override void OnVisibleChanged(EventArgs e)
        {
            if (SearchGrid.DataSource == null) Visible = false;
            base.OnVisibleChanged(e);
        }
    }
}
