using System.IO;
using System.Web;
using Transformalize.Main;

namespace Transformalize.Runner {
    public class ContentsFileReader : ContentsReader {

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

        public override Contents Read(string file) {
            var fileName = file.Contains("?") ? file.Substring(0, file.IndexOf('?')) : file;

            var fileInfo = Path.IsPathRooted(fileName) ?
                           new FileInfo(fileName) : (
                           PathProvided() ?
                                new FileInfo(_path.TrimEnd(_s) + @"\" + fileName) :
                                new FileInfo(fileName)
                            );

            if (!fileInfo.Exists) {
                throw new TransformalizeException(string.Empty, string.Empty, "Sorry. I can't find the file {0}.", fileInfo.FullName);
            }

            var content = File.ReadAllText(fileInfo.FullName);

            if (fileName.Equals(file))
                return new Contents {
                    Name = Path.GetFileNameWithoutExtension(fileInfo.FullName),
                    FileName = fileInfo.FullName,
                    Content = content
                };

            content = ReplaceParameters(content, HttpUtility.ParseQueryString(file.Substring(file.IndexOf('?'))));

            return new Contents {
                Name = Path.GetFileNameWithoutExtension(fileInfo.FullName),
                FileName = fileInfo.FullName,
                Content = content
            };

        }
    }
}