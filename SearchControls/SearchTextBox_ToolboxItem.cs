using System;
using System.Collections.Generic;
using System.Drawing.Design;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SearchControls
{
    [Serializable()]
    internal class SearchTextBox_ToolboxItem : ToolboxItem
    {
        public SearchTextBox_ToolboxItem(Type toolType) : base(toolType)
        {
        }

        public override void Initialize(Type toolType)
        {
            if (!toolType.Equals(typeof(SearchTextBox)))
            {
                throw new ArgumentException(
                    string.Format(CultureInfo.CurrentCulture,
                        "The {0} constructor argument must be of type {1}.",
                        GetType().FullName, typeof(SearchTextBox).FullName));
            }

            base.Initialize(toolType);
        }
    }
}
