using SearchControls;
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
            DataTable dataTable = new DataTable("Main");
            dataTable.Columns.Add("Name");
            dataTable.Columns.Add("Age");
            dataTable.Columns.Add("CountryCode");
            dataTable.Columns.Add("Country");
            dataTable.Rows.Add("小王", 15,"CN","中国");
            dataTable.Rows.Add("小明", 20,"JP","日本");
            dataTable.Rows.Add("王小李", 30, "DE", "德国");
            DataTable Country = new DataTable("Country");
            Country.Columns.Add("Code");
            Country.Columns.Add("Name");
            Country.Rows.Add("CN", "中国");
            Country.Rows.Add("JP", "日本");
            Country.Rows.Add("DE", "德国");
            Method.CreateManyInitialsDataColumn(dataTable, "Name", "Country");
            DataSet dataSet = new DataSet();
            dataSet.Tables.Add(dataTable);
            dataSet.Tables.Add(Country);
            BindingSource bindingSource = new BindingSource(dataSet, "Main");

            searchDataGridView1.AutoGenerateColumns = false;
            searchDataGridView1.Columns.AddRange(new DataGridViewTextBoxColumn
            {
                Name = "Name",
                DataPropertyName = "Name",
                HeaderText = "名字"
            }, new DataGridViewTextBoxColumn
            {
                Name = "Age",
                DataPropertyName = "Age",
                HeaderText = "年龄"
            }, new DataGridViewSearchTextBoxColumn
            {
                Name = "CountryCode",
                DataPropertyName = "CountryCode",
                HeaderText = "国家代码",
                SearchDataMember = "Country",
                DisplayDataName = "Code",
                AutoInputDataName = "Code",
                SearchColumns = new List<DataGridViewColumn>
                {
                    new DataGridViewTextBoxColumn
                    {
                        Name = "Code",
                        DataPropertyName = "Code",
                        HeaderText = "代码"
                    },
                    new DataGridViewTextBoxColumn
                    {
                        Name = "Name",
                        DataPropertyName = "Name",
                        HeaderText = "名称"
                    }
                }
            }, new DataGridViewSearchTextBoxColumn
            {
                Name = "Country",
                DataPropertyName = "Country",
                HeaderText = "国家名称",
                SearchDataMember = "Country",
                DisplayDataName = "Name",
                AutoInputDataName = "Code",
                IsMain = false,
                MainColumnName = "CountryCode"
            });
            searchDataGridView1.DataSource = bindingSource;
        }

        private void TextBox1_TextChanged(object sender, EventArgs e)
        {
            if (sender is TextBox tb && searchDataGridView1.DataSource is BindingSource bs && bs.List is DataView dv)
            {
                bs.Filter = Method.CreateFilter(dv.Table, tb.Text);
            }
        }
    }
}
