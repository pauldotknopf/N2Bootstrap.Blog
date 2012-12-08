using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using N2;
using N2.Details;
using N2.Integrity;
using N2.Web;
using N2.Web.UI;
using N2.Web.UI.WebControls;
using N2Bootstrap.Library.Details;

namespace N2Bootstrap.Blog.Library.Models
{
    [PageDefinition("Post")]
    [RestrictParents(typeof(BlogContainer))]
    [TabContainer("Blog", "Blog", int.MaxValue)]
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

        #region Gravatar

        public string GetGravitarUrl(string email, int size, string rating)
        {
            email = email.ToLower();
            email = GetMd5Hash(email);

            if (size < 1 | size > 600)
            {
                throw new ArgumentOutOfRangeException("size",
                    "The image size should be between 20 and 80");
            }

            rating = rating.ToLower();
            if (rating != "g" & rating != "pg" & rating != "r" & rating != "x")
            {
                throw new ArgumentOutOfRangeException("rating",
                    "The rating can only be one of the following: g, pg, r, x");
            }

            return "http://www.gravatar.com/avatar.php?gravatar_id=" + email + "&s=" + size.ToString() + "&r=" + rating;
        }

        private string GetMd5Hash(string input)
        {
            // Create a new instance of the MD5CryptoServiceProvider object.  
            var md5Hasher = MD5.Create();

            // Convert the input string to a byte array and compute the hash.  
            var data = md5Hasher.ComputeHash(Encoding.Default.GetBytes(input));

            // Create a new Stringbuilder to collect the bytes  
            // and create a string.  
            var sBuilder = new StringBuilder();

            // Loop through each byte of the hashed data  
            // and format each one as a hexadecimal string.  
            for (var i = 0; i < data.Length; i++)
            {
                sBuilder.Append(data[i].ToString("x2"));
            }

            return sBuilder.ToString();  // Return the hexadecimal string.  
        }

        #endregion

        #region ImageResizing

        public string ImageRecentThumbnail()
        {
            return GetResizedImage(75, 45);
        }

        public string ImageThumbnail()
        {
            return GetResizedImage(75, 75);
        }

        public string ImageFull()
        {
            return GetResizedImage(924, 275);
        }

        public string ImageFullWithSidebar()
        {
            return GetResizedImage(604, 180);
        }

        public string GetResizedImage(int width, int height)
        {
            return string.IsNullOrEmpty(Image)
                       ? (string)string.Empty
                       : new Url(Image).AppendQuery("width", width).AppendQuery("height", height).AppendQuery("mode", "crop").AppendQuery("scale", "both").ToString();
        }

        #endregion
    }
}