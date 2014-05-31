using System.Collections.Generic;
using System.IO;

namespace Transformalize.Main.Providers.File
{
    public class FileLineLoader {
        private readonly string _fileName;

        public FileLineLoader(string fileName) {
            _fileName = fileName;
        }

        public IEnumerable<string> Load() {
            var lines = new List<string>();
            using (var reader = new StreamReader(_fileName)) {
                for (var i = 0; i < 100; i++) {
                    if (!reader.EndOfStream) {
                        lines.Add(reader.ReadLine());
                    }
                }
            }
            return lines;
        }
    }
}