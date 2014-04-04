using System;
using System.IO;
using System.Net;
using Transformalize.Libs.NLog;
using Transformalize.Main;

namespace Transformalize.Runner {
    public class ContentsWebReader : IContentsReader {
        private readonly Logger _log = LogManager.GetLogger(string.Empty);

        public Contents Read(string file) {

            var uri = new Uri(file);

            var response = Web.Get(uri.OriginalString);
            if (response.Code != HttpStatusCode.OK) {
                throw new TransformalizeException("{0} returned from {1}", response.Code, file);
            }

            return new Contents {
                Content = response.Content,
                FileName = file,
                Name = Path.GetFileNameWithoutExtension(uri.LocalPath)
            };

        }
    }
}