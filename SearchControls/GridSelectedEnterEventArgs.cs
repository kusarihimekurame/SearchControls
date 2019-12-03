using System;
using System.Windows.Forms;

namespace Search
{
    /// <summary>
    /// 小表正在选择某项所需的事件参数
    /// </summary>
    public class GridSelectingEventArgs : EventArgs
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
        /// 获取一个值，该值表示是否处理过此事件
        /// </summary>
        public bool Handled { get; set; }
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
}