using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using NPOI.OpenXml4Net.OPC;
using NPOI.SS.UserModel;
using NPOI.SS.Util;
using NPOI.XSSF.UserModel;

namespace SearchControls.Export
{
    public class Excel : IDisposable
    {
        public DataTable DataTable { get; set; }
        public XSSFWorkbook Workbook { get; }
        public XSSFSheet Sheet { get; set; }
        public XSSFCellStyle HeaderStyle { get; }
        public XSSFCellStyle DateStyle { get; }
        public XSSFCellStyle CellStyle { get; }
        public XSSFFont Font { get; }
        private string title;
        public virtual string Title 
        {
            get => title;
            set 
            {
                if (Sheet == null && !string.IsNullOrEmpty(title)) throw new Exception("Sheet不能为空，请设置完Sheet后再给Title赋值");
                title = value;
                if (!string.IsNullOrEmpty(title)) Sheet.Header.Center = $"&16{title}";
            }
        }
        private CancellationTokenSource cts = new CancellationTokenSource();

        public Excel()
        {
            Workbook = new XSSFWorkbook();
            HeaderStyle = Workbook.CreateCellStyle() as XSSFCellStyle;
            DateStyle = Workbook.CreateCellStyle() as XSSFCellStyle;
            CellStyle = Workbook.CreateCellStyle() as XSSFCellStyle;
            Font = Workbook.CreateFont() as XSSFFont;
        }

        public Excel(FileInfo file)
        {
            Workbook = new XSSFWorkbook(file);
            HeaderStyle = Workbook.CreateCellStyle() as XSSFCellStyle;
            DateStyle = Workbook.CreateCellStyle() as XSSFCellStyle;
            CellStyle = Workbook.CreateCellStyle() as XSSFCellStyle;
            Font = Workbook.CreateFont() as XSSFFont;
        }

        public Excel(OPCPackage pkg)
        {
            Workbook = new XSSFWorkbook(pkg);
            HeaderStyle = Workbook.CreateCellStyle() as XSSFCellStyle;
            DateStyle = Workbook.CreateCellStyle() as XSSFCellStyle;
            CellStyle = Workbook.CreateCellStyle() as XSSFCellStyle;
            Font = Workbook.CreateFont() as XSSFFont;
        }

        public Excel(Stream is1)
        {
            Workbook = new XSSFWorkbook(is1);
            HeaderStyle = Workbook.CreateCellStyle() as XSSFCellStyle;
            DateStyle = Workbook.CreateCellStyle() as XSSFCellStyle;
            CellStyle = Workbook.CreateCellStyle() as XSSFCellStyle;
            Font = Workbook.CreateFont() as XSSFFont;
        }

        public Excel(string path)
        {
            Workbook = new XSSFWorkbook(path);
            HeaderStyle = Workbook.CreateCellStyle() as XSSFCellStyle;
            DateStyle = Workbook.CreateCellStyle() as XSSFCellStyle;
            CellStyle = Workbook.CreateCellStyle() as XSSFCellStyle;
            Font = Workbook.CreateFont() as XSSFFont;
        }

        public Excel(DataTable dataTable = null, string title = null, string sheetName = null) : this()
        {
            Sheet = Workbook.CreateSheet(string.IsNullOrEmpty(sheetName) ? "Sheet1" : sheetName) as XSSFSheet;
            DataTable = dataTable;
            Sheet.SetAutoFilter(new CellRangeAddress(0, 0, 0, dataTable.Columns.Count - 1));
            Title = title;
        }

        public Excel(DataView dataView, string[] columnNames, string[] headerTexts, string title = null, string sheetName = null) : this(Method.DataViewToTable(dataView, columnNames, headerTexts), title, sheetName)
        {
        }

