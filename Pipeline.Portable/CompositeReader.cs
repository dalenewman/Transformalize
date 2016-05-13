using System.Collections.Generic;
using System.Linq;
using Cfg.Net.Ext;
using Pipeline.Configuration;
using Pipeline.Contracts;

namespace Pipeline {

    public class CompositeReader : IRead {
        private readonly IEnumerable<IRead> _readers;

        public CompositeReader(params IRead[] readers) {
            _readers = readers;
        }

        public CompositeReader(IEnumerable<IRead> readers) {
            _readers = readers;
        }

        public IEnumerable<IRow> Read() {
            return _readers.SelectMany(reader => reader.Read());
        }
    }
}