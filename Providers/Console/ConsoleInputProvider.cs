using System.Collections.Generic;
using Transformalize.Configuration;
using Transformalize.Contracts;

namespace Transformalize.Providers.Console {
    public class ConsoleInputProvider : IInputProvider {
        private readonly IRead _reader;

        public ConsoleInputProvider(IRead reader) {
            _reader = reader;
        }

        public object GetMaxVersion() {
            return null;
        }

        public Schema GetSchema(Entity entity = null) {
            return new Schema();
        }

        public IEnumerable<IRow> Read() {
            return _reader.Read();
        }
    }
}
