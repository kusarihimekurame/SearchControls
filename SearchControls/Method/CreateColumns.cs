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
    public static class CreateColumns
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
    }
}
