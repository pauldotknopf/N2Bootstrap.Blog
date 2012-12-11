using System.Collections.Generic;
using System.Linq;
using N2;
using N2.Collections;
using N2.Definitions;
using N2.Details;
using N2.Integrity;
using N2.Persistence.Finder;
using N2.Engine;
using N2.Web;
using N2Bootstrap.Library.Details;

namespace N2Bootstrap.Blog.Library.Models
{
    [PartDefinition("Tag", IconUrl = "{IconsUrl}/tag_red.png")]
    [RestrictParents(typeof(BlogContainer))]
    [AllowedZones("Tag")]
    [WithEditableTitle, WithEditableName]
    [Throwable(AllowInTrash.No)]
    public class Tag : ContentItem, EditableMultipleItemProviderSelectionAttribute.IItemProvider
    {
        public virtual int NumberOfPosts
        {
            get
            {
                var key = string.Format("tag-{0}-numberofposts", ID);
                return (int)Context.Current.Resolve<CacheWrapper>().GetOrCreate<object>(key, () =>
                {
                    var postTypes = Context.Current.Container.Resolve<ITypeFinder>().Find(typeof (Post));

                    var posts = new ItemList<Post>(
                        Find.Items.Where.Type.In(postTypes.ToArray()).And.Detail("BlogTags").In(this).Select<Post>(),
                        Content.Is.All(
                            Content.Is.AccessiblePage(),
                            Content.Is.Published()))
                        .AsQueryable();

                    return (object)posts.Count();
                });
            }
        }

        public override string Url
        {
            get
            {
                // if we are on a category page, we want to render the parent url so that you can "unclick" a category.
                var blog = Parent as BlogContainer;
                if (blog != null)
                {
                    var selected = blog.GetSelectedTag();
                    if (selected != null && selected.ID.Equals(ID))
                        return Parent.Url;
                }

                // otherwise, render the url to the tag
                return new Url(Parent.Url).AppendSegment("tag/" + Name);
            }
        }

        public List<ContentItem> GetContentItems(ContentItem curent, System.Type linkedType, System.Type excludedType, int searchThreshold, EditableItemSelectionFilter filtler)
        {
            var blog = curent.Parent as BlogContainer;
            return blog == null ? new List<ContentItem>() : blog.Tags.Cast<ContentItem>().ToList();
        }
    }
}