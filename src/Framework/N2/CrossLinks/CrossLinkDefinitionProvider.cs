using System;
using System.Collections.Generic;
using System.Linq;
using N2.Definitions;
using N2.Definitions.Static;
using N2.Engine;

namespace N2.CrossLinks
{
    [Service(typeof(IDefinitionProvider))]
    public class CrossLinkDefinitionProvider : IDefinitionProvider
    {
        private readonly CrossLinkDefinitionBuilder definitionBuilder;

        public CrossLinkDefinitionProvider(CrossLinkDefinitionBuilder definitionBuilder)
        {
            this.definitionBuilder = definitionBuilder;
        }

        public IEnumerable<ItemDefinition> GetDefinitions()
        {
            return definitionBuilder.GetDefinitions();
        }

    }
}