using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics.Contracts;
using System.Drawing;
using System.Linq;
using System.Security.Permissions;
using System.Text;
using System.Windows.Forms;

namespace SearchControls
{
    internal partial class SearchDataGridView : DataGridView
    {
        public SearchDataGridView()
        {
            InitializeComponent();
            Columns.CollectionChanged += Columns_CollectionChanged;
        }

        private void Columns_CollectionChanged(object sender, CollectionChangeEventArgs e)
        {
            if (e.Action.Equals(CollectionChangeAction.Add) && e.Element is DataGridViewSearchTextBoxColumn stbc)
            {

            }
        }

        protected override void OnPaint(PaintEventArgs pe)
        {
            base.OnPaint(pe);
        }

        protected override void OnKeyUp(KeyEventArgs e)
        {
            base.OnKeyDown(e);
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (IsCurrentCellInEditMode)
            {
                switch (keyData)
                {
                    case Keys.Up:
                        OnKeyUp(new KeyEventArgs(keyData));
                        return true;
                    case Keys.Down:
                        OnKeyDown(new KeyEventArgs(keyData));
                        return true;
                }
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }
    }
}
