using System.Collections.Generic;
using System.Linq;
using System.Text;
using Pipeline.Configuration;
using Pipeline.Context;
using Pipeline.Contracts;

namespace Pipeline.Provider.File {
    public class FileReader : IRead {
        private readonly InputContext _context;
        private readonly IRowFactory _rowFactory;
        private readonly Field _field;

        public FileReader(InputContext context, IRowFactory rowFactory) {
            _context = context;
            _rowFactory = rowFactory;
            _field = context.Entity.GetAllFields().First(f => f.Input);
        }

        public IEnumerable<IRow> Read() {
            var encoding = Encoding.GetEncoding(_context.Connection.Encoding);
            foreach (var line in System.IO.File.ReadLines(_context.Connection.File, encoding)) {
                var row = _rowFactory.Create();
                row[_field] = line;
                yield return row;
            }
        }
    }
}