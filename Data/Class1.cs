using AngleSharp.Html.Dom;
using AngleSharp.Html.Parser;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;

namespace Data
{
    public class Post
    {
        public string Title { get; set; }
        public DateTime? Date { get; set; }
        public string Url { get; set; }
        public string ImageUrl { get; set; }
    }

    public static class Api
    {
        public static IEnumerable<Post> ScrapeLS()
        {
            string html = GetLSHtml();
            return GetPosts(html);
        }

        public static string GetLSHtml()
        {
            var handler = new HttpClientHandler();
            handler.AutomaticDecompression = System.Net.DecompressionMethods.GZip | DecompressionMethods.Deflate;
            using(var client=new HttpClient(handler))
            {
                client.DefaultRequestHeaders.Add("user-agent", "header");
                var url =$"https://www.thelakewoodscoop.com/";
                var html = client.GetStringAsync(url).Result;
                return html;
            }
        }

        private static IEnumerable<Post> GetPosts(string html)
        {
            var parser = new HtmlParser();
            IHtmlDocument document = parser.ParseDocument(html);
            var itemDivs = document.QuerySelectorAll(".post");
            List<Post> posts = new List<Post>();
            foreach (var div in itemDivs)
            {
                Post post = new Post();
                var tag = div.QuerySelector("a");
                post.Title = tag.TextContent;
                post.Url = tag.Attributes["href"].Value;

                var image = div.QuerySelector("img.aligncenter");
                if (image!=null)
                {
                    post.ImageUrl = image.Attributes["src"].Value;
                }
                
                var date = div.QuerySelector("div.postmetadata-top small");
                if (date != null && date.TextContent.Trim() != "")
                {
                    post.Date = DateTime.Parse(date.TextContent.Trim());
                }

                posts.Add(post);
            }

            return posts;
        }
    }
}
