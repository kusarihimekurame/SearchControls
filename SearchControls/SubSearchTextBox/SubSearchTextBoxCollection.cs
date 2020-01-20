using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace SearchControls
{
    /// <summary>
    /// SearchTextBox的附属文本框的集合
    /// </summary>
    public class SubSearchTextBoxCollection : List<SubSearchTextBox>
    {
        private CollectionChangeEventHandler onCollectionChanged;

        internal bool IsCollectionChangedListenedTo => onCollectionChanged != null;

        /// <summary>
        /// 当更改集合的内容时发生。
        /// </summary>
        public event CollectionChangeEventHandler CollectionChanged
        {
            add
            {
                onCollectionChanged += value;
            }
            remove
            {
                onCollectionChanged -= value;
            }
        }

        /// <summary>
        /// 引发 SearchControls.SubSearchTextBoxCollection.CollectionChanged 事件。
        /// </summary>
        /// <param name="e">包含事件数据的 System.ComponentModel.CollectionChangeEventArgs。</param>
        protected virtual void OnCollectionChanged(CollectionChangeEventArgs e)
        {
            onCollectionChanged?.Invoke(this, e);
        }

        /// <summary>
        /// 获取或设置指定文本框的元素。
        /// </summary>
        /// <param name="textBox">附属的文本框</param>
        /// <returns>指定文本框的元素</returns>
        public SubSearchTextBox this[TextBox textBox]
        {
            get => this[FindIndex(m => m.TextBox.Equals(textBox))];
            set => this[FindIndex(m => m.TextBox.Equals(textBox))] = value;
        }

        /// <summary>
        /// 将对象添加到 System.Collections.Generic.List`1 的结尾处。
        /// </summary>
        /// <param name="item">要添加到 System.Collections.Generic.List`1 的末尾处的对象。对于引用类型，该值可以为 null。</param>
        /// <exception cref="Exception">列表中不能出现重复的文本框</exception>
        public new void Add(SubSearchTextBox item)
        {
            CollectionChangeEventArgs e = new CollectionChangeEventArgs(CollectionChangeAction.Add, item);
            if (Contains(item.TextBox)) throw new Exception("列表中已经包含此文本框,无法添加相同的文本框");
            OnCollectionChanged(e);
            base.Add(item);
        }

        /// <summary>
        /// 将对象添加到 System.Collections.Generic.List`1 的结尾处。
        /// </summary>
        /// <param name="textBox">附属的文本框</param>
        /// <param name="displayDataName">要绑定显示的列名</param>
        /// <param name="autoInputDataName">自动输入的列名</param>
        /// <param name="isMoveGrid">模糊查找的表是否移到副表位置</param>
        /// <exception cref="Exception">列表中不能出现重复的文本框</exception>
        public void Add(TextBox textBox, string displayDataName, string autoInputDataName = null, bool isMoveGrid = false) => Add(new SubSearchTextBox(textBox, displayDataName, autoInputDataName, isMoveGrid));

        /// <summary>
        /// 将指定集合的元素添加到 System.Collections.Generic.List`1 的末尾。
        /// </summary>
        /// <param name="collection">一个集合，其元素应被添加到 System.Collections.Generic.List`1 的末尾。集合自身不能为 null，但它可以包含为 null的元素（如果类型 T 为引用类型）。</param>
        /// <exception cref="Exception">列表中不能出现重复的文本框</exception>
        public new void AddRange(IEnumerable<SubSearchTextBox> collection)
        {
            CollectionChangeEventArgs e = new CollectionChangeEventArgs(CollectionChangeAction.Add, collection);
            if (this.Any(sstb => collection.Any(_sstb => _sstb.TextBox.Equals(sstb.TextBox)))) throw new Exception("列表中已经包含此文本框,无法添加相同的文本框");
            OnCollectionChanged(e);
            base.AddRange(collection);
        }

        /// <summary>
        /// 将指定集合的元素添加到 System.Collections.Generic.List`1 的末尾。
        /// </summary>
        /// <param name="collection">一个集合，其元素应被添加到 System.Collections.Generic.List`1 的末尾。集合自身不能为 null，但它可以包含为 null的元素（如果类型 T 为引用类型）。</param>
        /// <exception cref="Exception">列表中不能出现重复的文本框</exception>
        public void AddRange(params SubSearchTextBox[] collection) => AddRange(collection.ToList());

        /// <summary>
        /// 从 System.Collections.Generic.List`1 中移除所有元素。
        /// </summary>
        public new void Clear()
        {
            CollectionChangeEventArgs e = new CollectionChangeEventArgs(CollectionChangeAction.Refresh, null);
            OnCollectionChanged(e);
            base.Clear();
        }

        /// <summary>
        /// 确定某元素是否在 System.Collections.Generic.List`1 中。
        /// </summary>
        /// <param name="textBox">附属的文本框</param>
        /// <param name="displayDataName">需要展示的数据列名</param>
        /// <param name="autoInputDataName">自动输入的列名</param>
        /// <returns>如果在 System.Collections.Generic.List`1 中找到 item，则为 true，否则为 false。</returns>
        public bool Contains(TextBox textBox, string displayDataName, string autoInputDataName = null) => Contains(new SubSearchTextBox(textBox, displayDataName, autoInputDataName));
        /// <summary>
        /// 确定某元素是否在 System.Collections.Generic.List`1 中。
        /// </summary>
        /// <param name="textBox">附属的文本框</param>
        /// <returns>如果在 System.Collections.Generic.List`1 中找到 item，则为 true，否则为 false。</returns>
        public bool Contains(TextBox textBox) => this.Any(sstb => sstb.TextBox.Equals(textBox));

        /// <summary>
        /// 搜索指定的对象，并返回整个 System.Collections.Generic.List`1 中第一个匹配项的从零开始的索引。
        /// </summary>
        /// <param name="item">附属的文本框</param>
        /// <returns>如果在整个 System.Collections.Generic.List`1 中找到 item 的第一个匹配项，则为该项的从零开始的索引；否则为 -1。</returns>
        public int IndexOf(TextBox item) => FindIndex(t => t.TextBox.Equals(item));

        /// <summary>
        /// 搜索指定的对象，并返回整个 System.Collections.Generic.List`1 中最后一个匹配项的从零开始的索引。
        /// </summary>
        /// <param name="item">附属的文本框</param>
        /// <returns>如果在整个 System.Collections.Generic.List`1 中找到 item 的最后一个匹配项，则为该项的从零开始的索引；否则为 -1。</returns>
        public int LastIndexOf(TextBox item) => FindLastIndex(t => t.TextBox.Equals(item));

        /// <summary>
        /// 将元素插入 System.Collections.Generic.List`1 的指定索引处。
        /// </summary>
        /// <param name="index">从零开始的索引，应在该位置插入 item。</param>
        /// <param name="item">要插入的对象。对于引用类型，该值可以为 null。</param>
        public new void Insert(int index, SubSearchTextBox item)
        {
            CollectionChangeEventArgs e = new CollectionChangeEventArgs(CollectionChangeAction.Add, item);
            OnCollectionChanged(e);
            base.Insert(index, item);
        }

        /// <summary>
        /// 将元素插入 System.Collections.Generic.List`1 的指定索引处。
        /// </summary>
        /// <param name="index">从零开始的索引，应在该位置插入 item。</param>
        /// <param name="textBox">附属的文本框</param>
        /// <param name="displayDataName">要展示的列名</param>
        /// <param name="autoInputDataName">自动输入的列名</param>
        public void Insert(int index, TextBox textBox, string displayDataName, string autoInputDataName = null) => Insert(index, new SubSearchTextBox(textBox, displayDataName, autoInputDataName));

        /// <summary>
        /// 将集合中的某个元素插入 System.Collections.Generic.List`1 的指定索引处。
        /// </summary>
        /// <param name="index">应在此处插入新元素的从零开始的索引。</param>
        /// <param name="collection">一个集合，应将其元素插入到 System.Collections.Generic.List`1 中。集合自身不能为 null，但它可以包含为 null 的元素（如果类型T 为引用类型）。</param>
        public new void InsertRange(int index, IEnumerable<SubSearchTextBox> collection)
        {
            CollectionChangeEventArgs e = new CollectionChangeEventArgs(CollectionChangeAction.Add, collection);
            OnCollectionChanged(e);
            base.InsertRange(index, collection);
        }

        /// <summary>
        /// 从 System.Collections.Generic.List`1 中移除特定对象的第一个匹配项。
        /// </summary>
        /// <param name="item">要从 System.Collections.Generic.List`1 中移除的对象。对于引用类型，该值可以为 null。</param>
        /// <returns>如果成功移除 item，则为 true；否则为 false。如果在 System.Collections.Generic.List`1 中没有找到 item，该方法也会返回false。</returns>
        public new bool Remove(SubSearchTextBox item)
        {
            CollectionChangeEventArgs e = new CollectionChangeEventArgs(CollectionChangeAction.Remove, item);
            OnCollectionChanged(e);
            return base.Remove(item);
        }

        /// <summary>
        /// 从 System.Collections.Generic.List`1 中移除特定对象的第一个匹配项。
        /// </summary>
        /// <param name="textBox">附属的文本框</param>
        /// <returns>如果成功移除 item，则为 true；否则为 false。如果在 System.Collections.Generic.List`1 中没有找到 item，该方法也会返回false。</returns>
        public bool Remove(TextBox textBox) => base.Remove(Find(m => m.TextBox.Equals(textBox)));

        /// <summary>
        /// 移除与指定的谓词所定义的条件相匹配的所有元素。
        /// </summary>
        /// <param name="match">System.Predicate`1 委托，用于定义要移除的元素应满足的条件。</param>
        /// <returns>从 System.Collections.Generic.List`1 中移除的元素的数目。</returns>
        public new int RemoveAll(Predicate<SubSearchTextBox> match)
        {
            CollectionChangeEventArgs e = new CollectionChangeEventArgs(CollectionChangeAction.Remove, FindAll(match));
            OnCollectionChanged(e);
            return base.RemoveAll(match);
        }

        /// <summary>
        /// 移除 System.Collections.Generic.List`1 的指定索引处的元素。
        /// </summary>
        /// <param name="index">要移除的元素的从零开始的索引。</param>
        public new void RemoveAt(int index)
        {
            CollectionChangeEventArgs e = new CollectionChangeEventArgs(CollectionChangeAction.Remove, this[index]);
            OnCollectionChanged(e);
            base.RemoveAt(index);
        }

        /// <summary>
        /// 从 System.Collections.Generic.List`1 中移除一定范围的元素。
        /// </summary>
        /// <param name="index">要移除的元素的范围从零开始的起始索引。</param>
        /// <param name="count">要移除的元素数。</param>
        public new void RemoveRange(int index, int count)
        {
            CollectionChangeEventArgs e = new CollectionChangeEventArgs(CollectionChangeAction.Remove, GetRange(index, count));
            OnCollectionChanged(e);
            base.RemoveRange(index, count);
        }
    }
}
