using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using N2.Details;
using N2Bootstrap.Blog.Library.Definitions;

namespace N2Bootstrap.Blog.Library.Models
{
    [CommentsPluginDefinition(Title = "Disqus")]
    public class DisqusCommentsPlugin : CommentsPlugin
    {
        [EditableTextBox(Title = "Disqus shortname")]
        public virtual string DisqusShortname { get; set; }
    }
}
