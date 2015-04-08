using System.Collections.Generic;
using System.IO;

namespace Transformalize.Main.Providers.File {

    public class FileLineLoader {

        private readonly string _fileName;
        private readonly int _limit;

        public FileLineLoader(string fileName, int limit) {
            _fileName = fileName;
            _limit = limit;
        }

        public IEnumerable<string> Load() {
            var lines = new List<string>();
            using (var reader = new StreamReader(_fileName)) {
                if (_limit == 0) {
                    while (!reader.EndOfStream) {
                        lines.Add(reader.ReadLine());
                    }
                } else {
                    var limit = _limit + 1;
                    for (var i = 0; i < limit; i++) {
                        if (!reader.EndOfStream) {
                            lines.Add(reader.ReadLine());
                        }
                    }
                }
            }
            return lines;
        }
    }
}