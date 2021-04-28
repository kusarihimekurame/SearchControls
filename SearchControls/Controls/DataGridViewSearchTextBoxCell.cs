using SearchControls.Interface;
using SearchControls.SearchGridForm;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace SearchControls
{
    /// <summary>
    /// 显示 System.Windows.Forms.DataGridView 控件中的可编辑文本信息。
    /// </summary>
    public class DataGridViewSearchTextBoxCell : DataGridViewTextBoxCell
    {
        /// <summary>
        /// 获取单元格的寄宿编辑控件的类型。
        /// </summary>
        /// <returns>
        /// 一个 <see cref="Type"/> 表示 <see cref="DataGridViewSearchTextBoxControl"/> 类型。
        /// </returns>
        public override Type EditType => typeof(DataGridViewSearchTextBoxControl);

        /// <summary>
        /// 搜索表格中的当前绑定的数据行
        /// </summary>
        public object SearchDataBoundItem { get; private set; }

        /// <summary>
        /// 附加并初始化寄宿的编辑控件。
        /// </summary>
        /// <param name="rowIndex">所编辑的行的索引。</param>
        /// <param name="initialFormattedValue">要在控件中显示的初始值。</param>
        /// <param name="dataGridViewCellStyle">用于确定寄宿控件外观的单元格样式。</param>
        public override void InitializeEditingControl(int rowIndex, object initialFormattedValue, DataGridViewCellStyle dataGridViewCellStyle)
        {
            base.InitializeEditingControl(rowIndex, initialFormattedValue, dataGridViewCellStyle);
            DataGridViewSearchTextBoxControl stb = DataGridView.EditingControl as DataGridViewSearchTextBoxControl;
            DataGridViewSearchTextBoxColumn column = OwningColumn as DataGridViewSearchTextBoxColumn;
            DataGridViewSearchTextBoxColumn dataColumn = column.IsMain ? column : DataGridView.Columns[column.MainColumnName] as DataGridViewSearchTextBoxColumn;
            stb.Columns.Clear();
            if (dataColumn.SearchColumns.Count > 0)
            {
                stb.SearchGrid.AutoGenerateColumns = false;
                stb.Columns.AddRange(dataColumn.SearchColumns.ToArray());
            }
            else
            {
                stb.SearchGrid.AutoGenerateColumns = true;
            }
            stb.DataSource = dataColumn.SearchDataSource ?? (DataGridView.DataSource is BindingSource bs ? bs.DataSource : DataGridView.DataSource);
            stb.DataMember = dataColumn.SearchDataMember;
            stb.AutoInputDataName = column.AutoInputDataName;
            stb.DisplayDataName = column.DisplayDataName;
            stb.DisplayRowCount = column.DisplayRowCount;
            stb.IsTextChanged = column.IsTextChanged;
            stb.IsAutoInput = column.IsAutoInput;
            stb.TextChangedColumnNames = column.TextChangedColumnNames;
        }

        /// <summary>
        /// 从 System.Windows.Forms.DataGridView 中删除单元格的编辑控件。
        /// </summary>
        public override void DetachEditingControl()
        {
            DataGridViewSearchTextBoxControl stb = DataGridView.EditingControl as DataGridViewSearchTextBoxControl;

            stb.IsTextChanged = false;

            SearchDataBoundItem = stb.CurrentRow?.DataBoundItem;
            if (SearchDataBoundItem != null && OwningColumn is DataGridViewSearchTextBoxColumn currentColumn)
            {
                DataGridViewSearchTextBoxColumn MainColumn =
                    currentColumn.IsMain
                    ? currentColumn
                    : DataGridView.Columns[currentColumn.MainColumnName] as DataGridViewSearchTextBoxColumn;

                if (stb.Text == stb.CurrentRow.Cells[currentColumn.DisplayDataName].Value.ToString())
                {
                    DataGridView.Columns.Cast<DataGridViewColumn>().Where(dgvc => dgvc != OwningColumn && dgvc is DataGridViewSearchTextBoxColumn).Cast<DataGridViewSearchTextBoxColumn>()
                        .Where(stbc =>
                            !string.IsNullOrEmpty(stbc.DisplayDataName)
                            && (stbc.Equals(MainColumn)
                                || !stbc.IsMain
                                && (stbc.MainColumnName.Equals(MainColumn.Name, StringComparison.OrdinalIgnoreCase) || stbc.MainColumnName.Equals(MainColumn.DataPropertyName, StringComparison.OrdinalIgnoreCase))
                            )
                        ).ToList().ForEach(stbc =>
                        {
                            if (!OwningRow.Cells[stbc.Name].Value.ToString().Equals(stb.CurrentRow.Cells[stbc.DisplayDataName].Value.ToString())) OwningRow.Cells[stbc.Name].Value = stb.CurrentRow.Cells[stbc.DisplayDataName].Value.ToString();
                        });
                }
                else
                {
                    DataGridView.Columns.Cast<DataGridViewColumn>().Where(dgvc => dgvc != OwningColumn && dgvc is DataGridViewSearchTextBoxColumn).Cast<DataGridViewSearchTextBoxColumn>()
                        .Where(stbc =>
                            !string.IsNullOrEmpty(stbc.DisplayDataName)
                            && (stbc.Equals(MainColumn)
                                || !stbc.IsMain
                                && (stbc.MainColumnName.Equals(MainColumn.Name, StringComparison.OrdinalIgnoreCase) || stbc.MainColumnName.Equals(MainColumn.DataPropertyName, StringComparison.OrdinalIgnoreCase))
                            )
                        ).ToList().ForEach(stbc =>
                        {
                            if (!OwningRow.Cells[stbc.Name].Value.ToString().Equals(stb.CurrentRow.Cells[stbc.DisplayDataName].Value.ToString())) OwningRow.Cells[stbc.Name].Value = string.Empty;
                        });
                }
            }
            base.DetachEditingControl();
        }

        /// <summary>
        /// 返回描述当前对象的字符串。
        /// </summary>
        /// <returns>表示当前对象的字符串。</returns>
        public override string ToString()
        {
            return "DataGridViewSearchTextBoxCell { ColumnIndex=" + ColumnIndex.ToString(CultureInfo.CurrentCulture) + ", RowIndex=" + RowIndex.ToString(CultureInfo.CurrentCulture) + " }";
        }
    }
}
