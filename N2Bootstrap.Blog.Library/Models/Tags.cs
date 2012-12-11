using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using N2;
using N2.Details;
using N2Bootstrap.Library.Models;

namespace N2Bootstrap.Blog.Library.Models
{
    [PartDefinition("Tags", IconUrl = "{IconsUrl}/tag_red.png")]
    [WithEditableTitle(Required = false)]
    public class Tags : PartModelBase
    {
        [EditableLink(Title = "Blog")]
        public virtual ContentItem Blog { get; set; }

        /// <summary>
        /// Get the bog container to render categories from
        /// </summary>
        /// <returns></returns>
        public BlogContainer GetBlogContainer()
        {
            if (Blog != null)
            {
                if (Blog is BlogContainer)
                    return Blog as BlogContainer;

                return null;
            }

            return Find.Closest<BlogContainer>(this);
        }
    }
}
