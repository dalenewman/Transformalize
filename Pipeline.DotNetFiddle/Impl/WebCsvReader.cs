using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading;
using Pipeline.Context;
using Pipeline.Contracts;

namespace Pipeline.DotNetFiddle.Impl {
    public class WebCsvReader : IRead {
        private readonly InputContext _context;
        private readonly Regex _regex = new Regex(@"""?\s*,\s*""?", RegexOptions.Compiled);
        private readonly IRowFactory _rowFactory;

        public WebCsvReader(InputContext context, IRowFactory rowFactory) {
            _context = context;
            _rowFactory = rowFactory;
        }

        public IEnumerable<IRow> Read() {

            var client = new WebClient();
            var stream = client.OpenRead(_context.Connection.Url);

            if (stream == null) {
                _context.Error("Could not open {0}.", _context.Connection.Url);
                yield break;
            }

            var start = _context.Connection.Start;
            var end = _context.Connection.End;

            if (_context.Entity.IsPageRequest()) {
                start += ((_context.Entity.Page * _context.Entity.PageSize) - _context.Entity.PageSize);
                end = start + _context.Entity.PageSize;
            }


            using (var reader = new StreamReader(stream)) {
                string line;
                var counter = 1;

                if (start > 1) {
                    for (var i = 1; i < start; i++) {
                        reader.ReadLine();
                        counter++;
                    }
                }

                while ((line = reader.ReadLine()) != null) {
                    var tokens = _regex.Split(line.Trim('"'));
                    if (tokens.Length > 0) {
                        var row = _rowFactory.Create();
                        for (var i = 0; i < _context.InputFields.Length && i < tokens.Length; i++) {
                            var field = _context.InputFields[i];
                            row[field] = field.Convert(tokens[i]);
                        }
                        yield return row;
                    }
                    counter++;
                    if (end > 0 && counter == end) {
                        yield break;
                    }
                }
            }
        }
    }
}