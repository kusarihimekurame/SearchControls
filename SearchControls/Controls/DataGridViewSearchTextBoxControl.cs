using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Permissions;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SearchControls
{
    /// <summary>
    /// 表示可以托管在一个文本框控件 <see cref="DataGridViewSearchTextBoxControl"/>。
    /// </summary>
    [
        ComVisible(true),
#if NET40_OR_GREATER
        ClassInterface(ClassInterfaceType.AutoDispatch)
#endif
    ]
    public class DataGridViewSearchTextBoxControl : SearchTextBox, IDataGridViewEditingControl
    {
        private static readonly DataGridViewContentAlignment anyTop = DataGridViewContentAlignment.TopLeft | DataGridViewContentAlignment.TopCenter | DataGridViewContentAlignment.TopRight;
        private static readonly DataGridViewContentAlignment anyRight = DataGridViewContentAlignment.TopRight | DataGridViewContentAlignment.MiddleRight | DataGridViewContentAlignment.BottomRight;
        private static readonly DataGridViewContentAlignment anyCenter = DataGridViewContentAlignment.TopCenter | DataGridViewContentAlignment.MiddleCenter | DataGridViewContentAlignment.BottomCenter;

        private bool repositionOnValueChange;

        /// <summary>
        /// 获取或设置 <see cref="DataGridView"/> ，其中包含文本框控件。
        /// </summary>
        public DataGridView EditingControlDataGridView { get; set; }
        /// <summary>
        /// 获取或设置文本框控件的当前值的格式的表示。
        /// </summary>
        public object EditingControlFormattedValue { get => GetEditingControlFormattedValue(DataGridViewDataErrorContexts.Formatting); set => Text = value.ToString(); }
        /// <summary>
        /// 获取或设置所属单元格的父行的索引。
        /// </summary>
        public int EditingControlRowIndex { get; set; }
        /// <summary>
        /// 获取或设置一个值，该值在文本框控件的当前值是否已更改。
        /// </summary>
        public bool EditingControlValueChanged 
        { 
            get; 
            set; 
        }
        /// <summary>
        /// 获取当鼠标指针位于时使用的光标 <see cref="DataGridView.EditingPanel"/> 但不是通过编辑控件。
        /// </summary>
        public Cursor EditingPanelCursor => Cursors.Default;
        /// <summary>
        /// 获取一个值，该值指示是否需要值发生更改时重新定位单元格的内容。
        /// </summary>
        /// <returns>
        /// true 如果该单元格的 <see cref="DataGridViewCellStyle.WrapMode"/> 设置为 true ，对齐方式属性未设置为其中一个 <see cref="DataGridViewContentAlignment"/> 到文件的顶部; 内容对齐值否则为 false。
        /// </returns>
        public bool RepositionEditingControlOnValueChange => repositionOnValueChange;

        /// <summary>
        /// 初始化 <see cref="DataGridViewSearchTextBoxControl"/> 类的新实例。
        /// </summary>
        public DataGridViewSearchTextBoxControl() : base()
        {
            TabStop = false;
            SearchGrid.RowsDefaultCellStyle.BackColor = Color.Bisque;
            IsAutoMoveFocus = false;
        }

        /// <summary>
        /// 计算指定工作区矩形的大小和位置（以屏幕坐标表示）。
        /// </summary>
        /// <returns>一个 <see cref="Rectangle"/>，表示转换后的 <see cref="Rectangle"/>、p（以屏幕坐标表示）。</returns>
        public override Rectangle GetBounds()
        {
            return EditingControlDataGridView.CurrentCell.OwningColumn is DataGridViewSearchTextBoxColumn stbc
                    ? stbc.IsMain
                        ? EditingControlDataGridView.RectangleToScreen(EditingControlDataGridView.GetCellDisplayRectangle(EditingControlDataGridView.CurrentCell.ColumnIndex, EditingControlDataGridView.CurrentCell.RowIndex, false))
                        : EditingControlDataGridView.RectangleToScreen(EditingControlDataGridView.GetCellDisplayRectangle(EditingControlDataGridView.Columns[stbc.MainColumnName].Index, EditingControlDataGridView.CurrentRow.Index, false))
                    : Rectangle.Empty;
        }

        /// <summary>
        /// 更改控件的用户界面 (UI) 与指定的单元格样式保持一致。
        /// </summary>
        /// <param name="dataGridViewCellStyle"><see cref="DataGridViewCellStyle"/> 以用作 UI 模型。</param>
        public void ApplyCellStyleToEditingControl(DataGridViewCellStyle dataGridViewCellStyle)
        {
            Font = dataGridViewCellStyle.Font;
            if (dataGridViewCellStyle.BackColor.A < 255)
            {
                // Our TextBox does not support transparent back colors
                Color opaqueBackColor = Color.FromArgb(255, dataGridViewCellStyle.BackColor);
                BackColor = opaqueBackColor;
                EditingControlDataGridView.EditingPanel.BackColor = opaqueBackColor;
            }
            else
            {
                BackColor = dataGridViewCellStyle.BackColor;
            }
            ForeColor = dataGridViewCellStyle.ForeColor;
            if (dataGridViewCellStyle.WrapMode == DataGridViewTriState.True)
            {
                WordWrap = true;
            }
            TextAlign = TranslateAlignment(dataGridViewCellStyle.Alignment);
            repositionOnValueChange = dataGridViewCellStyle.WrapMode == DataGridViewTriState.True && (dataGridViewCellStyle.Alignment & anyTop) == 0;
        }

        /// <summary>
        /// 确定指定的键是常规输入的键，编辑控件应处理还是的特殊键 <see cref="DataGridView"/> 应处理。
        /// </summary>
        /// <param name="keyData">一个 <see cref="Keys"/> ，表示已按下的键。</param>
        /// <param name="dataGridViewWantsInputKey">true 当 <see cref="DataGridView"/> 想要处理 keyData; 否则为 false。</param>
        /// <returns>true 如果指定的键是常规输入的键，应处理由编辑控件;否则为 false。</returns>
        public bool EditingControlWantsInputKey(Keys keyData, bool dataGridViewWantsInputKey)
        {
            switch (keyData & Keys.KeyCode)
            {
                case Keys.Right:
                    // If the end of the selection is at the end of the string
                    // let the DataGridView treat the key message
                    if ((RightToLeft == RightToLeft.No && !(SelectionLength == 0 && SelectionStart == Text.Length)) ||
                        (RightToLeft == RightToLeft.Yes && !(SelectionLength == 0 && SelectionStart == 0)))
                    {
                        return true;
                    }
                    break;

                case Keys.Left:
                    // If the end of the selection is at the begining of the string
                    // or if the entire text is selected and we did not start editing
                    // send this character to the dataGridView, else process the key event
                    if ((RightToLeft == RightToLeft.No && !(SelectionLength == 0 && SelectionStart == 0)) ||
                        (RightToLeft == RightToLeft.Yes && !(SelectionLength == 0 && SelectionStart == Text.Length)))
                    {
                        return true;
                    }
                    break;

                case Keys.Down:
                    // If the end of the selection is on the last line of the text then 
                    // send this character to the dataGridView, else process the key event
                    return true;

                case Keys.Up:
                    // If the end of the selection is on the first line of the text then 
                    // send this character to the dataGridView, else process the key event
                    return true;

                case Keys.Home:
                case Keys.End:
                    if (SelectionLength != Text.Length)
                    {
                        return true;
                    }
                    break;

                case Keys.Prior:
                case Keys.Next:
                    if (EditingControlValueChanged)
                    {
                        return true;
                    }
                    break;

                case Keys.Delete:
                    if (SelectionLength > 0 ||
                        SelectionStart < Text.Length)
                    {
                        return true;
                    }
                    break;

                case Keys.Enter:
                    //throw new Exception();
                    if ((keyData & (Keys.Control | Keys.Shift | Keys.Alt)) == Keys.Shift && Multiline && AcceptsReturn)
                    {
                        return true;
                    }
                    else
                    {
                        OnKeyDown(new KeyEventArgs(keyData));
                    }
                    break;
            }
            return !dataGridViewWantsInputKey;
        }

        /// <summary>
        /// 检索该单元格的格式化的值。
        /// </summary>
        /// <param name="context">其中一个 <see cref="DataGridViewDataErrorContexts"/> 值，该值指定数据错误上下文。</param>
        /// <returns><see cref="object"/> ，表示单元格内容的格式的版本。</returns>
        public object GetEditingControlFormattedValue(DataGridViewDataErrorContexts context)
        {
            //if (context == (DataGridViewDataErrorContexts.Parsing | DataGridViewDataErrorContexts.Commit | DataGridViewDataErrorContexts.CurrentCellChange))
            //{
            //    OnKeyDown(new KeyEventArgs(Keys.Enter));
            //}
            return Text;
        }

        /// <summary>
        /// 准备当前选定单元格以便编辑。
        /// </summary>
        /// <param name="selectAll">true 若要选择单元格的内容;否则为 false。</param>
        public void PrepareEditingControlForEdit(bool selectAll)
        {
            if (selectAll)
            {
                SelectAll();
            }
            else
            {
                // Do not select all the text, but
                // position the caret at the end of the text
                SelectionStart = Text.Length;
            }
        }

        private void NotifyDataGridViewOfValueChange()
        {
            EditingControlValueChanged = true;
            EditingControlDataGridView.NotifyCurrentCellDirty(true);
        }

        /// <summary>
        /// 引发 <see cref="Control.TextChanged"/> 事件。
        /// </summary>
        /// <param name="e">包含事件数据的 <see cref="EventArgs"/>。</param>
        protected override void OnTextChanged(EventArgs e)
        {
            base.OnTextChanged(e);
            // Let the DataGridView know about the value change
            NotifyDataGridViewOfValueChange();
        }

        /// <summary>
        /// 处理键消息并生成适当的控件事件。
        /// </summary>
        /// <param name="m">通过引用传递的 System.Windows.Forms.Message，表示要处理的窗口消息。</param>
        /// <returns>如果消息已由控件处理，则为 true；否则为 false。</returns>
#if NET40_OR_GREATER
        [
            SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.UnmanagedCode),
        ]
#endif
        protected override bool ProcessKeyEventArgs(ref Message m)
        {
            int WM_CHAR = 0x0102;
            int WM_KEYDOWN = 0x0100;

            //if (m.Msg == WM_CHAR)
            //{
            //    char c = (char)(ushort)(long)m.WParam;
            //    Match match = Regex.Match(c.ToString(), @"[a-zA-Z0-9]");
            //    if (match.Success)
            //    {
            //        Text = match.Value;
            //        SelectionStart = Text.Length;
            //    }
            //}

            switch ((Keys)(int)m.WParam)
            {
                case Keys.Enter:
                    if (m.Msg == WM_CHAR &&
                        !(ModifierKeys == Keys.Shift && Multiline && AcceptsReturn))
                    {
                        // Ignore the Enter key and don't add it to the textbox content. This happens when failing validation brings
                        // up a dialog box for example.
                        // Shift-Enter for multiline textboxes need to be accepted however.
                        return true;
                    }
                    break;

                case Keys.LineFeed:
                    if (m.Msg == WM_CHAR &&
                        ModifierKeys == Keys.Control && Multiline && AcceptsReturn)
                    {
                        // Ignore linefeed character when user hits Ctrl-Enter to commit the cell.
                        return true;
                    }
                    break;

                case Keys.A:
                    if (m.Msg == WM_KEYDOWN && ModifierKeys == Keys.Control)
                    {
                        SelectAll();
                        return true;
                    }
                    break;

            }
            return base.ProcessKeyEventArgs(ref m);
        }

        /// <summary>
        /// 处理对话框键。
        /// </summary>
        /// <param name="keyData">System.Windows.Forms.Keys 值之一，表示要处理的键。</param>
        /// <returns>如果键已由控件处理，则为 true；否则为 false。</returns>
        protected override bool ProcessDialogKey(Keys keyData)
        {
            switch(keyData)
            {
                case Keys.Enter:
                case Keys.Tab:
                    OnKeyDown(new KeyEventArgs(Keys.Enter));
                    break;
            }
            return base.ProcessDialogKey(keyData);
        }

        private static HorizontalAlignment TranslateAlignment(DataGridViewContentAlignment align)
        {
            if ((align & anyRight) != 0)
            {
                return HorizontalAlignment.Right;
            }
            else if ((align & anyCenter) != 0)
            {
                return HorizontalAlignment.Center;
            }
            else
            {
                return HorizontalAlignment.Left;
            }
        }
    }
}
