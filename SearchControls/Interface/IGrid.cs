using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace SearchControls
{
    internal interface IGrid
    {
        int DisplayRowCount { get; set; }
        bool IsUp { get; set; }
        bool IsAutoReset { get; set; }
        bool isAutoReset { get; }
        TextBox textBox { get; }
        void Reset();
        void ShowSearchGrid();
        void SetSearchGridSize();
        void SetSearchGridLocation();
        void OnSearchGridLocationSizeChanged(SubFormLocationSizeEventArgs e);
    }
}
