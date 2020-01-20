using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace SearchControls.Export
{
    public class ExcelProgressEventArgs : EventArgs
    {
        public Excel Excel { get; }
        public CancellationTokenSource CancellationTokenSource { get; }
        public int RowIndex { get; }
        public int ColumnIndex { get; }

        public ExcelProgressEventArgs(Excel excel, CancellationTokenSource cancellationTokenSource, int rowIndex, int columnIndex)
        {
            Excel = excel;
            CancellationTokenSource = cancellationTokenSource;
            RowIndex = rowIndex;
            ColumnIndex = columnIndex;
        }
    }
}
