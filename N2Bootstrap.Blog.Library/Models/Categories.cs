using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using N2;
using N2.Details;
using N2Bootstrap.Library.Models;

namespace N2Bootstrap.Blog.Library.Models
{
    [PartDefinition("Categories", IconUrl = "{IconsUrl}/tag_blue.png")]
    [WithEditableTitle(Required = false)]
    public class Categories : SubNavigation
    {
        /// <summary>
        /// Override to remove from edit and make false.
        /// </summary>
        public override bool AllowDropDown { get { return false; } }

        [EditableLink(Title = "Blog")]
        public override ContentItem StartFrom
        {
            get { return base.GetDetail<ContentItem>("StartFrom", null); }
            set { base.SetDetail("StartFrom", value, null); }
        }

        /// <summary>
        /// Get the bog container to render categories from
        /// </summary>
        /// <returns></returns>
        public BlogContainer GetBlogContainer()
        {
            if (StartFrom != null)
            {
                if (StartFrom is BlogContainer)
                    return StartFrom as BlogContainer;

                return null;
            }

            return Find.Closest<BlogContainer>(this);
        }
    }
}
