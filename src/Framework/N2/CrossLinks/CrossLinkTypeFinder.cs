using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using N2.Definitions;
using N2.Engine;

namespace N2.CrossLinks
{
    [Service]
    public class CrossLinkTypeFinder : ITypeFinder
    {
        private readonly AttributeExplorer _explorer;
        private readonly ITypeFinder _typeFinder;

        public CrossLinkTypeFinder(AttributeExplorer explorer, ITypeFinder typeFinder)
        {
            _explorer = explorer;
            _typeFinder = typeFinder;
        }

        public IList<Type> Find(Type requestedType)
        {
            var linkTypes = from type in _typeFinder.Find(requestedType)
                        from editableCrossLink
                            in _explorer.Find<EditableCrossLinksAttribute>(type)
                            select editableCrossLink.LinkedType.GetCrossLinkType();

            return linkTypes.ToList();
        }

        public IList<Assembly> GetAssemblies()
        {
            return _typeFinder.GetAssemblies();
        }
    }
}