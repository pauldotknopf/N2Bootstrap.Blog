using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using N2.Details;
using N2Bootstrap.Blog.Library.Definitions;

namespace N2Bootstrap.Blog.Library.Models
{
    [CommentsPluginDefinition(Title = "Livefyre")]
    public class LivefyreCommentsPlugin : CommentsPlugin
    {
        [EditableTextBox(Title = "Network", DefaultValue="livefyre.com")]
        public virtual string Network { get; set; }

        [EditableTextBox(Title="Site id")]
        public virtual string SiteId{get;set;}
    }
}
