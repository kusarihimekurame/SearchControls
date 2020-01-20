using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace SearchControls.Export
{
    public class Progress<T> : IProgress<T>
    {
        private Control c;
        public delegate void ProgressChangedEventHandler(object sender, T e);
        public event ProgressChangedEventHandler ProgressChanged;
        public Action<T> Handler;

        public void Report(T value)
        {
            if (c.InvokeRequired)
            {
                try
                {
                    if (ProgressChanged != null)
                        c.Invoke(ProgressChanged, this, value);
                    else
                        c.Invoke(Handler, value);
                }
                catch { }
            }
        }

        public Progress(Control control)
        {
            c = control;
        }

        public Progress(Control control, Action<T> handler) : this(control)
        {
            Handler = handler;
        }
    }
}
