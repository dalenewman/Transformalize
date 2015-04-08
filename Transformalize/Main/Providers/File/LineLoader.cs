using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Transformalize.Main.Providers.File {

    public class LineLoader {
        private const char DOUBLE_QUOTE = '\"';
        private readonly FileInspectionRequest _request;
        private readonly FileLineLoader _loader;
        private readonly bool _isCsv;

        public LineLoader(FileSystemInfo fileInfo, FileInspectionRequest request) {
            _request = request;
            _loader = new FileLineLoader(fileInfo.FullName, request.LineLimit);
            _isCsv = fileInfo.Extension.Equals(".csv", StringComparison.OrdinalIgnoreCase);
        }

        public IEnumerable<Line> Load() {

            return _loader.Load().Select(
                content => _isCsv ?
                    new Line(content, DOUBLE_QUOTE, _request) :
                    new Line(content, _request)
            );
        }
    }
}