using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace SearchControls
{
    /// <summary>
    /// 表示 Windows 状态栏控件。
    /// </summary>
    public partial class GridStatusStrip : StatusStrip
    {
        private DataGridView dataGridView;
        /// <summary>
        /// 绑定网格，为了显示网格状态
        /// </summary>
        public DataGridView DataGridView
        {
            get => dataGridView;
            set
            {
                dataGridView = value;
                if (dataGridView != null)
                {
                    if (dataGridView.DataSource is BindingSource bs)
                    {
                        bs.ListChanged += (sender, e) => DataRefresh();
                        bs.CurrentItemChanged += (sender, e) => DataRefresh();
                    }
                    else
                    {
                        dataGridView.CellValueChanged += (sender, e) => DataRefresh();
                        dataGridView.Rows.CollectionChanged += (sender, e) => DataRefresh();
                    }
                }
            }
        }

        /// <summary>
        /// 初始化
        /// </summary>
        public GridStatusStrip()
        {
            InitializeComponent();
            SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.AllPaintingInWmPaint, true);
        }

        /// <summary>
        /// 数据刷新
        /// </summary>
        public void DataRefresh()
        {
            object dataSource;
            string dataMember;
            if (dataGridView.DataSource is BindingSource bs)
            {
                bs.EndEdit();
                ToolRecordCount.Text = string.Format("总记录数：{0}条", bs.Count);
                dataSource = bs.DataSource;
                dataMember = bs.DataMember;
            }
            else
            {
                ToolRecordCount.Text = string.Format("总记录数：{0}条", dataGridView.Rows.Cast<DataGridViewRow>().Count(dgvr => !dgvr.IsNewRow));
                dataSource = dataGridView.DataSource;
                dataMember = dataGridView.DataMember;
            }

            DataTable dt = null;
            if (dataSource is DataSet ds) dt = ds.Tables[dataMember];
            else if (dataSource is DataTable _dt) dt = _dt;
            else if (dataSource is DataView dv) dt = dv.Table;

            if (dt != null && dt.GetChanges() != null)
            {
                if (ToolUpdateStatus.ForeColor.Equals(Color.Red)) ToolUpdateStatus.ForeColor = Color.Black;
                ToolChangeRecordCount.Text = dt.GetChanges() == null ? string.Empty : string.Format("更改的记录数：{0}条", dt.GetChanges().Rows.Count);
                ToolUpdateStatus.Text = string.IsNullOrEmpty(ToolChangeRecordCount.Text) ? string.Empty : "未提交";
            }
            else
            {
                ToolChangeRecordCount.Text = "";
                ToolUpdateStatus.Text = "";
            }
        }
    }
}
