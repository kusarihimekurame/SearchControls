using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Example
{
    public partial class searchDataGridView : Form
    {
        public searchDataGridView()
        {
            InitializeComponent();
            DataTable dataTable = new DataTable();
            dataTable.Columns.Add("Name");
            dataTable.Columns.Add("Age");
            dataTable.Rows.Add("小王", 15);
            dataTable.Rows.Add("小明", 20);
            dataTable.Rows.Add("王小李", 30);
        }
    }
}
