using System.Collections.Generic;
using N2.Definitions;
using N2.Engine;

namespace N2.CrossLinks
{
    [Service(typeof(IDefinitionProvider))]
    class CrossLinkDefinitionProvider : IDefinitionProvider
    {
        CrossLinkDefinitionBuilder definitionBuilder;

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