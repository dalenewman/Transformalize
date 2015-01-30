using System;
using System.IO;
using System.Net;
using System.Web;
using Transformalize.Main;

namespace Transformalize.Runner {
    public class ContentsWebReader : ContentsReader {

        public override Contents Read(string resource) {

            Uri uri;
            try {
                uri = new Uri(resource);
            } catch (Exception) {
                resource = HttpUtility.UrlDecode(resource);
                try {
                    uri = new Uri(resource);
                } catch (Exception ex) {
                    throw new TransformalizeException("Trouble fetching {0}. {1}", resource, ex.Message);
                }
            }

            var response = Web.Get(uri.OriginalString);
            if (response.Code == HttpStatusCode.OK) {
                response.Content = response.Content;
            } else {
                throw new TransformalizeException(string.Empty, string.Empty, "{0} returned from {1}", response.Code, resource);
            }

            return new Contents {
                Content = response.Content,
                FileName = Path.GetFileName(uri.LocalPath),
                Name = Path.GetFileNameWithoutExtension(uri.LocalPath)
            };

        }
    }
}