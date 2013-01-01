using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using N2;
using N2.Details;
using N2.Persistence;
using N2.Persistence.NH;
using N2.Web;
using N2.Web.UI;
using N2Bootstrap.Blog.Library.Definitions;
using N2Bootstrap.Library;
using NHibernate;

namespace N2Bootstrap.Blog.Library.Models
{
    [PageDefinition("Blog", IconUrl = "/Bootstrap/Themes/Default/Content/Images/blog-icon.png")]
    [TabContainer("TagTab", "Tags", 2000)]
    [TabContainer("CategoryTab", "Categories", 3000)]
    [TabContainer("Blog", "Blog", 4000)]
    [BlogContainerIntegrity]
    [NotVersionable]
    public class BlogContainer : N2Bootstrap.Library.Models.ContentPage
    {
        #region Blog

        [EditableChildren("Comments Plugin", "CommentsPlugin", 0, ContainerName = "Blog", SortOrder = 200)]
        public IList<CommentsPlugin> CommentsPlugins
        {
            get
            {
                return GetChildren(Content.Is.All(Content.Is.InZone("CommentsPlugin"), Content.Is.Type<CommentsPlugin>()))
                    .Cast<CommentsPlugin>();
            }
        }

        [EditableNumber(Title = "Posts per page", DefaultValue = 5, MinimumValue = "1", MaximumValue = "50", InvalidRangeText = "Please specify a valid number of posts per page (1-50).", SortOrder = 201, ContainerName = "Blog")]
        public virtual int PostsPerPage { get; set; }

        [EditableEnum(Title = "List template", DefaultValue = ListTemplateEnum.List, EnumType = typeof(ListTemplateEnum), SortOrder = 202, ContainerName = "Blog")]
        public virtual ListTemplateEnum ListTemplate { get; set; }

        [EditableEnum(Title = "List image display type", SortOrder = 203, ContainerName = "Blog", DefaultValue = Defaults.ImageDisplayTypeEnum.Polaroid, EnumType = typeof(Defaults.ImageDisplayTypeEnum))]
        public virtual Defaults.ImageDisplayTypeEnum ListImageDisplayType { get; set; }

        #endregion

        #region Tags/Categories

        [EditableChildren("Tags", "Tag", 0, ContainerName = "TagTab", SortOrder = int.MaxValue)]
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

        [EditableChildren("Categories", "Category", 1, ContainerName = "CategoryTab", SortOrder = int.MaxValue)]
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

        #endregion

        #region Overrides

        /// <summary>
        /// This is overriden so that this page can be used to match:
        /// thisPage/category/categoryName
        /// Or
        /// thisPage/tag/tagName
        /// </summary>
        /// <param name="remainingUrl"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Use the "Blog" template
        /// </summary>
        public override string ViewTemplate
        {
            get { return "Blog"; }
        }

        #endregion

        #region BL

        public PagedList<Post> GetBlogPosts(int pageNumber, int pageSize, PostsSortEnum sort = PostsSortEnum.Newest, Tag tag = null, Category category = null)
        {
            var queryActions = new List<Action<IQuery>>();
            var session = Context.Current.Resolve<ISessionProvider>().SessionFactory.OpenSession();

            // base where query
            var where = "where VersionOf.ID Is Null and (  ci.class  =  :class And ci.Parent.ID  =  :parentId)";
            queryActions.Add(q =>
            {
                q.SetCacheable(true);
                q.SetParameter("class", "Post");
                q.SetParameter("parentId", ID);
            });

            // are we filtering by a tag?
            if (tag != null)
            {
                where += " and (ci in (select cd.EnclosingItem from ContentDetail cd where cd.LinkedItem in (:tagItem) AND cd.Name = :tagDetailName))";
                queryActions.Add(q =>
                {
                    q.SetParameter("tagItem", tag);
                    q.SetParameter("tagDetailName", "BlogTags");
                });
            }

            // are we filtering by a category?
            if (category != null)
            {
                where += " and (ci in (select cd.EnclosingItem from ContentDetail cd where cd.LinkedItem in (:categoryItem) AND cd.Name = :categoryDetailName))";
                queryActions.Add(q =>
                {
                    q.SetParameter("categoryItem", category);
                    q.SetParameter("categoryDetailName", "BlogCategories");
                });
            }

            // get the posts paged
            var query = session.CreateQuery("select ci from ContentItem ci " + where + " order by (select MAX(DateTimeValue) from ContentDetail cd where cd.EnclosingItem = ci and cd.Name = 'postCreatedDate') desc");
            query.SetFirstResult((pageNumber - 1) * pageSize);
            query.SetMaxResults(pageSize);
            Array.ForEach(queryActions.ToArray(), a => a(query));
            var posts = query.List<Post>();

            // get the total number of posts
            query = session.CreateQuery("select count(*) from ContentItem ci " + where);
            Array.ForEach(queryActions.ToArray(), a => a(query));
            var total = Convert.ToInt32(query.List()[0]);

            // return the paged list
            var pagedList = new PagedList<Post>();
            pagedList.AddRange(posts);
            pagedList.PageNumber = pageNumber;
            pagedList.PageSize = pageSize;
            pagedList.TotalCount = total;
            pagedList.TotalPages = (int)Math.Ceiling((decimal)total / pageSize);
            return pagedList;
        }

        public Category GetSelectedCategory()
        {
            if (System.Web.HttpContext.Current.Items["blogSelectedCategory"] != null)
                return System.Web.HttpContext.Current.Items["blogSelectedCategory"] as Category;

            // current path info
            var pathData = N2.Context.Current.RequestContext.CurrentPath;
            Category category = null;
            bool redirect = false;

            if (pathData.QueryParameters.ContainsKey("category"))
            {
                category = Categories.SingleOrDefault(x => x.Name.Equals(pathData.QueryParameters["category"], StringComparison.InvariantCultureIgnoreCase));

                if (category == null)
                {
                    redirect = true;
                }
            }

            if (redirect)
            {
                System.Web.HttpContext.Current.Response.Redirect(Url);
            }

            System.Web.HttpContext.Current.Items["blogSelectedCategory"] = category;

            return category;
        }

        public Tag GetSelectedTag()
        {
            if (System.Web.HttpContext.Current.Items["blogSelectedTag"] != null)
                return System.Web.HttpContext.Current.Items["blogSelectedTag"] as Tag;

            // current path info
            var pathData = N2.Context.Current.RequestContext.CurrentPath;
            Tag tag = null;
            bool redirect = false;

            if (pathData.QueryParameters.ContainsKey("tag"))
            {
                tag = Tags.SingleOrDefault(x => x.Name.Equals(pathData.QueryParameters["tag"], StringComparison.InvariantCultureIgnoreCase));

                if (tag == null)
                {
                    redirect = true;
                }
            }

            if (redirect)
            {
                System.Web.HttpContext.Current.Response.Redirect(Url);
            }

            System.Web.HttpContext.Current.Items["blogSelectedTag"] = tag;

            return tag;
        }

        #endregion

        #region Nested Types

        public enum PostsSortEnum
        {
            Newest
        }

        public enum ListTemplateEnum
        {
            List,
            Grid
        }

        #endregion
    }
}