using System.IO;
using System.Web;
using Transformalize.Libs.Rhino.Etl;
using Transformalize.Main;

namespace Transformalize.Runner {
    public class ContentsFileReader : WithLoggingMixin, IContentsReader {

        private readonly string _path;
        private readonly char[] _s = new[] { '\\' };

        public ContentsFileReader() {
            _path = string.Empty;
        }

        public ContentsFileReader(string path) {
            _path = path ?? string.Empty;
        }

        private bool PathProvided() {
            return !string.IsNullOrEmpty(_path);
        }

        public Contents Read(string file) {
            var fileName = file.Contains("?") ? file.Substring(0, file.IndexOf('?')) : file;

            var fileInfo = Path.IsPathRooted(fileName) ?
                           new FileInfo(fileName) : (
                           PathProvided() ?
                                new FileInfo(_path.TrimEnd(_s) + @"\" + fileName) :
                                new FileInfo(fileName)
                            );

            if (!fileInfo.Exists) {
                throw new TransformalizeException("Sorry. I can't find the file {0}.", fileInfo.FullName);
            }

            var content = File.ReadAllText(fileInfo.FullName);

            if (fileName.Equals(file))
                return new Contents {
                    Name = Path.GetFileNameWithoutExtension(fileInfo.FullName),
                    FileName = fileInfo.FullName,
                    Content = content
                };

            var query = HttpUtility.ParseQueryString(file.Substring(file.IndexOf('?')));
            foreach (var key in query.AllKeys) {
                content = content.Replace("@" + key, query[key]);
                Debug("Replaced {0} with {1} per file's query string.", "@" + key, query[key]);
            }

            return new Contents {
                Name = Path.GetFileNameWithoutExtension(fileInfo.FullName),
                FileName = fileInfo.FullName,
                Content = content
            };

        }
    }
}