using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing.Design;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows.Forms;

namespace SearchControls.Design
{
    /// <summary>
    /// Collection的设计器
    /// </summary>
    public class HelpCollectionEditor : CollectionEditor
    {
        /// <summary>
        /// Collection的设计器
        /// </summary>
        public HelpCollectionEditor(Type type) : base(type)
        { 
        }

        /// <summary>
        /// 获取此集合编辑器可包含的类型
        /// </summary>
        /// <returns>此集合可包含的数据类型数组</returns>
        protected override Type[] CreateNewItemTypes()
        {
            if (CollectionType.Equals(typeof(List<DataGridViewColumn>)))
            {
                return new Type[]
                {
                    typeof(DataGridViewTextBoxColumn),
                    typeof(DataGridViewImageColumn)
                };
            }
            else return base.CreateNewItemTypes();
        }

        /// <summary>
        /// 指示是否可一次选择多个集合项。
        /// </summary>
        /// <returns>如果可以同时选择多个集合成员，则为 true；否则，为 false。默认情况下，它将返回 true。</returns>
        protected override bool CanSelectMultipleInstances()
        {
            if (CollectionType.Equals(typeof(List<DataGridViewColumn>))) return true;
            else return base.CanSelectMultipleInstances();
        }

        /// <summary>
        /// 创建新的窗体，以显示和编辑当前集合。
        /// </summary>
        /// <returns>作为用于编辑集合的用户界面提供的 System.ComponentModel.Design.CollectionEditor.CollectionForm。</returns>
        protected override CollectionForm CreateCollectionForm()
        {
            CollectionForm CollectionForm = base.CreateCollectionForm();
            FieldInfo fileinfo = CollectionForm.GetType().GetField("propertyBrowser", BindingFlags.NonPublic | BindingFlags.Instance);
            (fileinfo?.GetValue(CollectionForm) as PropertyGrid).HelpVisible = true;
            return CollectionForm;
        }
    }
}
