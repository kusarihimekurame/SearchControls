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
    /// <summary>
    /// 以NPOI为基础的类（为了将DataTable表格导入到新的Excel文件中）
    /// </summary>
    /// <example>
    /// 利用示例
    /// <code>
    /// Export.Progress&lt;ExcelProgressEventArgs&gt; progress = new Export.Progress&lt;ExcelProgressEventArgs&gt;(epe =>
    /// {
    ///     设置进度条以及取消措施
    /// });
    /// using (Excel excel = new Excel(DataView, 需要转换的列名数组, 需要显示的列名数组, 标题，页名))
    /// {
    ///     excel.ResetDefault();
    ///     await excel.DataTableToWordAsync(progress);
    ///     excel.SaveAs(path, true);
    /// }
    /// </code>
    /// </example>
    public class Excel : IDisposable
    {
        /// <summary>
        /// 表示内存中数据的一个表
        /// </summary>
        public DataTable DataTable { get; set; }
        /// <summary>
        /// Excel的当前的工作区域
        /// </summary>
        public XSSFWorkbook Workbook { get; }
        /// <summary>
        /// Excel当前的表
        /// </summary>
        public XSSFSheet Sheet { get; set; }
        /// <summary>
        /// Excel的列头样式
        /// </summary>
        public XSSFCellStyle HeaderStyle { get; }
        /// <summary>
        /// 需要填充到Excel表中的时间样式
        /// </summary>
        public XSSFCellStyle DateStyle { get; }
        /// <summary>
        /// Excel中的单元格样式
        /// </summary>
        public XSSFCellStyle CellStyle { get; }
        /// <summary>
        /// 设置字体
        /// </summary>
        public XSSFFont Font { get; }
        private string title;
        /// <summary>
        /// 标题（赋值时将写入页眉）
        /// </summary>
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
        /// <summary>
        /// 以NPOI为基础的类的构造函数（为了将DataTable表格导入到新的Excel文件中）
        /// </summary>
        /// <example>
        /// 利用示例
        /// <code>
        /// Export.Progress&lt;ExcelProgressEventArgs&gt; progress = new Export.Progress&lt;ExcelProgressEventArgs&gt;(epe =>
        /// {
        ///     设置进度条以及取消措施
        /// });
        /// using (Excel excel = new Excel(DataView, 需要转换的列名数组, 需要显示的列名数组, 标题，页名))
        /// {
        ///     excel.ResetDefault();
        ///     await excel.DataTableToWordAsync(progress);
        ///     excel.SaveAs(path, true);
        /// }
        /// </code>
        /// </example>
        public Excel()
        {
            Workbook = new XSSFWorkbook();
            HeaderStyle = Workbook.CreateCellStyle() as XSSFCellStyle;
            DateStyle = Workbook.CreateCellStyle() as XSSFCellStyle;
            CellStyle = Workbook.CreateCellStyle() as XSSFCellStyle;
            Font = Workbook.CreateFont() as XSSFFont;
        }
        /// <summary>
        /// <see cref="Excel()"/>
        /// </summary>
        /// <param name="file"><see cref="FileInfo"/></param>
        public Excel(FileInfo file)
        {
            Workbook = new XSSFWorkbook(file);
            HeaderStyle = Workbook.CreateCellStyle() as XSSFCellStyle;
            DateStyle = Workbook.CreateCellStyle() as XSSFCellStyle;
            CellStyle = Workbook.CreateCellStyle() as XSSFCellStyle;
            Font = Workbook.CreateFont() as XSSFFont;
        }
        /// <summary>
        /// <see cref="Excel()"/>
        /// </summary>
        /// <param name="pkg"></param>
        public Excel(OPCPackage pkg)
        {
            Workbook = new XSSFWorkbook(pkg);
            HeaderStyle = Workbook.CreateCellStyle() as XSSFCellStyle;
            DateStyle = Workbook.CreateCellStyle() as XSSFCellStyle;
            CellStyle = Workbook.CreateCellStyle() as XSSFCellStyle;
            Font = Workbook.CreateFont() as XSSFFont;
        }
        /// <summary>
        /// <see cref="Excel()"/>
        /// </summary>
        /// <param name="is1"><see cref="Stream"/></param>
        public Excel(Stream is1)
        {
            Workbook = new XSSFWorkbook(is1);
            HeaderStyle = Workbook.CreateCellStyle() as XSSFCellStyle;
            DateStyle = Workbook.CreateCellStyle() as XSSFCellStyle;
            CellStyle = Workbook.CreateCellStyle() as XSSFCellStyle;
            Font = Workbook.CreateFont() as XSSFFont;
        }
        /// <summary>
        /// <see cref="Excel()"/>
        /// </summary>
        /// <param name="path">路径</param>
        public Excel(string path)
        {
            Workbook = new XSSFWorkbook(path);
            HeaderStyle = Workbook.CreateCellStyle() as XSSFCellStyle;
            DateStyle = Workbook.CreateCellStyle() as XSSFCellStyle;
            CellStyle = Workbook.CreateCellStyle() as XSSFCellStyle;
            Font = Workbook.CreateFont() as XSSFFont;
        }
        /// <summary>
        /// <see cref="Excel()"/>
        /// </summary>
        /// <param name="dataTable">表</param>
        /// <param name="title">标题</param>
        /// <param name="sheetName">页名</param>
        public Excel(DataTable dataTable = null, string title = null, string sheetName = null) : this()
        {
            Sheet = Workbook.CreateSheet(string.IsNullOrEmpty(sheetName) ? "Sheet1" : sheetName) as XSSFSheet;
            DataTable = dataTable;
            Sheet.SetAutoFilter(new CellRangeAddress(0, 0, 0, dataTable.Columns.Count - 1));
            Title = title;
        }
        /// <summary>
        /// <see cref="Excel()"/>
        /// </summary>
        /// <param name="dataView">视图表</param>
        /// <param name="columnNames">需要转换的列名数组</param>
        /// <param name="headerTexts">需要显示的列名数组</param>
        /// <param name="title">标题</param>
        /// <param name="sheetName">页名</param>
        public Excel(DataView dataView, string[] columnNames, string[] headerTexts, string title = null, string sheetName = null) : this(Method.DataViewToTable(dataView, columnNames, headerTexts), title, sheetName)
        {
        }
        /// <summary>
        /// 重置为默认设置
        /// </summary>
        /// <example>
        /// 详细的默认设置
        /// <code>
        /// Sheet.DefaultRowHeightInPoints = 24; //默认的行高
        /// HeaderStyle.Alignment = HorizontalAlignment.Center; //列头中的文本水平居中
        /// HeaderStyle.VerticalAlignment = VerticalAlignment.Center; //列头中的文本垂直居中
        /// DateStyle.SetDataFormat(Workbook.CreateDataFormat().GetFormat("yyyy-mm-dd")); //设置时间样式
        /// CellStyle.SetVerticalAlignment((short)VerticalAlignment.Center); //单元格中的文字设置为垂直居中
        /// Font.FontHeightInPoints = 9; //字体大小
        /// HeaderStyle.SetFont(Font);
        /// CellStyle.SetFont(Font);
        /// DateStyle.SetFont(Font);
        /// //打印设置
        /// Sheet.PrintSetup.PaperSize = (short)PaperSize.A4_Small; //设置A4
        /// Sheet.SetZoom(100); //100%的倍率
        /// Sheet.PrintSetup.Landscape = false; //纸张方向为纵向
        /// Sheet.SetMargin(MarginType.LeftMargin, 1 / 2.5); //设置边框
        /// Sheet.SetMargin(MarginType.RightMargin, 0.5 / 2.5);
        /// Sheet.SetMargin(MarginType.TopMargin, 1.5 / 2.5);
        /// Sheet.SetMargin(MarginType.BottomMargin, 1 / 2.5);
        /// Sheet.SetMargin(MarginType.HeaderMargin, 0.5 / 2.5);
        /// Sheet.SetMargin(MarginType.FooterMargin, 0.5 / 2.5);
        /// Sheet.RepeatingRows = CellRangeAddress.ValueOf("$1:$1");
        /// Sheet.Footer.Center = "&amp;9第 &amp;P &amp;9页      &amp;9共 &amp;N &amp;9页";
        /// Sheet.IsPrintGridlines = true;
        /// </code>
        /// </example>
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

        /// <summary>
        /// DataTable的数据写入到Excel表中
        /// </summary>
        /// <param name="progress">进度条</param>
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
        /// <summary>
        /// DataTable的数据写入到Excel表中
        /// </summary>
        public void DataTableToExcel() => DataTableToExcel(null);
        /// <summary>
        /// <see cref="DataTableToExcel(IProgress&lt;ExcelProgressEventArgs&gt;)"/>的线程方法
        /// </summary>
        /// <param name="progress">进度条</param>
        /// <returns>线程</returns>
        [DebuggerStepThrough]
        public Task DataTableToExcelAsync(IProgress<ExcelProgressEventArgs> progress) => Task.Run(() => DataTableToExcel(progress), cts.Token);
        /// <summary>
        /// 保存文件
        /// </summary>
        /// <param name="fullPath">完整路径</param>
        /// <param name="isOpen">完成之后是否需要打开文件</param>
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
        /// <summary>
        /// 关闭释放资源
        /// </summary>
        /// <param name="disposing">是否在释放进程中。</param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                Workbook.Close();
            }
        }
        /// <summary>
        /// 关闭释放资源
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        /// <summary>
        /// 析构函数
        /// </summary>
        ~Excel() => Dispose(false);
    }
}
