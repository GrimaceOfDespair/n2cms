using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using N2.Definitions;
using N2.Engine;

namespace N2.CrossLinks
{
    [Service]
    public class CrossLinkTypeFinder : ITypeFinder
    {
        private readonly CrossLinkTypesRepository typeMap;

        public CrossLinkTypeFinder(CrossLinkTypesRepository typeMap)
        {
            this.typeMap = typeMap;
        }

        public IList<Type> Find(Type requestedType)
        {
            return typeMap.TypeMap.Values
                .Where(requestedType.IsAssignableFrom)
                .ToList();
        }

        public IList<Assembly> GetAssemblies()
        {
            return new[] {typeMap.Assembly};
        }
    }
}