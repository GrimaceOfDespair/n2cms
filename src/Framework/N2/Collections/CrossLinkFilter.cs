using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace N2.Collections
{
    public class CrossLinkFilter<T> : TypeFilter where T : ContentItem
    {
        public CrossLinkFilter(params Type[] allowedTypes) : base(allowedTypes)
        {
        }

        public CrossLinkFilter(IEnumerable<Type> allowedTypes) : base(allowedTypes)
        {
        }

        public CrossLinkFilter(bool inverse, params Type[] allowedTypes) : base(inverse, allowedTypes)
        {
        }
    }
}
