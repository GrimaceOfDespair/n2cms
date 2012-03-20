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
            var types = new List<Type>();
            foreach (var assembly in GetAssemblies())
            {
                try
                {
                    types.AddRange(assembly.GetTypes()
                        .Where(requestedType.IsAssignableFrom));
                }
                catch (ReflectionTypeLoadException ex)
                {
                    var loaderErrors = string.Empty;
                    foreach (var loaderEx in ex.LoaderExceptions)
                    {
                        Trace.TraceError(loaderEx.ToString());
                        loaderErrors += ", " + loaderEx.Message;
                    }

                    throw new N2Exception("Error getting types from assembly " + assembly.FullName + loaderErrors, ex);
                }
            }

            return types;
        }

        public IList<Assembly> GetAssemblies()
        {
            return new[] {typeMap.Assembly};
        }
    }
}