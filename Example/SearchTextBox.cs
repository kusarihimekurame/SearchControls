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
    public partial class searchTextBox : Form
    {
        public searchTextBox()
        {
            InitializeComponent();
            DataTable dataTable = new DataTable();
            dataTable.Columns.Add("Name");
            dataTable.Columns.Add("Age");
            dataTable.Rows.Add("小王", 15);
            dataTable.Rows.Add("小明", 20);
            dataTable.Rows.Add("王小李", 30);
            SearchControls.Method.CreateManyInitialsDataColumn(dataTable, "Name");
            searchTextBox1.DataSource = dataTable;
            searchTextBox1.DisplayDataName = "Name";
            searchTextBox1.SubSearchTextBoxes.Add(textBox1, "Age");
        }
    }
}
