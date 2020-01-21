using SearchControls.Export;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace SearchControls.Export
{
    /// <summary>
    /// Excel初始化完成事件
    /// </summary>
    /// <param name="excel"><see cref="Excel"/></param>
    /// <param name="e"><see cref="EventArgs"/></param>
    public delegate void ExcelInitiatedEventHandler(Excel excel, EventArgs e);
    /// <summary>
    /// DataTable转换Excel完成事件
    /// </summary>
    /// <param name="excel"><see cref="Excel"/></param>
    /// <param name="e"><see cref="EventArgs"/></param>
    public delegate void ExcelCompletedEventHandler(Excel excel, EventArgs e);
    /// <summary>
    /// Word初始化完成事件
    /// </summary>
    /// <param name="word"><see cref="Word"/></param>
    /// <param name="e"><see cref="EventArgs"/></param>
    public delegate void WordInitiatedEventHandler(Word word, EventArgs e);
    /// <summary>
    /// DataTable转换Word完成事件
    /// </summary>
    /// <param name="word"><see cref="Word"/></param>
    /// <param name="e"><see cref="EventArgs"/></param>
    public delegate void WordCompletedEventHandler(Word word, EventArgs e);
}
