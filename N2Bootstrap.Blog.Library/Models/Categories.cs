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
    public class Categories : SubNavigation
    {
        [EditableLink(Title = "Blog")]
        public override ContentItem StartFrom
        {
            get { return base.GetDetail<ContentItem>("StartFrom", null); }
            set { base.SetDetail("StartFrom", value, null); }
        }
    }
}
