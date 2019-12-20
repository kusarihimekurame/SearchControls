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
    internal class HelpCollectionEditor : CollectionEditor
    {
        public HelpCollectionEditor(Type type) : base(type)
        { 
        }

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

        protected override bool CanSelectMultipleInstances()
        {
            if (CollectionType.Equals(typeof(List<DataGridViewColumn>))) return true;
            else return base.CanSelectMultipleInstances();
        }

        protected override CollectionForm CreateCollectionForm()
        {
            CollectionForm CollectionForm = base.CreateCollectionForm();
            FieldInfo fileinfo = CollectionForm.GetType().GetField("propertyBrowser", BindingFlags.NonPublic | BindingFlags.Instance);
            (fileinfo?.GetValue(CollectionForm) as PropertyGrid).HelpVisible = true;
            return CollectionForm;
        }
    }
}
