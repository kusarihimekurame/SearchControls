using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        public static DataColumn[] CreateManyInitialsDataColumn(DataTable dataTable, params DataColumn[] dataColumns) => PinYinConverter.PinYin.CreateManyInitialsDataColumn(dataTable, dataColumns);
        /// <summary>
        /// 创建多音首字母列(多线程)
        /// </summary>
        /// <param name="dataTable">需要创建列的对应表单DataTable</param>
        /// <param name="dataColumns">需要创建的对应列</param>
        /// <returns>拼音列</returns>
#if NET40
        public static Task<DataColumn[]> CreateManyInitialsDataColumnAsync(DataTable dataTable, params DataColumn[] dataColumns) => Task.Factory.StartNew(() => CreateManyInitialsDataColumn(dataTable, dataColumns));
#else
        public static Task<DataColumn[]> CreateManyInitialsDataColumnAsync(DataTable dataTable, params DataColumn[] dataColumns) => Task.Run(() => CreateManyInitialsDataColumn(dataTable, dataColumns));
#endif
        /// <summary>
        /// 创建多音首字母列
        /// </summary>
        /// <param name="dataTable">需要创建列的对应表单DataTable</param>
        /// <param name="columnNames">需要创建的对应列名</param>
        /// <returns>拼音列</returns>
        public static DataColumn[] CreateManyInitialsDataColumn(DataTable dataTable, params string[] columnNames) => PinYinConverter.PinYin.CreateManyInitialsDataColumn(dataTable, columnNames);
        /// <summary>
        /// 创建多音首字母列(多线程)
        /// </summary>
        /// <param name="dataTable">需要创建列的对应表单DataTable</param>
        /// <param name="columnNames">需要创建的对应列名</param>
        /// <returns>拼音列</returns>
#if NET40
        public static Task<DataColumn[]> CreateManyInitialsDataColumnAsync(DataTable dataTable, params string[] columnNames) => Task.Factory.StartNew(() => CreateManyInitialsDataColumn(dataTable, columnNames));
#else
        public static Task<DataColumn[]> CreateManyInitialsDataColumnAsync(DataTable dataTable, params string[] columnNames) => Task.Run(() => CreateManyInitialsDataColumn(dataTable, columnNames));
#endif

        /// <summary>
        /// 根据列名进行模糊查找
        /// </summary>
        /// <param name="dataTable">内存表</param>
        /// <param name="text">文本</param>
        /// <param name="columnNames">列名（为空时根据所有的列名进行查找）</param>
        /// <returns>dataTable.DefaultView.RowFilter</returns>
        public static string CreateFilter(DataTable dataTable, string text, params string[] columnNames)
        {
            columnNames = columnNames.Count().Equals(0) ? dataTable.Columns.Cast<DataColumn>().Select(dc => dc.ColumnName.ToUpper()).ToArray() : columnNames.Select(name => name.ToUpper()).ToArray();
            List<string> rowFilter = new List<string>();
            string[] Texts = text.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            if (Texts.Count() > 0)
            {
                Texts.ToList().ForEach(t =>
                    rowFilter.Add("(" + string.Join("OR ",
                        dataTable.Columns.Cast<DataColumn>().
                            Where(dc => (columnNames.Contains(dc.ColumnName.ToUpper()) || columnNames.Select(cn => "PY_" + cn).Contains(dc.ColumnName.ToUpper())) && dc.DataType.Equals(typeof(string))).
                            Select(dc => string.Format("{0} LIKE '%{1}%' ", dc.ColumnName.ToUpper(), t.Replace("'", "''").Replace("[", "[[ ").Replace("]", " ]]").Replace("*", "[*]").Replace("%", "[%]").Replace("[[ ", "[[]").Replace(" ]]", "[]]")))
                        ) + ") ")
                );
            }
            return string.Join("AND ", rowFilter);
        }

        /// <summary>
        /// 根据DataView视图以及字段列名，创建新的DataTable表
        /// </summary>
        /// <param name="dataView">视图表</param>
        /// <param name="columnNames">原有列名</param>
        /// <param name="headerTexts">新的列名</param>
        /// <returns>新的DataTable表</returns>
        public static DataTable DataViewToTable(DataView dataView, string[] columnNames, string[] headerTexts)
        {
            if (columnNames == null && headerTexts == null) return dataView.ToTable();
            else
            {
                DataTable dataTable = dataView.ToTable(false, columnNames);
                for (int i = 0; i < dataTable.Columns.Count; i++)
                {
                    dataTable.Columns[i].ColumnName = headerTexts[i];
                }
                return dataTable;
            }
        }
    }
}
