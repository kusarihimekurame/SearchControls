using Microsoft.International.Converters.PinYinConverter;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace PinYinConverter
{
    /// <summary>
    /// 中文转换为拼音
    /// </summary>
    public static class PinYin
    {
        /// <summary>
        /// 得到首字母
        /// </summary>
        /// <param name="text">中文文本</param>
        /// <returns>中文文本的拼音首字母</returns>
        /// <remarks>
        /// 源代码
        /// <code>
        /// public static string[] GetInitials(string text) => string.IsNullOrEmpty(text) ? null :
        ///     new List&lt;ChineseChar&gt;(text.Where(ch => Regex.IsMatch(ch.ToString(), @"[\u4e00-\u9fbb]")).Select(ch => new ChineseChar(ch))).
        ///         Select(cc => cc.Pinyins.Take(cc.PinyinCount).Select(pinyin => pinyin[0]).Distinct())
        ///         .Aggregate((IEnumerable&lt;IEnumerable&lt;char&gt;&gt;)new IEnumerable&lt;char&gt;[] { Enumerable.Empty&lt;char&gt;() },
        ///         (accumulator, sequence) =>
        ///             from accseq in accumulator
        ///             from item in sequence
        ///             select accseq.Concat(new[] { item }),
        ///         (result) => result.Select(r => new string (r.ToArray()))).ToArray();
        /// </code>
        /// </remarks>
        public static string[] GetInitials(string text) => string.IsNullOrEmpty(text) ? null :
            new List<ChineseChar>(text.Where(ch => Regex.IsMatch(ch.ToString(), @"[\u4e00-\u9fbb]")).Select(ch => new ChineseChar(ch))).
                Select(cc => cc.Pinyins.Take(cc.PinyinCount).Select(pinyin => pinyin[0]).Distinct())
                .Aggregate((IEnumerable<IEnumerable<char>>)new IEnumerable<char>[] { Enumerable.Empty<char>() },
                (accumulator, sequence) =>
                    from accseq in accumulator
                    from item in sequence
                    select accseq.Concat(new[] { item }),
                (result) => result.Select(r => new string(r.ToArray()))).ToArray();

        /// <summary>
        /// 得到拼音
        /// </summary>
        /// <param name="text">中文文本</param>
        /// <returns>中文文本的拼音</returns>
        /// <remarks>
        /// 源代码
        /// <code>
        /// public static string[] GetPinYin(string text) => string.IsNullOrEmpty(text) ? null :
        ///    new List&lt;ChineseChar&gt;(text.Where(ch => Regex.IsMatch(ch.ToString(), @"[\u4e00-\u9fbb]")).Select(ch => new ChineseChar(ch))).
        ///        Select(cc => cc.Pinyins.Take(cc.PinyinCount).Select(pinyin => pinyin.Substring(0, pinyin.Length - 1)).Distinct())
        ///        .Aggregate((IEnumerable&lt;IEnumerable&lt;string&gt;&gt;)new IEnumerable&lt;string>[] { Enumerable.Empty&lt;string&gt;() },
        ///        (accumulator, sequence) =>
        ///            from accseq in accumulator
        ///            from item in sequence
        ///            select accseq.Concat(new[] { item }),
        ///        (result) => result.Select(r => string.Join(" ", r.Select(s => string.Concat(s[0], s.Substring(1).ToLower()))))).ToArray();
        /// </code>
        /// </remarks>
        public static string[] GetPinYin(string text) => string.IsNullOrEmpty(text) ? null :
            new List<ChineseChar>(text.Where(ch => Regex.IsMatch(ch.ToString(), @"[\u4e00-\u9fbb]")).Select(ch => new ChineseChar(ch))).
                Select(cc => cc.Pinyins.Take(cc.PinyinCount).Select(pinyin => pinyin.Substring(0, pinyin.Length - 1)).Distinct())
                .Aggregate((IEnumerable<IEnumerable<string>>)new IEnumerable<string>[] { Enumerable.Empty<string>() },
                (accumulator, sequence) =>
                    from accseq in accumulator
                    from item in sequence
                    select accseq.Concat(new[] { item }),
                (result) => result.Select(r => string.Join(" ", r.Select(s => string.Concat(s[0], s.Substring(1).ToLower()))))).ToArray();

        /// <summary>
        /// 创建多音首字母列
        /// </summary>
        /// <param name="dataTable">需要创建列的对应表单DataTable</param>
        /// <param name="dataColumns">需要创建的对应列</param>
        /// <returns>拼音列</returns>
        public static DataColumn[] CreateManyInitialsDataColumn(DataTable dataTable, DataColumn[] dataColumns)
        {
            Hashtable hashtable = new Hashtable();
            DataColumn[] PinYinDataColumns = dataColumns.Select(dc => new DataColumn("PY_" + dc.ColumnName)).ToArray();
            dataTable.Columns.AddRange(PinYinDataColumns);
            try
            {
                foreach (DataRow dr in dataTable.Rows)
                {
                    lock (dr)
                    {
                        foreach (DataColumn pydc in PinYinDataColumns)
                        {
                            string zw = dr.Field<string>(pydc.ColumnName.Substring(3));
                            if (!hashtable.ContainsKey(zw))
                            {
                                hashtable.Add(zw, string.Join(",", GetInitials(dr.Field<string>(pydc.ColumnName.Substring(3))) ?? new string[] { "" }));
                            }
                            dr.SetField(pydc, hashtable[zw]);
                        }
                    }
                }
                dataTable.AcceptChanges();
            }
            catch { }
            dataTable.RowChanged += DataTable_RowChanged;

            void DataTable_RowChanged(object sender, DataRowChangeEventArgs e)
            {
                dataTable.RowChanged -= DataTable_RowChanged;
                switch (e.Action)
                {
                    case DataRowAction.ChangeOriginal:
                    case DataRowAction.ChangeCurrentAndOriginal:
                    case DataRowAction.Change:
                    case DataRowAction.Add:
                        lock (e.Row)
                        {
                            foreach (DataColumn pydc in PinYinDataColumns)
                            {
                                string zw = e.Row.Field<string>(pydc.ColumnName.Substring(3));
                                if (!hashtable.ContainsKey(zw))
                                {
                                    hashtable.Add(zw, string.Join(",", GetInitials(e.Row.Field<string>(pydc.ColumnName.Substring(3))) ?? new string[] { "" }));
                                }
                                if (string.IsNullOrEmpty(e.Row.Field<string>(pydc)) || !e.Row.Field<string>(pydc).Equals(hashtable[zw])) e.Row.SetField(pydc, hashtable[zw]);
                            }
                        }
                        break;
                }
                dataTable.RowChanged += DataTable_RowChanged;
            }

            return PinYinDataColumns;
        }

        /// <summary>
        /// 创建多音首字母列
        /// </summary>
        /// <param name="dataTable">需要创建列的对应表单DataTable</param>
        /// <param name="columnNames">需要创建的对应列名</param>
        /// <returns>拼音列</returns>
        public static DataColumn[] CreateManyInitialsDataColumn(DataTable dataTable, params string[] columnNames)
            => CreateManyInitialsDataColumn(dataTable, columnNames.Select(cn => dataTable.Columns[cn]).ToArray());
    }
}
