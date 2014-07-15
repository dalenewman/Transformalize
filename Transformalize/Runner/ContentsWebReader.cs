using System;
using System.IO;
using System.Net;
using System.Web;
using Transformalize.Main;

namespace Transformalize.Runner {
    public class ContentsWebReader : ContentsReader {

        public override Contents Read(string resource) {

            var uri = new Uri(resource);

            var response = Web.Get(uri.OriginalString);
            if (response.Code == HttpStatusCode.OK) {
                if (!string.IsNullOrEmpty(uri.Query)) {
                    response.Content = ReplaceParameters(response.Content, HttpUtility.ParseQueryString(uri.Query));
                }
            } else {
                throw new TransformalizeException("{0} returned from {1}", response.Code, resource);
            }

            return new Contents {
                Content = response.Content,
                FileName = Path.GetFileName(uri.LocalPath),
                Name = Path.GetFileNameWithoutExtension(uri.LocalPath)
            };

        }
    }
}