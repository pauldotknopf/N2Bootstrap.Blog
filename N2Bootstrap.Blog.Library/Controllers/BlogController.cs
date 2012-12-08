using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using N2;
using N2.Web;
using N2Bootstrap.Blog.Library.Models;

namespace N2Bootstrap.Blog.Library.Controllers
{
    [Controls(typeof(BlogContainer))]
    [Controls(typeof(Category))]
    [Controls(typeof(Post))]
    [Controls(typeof(Tag))]
    public class BlogController : N2Bootstrap.Library.Controllers.TemplatesControllerBase<ContentItem>
    {
    }
}
