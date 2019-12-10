using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing.Design;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows.Forms;

namespace SearchControls
{
    internal class SubSearchTextBoxCollectionEditor : CollectionEditor
    {
        public SubSearchTextBoxCollectionEditor(Type type) : base(type)
        { 
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
