using NPOI.OpenXml4Net.OPC;
using NPOI.OpenXmlFormats.Wordprocessing;
using NPOI.XWPF.UserModel;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace SearchControls.Export
{
    /// <summary>
    /// 以NPOI为基础的类（为了将DataTable表格导入到新的Word文件中）
    /// </summary>
    /// <example>
    /// 利用示例(<see cref="ResetDefault"/>、<see cref="DataTableToWordAsync(IProgress{WordProgressEventArgs})"/>、<see cref="SaveAs(string, bool)"/>)
    /// <code>
    /// Export.Progress&lt;WordProgressEventArgs&gt; progress = new Export.Progress&lt;WordProgressEventArgs&gt;(epe =>
    /// {
    ///     设置进度条以及取消措施
    /// });
    /// using (Word word = new Word(DataView, 需要转换的列名数组, 需要显示的列名数组, 标题，页名))
    /// {
    ///     word.ResetDefault(); 
    ///     await word.DataTableToWordAsync(progress); 
    ///     word.SaveAs(path, true); 
    /// }
    /// </code>
    /// </example>
    public class Word : IDisposable
    {
        /// <summary>
        /// 表示内存中数据的一个表
        /// </summary>
        public DataTable DataTable { get; set; }
        /// <summary>
        /// XWPF文档
        /// </summary>
        public XWPFDocument XWPFDocument { get; }
        /// <summary>
        /// 文档
        /// </summary>
        public CT_Document Document => XWPFDocument.Document;
        /// <summary>
        /// 本体
        /// </summary>
        public CT_Body Body => Document.body;
        /// <summary>
        /// 打印设置
        /// </summary>
        public CT_SectPr Print => Body.sectPr;
        /// <summary>
        /// 页面尺寸
        /// </summary>
        public CT_PageSz PageSize => Print.pgSz;
        /// <summary>
        /// 页面边距
        /// </summary>
        public CT_PageMar PageMargin => Print.pgMar;
        /// <summary>
        /// 纸张方向
        /// </summary>
        public ST_PageOrientation PageOrientation
        {
            get => PageSize.orient;
            set
            {
                if (!PageSize.orient.Equals(value))
                {
                    PageSize.orient = value;
                    ulong h = PageSize.h;
                    ulong w = PageSize.w;
                    PageSize.h = w;
                    PageSize.w = h;
                }
            }
        }
        /// <summary>
        /// 页眉
        /// </summary>
        public XWPFHeader Header { get; }
        /// <summary>
        /// 标题段落
        /// </summary>
        public XWPFParagraph TitleParagraph { get; }
        /// <summary>
        /// 标题字体
        /// </summary>
        public XWPFRun TitleRun { get; }
        private string title;
        /// <summary>
        /// 标题（赋值时将写入页眉）
        /// </summary>
        public virtual string Title
        {
            get => title;
            set
            {
                title = value;
                if (!string.IsNullOrEmpty(title))
                {
                    CT_Hdr hdr = new CT_Hdr();
                    CT_P p = hdr.AddNewP();
                    CT_R r = p.AddNewR();
                    r.AddNewRPr().AddNewSz().val = 18;
                    r.AddNewT().Value = title;
                    Header.SetHeaderFooter(hdr);
                    Header.GetParagraph(p).Alignment = ParagraphAlignment.CENTER;
                    CT_HdrFtrRef hdrFtrRef = Print.AddNewHeaderReference();
                    hdrFtrRef.type = ST_HdrFtr.@default;
                    hdrFtrRef.id = Header.GetPackageRelationship().Id;

                    TitleRun.SetText(title);
                }
            }
        }
        /// <summary>
        /// XWPF表格
        /// </summary>
        public XWPFTable Table { get; set; }
        /// <summary>
        /// 将厘米转换成Twip
        /// </summary>
        /// <param name="CM">以厘米为单位的数字</param>
        /// <returns>以Twip为单位的整数</returns>
        public static ulong CMToTwip(double CM) => Convert.ToUInt64(Math.Round(CM * 0.3937008 * 1440, 0));

        private readonly CancellationTokenSource cts = new CancellationTokenSource();
        /// <summary>
        /// 以NPOI为基础的类的构造函数（为了将DataTable表格导入到新的Word文件中）
        /// </summary>
        /// <example>
        /// 利用示例(<see cref="ResetDefault"/>、<see cref="DataTableToWordAsync(IProgress{WordProgressEventArgs})"/>、<see cref="SaveAs(string, bool)"/>)
        /// <code>
        /// Export.Progress&lt;WordProgressEventArgs&gt; progress = new Export.Progress&lt;WordProgressEventArgs&gt;(epe =>
        /// {
        ///     设置进度条以及取消措施
        /// });
        /// using (Word word = new Word(DataView, 需要转换的列名数组, 需要显示的列名数组, 标题，页名))
        /// {
        ///     word.ResetDefault(); 
        ///     await word.DataTableToWordAsync(progress); 
        ///     word.SaveAs(path, true); 
        /// }
        /// </code>
        /// </example>
        public Word()
        {
            XWPFDocument = new XWPFDocument();
            Body.sectPr = new CT_SectPr();
            Header = XWPFDocument.CreateRelationship(XWPFRelation.HEADER, XWPFFactory.GetInstance(), XWPFDocument.HeaderList.Count + 1) as XWPFHeader;
            TitleParagraph = XWPFDocument.CreateParagraph();
            TitleParagraph.Alignment = ParagraphAlignment.CENTER;
            TitleRun = TitleParagraph.CreateRun();
        }
        /// <summary>
        /// <see cref="Word()"/>
        /// </summary>
        /// <param name="pkg"></param>
        public Word(OPCPackage pkg)
        {
            XWPFDocument = new XWPFDocument(pkg);
            Body.sectPr = new CT_SectPr();
            Header = XWPFDocument.CreateRelationship(XWPFRelation.HEADER, XWPFFactory.GetInstance(), XWPFDocument.HeaderList.Count + 1) as XWPFHeader;
            TitleParagraph = XWPFDocument.CreateParagraph();
            TitleParagraph.Alignment = ParagraphAlignment.CENTER;
            TitleRun = TitleParagraph.CreateRun();
        }
        /// <summary>
        /// <see cref="Word()"/>
        /// </summary>
        /// <param name="is1"><see cref="Stream"/></param>
        public Word(Stream is1)
        {
            XWPFDocument = new XWPFDocument(is1);
            Body.sectPr = new CT_SectPr();
            Header = XWPFDocument.CreateRelationship(XWPFRelation.HEADER, XWPFFactory.GetInstance(), XWPFDocument.HeaderList.Count + 1) as XWPFHeader;
            TitleParagraph = XWPFDocument.CreateParagraph();
            TitleParagraph.Alignment = ParagraphAlignment.CENTER;
            TitleRun = TitleParagraph.CreateRun();
        }
        /// <summary>
        /// <see cref="Word()"/>
        /// </summary>
        /// <param name="dataTable">表</param>
        /// <param name="title">标题</param>
        public Word(DataTable dataTable = null, string title = null) : this()
        {
            DataTable = dataTable;
            Title = title;
        }
        /// <summary>
        /// <see cref="Word()"/>
        /// </summary>
        /// <param name="dataView">视图表</param>
        /// <param name="columnNames">需要转换的列名数组</param>
        /// <param name="headerTexts">需要显示的列名数组</param>
        /// <param name="title">标题</param>
        public Word(DataView dataView, string[] columnNames, string[] headerTexts, string title = null) : this(Method.DataViewToTable(dataView, columnNames, headerTexts), title)
        {
        }
        /// <summary>
        /// 重置为默认设置
        /// </summary>
        /// <example>
        /// 详细的默认设置
        /// <code>
        /// //设置页面边距
        /// PageMargin.top = CMToTwip(1.41).ToString();
        /// PageMargin.bottom = CMToTwip(1.23).ToString();
        /// PageMargin.left = CMToTwip(1.23);
        /// PageMargin.right = CMToTwip(1.23);
        /// PageMargin.header = CMToTwip(0.49);
        /// //设置标题字体
        /// TitleRun.GetCTR().AddNewRPr().AddNewB().val = true; //是否粗体
        /// TitleRun.GetCTR().AddNewRPr().AddNewSz().val = 32;  //字体大小
        /// //创建并设置段落
        /// XWPFParagraph p = XWPFDocument.CreateParagraph();
        /// p.Alignment = ParagraphAlignment.CENTER;
        /// XWPFRun r = p.CreateRun();
        /// r.FontFamily = "宋体";
        /// r.FontSize = 8;
        /// r.SetText(" ");
        /// //创建并设置表格
        /// if (DataTable != null)
        /// {
        ///     Table = XWPFDocument.CreateTable(DataTable.Rows.Count + 1, DataTable.Columns.Count);
        ///     CT_Tbl tbl = XWPFDocument.Document.body.GetTblArray(0);
        ///     CT_TblPr tblPr = tbl.AddNewTblPr();
        ///     
        ///     tblPr.jc = new CT_Jc() { val = ST_Jc.center }; 
        ///     //设置表格边框线条
        ///     Table.SetTopBorder(XWPFTable.XWPFBorderType.DOUBLE, 6, 0, "Black");
        ///     Table.SetLeftBorder(XWPFTable.XWPFBorderType.DOUBLE, 6, 0, "Black");
        ///     Table.SetBottomBorder(XWPFTable.XWPFBorderType.DOUBLE, 6, 0, "Black");
        ///     Table.SetRightBorder(XWPFTable.XWPFBorderType.DOUBLE, 6, 0, "Black");
        ///     Table.SetInsideHBorder(XWPFTable.XWPFBorderType.DOTTED, 6, 0, "Black");
        ///     Table.SetInsideVBorder(XWPFTable.XWPFBorderType.DOTTED, 6, 0, "Black");
        ///     
        ///     Table.SetCellMargins(0, Convert.ToInt32(CMToTwip(0.19)), 0, Convert.ToInt32(CMToTwip(0.19)));
        ///     //设置单元格样式
        ///     foreach (XWPFTableRow row in Table.Rows)
        ///     {
        ///         row.Height = Convert.ToInt32(CMToTwip(0.8));
        ///         foreach (XWPFTableCell cell in row.GetTableCells())
        ///         {
        ///             cell.SetVerticalAlignment(XWPFTableCell.XWPFVertAlign.CENTER);
        ///             XWPFRun run = cell.Paragraphs[0].CreateRun();
        ///             run.FontFamily = "宋体";
        ///             run.FontSize = 8;
        ///         }
        ///     }
        ///     //表格下面创建新的段落
        ///     XWPFParagraph p1 = XWPFDocument.CreateParagraph();
        ///     p1.Alignment = ParagraphAlignment.CENTER;
        ///     XWPFRun r1 = p1.CreateRun();
        ///     r1.FontFamily = "宋体";
        ///     r1.FontSize = 8;
        ///     r1.SetText(title);
        /// }
        /// </code>
        /// </example>
        public virtual void ResetDefault()
        {
            PageMargin.top = CMToTwip(1.41).ToString();
            PageMargin.bottom = CMToTwip(1.23).ToString();
            PageMargin.left = CMToTwip(1.23);
            PageMargin.right = CMToTwip(1.23);
            PageMargin.header = CMToTwip(0.49);

            TitleRun.GetCTR().AddNewRPr().AddNewB().val = true;
            TitleRun.GetCTR().AddNewRPr().AddNewSz().val = 32;

            XWPFParagraph p = XWPFDocument.CreateParagraph();
            p.Alignment = ParagraphAlignment.CENTER;
            XWPFRun r = p.CreateRun();
            r.FontFamily = "宋体";
            r.FontSize = 8;
            r.SetText(" ");

            if (DataTable != null)
            {
                Table = XWPFDocument.CreateTable(DataTable.Rows.Count + 1, DataTable.Columns.Count);
                CT_Tbl tbl = XWPFDocument.Document.body.GetTblArray(0);
                CT_TblPr tblPr = tbl.AddNewTblPr();

                tblPr.jc = new CT_Jc() { val = ST_Jc.center };

                Table.SetTopBorder(XWPFTable.XWPFBorderType.DOUBLE, 6, 0, "Black");
                Table.SetLeftBorder(XWPFTable.XWPFBorderType.DOUBLE, 6, 0, "Black");
                Table.SetBottomBorder(XWPFTable.XWPFBorderType.DOUBLE, 6, 0, "Black");
                Table.SetRightBorder(XWPFTable.XWPFBorderType.DOUBLE, 6, 0, "Black");
                Table.SetInsideHBorder(XWPFTable.XWPFBorderType.DOTTED, 6, 0, "Black");
                Table.SetInsideVBorder(XWPFTable.XWPFBorderType.DOTTED, 6, 0, "Black");

                Table.SetCellMargins(0, Convert.ToInt32(CMToTwip(0.19)), 0, Convert.ToInt32(CMToTwip(0.19)));

                foreach (XWPFTableRow row in Table.Rows)
                {
                    row.Height = Convert.ToInt32(CMToTwip(0.8));
                    foreach (XWPFTableCell cell in row.GetTableCells())
                    {
                        cell.SetVerticalAlignment(XWPFTableCell.XWPFVertAlign.CENTER);
                        XWPFRun run = cell.Paragraphs[0].CreateRun();
                        run.FontFamily = "宋体";
                        run.FontSize = 8;
                    }
                }
                
                XWPFParagraph p1 = XWPFDocument.CreateParagraph();
                p1.Alignment = ParagraphAlignment.CENTER;
                XWPFRun r1 = p1.CreateRun();
                r1.FontFamily = "宋体";
                r1.FontSize = 8;
                r1.SetText(title);
            }
        }
        /// <summary>
        /// DataTable的数据写入到Word表中
        /// </summary>
        /// <param name="progress">进度条</param>
        protected virtual void DataTableToWord(IProgress<WordProgressEventArgs> progress = null)
        {
            if (DataTable == null) throw new Exception("表格DataTable不能为空");
            if (Table == null) Table = XWPFDocument.CreateTable(DataTable.Rows.Count + 1, DataTable.Columns.Count);

            foreach (DataColumn dc in DataTable.Columns)
            {
                XWPFTableCell cell = Table.Rows[0].GetCell(dc.Ordinal);
                cell.Paragraphs[0].Alignment = ParagraphAlignment.CENTER;
                cell.Paragraphs[0].Runs[0].SetText(dc.ColumnName);

                if (progress != null)
                {
                    progress.Report(new WordProgressEventArgs(this, cts, 0, dc.Ordinal));
                }
                cts.Token.ThrowIfCancellationRequested();
            }

            for (int i = 0; i < DataTable.Rows.Count; i++)
            {
                foreach (DataColumn dc in DataTable.Columns)
                {
                    XWPFTableCell Cell = Table.Rows[i + 1].GetCell(dc.Ordinal);

                    XWPFRun run = Cell.Paragraphs[0].CreateRun();

                    Cell.Paragraphs[0].Runs[0].SetText(DataTable.Rows[i][dc].ToString());

                    if (progress != null)
                    {
                        progress.Report(new WordProgressEventArgs(this, cts, i + 1, dc.Ordinal));
                    }
                    cts.Token.ThrowIfCancellationRequested();
                }
            }
        }
        /// <summary>
        /// DataTable的数据写入到Word表中
        /// </summary>
        public void DataTableToWord() => DataTableToWord(null);
        /// <summary>
        /// <see cref="DataTableToWord(IProgress&lt;WordProgressEventArgs&gt;)"/>的线程方法
        /// </summary>
        /// <param name="progress">进度条</param>
        /// <returns>线程</returns>
        [DebuggerStepThrough]
        public Task DataTableToWordAsync(IProgress<WordProgressEventArgs> progress) => Task.Run(() => DataTableToWord(progress), cts.Token);
        /// <summary>
        /// 保存文件
        /// </summary>
        /// <param name="fullPath">完整路径</param>
        /// <param name="isOpen">完成之后是否需要打开文件</param>
        public void SaveAs(string fullPath, bool isOpen = false)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                XWPFDocument.Write(ms);
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
                XWPFDocument.Close();
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
        ~Word() => Dispose(false);
    }
}
