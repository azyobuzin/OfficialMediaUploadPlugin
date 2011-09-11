using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Xml.Linq;
using Acuerdo.External.Uploader;
using Acuerdo.Injection;
using Dulcet.Network;
using Dulcet.Twitter;
using Dulcet.Twitter.Credential;
using Inscribe;
using Inscribe.Communication.Posting;
using Inscribe.Storage;

namespace OfficialMediaUploadPlugin
{
    public class TwitterUploader : IUploader
    {
        private const string ApiUrl = "https://upload.twitter.com/1/statuses/update_with_media.xml";

        private OAuth lastCredential = null;
        private string lastPath = null;
        private string lastComment = null;

        private bool initializedInjection = false;

        public string UploadImage(OAuth credential, string path, string comment)
        {
            this.lastCredential = credential;
            this.lastPath = path;
            this.lastComment = comment;

            if (!initializedInjection)
                InitializeInjection();

            return string.Empty;//何もしない
        }

        private void InitializeInjection()//できるだけ遅くUpdateInjectionに登録する
        {
            //UpdateInjectionを支配して画像投稿させる
            PostOffice.UpdateInjection.Injection(
                (arg, next, last) =>
                {
                    if (lastComment == null || arg.Item2.Trim() != lastComment)
                        return next.Invoke(arg);

                    var req = Http.CreateRequest(
                        new Uri(ApiUrl),
                        "POST",
                        "multipart/form-data");

                    //タイムアウト時間を2倍に
                    req.Timeout *= 2;

                    //リフレクションでGetHeaderを呼び出す
                    var reg = (string)typeof(OAuth).InvokeMember(
                        "GetHeader",
                        BindingFlags.InvokeMethod | BindingFlags.NonPublic | BindingFlags.Instance,
                        null,
                        this.lastCredential,
                        new object[] { ApiUrl, CredentialProvider.RequestMethod.POST, null, null, false });

                    req.Headers.Add("Authorization", "OAuth " + reg);

                    var data = new List<SendData>();
                    data.Add(new SendData("status", text: this.lastComment));
                    data.Add(new SendData("media[]", file: this.lastPath));

                    if (arg.Item3.HasValue)
                        data.Add(new SendData("in_reply_to_status_id", text: arg.Item3.Value.ToString()));

                    //投稿
                    var res = Http.WebUpload(req, data, Encoding.UTF8, s => TwitterStatus.FromNode(XElement.Load(s)));

                    if (res.ThrownException != null)
                        throw res.ThrownException;

                    TweetStorage.Register(res.Data);
                    NotifyStorage.Notify("ツイートしました:@" + arg.Item1.ScreenName + ": " + res.Data.Text);

                    this.lastCredential = null;
                    this.lastComment = null;
                    this.lastPath = null;
                    
                    var chunk = PostOffice.GetUnderControlChunk(arg.Item1);
                    if (chunk.Item2 > TwitterDefine.UnderControlWarningThreshold)
                    {
                        throw new TweetAnnotationException(TweetAnnotationException.AnnotationKind.NearUnderControl);
                    }
                    return chunk.Item2;
                },
                InjectionMode.InjectionLast
            );

            this.initializedInjection = true;
        }

        public string ServiceName
        {
            get { return "Twitter Official"; }
        }

        public bool IsResolvable(string url)
        {
            throw new NotImplementedException();
        }

        public string Resolve(string url)
        {
            throw new NotImplementedException();
        }
    }
}
