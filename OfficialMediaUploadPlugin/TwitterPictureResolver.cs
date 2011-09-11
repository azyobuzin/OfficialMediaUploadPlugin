using System.Collections.Generic;
using System.Linq;
using Dulcet.Twitter;
using Inscribe.Storage;

namespace OfficialMediaUploadPlugin
{
    static class TwitterPictureResolver
    {
        private static Dictionary<string, string> imageUrlDic = new Dictionary<string, string>();

        public static void Init()
        {
            TweetStorage.TweetStorageChanged += (sender, e) =>
            {
                if (e.ActionKind == TweetActionKind.Added && e.Tweet != null)
                {
                    var media = e.Tweet.Status.Entities.GetChildNode("media");
                    if (media != null)
                    {
                        media.Children.OfType<TwitterEntityNode>().ForEach(entity =>
                            imageUrlDic.Add(entity.GetChildValue("expanded_url").Value, entity.GetChildValue("media_url").Value));
                    }
                }
            };
        }

        public static bool IsResolvable(string url)
        {
            return imageUrlDic.ContainsKey(url);
        }

        public static string Resolve(string url)
        {
            return imageUrlDic[url];
        }
    }
}
