﻿using SearchControls.Classes;
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

            DataSet ds = new();
            DataTable dataTable = new("Users");
            dataTable.Columns.Add("Name");
            dataTable.Columns.Add("Age");
            dataTable.Rows.Add("小王", 15);
            dataTable.Rows.Add("小明", 20);
            dataTable.Rows.Add("王小李", 30);
            SearchControls.Method.CreateManyInitialsDataColumn(dataTable, "Name");
            ds.Tables.Add(dataTable);

            List<user> users = new();
            users.Add(new() { Name = "小王", Age = 15, PinYin = "XW" });
            users.Add(new() { Name = "小明", Age = 20, PinYin = "XM" });
            users.Add(new() { Name = "王小李", Age = 30, PinYin = "WXL" });

            //searchTextBox1.DataSource = ds;
            //searchTextBox1.DataMember = "Users";
            //searchTextBox1.DataSource = dataTable;
            searchTextBox1.DataSource = users.ToBindingListView();

            searchTextBox1.DisplayDataName = "Name";
            searchTextBox1.SubSearchTextBoxes.Add(textBox1, "Age");
        }

        class user
        {
            public string Name { get; set; }
            public int Age { get; set; }
            public string PinYin { get; set; }
        }
    }
}
