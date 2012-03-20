using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using N2.CrossLinks;

namespace N2.Collections
{
    public class CrossLinkFilter<T> : TypeFilter where T : ContentItem
    {
        public CrossLinkFilter() : base(typeof(T).GetCrossLinkType())
        {
        }
    }
}
