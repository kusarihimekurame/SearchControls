using System;
using System.Collections.Generic;
using System.Text;

namespace SearchControls
{
    /// <include file='Include_Tag.xml' path='Tab/Members/Member[@Name="GridSelectingEventHandler"]/*'/>
    public delegate void GridSelectingEventHandler(object sender, GridSelectingEventArgs e);
    /// <include file='Include_Tag.xml' path='Tab/Members/Member[@Name="GridSelectedEventHandler"]/*'/>
    public delegate void GridSelectedEventHandler(object sender, GridSelectedEventArgs e);
    /// <include file='Include_Tag.xml' path='Tab/Members/Member[@Name="SearchGridLocationSizeChangedEventHandler"]/*'/>
    public delegate void SearchGridLocationSizeChangedEventHandler(object sender, SearchFormLocationSizeEventArgs e);
}
