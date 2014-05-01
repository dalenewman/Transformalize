using System;
using System.IO;
using System.Net;
using System.Security.Policy;
using System.Web;
using Transformalize.Libs.NLog;
using Transformalize.Main;

namespace Transformalize.Runner {
    public class ContentsWebReader : IContentsReader {
        private readonly Logger _log = LogManager.GetLogger("tfl");

        public Contents Read(string file) {

            var uri = new Uri(file);

            var response = Web.Get(uri.OriginalString);
            if (response.Code == HttpStatusCode.OK) {
                if (!string.IsNullOrEmpty(uri.Query)) {
                    var query = HttpUtility.ParseQueryString(uri.Query);
                    foreach (var key in query.AllKeys) {
                        response.Content = response.Content.Replace("@" + key, query[key]);
                        _log.Debug("Replaced {0} with {1} per url's query string.", "@" + key, query[key]);
                    }
                }
            } else {
                throw new TransformalizeException("{0} returned from {1}", response.Code, file);
            }

            return new Contents {
                Content = response.Content,
                FileName = Path.GetFileName(uri.LocalPath),
                Name = Path.GetFileNameWithoutExtension(uri.LocalPath)
            };

        }
    }
}