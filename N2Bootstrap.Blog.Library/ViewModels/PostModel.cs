using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using N2Bootstrap.Blog.Library.Models;

namespace N2Bootstrap.Blog.Library.ViewModels
{
    public class PostModel
    {
        /// <summary>
        /// Is the post being displayed in a list of on its own page?
        /// </summary>
        public bool IsList { get; set; }

        /// <summary>
        /// The post
        /// </summary>
        public Post Post { get; set; }
    }
}
