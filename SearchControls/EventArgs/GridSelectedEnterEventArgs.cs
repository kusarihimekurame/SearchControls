﻿using System;
using System.ComponentModel;
using System.Windows.Forms;

namespace SearchControls
{
    /// <summary>
    /// 小表正在选择某项所需的事件参数
    /// </summary>
    public class GridSelectingEventArgs : HandledEventArgs
    {
        /// <summary>
        /// 模糊查找的DataGridView表<see cref="DataGridView"/>
        /// </summary>
        public DataGridView SearchGrid { get; }
        /// <summary>
        /// 获取一个值，此值指示发生此事件的单元格的行索引。
        /// </summary>
        public int RowIndex { get; }
        /// <summary>
        /// 选择的行<see cref="DataGridViewRow"/>
        /// </summary>
        public DataGridViewRow CurrentRow => SearchGrid.Rows[RowIndex];
        /// <summary>
        /// 键盘数据
        /// </summary>
        public KeyEventArgs Key { get; }
        /// <summary>
        /// 初始化 GridSelectedEnterEventArgs 类的新实例。
        /// </summary>
        /// <param name="searchGrid">模糊查找的DataGridView表<see cref="DataGridView"/></param>
        /// <param name="rowIndex">行的索引，该行包含发生此事件的单元格。</param>
        /// <param name="key">键盘数据</param>
        public GridSelectingEventArgs(DataGridView searchGrid, int rowIndex, KeyEventArgs key)
        {
            SearchGrid = searchGrid;
            RowIndex = rowIndex;
            Key = key;
        }
    }

    /// <summary>
    /// 小表选择完某项后所需的事件参数
    /// </summary>
    public class GridSelectedEventArgs : EventArgs
    {
        /// <summary>
        /// 模糊查找的DataGridView表<see cref="DataGridView"/>
        /// </summary>
        public DataGridView SearchGrid { get; }
        /// <summary>
        /// 获取一个值，此值指示发生此事件的单元格的行索引。
        /// </summary>
        public int RowIndex { get; }
        /// <summary>
        /// 选择的行<see cref="DataGridViewRow"/>
        /// </summary>
        public DataGridViewRow CurrentRow => SearchGrid.Rows[RowIndex];
        /// <summary>
        /// 键盘数据
        /// </summary>
        public KeyEventArgs Key { get; }
        /// <summary>
        /// 初始化 GridSelectedEnterEventArgs 类的新实例。
        /// </summary>
        /// <param name="searchGrid">模糊查找的DataGridView表<see cref="DataGridView"/></param>
        /// <param name="rowIndex">行的索引，该行包含发生此事件的单元格。</param>
        /// <param name="key">键盘数据</param>
        public GridSelectedEventArgs(DataGridView searchGrid, int rowIndex, KeyEventArgs key)
        {
            SearchGrid = searchGrid;
            RowIndex = rowIndex;
            Key = key;
        }
    }
}