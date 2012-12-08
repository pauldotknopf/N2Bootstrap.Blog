using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using N2;
using N2.Web;

namespace N2Bootstrap.Blog.Library
{
    public static class HtmlHelper
    {
        public static IHtmlString ContentItemLinkList<TModel, TContentItem>(this HtmlHelper<TModel> helper,
                                                                            IEnumerable<TContentItem> items,
                                                                            Action<ILinkBuilder> linkModifier = null) 
                                                                            where TContentItem : ContentItem
        {
            var html = string.Join(", ", items.Select(x =>
            {
                var link = Link.To(x);
                if (linkModifier != null)
                    linkModifier(link);
                return link.ToString();
            }));
            return MvcHtmlString.Create(html);
        }
    }
}
