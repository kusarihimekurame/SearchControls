using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace SearchControls.Export
{
    public class WordProgressEventArgs : EventArgs
    {
        public Word Word { get; }
        public CancellationTokenSource CancellationTokenSource { get; }
        public int RowIndex { get; }
        public int ColumnIndex { get; }

        public WordProgressEventArgs(Word word, CancellationTokenSource cancellationTokenSource, int rowIndex, int columnIndex)
        {
            Word = word;
            CancellationTokenSource = cancellationTokenSource;
            RowIndex = rowIndex;
            ColumnIndex = columnIndex;
        }
    }
}
