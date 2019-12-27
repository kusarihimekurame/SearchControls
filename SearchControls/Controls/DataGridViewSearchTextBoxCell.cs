using SearchControls.SearchGridForm;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
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
        /// 附加并初始化寄宿的编辑控件。
        /// </summary>
        /// <param name="rowIndex">所编辑的行的索引。</param>
        /// <param name="initialFormattedValue">要在控件中显示的初始值。</param>
        /// <param name="dataGridViewCellStyle">用于确定寄宿控件外观的单元格样式。</param>
        public override void InitializeEditingControl(int rowIndex, object initialFormattedValue, DataGridViewCellStyle dataGridViewCellStyle)
        {
            base.InitializeEditingControl(rowIndex, initialFormattedValue, dataGridViewCellStyle);
            if (DataGridView is SearchDataGridView sdgv && sdgv.SearchGrid is SearchGrid sg)
            {
                sg.Add_TextBoxEvent();
            }
        }

        /// <summary>
        /// 从 System.Windows.Forms.DataGridView 中删除单元格的编辑控件。
        /// </summary>
        public override void DetachEditingControl()
        {
            if (DataGridView is SearchDataGridView sdgv && sdgv.SearchGrid is SearchGrid sg)
            {
                sg.Remove_TextBoxEvent();
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
