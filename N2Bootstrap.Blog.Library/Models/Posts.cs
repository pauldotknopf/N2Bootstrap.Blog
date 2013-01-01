using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using N2;
using N2.Details;

namespace N2Bootstrap.Blog.Library.Models
{
    [PartDefinition("Posts")]
    public class Posts : N2Bootstrap.Library.Models.SidebarPart
    {
        [EditableNumber(Title = "Number of posts", DefaultValue = 3, MinimumValue = "1", MaximumValue = "10", InvalidRangeText = "Please specify a valid number of posts to display (1-10).", SortOrder = 200)]
        public virtual int NumberOfPosts { get; set; }

        [EditableEnum(Title="Sort", DefaultValue=BlogContainer.PostsSortEnum.Newest, EnumType=typeof(BlogContainer.PostsSortEnum), Required=true, SortOrder=201)]
        public virtual BlogContainer.PostsSortEnum Sort { get; set; }

        [EditableLink(Title = "Blog", SortOrder = 202)]
        public virtual ContentItem Blog { get; set; }

        [EditableCheckBox(Title = "Show image", CheckBoxText = "", SortOrder = 204)]
        public virtual bool ShowImage { get; set; }

        [EditableNumber(Title = "Image size", DefaultValue = 46, Required = true, MinimumValue = "16", MaximumValue = "96", InvalidRangeText = "Please specify a valid image size (16-96).", SortOrder = 205)]
        public virtual int ImageSize { get; set; }

        [EditableNumber(Title = "Maximum characters", DefaultValue = 140, Required = true, MinimumValue = "20", MaximumValue = "500", InvalidRangeText = "Please specify a valid number of characters (20-500).", SortOrder = 206)]
        public virtual int MaxCharacters { get; set; }

        /// <summary>
        /// Get the bog container to render posts from
        /// </summary>
        /// <returns></returns>
        public BlogContainer GetBlogContainer()
        {
            if (Blog != null)
            {
                if (Blog is BlogContainer)
                    return Blog as BlogContainer;

                return null;
            }

            return Find.Closest<BlogContainer>(this);
        }
    }
}
