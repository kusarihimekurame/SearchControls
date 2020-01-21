using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace SearchControls.Export
{
    /// <summary>
    /// Word转换时通知进度所需的事件参数
    /// </summary>
    public class WordProgressEventArgs : EventArgs
    {
        /// <summary>
        /// <see cref="Export.Word"/>
        /// </summary>
        public Word Word { get; }
        /// <summary>
        /// 向应该被取消的CancellationToken发送信号<see cref="System.Threading.CancellationTokenSource"/>
        /// </summary>
        public CancellationTokenSource CancellationTokenSource { get; }
        /// <summary>
        /// 现在正在转换的行数
        /// </summary>
        public int RowIndex { get; }
        /// <summary>
        /// 现在正在转换的列数
        /// </summary>
        public int ColumnIndex { get; }
        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="word"><see cref="Word"/></param>
        /// <param name="cancellationTokenSource"><see cref="CancellationTokenSource"/></param>
        /// <param name="rowIndex"><see cref="RowIndex"/></param>
        /// <param name="columnIndex"><see cref="ColumnIndex"/></param>
        public WordProgressEventArgs(Word word, CancellationTokenSource cancellationTokenSource, int rowIndex, int columnIndex)
        {
            Word = word;
            CancellationTokenSource = cancellationTokenSource;
            RowIndex = rowIndex;
            ColumnIndex = columnIndex;
        }
    }
}
