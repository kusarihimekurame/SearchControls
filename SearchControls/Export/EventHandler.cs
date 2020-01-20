using SearchControls.Export;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace SearchControls.Export
{

    public delegate void ExcelInitiatedEventHandler(Excel excel, EventArgs e);

    public delegate void ExcelCompletedEventHandler(Excel excel, EventArgs e);
    public delegate void WordInitiatedEventHandler(Word word, EventArgs e);

    public delegate void WordCompletedEventHandler(Word word, EventArgs e);
}