        public virtual void ResetDefault()
        {
            Sheet.DefaultRowHeightInPoints = 24;
            HeaderStyle.Alignment = HorizontalAlignment.Center;
            HeaderStyle.VerticalAlignment = VerticalAlignment.Center;
            DateStyle.SetDataFormat(Workbook.CreateDataFormat().GetFormat("yyyy-mm-dd"));
            CellStyle.SetVerticalAlignment((short)VerticalAlignment.Center);
            Font.FontHeightInPoints = 9;
            Font.FontName = "等线";
            HeaderStyle.SetFont(Font);
            CellStyle.SetFont(Font);
            DateStyle.SetFont(Font);

            Sheet.PrintSetup.PaperSize = (short)PaperSize.A4_Small;
            Sheet.SetZoom(100);
            Sheet.PrintSetup.Landscape = false;
            Sheet.SetMargin(MarginType.LeftMargin, 1 / 2.5);
            Sheet.SetMargin(MarginType.RightMargin, 0.5 / 2.5);
            Sheet.SetMargin(MarginType.TopMargin, 1.5 / 2.5);
            Sheet.SetMargin(MarginType.BottomMargin, 1 / 2.5);
            Sheet.SetMargin(MarginType.HeaderMargin, 0.5 / 2.5);
            Sheet.SetMargin(MarginType.FooterMargin, 0.5 / 2.5);
            Sheet.RepeatingRows = CellRangeAddress.ValueOf("$1:$1");
            Sheet.Footer.Center = "&9第 &P &9页      &9共 &N &9页";
            Sheet.IsPrintGridlines = true;
        }

        [DebuggerStepThrough]
        protected virtual void DataTableToExcel(IProgress<ExcelProgressEventArgs> progress = null)
        {
            if (DataTable == null) throw new Exception("表格DataTable不能为空");

            XSSFRow HeaderRow = Sheet.CreateRow(0) as XSSFRow;

            foreach (DataColumn dc in DataTable.Columns)
            {
                XSSFCell headerCell = HeaderRow.CreateCell(dc.Ordinal) as XSSFCell;
                headerCell.SetCellValue(dc.ColumnName);
                headerCell.CellStyle = HeaderStyle;

                if (progress != null)
                {
                    progress.Report(new ExcelProgressEventArgs(this, cts, 0, dc.Ordinal));
                }
                cts.Token.ThrowIfCancellationRequested();
            }

            for (int i = 0; i < DataTable.Rows.Count; i++)
            {
                XSSFRow newRow = Sheet.CreateRow(i + 1) as XSSFRow;
                foreach (DataColumn dc in DataTable.Columns)
                {
                    XSSFCell newCell = newRow.CreateCell(dc.Ordinal) as XSSFCell;
                    switch (dc.DataType.ToString())
                    {
                        case "System.DateTime":
                            newCell.SetCellValue(DateTime.Parse(DataTable.Rows[i][dc].ToString()));
                            newCell.CellStyle = DateStyle;
                            break;
                        default:
                            newCell.SetCellValue(DataTable.Rows[i][dc].ToString());
                            newCell.CellStyle = CellStyle;
                            break;
                    }

                    if (progress != null)
                    {
                        progress.Report(new ExcelProgressEventArgs(this, cts, i + 1, dc.Ordinal));
                    }
                    cts.Token.ThrowIfCancellationRequested();
                }
            }
        }

        public void DataTableToExcel() => DataTableToExcel(null);

        [DebuggerStepThrough]
        public Task DataTableToExcelAsync(IProgress<ExcelProgressEventArgs> progress) => Task.Run(() => DataTableToExcel(progress), cts.Token);

        public void SaveAs(string fullPath, bool isOpen = false)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                Workbook.Write(ms);
                ms.Flush();

                using (FileStream fs = new FileStream(fullPath, FileMode.Create, FileAccess.Write))
                {
                    byte[] data = ms.ToArray();
                    fs.Write(data, 0, data.Length);
                    fs.Flush();
                }
            }
            if (isOpen) Process.Start(fullPath);
        }

        public virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                Workbook.Close();
            }
        }

        public void Dispose()
        {
            Dispose(true);
        }

        ~Excel() => Dispose(false);
    }
}
