using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace SearchControls
{
    /// <summary>
    /// 创建列的方法
    /// </summary>
    public static class Method
    {
        /// <summary>
        /// 创建多音首字母列
        /// </summary>
        /// <param name="dataTable">需要创建列的对应表单DataTable</param>
        /// <param name="dataColumns">需要创建的对应列</param>
        /// <returns>拼音列</returns>
        public static DataColumn[] CreateManyInitialsDataColumn(DataTable dataTable, DataColumn[] dataColumns) => PinYinConverter.PinYin.CreateManyInitialsDataColumn(dataTable, dataColumns);
        /// <summary>
        /// 创建多音首字母列
        /// </summary>
        /// <param name="dataTable">需要创建列的对应表单DataTable</param>
        /// <param name="columnNames">需要创建的对应列名</param>
        /// <returns>拼音列</returns>
        public static DataColumn[] CreateManyInitialsDataColumn(DataTable dataTable, string[] columnNames) => PinYinConverter.PinYin.CreateManyInitialsDataColumn(dataTable, columnNames);

        /// <summary>
        /// 根据列名进行模糊查找
        /// </summary>
        /// <param name="dataTable">内存表</param>
        /// <param name="text">文本</param>
        /// <param name="columnNames">列名（为空时根据所有的列名进行查找）</param>
        /// <returns>dataTable.DefaultView.RowFilter</returns>
        public static string CreateRowFilter(DataTable dataTable, string text, params string[] columnNames)
        {
            if (columnNames.Count().Equals(0)) columnNames = dataTable.Columns.Cast<DataColumn>().Select(dc => dc.ColumnName).ToArray();
            List<string> rowFilter = new List<string>();
            string[] Texts = text.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            if (Texts.Count() > 0)
            {
                Texts.ToList().ForEach(t =>
                    rowFilter.Add("(" + string.Join("OR ",
                        dataTable.Columns.Cast<DataColumn>().
                            Where(dc => (columnNames.Contains(dc.ColumnName) || columnNames.Select(cn => "PY_" + cn).Contains(dc.ColumnName)) && dc.DataType.Equals(typeof(string))).
                            Select(dc => string.Format("{0} LIKE '%{1}%' ", dc.ColumnName, t.Replace("'", "''").Replace("[", "[[ ").Replace("]", " ]]").Replace("*", "[*]").Replace("%", "[%]").Replace("[[ ", "[[]").Replace(" ]]", "[]]")))
                        ) + ") ")
                );
            }
            return dataTable.DefaultView.RowFilter = string.Join("AND ", rowFilter);
        }
    }
}
