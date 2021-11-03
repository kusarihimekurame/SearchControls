using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SearchControls.Classes
{
    /// <summary>
    /// 扩展方法
    /// </summary>
    public static partial class Extension
    {
        /// <summary>
        /// 生成列表试图
        /// </summary>
        /// <typeparam name="T">类</typeparam>
        /// <param name="bindingList">数据绑定的泛型集合</param>
        /// <returns>数据绑定的泛型集合的视图</returns>
        public static BindingListView<T> ToBindingListView<T>(this BindingList<T> bindingList) where T : class => new(bindingList as IBindingList);
        /// <summary>
        /// 生成列表试图
        /// </summary>
        /// <typeparam name="T">类</typeparam>
        /// <param name="bindingList">数据绑定的泛型集合</param>
        /// <returns>数据绑定的泛型集合的视图</returns>
        public static BindingListView<T> ToBindingListView<T>(this IBindingList bindingList) where T : class => new(bindingList);
        /// <summary>
        /// 生成列表试图
        /// </summary>
        /// <typeparam name="T">类</typeparam>
        /// <param name="list">集合</param>
        /// <returns>数据绑定的泛型集合的视图</returns>
        public static BindingListView<T> ToBindingListView<T>(this IList<T> list) where T : class => new(list);
    }
    /// <summary>
    /// 数据绑定的泛型集合的视图
    /// </summary>
    /// <typeparam name="T">类</typeparam>
    public class BindingListView<T> : IBindingListView, ITypedList where T : class
    {
        List<T> View;
        IBindingList _bindingList;
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="bindingList">数据绑定的泛型集合</param>
        public BindingListView(IBindingList bindingList)
        {
            _bindingList = bindingList;
            View = _bindingList.Cast<T>().ToList();

            _bindingList.ListChanged += (sender, e) =>
            {
                switch (e.ListChangedType)
                {
                    case ListChangedType.Reset:
                        if (string.IsNullOrWhiteSpace(filter))
                        {
                            View = _bindingList.Cast<T>().ToList();
                            OnListChanged(e);
                        }
                        else
                        {
                            string _filter = filter;
                            filter = null;
                            Filter = _filter;
                        }
                        break;
                    case ListChangedType.ItemAdded:
                        if (!View.Contains(_bindingList[e.NewIndex]))
                            OnListChanged(new ListChangedEventArgs(e.ListChangedType, ((IList)View).Add(_bindingList[e.NewIndex])));
                        break;
                }
            };
        }
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="list">集合</param>
        public BindingListView(IList<T> list) : this(new BindingList<T>(list) as IBindingList) { }
        /// <summary>
        /// 构造函数
        /// </summary>
        public BindingListView() : this(new BindingList<T>() as IBindingList) { }
        /// <summary>
        /// 获取或设置指定索引处的元素。
        /// </summary>
        /// <param name="index">要获取或设置的元素的从零开始的索引。</param>
        /// <returns>指定索引处的元素。</returns>
        /// <exception cref="ArgumentOutOfRangeException">index 小于 0。 或 - index 等于或大于 <see cref="List{T}.Count"/>。</exception>
        public virtual T this[int index]
        {
            get => View[index];
            set
            {
                View[index] = value;
                _bindingList[_bindingList.IndexOf(View[index])] = value;
                OnListChanged(new ListChangedEventArgs(ListChangedType.ItemChanged, index));
            }
        }
        object IList.this[int index] { get => this[index]; set => this[index] = (T)value; }

        private string filter;
        /// <summary>
        /// 获取或设置筛选器，以用于从数据源返回的项的集合中排除项。
        /// </summary>
        /// <returns>用于在数据源返回的项集合中筛选掉项的字符串。</returns>
        public virtual string Filter
        {
            get => filter;
            set
            {
                if (filter != value)
                {
                    filter = value;

                    if (!string.IsNullOrWhiteSpace(filter))
                    {
                        filter = Regex.Replace(filter, @" +like +'([^']*)'", match =>
                        {
                            string v = match.Groups[1].Value;
                            if (v.StartsWith("%") && v.EndsWith("%"))
                                return $".ToLower().Contains(\"{v.Substring(1, v.Length - 2).ToLower()}\")";
                            else if (v.StartsWith("%"))
                                return $".EndsWith(\"{v.Substring(1)}\" ,StringComparison.OrdinalIgnoreCase)";
                            else if (v.EndsWith("%"))
                                return $".StartsWith(\"{v.Substring(0, v.Length - 2)}\" ,StringComparison.OrdinalIgnoreCase)";
                            else
                                return $".Equals(\"{v}\" ,StringComparison.OrdinalIgnoreCase)";
                        }, RegexOptions.IgnoreCase);
                        if (filter.Contains("'")) filter = filter.Replace("'", "\"");
                        View = _bindingList.Cast<T>().ToList().AsQueryable().Where(filter).ToList();
                    }
                    else
                    {
                        View = _bindingList.Cast<T>().ToList();
                    }

                    OnListChanged(new ListChangedEventArgs(ListChangedType.Reset, -1));
                }
            }
        }
        /// <summary>
        /// 获取当前应用于数据源的排序说明的集合。
        /// </summary>
        /// <returns>当前应用于数据源的 <see cref="ListSortDescriptionCollection"/>。</returns>
        public virtual ListSortDescriptionCollection SortDescriptions => throw new NotImplementedException();
        /// <summary>
        /// 获取一个值，指示数据源是否支持高级排序。
        /// </summary>
        /// <returns>如果数据源支持高级排序，则为 true；否则为 false。</returns>
        public virtual bool SupportsAdvancedSorting => false;
        /// <summary>
        /// 获取一个值，该值指示数据源是否支持筛选。
        /// </summary>
        /// <returns>如果数据源支持筛选，则为 true；否则为 false。</returns>
        public virtual bool SupportsFiltering => true;
        /// <summary>
        /// 获取是否可以使用 <see cref="AddNew"/> 向列表中添加项。
        /// </summary>
        /// <returns>如果可以使用 <see cref="AddNew"/> 向列表中添加项，则为 true；否则为 false。</returns>
        public virtual bool AllowNew => _bindingList.AllowNew;
        /// <summary>
        /// 获取是否可更新列表中的项。
        /// </summary>
        /// <returns>如果可以更新列表中的项，则为 true；否则为 false。</returns>
        public virtual bool AllowEdit => _bindingList.AllowEdit;
        /// <summary>
        /// 获取是否可以使用 <see cref="Remove(object)"/> 或 <see cref="RemoveAt(int)"/> 从列表中移除项。
        /// </summary>
        /// <returns>如果可以从列表中移除项，则为 true；否则为 false。</returns>
        public virtual bool AllowRemove => _bindingList.AllowRemove;
        /// <summary>
        /// 获取当列表更改或列表中的项更改时是否引发 <see cref="ListChanged"/> 事件。
        /// </summary>
        /// <returns>如果当列表更改或项更改时引发了 <see cref="ListChanged"/> 事件，则为 true；否则为false。</returns>
        public virtual bool SupportsChangeNotification => _bindingList.SupportsChangeNotification;
        /// <summary>
        /// 获取列表是否支持使用 <see cref="Find(PropertyDescriptor, object)"/> 方法进行搜索。
        /// </summary>
        /// <returns>如果列表支持使用 <see cref="Find(PropertyDescriptor, object)"/> 方法进行搜索，则为 true；否则为 false。</returns>
        public virtual bool SupportsSearching => true;
        /// <summary>
        /// 获取列表是否支持排序。
        /// </summary>
        /// <returns>如果列表支持排序，则为 true；否则为 false。</returns>
        public virtual bool SupportsSorting => true;
        private bool isSorted = false;
        /// <summary>
        /// 获取是否对列表中的项进行排序。
        /// </summary>
        /// <returns>
        /// 如果已调用 <see cref="ApplySort(ListSortDescriptionCollection)"/> 并且未调用 <see cref="RemoveSort"/>，则为 true；否则为 false。
        /// </returns>
        /// <exception cref="NotSupportedException"><see cref="SupportsSorting"/> 为 false。</exception>
        public virtual bool IsSorted => SupportsSorting ? _bindingList.SupportsSorting ? _bindingList.IsSorted : isSorted : throw new NotSupportedException($"{nameof(SupportsSorting)} 为 false");
        private PropertyDescriptor sortProperty;
        /// <summary>
        /// 获取正在用于排序的 <see cref="PropertyDescriptor"/>。
        /// </summary>
        /// <returns>正在用于排序的 <see cref="PropertyDescriptor"/>。</returns>
        /// <exception cref="NotSupportedException"><see cref="SupportsSorting"/> 为 false。</exception>
        public virtual PropertyDescriptor SortProperty => SupportsSorting ? _bindingList.SupportsSorting ? _bindingList.SortProperty : sortProperty : throw new NotSupportedException($"{nameof(SupportsSorting)} 为 false");
        private ListSortDirection sortDirection;
        /// <summary>
        /// 获取排序的方向。
        /// </summary>
        /// <returns><see cref="ListSortDirection"/> 值之一。</returns>
        /// <exception cref="NotSupportedException"><see cref="SupportsSorting"/> 为 false。</exception>
        public virtual ListSortDirection SortDirection => SupportsSorting ? _bindingList.SupportsSorting ? _bindingList.SortDirection : sortDirection : throw new NotSupportedException($"{nameof(SupportsSorting)} 为 false");
        /// <summary>
        /// 获取一个值，该值指示 <see cref="IList"/> 是否为只读。
        /// </summary>
        /// <returns>如果 true 是只读的，则为 <see cref="IList"/>；否则为 false。</returns>
        public virtual bool IsReadOnly => _bindingList.IsReadOnly;
        /// <summary>
        /// 获取一个值，该值指示 <see cref="IList"/> 是否具有固定大小。
        /// </summary>
        /// <returns>如果 true 具有固定大小，则为 <see cref="IList"/>；否则为 false。</returns>
        public virtual bool IsFixedSize => _bindingList.IsFixedSize;
        /// <summary>
        /// 获取 <see cref="List{T}.Count"/> 中包含的元素数。
        /// </summary>
        public virtual int Count => View.Count;
        /// <summary>
        /// 获取可用于同步对 <see cref="ICollection"/> 的访问的对象。
        /// </summary>
        /// <returns>可用于同步对 <see cref="ICollection"/> 的访问的对象。</returns>
        public virtual object SyncRoot => _bindingList.SyncRoot;
        /// <summary>
        /// 获取一个值，该值指示是否同步对 <see cref="ICollection"/> 的访问（线程安全）。
        /// </summary>
        /// <returns>如果对 true 的访问是同步的（线程安全），则为 <see cref="ICollection"/>；否则为 false。</returns>
        public virtual bool IsSynchronized => _bindingList.IsSynchronized;
        /// <summary>
        /// 当列表或列表中的项更改时发生。
        /// </summary>
        public event ListChangedEventHandler ListChanged;
        /// <summary>
        /// 引发 <see cref="ListChanged"/> 事件。
        /// </summary>
        /// <param name="e">包含事件数据的 <see cref="ListChangedEventArgs"/>。</param>
        public virtual void OnListChanged(ListChangedEventArgs e) => ListChanged?.Invoke(this, e);
        /// <summary>
        /// 将某项添加到 <see cref="IList"/> 中。
        /// </summary>
        /// <param name="value">要添加到 <see cref="IList"/> 的对象。</param>
        /// <returns>插入了新元素的位置，-1 指示该项未插入到集合中。</returns>
        public virtual int Add(object value) => _bindingList.Add(value);
        /// <summary>
        /// 将 <see cref="PropertyDescriptor"/> 添加到用于搜索的索引。
        /// </summary>
        /// <param name="property">将 <see cref="PropertyDescriptor"/> 添加到用于搜索的索引。</param>
        public virtual void AddIndex(PropertyDescriptor property) => _bindingList.AddIndex(property);
        /// <summary>
        /// 将新项添加到列表。
        /// </summary>
        /// <returns>添加到列表的项。</returns>
        /// <exception cref="NotSupportedException"><see cref="AllowNew"/> 为 false。</exception>
        public virtual object AddNew() => AllowNew ? _bindingList.AddNew() : throw new NotSupportedException($"{nameof(AllowNew)} 为 false");
        /// <summary>
        /// 目前没有实现
        /// </summary>
        /// <param name="sorts"></param>
        public virtual void ApplySort(ListSortDescriptionCollection sorts)
        {
            throw new NotImplementedException();
        }
        /// <summary>
        /// 根据给定的 <see cref="ListSortDescriptionCollection"/> 对数据源进行排序。
        /// </summary>
        /// <param name="property">以其为根据进行排序的 System.ComponentModel.PropertyDescriptor。</param>
        /// <param name="direction">System.ComponentModel.ListSortDirection 值之一。</param>
        /// <exception cref="NotSupportedException"><see cref="SupportsSorting"/> 为 false。</exception>
        public virtual void ApplySort(PropertyDescriptor property, ListSortDirection direction)
        {
            if (!SupportsSorting) throw new NotSupportedException($"{nameof(SupportsSorting)} 为 false。");
            if (_bindingList.SupportsSorting)
                _bindingList.ApplySort(property, direction);
            else
            {
                Type comparerForPropertyType = typeof(Comparer<>).MakeGenericType(property.PropertyType);
                IComparer comparer1 = (IComparer)comparerForPropertyType.InvokeMember("Default", BindingFlags.Static | BindingFlags.GetProperty | BindingFlags.Public, null, null, null);
                int reverse = direction == ListSortDirection.Ascending ? 1 : -1;

#if NET40
                Comparer<T> comparer2 = new ComparisonComparer<T>(new Comparison<T>((a, b) => reverse * comparer1.Compare(property.GetValue(a), property.GetValue(b))));
#else
                Comparer<T> comparer2 = Comparer<T>.Create(new Comparison<T>((a, b) => reverse * comparer1.Compare(property.GetValue(a), property.GetValue(b))));
#endif
                View.Sort(comparer2);

                sortProperty = property;
                sortDirection = direction;
                isSorted = true;

                OnListChanged(new ListChangedEventArgs(ListChangedType.Reset, -1));
            }
        }

#if NET40
        private class ComparisonComparer<T1> : Comparer<T1>
        {
            private readonly Comparison<T1> comparison;
            public ComparisonComparer(Comparison<T1> comparison)
            {
                this.comparison = comparison;
            }
            public override int Compare(T1 x, T1 y) => comparison(x, y);
        }
#endif
        /// <summary>
        /// 从 <see cref="IList"/> 中移除所有项。
        /// </summary>
        public virtual void Clear() => _bindingList.Clear();
        /// <summary>
        /// 确定 <see cref="IList"/> 是否包含特定值。
        /// </summary>
        /// <param name="value">要在 <see cref="IList"/> 中定位的对象。</param>
        /// <returns>如果在 <see cref="IList"/> 中找到了 <see cref="object"/>，则为 true；否则为 false。</returns>
        public virtual bool Contains(object value) => View.Contains(value);
        /// <summary>
        /// 从特定的 <see cref="ICollection"/> 索引处开始，将 <see cref="Array"/> 的元素复制到一个 <see cref="Array"/> 中。
        /// </summary>
        /// <param name="array">一维 <see cref="Array"/>，它是从 <see cref="ICollection"/> 复制的元素的目标。 <see cref="Array"/> 必须具有从零开始的索引。</param>
        /// <param name="index">array 中从零开始的索引，从此处开始复制。</param>
        /// <exception cref="ArgumentNullException">array 为 null。</exception>
        /// <exception cref="ArgumentOutOfRangeException">index 小于零。</exception>
        /// <exception cref="ArgumentException">array 是多维的。 或 源 <see cref="ICollection"/> 中的元素个数大于从 index 到目标 array 末尾之间的可用空间。或 无法自动将源 <see cref="ICollection"/> 的类型转换为目标 array 的类型。</exception>
        public virtual void CopyTo(Array array, int index) => View.CopyTo(array as T[], index);

        /// <summary>
        /// 返回具有给定 <see cref="PropertyDescriptor"/> 的行的索引。
        /// </summary>
        /// <param name="property">要对其进行搜索的 <see cref="PropertyDescriptor"/>。</param>
        /// <param name="key">要搜索的 property 参数的值。</param>
        /// <returns>具有给定 <see cref="PropertyDescriptor"/> 的行的索引。</returns>
        /// <exception cref="NotSupportedException"><see cref="SupportsSearching"/> 为 false。</exception>
        public virtual int Find(PropertyDescriptor property, object key)
            => SupportsSearching
            ? _bindingList.SupportsSearching
                ? IndexOf(_bindingList[_bindingList.Find(property, key)])
                : View.FindIndex(view => 
                    property.GetValue(view) is string str 
                        ? str.Trim().Equals(key.ToString().Trim(), StringComparison.OrdinalIgnoreCase) 
                        : property.GetValue(view) == key)
            : throw new NotSupportedException($"{nameof(SupportsSearching)} 为 false。");
        /// <summary>
        /// 返回循环访问集合的枚举数。
        /// </summary>
        /// <returns>一个可用于循环访问集合的 <see cref="IEnumerator"/> 对象。</returns>
        public virtual IEnumerator GetEnumerator() => View.GetEnumerator();
        /// <summary>
        /// 确定 <see cref="IList"/> 中特定项的索引。
        /// </summary>
        /// <param name="value">要在 <see cref="IList"/> 中定位的对象。</param>
        /// <returns>如果在列表中找到，则为 value 的索引；否则为 -1。</returns>
        public virtual int IndexOf(object value) => View.IndexOf(value as T);
        /// <summary>
        /// 在 <see cref="IList"/> 中的指定索引处插入一个项。
        /// </summary>
        /// <param name="index">应插入 value 的从零开始的索引。</param>
        /// <param name="value">要插入到 <see cref="IList"/> 中的对象。</param>
        /// <exception cref="ArgumentOutOfRangeException">index 不是 <see cref="IList"/> 中的有效索引。</exception>
        public virtual void Insert(int index, object value)
        {
            _bindingList.Insert(_bindingList.IndexOf(View[index]), value);
            View.Insert(index, value as T);
            OnListChanged(new ListChangedEventArgs(ListChangedType.ItemAdded, index));
        }
        /// <summary>
        /// 从 <see cref="IList"/> 中移除特定对象的第一个匹配项。
        /// </summary>
        /// <param name="value">要从 <see cref="IList"/> 中删除的对象。</param>
        public virtual void Remove(object value)
        {
            _bindingList.Remove(value);
            int index = View.IndexOf(value as T);
            View.Remove(value as T);
            OnListChanged(new ListChangedEventArgs(ListChangedType.ItemDeleted, index));
        }
        /// <summary>
        /// 移除位于指定索引处的 <see cref="IList"/> 项。
        /// </summary>
        /// <param name="index">要移除的项的从零开始的索引。</param>
        /// <exception cref="ArgumentOutOfRangeException">index 不是 <see cref="IList"/> 中的有效索引。</exception>
        public virtual void RemoveAt(int index)
        {
            _bindingList.RemoveAt(IndexOf(View[index]));
            View.RemoveAt(index);
            OnListChanged(new ListChangedEventArgs(ListChangedType.ItemDeleted, index));
        }
        /// <summary>
        /// 移除应用于数据源的当前筛选器。
        /// </summary>
        public virtual void RemoveFilter()
        {
            Filter = null;
        }
        /// <summary>
        /// 将 <see cref="PropertyDescriptor"/> 从用于搜索的索引中移除。
        /// </summary>
        /// <param name="property">要从用于搜索的索引中移除的 <see cref="PropertyDescriptor"/>。</param>
        public virtual void RemoveIndex(PropertyDescriptor property) => _bindingList.RemoveIndex(property);
        /// <summary>
        /// 使用 <see cref="ApplySort(PropertyDescriptor, ListSortDirection)"/> 移除任何已应用的排序。
        /// </summary>
        /// <exception cref="NotSupportedException"><see cref="SupportsSorting"/> 为 false。</exception>
        public virtual void RemoveSort()
        {
            if (!SupportsSorting) throw new NotSupportedException($"{nameof(SupportsSorting)} 为 false。");
            if (_bindingList.SupportsSorting)
            {
                _bindingList.RemoveSort();
            }
            else
            {
                isSorted = false;
                sortProperty = null;
                sortDirection = ListSortDirection.Ascending;

                OnListChanged(new ListChangedEventArgs(ListChangedType.Reset, -1));
            }
        }
        /// <summary>
        /// 根据需要查找的文本内容，自动生成Filter语句
        /// </summary>
        /// <param name="text">需要查找的文本内容</param>
        /// <param name="propertyName">需要查找的列名（如果为空则默认为全部）</param>
        /// <returns></returns>
        public static string CreateFilter(string text, params string[] propertyName)
        {
            List<string> filters = new List<string>();
            PropertyInfo[] properties =
                propertyName == null || propertyName.Count() == 0
                ? typeof(T).GetProperties(BindingFlags.Instance | BindingFlags.Public)
                : propertyName.Select(name => typeof(T).GetProperty(name, BindingFlags.Instance | BindingFlags.Public)).ToArray();

            string[] Texts = text.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

            foreach (string _text in Texts)
            {
                foreach (PropertyInfo property in properties)
                {
                    switch (property.PropertyType.Name)
                    {
                        case nameof(String):
                            filters.Add($"{property.Name}.ToLower().Contains(\"{_text}\")");
                            break;
                        case nameof(Int16):
                        case nameof(Int32):
                        case nameof(Int64):
                        case nameof(Decimal):
                            filters.Add($"{property.Name}.ToString().Contains(\"{text}\")");
                            break;
                    }
                }
            }
            return string.Join(" OR ", filters);
        }

        string ITypedList.GetListName(PropertyDescriptor[] listAccessors)
        {
            return typeof(T).Name;
        }

        PropertyDescriptorCollection ITypedList.GetItemProperties(PropertyDescriptor[] listAccessors)
        {
            PropertyDescriptorCollection pdc = TypeDescriptor.GetProperties(
                listAccessors == null
                    ? typeof(T)
                    : listAccessors.Last().PropertyType.GetGenericArguments()[0]);

            return new PropertyDescriptorCollection(pdc.Cast<PropertyDescriptor>().Select(pd => pd.PropertyType.IsGenericType ? new BasePropertyDescriptor(pd) : pd).ToArray());
        }

        private class BasePropertyDescriptor : PropertyDescriptor
        {
            PropertyDescriptor PropertyDescriptor;
            bool IsList => PropertyDescriptor.PropertyType.GetGenericTypeDefinition() == typeof(IList<>) || PropertyDescriptor.PropertyType.GetGenericTypeDefinition().GetInterface("IList") != null;
            public BasePropertyDescriptor(PropertyDescriptor propertyDescriptor) : base(propertyDescriptor.Name, propertyDescriptor.Attributes.Cast<Attribute>().ToArray())
            {
                PropertyDescriptor = propertyDescriptor;
            }

            public override Type ComponentType => PropertyDescriptor.ComponentType;

            public override bool IsReadOnly => PropertyDescriptor.IsReadOnly;

            public override Type PropertyType => PropertyDescriptor.PropertyType;

            public override bool CanResetValue(object component)
            {
                return PropertyDescriptor.CanResetValue(component);
            }

            public override object GetValue(object component)
            {
                return IsList ? Activator.CreateInstance(typeof(BindingListView<>).MakeGenericType(PropertyDescriptor.PropertyType.GetGenericArguments()[0]), PropertyDescriptor.GetValue(component)) : PropertyDescriptor.GetValue(component);
            }

            public override void ResetValue(object component)
            {
                PropertyDescriptor.ResetValue(component);
            }

            public override void SetValue(object component, object value)
            {
                PropertyDescriptor.SetValue(component, value);
            }

            public override bool ShouldSerializeValue(object component)
            {
                return PropertyDescriptor.ShouldSerializeValue(component);
            }
        }
    }
}
