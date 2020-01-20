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
    public class Word : IDisposable
    {
        public DataTable DataTable { get; set; }
        public XWPFDocument XWPFDocument { get; }
        public CT_Document Document => XWPFDocument.Document;
        public CT_Body Body => Document.body;
        public CT_SectPr Print => Body.sectPr;
        public CT_PageSz PageSize => Print.pgSz;
        public CT_PageMar PageMargin => Print.pgMar;
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
        public XWPFHeader Header { get; }
        public XWPFParagraph TitleParagraph { get; }
        public XWPFRun TitleRun { get; }
        private string title;
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
        public XWPFTable Table { get; set; }

        public static ulong CMToTwip(double CM) => Convert.ToUInt64(Math.Round(CM * 0.3937008 * 1440, 0));

        private CancellationTokenSource cts = new CancellationTokenSource();

        public Word()
        {
            XWPFDocument = new XWPFDocument();
            Body.sectPr = new CT_SectPr();
            Header = XWPFDocument.CreateRelationship(XWPFRelation.HEADER, XWPFFactory.GetInstance(), XWPFDocument.HeaderList.Count + 1) as XWPFHeader;
            TitleParagraph = XWPFDocument.CreateParagraph();
            TitleParagraph.Alignment = ParagraphAlignment.CENTER;
            TitleRun = TitleParagraph.CreateRun();
        }

        public Word(DataTable dataTable = null, string title = null) : this()
        {
            DataTable = dataTable;
            Title = title;
        }

        public Word(DataView dataView, string[] columnNames, string[] headerTexts, string title = null) : this(Method.DataViewToTable(dataView, columnNames, headerTexts), title)
        {
        }

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

        public void DataTableToWord() => DataTableToWord(null);

        [DebuggerStepThrough]
        public Task DataTableToWordAsync(IProgress<WordProgressEventArgs> progress) => Task.Run(() => DataTableToWord(progress), cts.Token);


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

        public virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                XWPFDocument.Close();
            }
        }

        public void Dispose()
        {
            Dispose(true);
        }

        ~Word() => Dispose(false);
    }
}
