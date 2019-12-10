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

        protected override void OnEditingControlShowing(DataGridViewEditingControlShowingEventArgs e)
        {
            base.OnEditingControlShowing(e);
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (IsCurrentCellInEditMode)
            {
                switch (keyData)
                {
                    case Keys.Up:
                        //OnKeyUp(new KeyEventArgs(keyData));
                        return true;
                    case Keys.Down:
                        //OnKeyDown(new KeyEventArgs(keyData));
                        return true;
                }
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }
    }
}
