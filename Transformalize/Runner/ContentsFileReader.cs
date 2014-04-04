using System.IO;
using Transformalize.Libs.FileHelpers.Events;
using Transformalize.Main;

namespace Transformalize.Runner
{
    public class ContentsFileReader : IContentsReader {
        
        private readonly string _path;
        private readonly char[] _s = new[] { '\\' };

        public ContentsFileReader()
        {
            _path = string.Empty;
        }

        public ContentsFileReader(string path)
        {
            _path = path ?? string.Empty;
        }

        private bool PathProvided()
        {
            return !string.IsNullOrEmpty(_path);
        }

        public Contents Read(string file) {

            var fileInfo = Path.IsPathRooted(file) ?
                           new FileInfo(file) : ( 
                           PathProvided() ?  
                                new FileInfo(_path.TrimEnd(_s) + @"\" + file) :
                                new FileInfo(file)
                            );

            if (!fileInfo.Exists) {
                throw new TransformalizeException("Sorry. I can't the find file {0}.", fileInfo.FullName);
            }

            return new Contents {
                Name = Path.GetFileNameWithoutExtension(fileInfo.FullName),
                FileName = fileInfo.FullName,
                Content = File.ReadAllText(fileInfo.FullName)
            };

        }
    }
}