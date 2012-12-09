using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using N2.Definitions;
using N2Bootstrap.Blog.Library.Models;

namespace N2Bootstrap.Blog.Library.Definitions
{
    public class BlogContainerIntegrityAttribute : Attribute, IDefinitionRefiner, IAllowedDefinitionFilter
    {
        public N2.Definitions.AllowedDefinitionResult IsAllowed(N2.Definitions.AllowedDefinitionQuery query)
        {
            if (query.ChildDefinition.ItemType.IsAssignableFrom(typeof (BlogContainer)))
            {
                return AllowedDefinitionResult.Deny;
            }
            return AllowedDefinitionResult.DontCare;
        }

        public void Refine(ItemDefinition currentDefinition, IList<ItemDefinition> allDefinitions)
        {
            if (currentDefinition.ItemType.IsAssignableFrom(typeof (BlogContainer)))
            {
                currentDefinition.AllowedChildFilters.Add(this);
            }
        }

        public int RefinementOrder
        {
            get { return int.MaxValue; }
        }

        public int CompareTo(ISortableRefiner other)
        {
            return RefinementOrder - other.RefinementOrder;
        }
    }
}
