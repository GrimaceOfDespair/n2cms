using System.Linq;
using System.Text;
using N2.Configuration;
using N2.Definitions;
using N2.Definitions.Static;
using N2.Engine;
using N2.Plugin;

namespace N2.CrossLinks
{
    [Service]
    public class CrossLinkDefinitionBuilder : DefinitionBuilder
    {
        public CrossLinkDefinitionBuilder(DefinitionMap staticDefinitions, CrossLinkTypeFinder typeFinder, TransformerBase<IUniquelyNamed>[] transformers, EngineSection config)
            : base(staticDefinitions, typeFinder, transformers, config)
        {
        }
    }
}
