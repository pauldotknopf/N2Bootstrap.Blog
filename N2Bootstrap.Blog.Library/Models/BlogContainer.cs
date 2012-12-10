using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using N2;
using N2.Collections;
using N2.Details;
using N2.Integrity;
using N2.Persistence;
using N2.Web.UI;
using N2Bootstrap.Blog.Library.Definitions;

namespace N2Bootstrap.Blog.Library.Models
{
    [PageDefinition("Blog", IconUrl="/Bootstrap/Themes/Default/Content/Images/blog-icon.png")]
    [TabContainer("TagTab", "Tags", int.MaxValue)]
    [TabContainer("CategoryTab", "Categories", int.MaxValue)]
    [BlogContainerIntegrity]
    [NotVersionable]
    public class BlogContainer : N2Bootstrap.Library.Models.ContentPage
    {
        public override string ViewTemplate
        {
            get { return "Blog"; }
        }

        public List<Post> BlogPosts
        {
            get
            {
                return GetChildren(new TypeFilter(typeof(Post)), new PublishedFilter(), new AncestorFilter(this))
                    .Cast<Post>()
                    .OrderByDescending(x => x.PostCreatedDate)
                    .ToList();
            }
        }

        public List<Post> GetBlogPosts(Tag tag = null, Category category = null)
        {
            var query = new ItemList<Post>(BlogPosts, N2.Content.Is.All(
                N2.Content.Is.AccessiblePage(),
                N2.Content.Is.Published()))
                .AsQueryable();

            if (tag != null)
                query = query.Where(x => x.BlogTags.Any(y => y.ID == tag.ID));

            if (category != null)
                query = query.Where(x => x.BlogCategories.Any(y => y.ID == category.ID));

            return query.ToList();
        }

        [EditableChildren("Tags", "Tag", 0, ContainerName = "TagTab", SortOrder=int.MaxValue)]
        public IList<Tag> Tags
        {
            get
            {
                return GetChildren(N2.Content.Is.All(
                        N2.Content.Is.Accessible(), 
                        N2.Content.Is.Published(),
                        N2.Content.Is.Visible(), 
                        N2.Content.Is.Type<Tag>(),
                        N2.Content.Is.InZone("Tag")))
                    .Cast<Tag>();
            }
        }

        [EditableChildren("Categories", "Category", 1, ContainerName = "CategoryTab", SortOrder=int.MaxValue)]
        public IList<Category> Categories
        {
            get
            {
                return GetChildren(N2.Content.Is.All(
                        N2.Content.Is.Accessible(),
                        N2.Content.Is.Published(),
                        N2.Content.Is.Visible(),
                        N2.Content.Is.Type<Category>(),
                        N2.Content.Is.InZone("Category")))
                    .Cast<Category>();
            }
        }

        public override N2.Web.PathData FindPath(string remainingUrl)
        {
            const string pattern = @"^\/(category|tag)/([^/]+)/?$";

            if (!string.IsNullOrEmpty(remainingUrl) && Regex.IsMatch(remainingUrl, pattern))
            {
                var match = Regex.Match(remainingUrl, pattern);
                var key = match.Groups[1].Value;
                var value = match.Groups[2].Value;

                var pathData = base.FindPath(null);
                pathData.QueryParameters.Add(new KeyValuePair<string, string>(key, value));

                return pathData;
            }

            return base.FindPath(remainingUrl);
        }
    }
}