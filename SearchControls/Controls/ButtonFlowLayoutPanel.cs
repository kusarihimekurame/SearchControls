using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace SearchControls.Controls
{
    internal partial class ButtonFlowLayoutPanel : FlowLayoutPanel
    {
        public DataGridView DataGridView { get; set; }

        public ButtonFlowLayoutPanel()
        {
            InitializeComponent();
        }

        public void BtnFirst_Click(object sender, EventArgs e)
        {
            #region "第一"按钮btnFirst单击事件

            // 网格显示的总记录数必需 > 0
            if (DataGridView.RowCount > 0)
            {
                DataGridView.CurrentCell = DataGridView[0, 0];
            }

            #endregion
        }

        public void BtnDown_Click(object sender, EventArgs e)
        {
            #region  "向下"按钮btnDown单击事件(dataGridView1)

            // 网格显示的总记录数必需 > 0
            if (DataGridView.RowCount > 0)
            {
                int row = DataGridView.CurrentRow.Index + 1;
                if (row > DataGridView.RowCount - 1)      // 如果row大于总行数
                {
                    row = 0;
                }
                DataGridView.CurrentCell = DataGridView[0, row];
            }

            #endregion
        }

        public void BtnUp_Click(object sender, EventArgs e)
        {
            #region "向上"按钮btnUp单击事件(dataGridView1)

            // 网格显示的总记录数必需 > 0
            if (DataGridView.RowCount > 0)
            {
                int row = DataGridView.CurrentRow.Index - 1;
                if (row < 0)
                {
                    row = DataGridView.RowCount - 1;
                }
                DataGridView.CurrentCell = DataGridView[0, row];
            }

            #endregion
        }

        public void BtnLast_Click(object sender, EventArgs e)
        {
            #region "最后"按钮btnLast单击事件

            // 网格显示的总记录数必需 > 0
            if (DataGridView.RowCount > 0)
            {
                int row;
                row = DataGridView.RowCount - 1;
                DataGridView.CurrentCell = DataGridView[0, row];
            }

            #endregion
        }

        public void BtnInsert_Click(object sender, EventArgs e)
        {
            #region “添加”按钮btnInsert的单击事件

            DataGridView.Rows.Add();
            //DataRow dr = dataSet.Tables[TableName].NewRow();

            //if (DataGridView.DataSource is DataTable dt  dataSet.Tables[TableName].DefaultView.RowFilter != "")
            //    dataSet.Tables[TableName].DefaultView.RowFilter += "OR " + PrimaryKey[0].ColumnName + " = " + dr[PrimaryKey[0].ColumnName];

            //dataSet.Tables[TableName].Rows.Add(dr); //在dt表的末尾添加一条白记录(其中NID列已赋值)
            DataGridView.CurrentCell = DataGridView[DataGridView.CurrentCell.ColumnIndex, DataGridView.Rows.Cast<DataGridViewRow>().Last(dgvr => !dgvr.IsNewRow).Index];    // 将焦点移到最后新添加的一行

            #endregion
        }

        public void BtnDelete_Click(object sender, EventArgs e)
        {
            #region "删除"按钮btnDelete单击事件

            DataGridView.Rows.Remove(DataGridView.CurrentRow);

            //string cNid = dataGridView.CurrentRow.Cells[PrimaryKey[0].ColumnName].Value.ToString(); //当前网格NID列的值   

            //if (cNid != "")
            //{
            //    int ColumnIndex = dataGridView.CurrentCell.ColumnIndex;
            //    int RowIndex = dataGridView.CurrentCell.RowIndex - 1;
            //    if (RowIndex < 0) RowIndex = 0;

            //    DataRow dr = dataSet.Tables[TableName].Rows.Find(cNid);  //用Find()方法在dt表中定位 “dt表NID列的值 = 网格NID列的值” 的行(dr是DataRow行，包含该行各列的值)

            //    //dr[PrimaryKey[0].ColumnName] = cNid; //求当前行中"NID"行的值
            //    dr.Delete(); //删除dt表中被Find()定位后的dr行

            //    if (dataGridView.RowCount == 0) dataGridView.AllowUserToAddRows = true;
            //    dataGridView.CurrentCell = dataGridView[ColumnIndex, RowIndex];    // 将焦点移到删除的单元格的上面一格

            //    dataGridView_CellValueChanged(null, null);
            //}

            #endregion
        }

        public void BtnUpdate_Click(object sender, EventArgs e)
        {
            #region "提交"按钮btnUpdate单击事件

            //if (dataSet.GetChanges() == null)
            //{
            //    toolUpdateStatus.Text = "没有更改任何记录，请更改后再按“提交”按钮";
            //}
            //else
            //{
            //    try
            //    {
            //        string sql = sqlDataAdapter.SelectCommand.CommandText;
            //        int val;

            //        // 如何是多表则执行多表更新（Class_Update），否则执行单表更新（Class_Updata_DB).
            //        if (sql.Contains("JOIN")) val = Class_Update.sqlupdate(sql, dataSet, TableName);
            //        else if (Connection.Equals(SqlHelp.ReturnSqlCon(""))) val = Class_Update_DB.Update(sql, dataSet, TableName);
            //        else val = Class_Update_DB.Update(sql, dataSet, TableName, "WZH");

            //        dataSet.AcceptChanges();  //DataSet接受更改，以便为下一次更改做准备
            //                                  //MessageBox.Show("共计更新记录数为： " + val + "  条，已将所有需更新的记录提交到“ SQL SERVER ”数据库，更新成功!", "提交更新信息框：");
            //        toolUpdateStatus.Text = "更新成功，共计更新" + val + "条记录";

            //    }
            //    catch (Exception ex)
            //    {
            //        MessageBox.Show(ex.Message);
            //    }
            //}

            #endregion
        }

        bool IsHavTask = false;
        public async void BtnFound_Click(object sender, EventArgs e)
        {
            #region "查询"按钮btnFound单击事件

            //if (e != null)
            //{
            //    DataSet _ds = await FoundAsync();
            //    if (_ds == null) return;
            //    dataSet = _ds;
            //    dataGridView.DataSource = dataSet.Tables[TableName];
            //}
            //else
            //{
            //    string dataSetSql;
            //    dataSet = new DataSet();
            //    ///*
            //    //if (e == null)
            //    //{
            //    //    if (Regex.IsMatch(sql, "(DISTINCT)")) dataSetSql = Regex.Replace(sql, "(DISTINCT)", "$1 TOP 1001");
            //    //    else dataSetSql = Regex.Replace(sql, "(SELECT)", "$1 TOP 1001");
            //    //    dataSetSql = Class_Sql.Topspl(dataSetSql);  //如果没有点击查找按钮，则进行模糊查找前1001条记录
            //    //    cMessage = "模糊";
            //    //}
            //    //else
            //    //{
            //    //    cMessage = "查找";
            //    //}
            //    //*/
            //    if (Sql.Contains("WHERE")) dataSetSql = Sql;
            //    else dataSetSql = Regex.Replace(Sql, "(ORDER)", cWHERE + " $1");

            //    sqlDataAdapter = new SqlDataAdapter(dataSetSql, Connection) { MissingSchemaAction = MissingSchemaAction.AddWithKey /*获取主键信息*/};
            //    sqlDataAdapter.Fill(dataSet, TableName);

            //    dataGridView.DataSource = dataSet.Tables[TableName];
            //    dataGridView.Columns[PrimaryKey[0].ColumnName].Visible = false;

            //    if (_cHZPY != null)
            //    {
            //        HZPY = Class_HZPY.HZPY(_cHZPY, dataSet.Tables[TableName]);
            //    }

            //    //Class_QJBL.cF1 = dataSet.Tables[TableName].Columns["F1"].DefaultValue.ToString().Trim(); //将业务编号“F1”存储到全局变量类“Class_QJBL.cF1”中
            //    //Class_QJBL.cF31 = dataSet.Tables[TableName].Columns["F31"].DefaultValue.ToString().Trim(); //将提单号“F31”存储到全局变量类“Class_QJBL.cF31”中

            //}

            //if (_RowFilter != null) RowFilter = _RowFilter;

            #endregion
        }

        //public async Task<DataSet> FoundAsync() => await Task.Factory.StartNew(() =>
        //{
        //    IsHavTask = true;

        //    string dataSetSql;
        //    DataSet dataSet = new DataSet();
        //    /*
        //    if (e == null)
        //    {
        //        if (Regex.IsMatch(sql, "(DISTINCT)")) dataSetSql = Regex.Replace(sql, "(DISTINCT)", "$1 TOP 1001");
        //        else dataSetSql = Regex.Replace(sql, "(SELECT)", "$1 TOP 1001");
        //        dataSetSql = Class_Sql.Topspl(dataSetSql);  //如果没有点击查找按钮，则进行模糊查找前1001条记录
        //        cMessage = "模糊";
        //    }
        //    else
        //    {
        //        cMessage = "查找";
        //    }
        //    */
        //    if (Sql.Contains("WHERE")) dataSetSql = Sql;
        //    else dataSetSql = Regex.Replace(Sql, "(ORDER)", cWHERE + " $1");

        //    sqlDataAdapter = new SqlDataAdapter(dataSetSql, Connection) { MissingSchemaAction = MissingSchemaAction.AddWithKey /*获取主键信息*/};
        //    sqlDataAdapter.Fill(dataSet, TableName);

        //    if (!IsHavTask) return null;
        //    else
        //    {
        //        IsHavTask = false;
        //        return dataSet;
        //    }
        //}, TaskCreationOptions.PreferFairness);

        public async void BtnExcel_Click(object sender, EventArgs e)
        {
            #region 导出Excel按钮单击事件

            //BtnExcel.Enabled = false;
            //BtnQuit.Enabled = false;
            //toolProgressBarStatus.Text = "正在转换Excel";

            //string title = Title ?? TopLevelControl.Text;

            //string fileExtension = ".xlsx"; //文件后缀名
            //string path = Class_QJBL.TMP_Path + title.Replace(" ", "") + fileExtension;

            //if (!Directory.Exists(Class_QJBL.TMP_Path)) //如果文件夹不存在则创建一个文件夹
            //{
            //    Directory.CreateDirectory(Class_QJBL.TMP_Path);
            //}

            ////检查 control_BigGrid1.dataSet.Tables 表是否存在
            //if (dataSet != null)
            //{
            //    if (dataSet.Tables.Contains(dataSet.Tables[TableName].TableName)) //查找 dt 表是否存在
            //    {
            //        if (WordExport[0] == null)
            //        {
            //            ExportDataTable = dataSet.Tables[TableName].DefaultView.ToTable(TableName);
            //            if (ExportDataTable.Columns.Contains("NID")) ExportDataTable.Columns.Remove("NID");
            //            for (int i = 0; i < ExportDataTable.Columns.Count; i++)
            //            {
            //                ExportDataTable.Columns[i].ColumnName = dataGridView.Columns[i].HeaderText;
            //            }
            //        }
            //        else
            //        {
            //            //筛选DataTable中某些字段为一张新表：  
            //            ExportDataTable = dataSet.Tables[TableName].DefaultView.ToTable(false, ExcelExport[0]);
            //            for (int i = 0; i < ExportDataTable.Columns.Count; i++)
            //            {
            //                ExportDataTable.Columns[i].ColumnName = ExcelExport[1][i];
            //            }
            //        }

            //        Class_QJBL.delete(path);

            //        toolStripProgressBar.Value = 0;
            //        toolStripProgressBar.Maximum = ExportDataTable.Rows.Count;
            //        excel = await Task.Factory.StartNew(() => ExcelHelper.ExportToExcel(ExportDataTable, title));

            //        SetExcelState?.Invoke(excel);

            //        excel.Application.ActiveWorkbook.SaveAs(path); // 将文件另存为 @"D:\无纸化TMP\业务清单.xls"

            //        excel.Visible = true;

            //        if (TopLevelControl != null)
            //        {
            //            toolStripProgressBar.Value = toolStripProgressBar.Maximum;
            //            toolProgressBarStatus.Text = "转换Excel成功";
            //        }
            //    }

            //    else
            //    {
            //        MessageBox.Show("注意：表不存在 ！请统计好表后再打印报表 ！！");
            //        toolProgressBarStatus.Text = "转换Excel失败";
            //        return;
            //    }
            //}

            //if (TopLevelControl != null)
            //{
            //    btnExcel.Enabled = true;
            //    btnQuit.Enabled = true;
            //}

            #endregion
        }

        public async void BtnWord_Click(object sender, EventArgs e)
        {
            #region 导出Word按钮单击事件

            //btnWord.Enabled = false; //按钮变暗(使其不可用，防止两次单击按钮)
            //btnQuit.Enabled = false; //按钮变暗(使其不可用，word转换过程中退出会造成出错）
            //toolProgressBarStatus.Text = "正在转换Word";

            //string title = Title ?? TopLevelControl.Text;

            //string fileExtension = ".docx"; //文件后缀名
            //string picName = @"E:\VS2010编程文件\WindowsFormsApplication10\金鹏公司.bmp";
            //string path = Class_QJBL.TMP_Path + title.Replace(" ", "") + fileExtension;

            //if (!Directory.Exists(Class_QJBL.TMP_Path)) //如果文件夹不存在则创建一个文件夹
            //{
            //    Directory.CreateDirectory(Class_QJBL.TMP_Path);
            //}

            ////检查 control_BigGrid1.dataSet.Tables 表是否存在
            //if (dataSet != null)
            //{
            //    if (dataSet.Tables.Contains(dataSet.Tables[TableName].TableName)) //查找 dt 表是否存在
            //    {
            //        if (WordExport[0] == null)
            //        {
            //            ExportDataTable = dataSet.Tables[TableName].DefaultView.ToTable(TableName);
            //            if (ExportDataTable.Columns.Contains("NID")) ExportDataTable.Columns.Remove("NID");
            //            for (int i = 0; i < ExportDataTable.Columns.Count; i++)
            //            {
            //                ExportDataTable.Columns[i].ColumnName = dataGridView.Columns[i].HeaderText;
            //            }
            //        }
            //        else
            //        {
            //            //筛选DataTable中某些字段为一张新表：  
            //            ExportDataTable = dataSet.Tables[TableName].DefaultView.ToTable(false, WordExport[0]);
            //            for (int i = 0; i < ExportDataTable.Columns.Count; i++)
            //            {
            //                ExportDataTable.Columns[i].ColumnName = WordExport[1][i];
            //            }
            //        }

            //        toolStripProgressBar.Value = 0;
            //        toolStripProgressBar.Maximum = ExportDataTable.Rows.Count;
            //        await Task.Factory.StartNew(() => WordPlayer.ExportToWord(ExportDataTable, isShowWord, path, picName, zzfx, title));

            //        if (TopLevelControl != null)
            //        {
            //            toolStripProgressBar.Value = toolStripProgressBar.Maximum;
            //            toolProgressBarStatus.Text = "转换Word成功";
            //        }
            //    }

            //    else
            //    {
            //        MessageBox.Show("注意：表不存在 ！请统计好表后再打印报表 ！！");
            //        toolProgressBarStatus.Text = "转换Word失败";
            //        return;
            //    }
            //}

            //if (TopLevelControl != null)
            //{
            //    btnWord.Enabled = true;
            //    btnQuit.Enabled = true;
            //}

            #endregion
        }

        public void BtnCancel_Click(object sender, EventArgs e)
        {
            #region "撤销"按钮btnCancel单击事件
            //dataSet.Tables[TableName].RejectChanges(); //取消对DataTable的更改
            //dataGridView_CellValueChanged(null, null);
            #endregion
        }

        public void BtnQuit_Click(object sender, EventArgs e)
        {
            #region 退出按钮事件

            if (TopLevelControl is Form f)
            {
                f.Close();
            }

            #endregion
        }
    }
}
