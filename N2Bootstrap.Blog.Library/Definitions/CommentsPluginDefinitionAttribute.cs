using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using N2;
using N2.Definitions;
using N2.Web.UI.WebControls;
using N2Bootstrap.Blog.Library.Models;

namespace N2Bootstrap.Blog.Library.Definitions
{
    public class CommentsPluginDefinitionAttribute : AbstractDefinition, IAllowedDefinitionFilter
    {
        public override void Refine(ItemDefinition currentDefinition)
        {
            currentDefinition.Enabled = true;
            currentDefinition.IsPage = false;
            currentDefinition.AllowedIn = N2.Integrity.AllowedZones.AllNamed;
            currentDefinition.AllowedZoneNames.Add("CommentsPlugin");
            currentDefinition.AllowedParentFilters.Add(this);
            base.Refine(currentDefinition);
        }

        public AllowedDefinitionResult IsAllowed(AllowedDefinitionQuery query)
        {
            var isDragAndDrop = N2.Web.UI.WebControls.ControlPanel.GetState(N2.Context.Current).HasFlag(ControlPanelState.DragDrop);

            // dont show the plugin when drag and dropping (only for blogcontainer editing).
            if (isDragAndDrop)
                return AllowedDefinitionResult.Deny;

            // only allowed below blog container
            return !typeof(BlogContainer).IsAssignableFrom(query.ParentDefinition.ItemType) 
                ? AllowedDefinitionResult.Deny 
                : AllowedDefinitionResult.Allow;
        }
    }
}
