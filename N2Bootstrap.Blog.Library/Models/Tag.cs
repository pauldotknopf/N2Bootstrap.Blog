using System.Collections.Generic;
using System.Linq;
using N2;
using N2.Collections;
using N2.Details;
using N2.Integrity;
using N2.Persistence.Finder;
using N2.Engine;
using N2.Web;
using N2Bootstrap.Library.Details;

namespace N2Bootstrap.Blog.Library.Models
{
    [PartDefinition("Tag")]
    [RestrictParents(typeof(BlogContainer))]
    [AllowedZones("Tag")]
    [WithEditableTitle, WithEditableName]
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
                            Content.Is.Published(),
                            Content.Is.Visible()))
                        .AsQueryable();

                    return (object)posts.Count();
                });
            }
        }

        public override string Url
        {
            get { return new N2.Web.Url(Parent.Url).AppendSegment("tag/" + Name); }
        }

        public List<ContentItem> GetContentItems(ContentItem curent, System.Type linkedType, System.Type excludedType, int searchThreshold, EditableItemSelectionFilter filtler)
        {
            return Context.Current.Resolve<IItemFinder>()
                       .Where.Type.Eq(typeof(Tag)).And.Parent.Eq(curent.Parent).Select().ToList();
        }
    }
}