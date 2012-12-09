using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using N2;
using N2.Details;
using N2.Integrity;
using N2.Persistence;
using N2.Web;
using N2.Web.UI;
using N2.Web.UI.WebControls;
using N2Bootstrap.Library.Details;

namespace N2Bootstrap.Blog.Library.Models
{
    [PageDefinition("Post", IconUrl = "{IconsUrl}/page_edit.png")]
    [RestrictParents(typeof(BlogContainer))]
    [TabContainer("Blog", "Blog", int.MaxValue)]
    [NotVersionable]
    public class Post : N2Bootstrap.Library.Models.ContentPage
    {
        [EditableImage(ContainerName = "Blog")]
        public virtual string Image { get; set; }

        [EditableMultipleItemProviderSelection(typeof(Category), ContainerName = "Blog")]
        public List<Category> BlogCategories
        {
            get
            {
                return EditableMultipleItemProviderSelectionAttribute.GetStoredItems<Category>("BlogCategories", this);
            }
            set
            {
                EditableMultipleItemProviderSelectionAttribute.ReplaceStoredValue("BlogCategories", this, value);
            }
        }

        [EditableMultipleItemProviderSelection(typeof(Tag), ContainerName = "Blog")]
        public List<Tag> BlogTags
        {
            get
            {
                return EditableMultipleItemProviderSelectionAttribute.GetStoredItems<Tag>("BlogTags", this);
            }
            set
            {
                EditableMultipleItemProviderSelectionAttribute.ReplaceStoredValue("BlogTags", this, value);
            }
        }

        [EditableDate(Title = "Created date", ContainerName = "Blog")]
        public DateTime PostCreatedDate
        {
            get { return GetDetail("postCreatedDate", DateTime.Now); }
            set { SetDetail("postCreatedDate", value); }
        }

        /// <summary>
        /// The following is overriden to do some foolery so that we can get more semantic markup for posts.
        /// We want to allow editing of "show page title" for the post, but return false in display mode always
        /// so that the laout will never render the page title. This is because I wan't the post view itself
        /// responsible for creating the page title in the semantically correct position.
        /// </summary>
        [EditableCheckBox(CheckBoxText = "Show page title", ContainerName = N2Bootstrap.Library.Defaults.Containers.Layout, DefaultValue = true, Title = "")]
        public override bool ShowPageTitle
        {
            get
            {
                var isEditing = new N2.Web.Url(System.Web.HttpContext.Current.Request.RawUrl).Path.StartsWith(N2.Web.Url.ResolveTokens("{ManagementUrl}"));
                if (isEditing)
                {
                    // allow the editor to get (and later save) the real value "show page title".
                    return GetDetail("ShowPageTitle", true);
                }
                // otherwise, return false so that the layout will NOT render the page title giving responsibility to the post view itself.
                return false;
            }
            set { SetDetail("ShowPageTitle", value, true); }
        }

        public override bool Visible
        {
            get
            {
                return false;
            }
        }
    }
}