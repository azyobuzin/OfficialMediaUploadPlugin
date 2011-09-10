using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.Composition;
using Inscribe.Communication;
using Acuerdo;
using Acuerdo.External.Uploader;
using Dulcet.Twitter.Credential;
using Dulcet;
using Dulcet.Twitter;
using Dulcet.Network;
using Dulcet.Util;

namespace OfficialMediaUploadPlugin
{
    public class UpdateWithMedia : IUploader
    {
        public string UploadImage(OAuth credential, string path, string comment)
        {
            var req = Http.CreateRequest(
                new Uri("https://upload.twitter.com/1/statuses/update_with_media.xml"),
                "POST",
                "multipart/form-data");
            //TODO:ヘッダ作成
            throw new NotImplementedException();
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
