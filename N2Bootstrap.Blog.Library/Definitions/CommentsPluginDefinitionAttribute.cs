using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using N2;
using N2.Definitions;

namespace N2Bootstrap.Blog.Library.Definitions
{
    public class CommentsPluginDefinitionAttribute : AbstractDefinition, IAllowedDefinitionFilter
    {
        public override void Refine(ItemDefinition currentDefinition)
        {
            currentDefinition.Enabled = true;
            base.Refine(currentDefinition);
        }

        public AllowedDefinitionResult IsAllowed(AllowedDefinitionQuery query)
        {
            return AllowedDefinitionResult.Allow;
        }
    }
}
