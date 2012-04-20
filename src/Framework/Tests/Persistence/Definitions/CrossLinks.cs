using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using N2.CrossLinks;
using N2.Details;

namespace N2.Tests.Persistence.Definitions
{
    [PageDefinition]
    public class ParentItem : ContentItem
    {
        [EditableCrossLinks(typeof(ChildItem), "ChildItems")]
        public virtual IList<ChildItem> ChildItems
        {
            get { return this.GetCrossLinks<ChildItem>("ChildItems"); }
        }
    }

    [PageDefinition]
    public class ChildItem : ContentItem
    {
    }

}
