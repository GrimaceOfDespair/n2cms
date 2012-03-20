using System;
using System.Collections.Generic;
using System.Linq;
using N2.Definitions;
using N2.Engine;

namespace N2.CrossLinks
{
    [Service(StaticAccessor = "Instance")]
    public class CrossLinkTypesRepository : DynamicTypeMapRepository
    {
        static CrossLinkTypesRepository()
        {
            Instance = new CrossLinkTypesRepository();
        }

        public static CrossLinkTypesRepository Instance
        {
            get { return Singleton<CrossLinkTypesRepository>.Instance; }
            protected set { Singleton<CrossLinkTypesRepository>.Instance = value; }
        }

        public override IEnumerable<MappedType> MapTypes(IEnumerable<Type> types)
        {
            return from type in types
                   from editableCrossLink
                       in Context.Current.Resolve<AttributeExplorer>().Find<EditableCrossLinksAttribute>(type)
                   select new MappedType
                              {
                                  LinkedType = editableCrossLink.LinkedType,
                                  LinkType = typeof(CrossLink<>).MakeGenericType(editableCrossLink.LinkedType)
                              };
        }
    }
}