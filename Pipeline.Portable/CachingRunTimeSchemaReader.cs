using System.Collections.Generic;
using Pipeline.Configuration;
using Pipeline.Contracts;

namespace Pipeline {
    public class CachingRunTimeSchemaReader : IRunTimeSchemaReader {

        private readonly IRunTimeSchemaReader _schemaReader;
        private readonly Dictionary<string, Schema> _cache = new Dictionary<string, Schema>();

        public CachingRunTimeSchemaReader(IRunTimeSchemaReader schemaReader) {
            _schemaReader = schemaReader;
        }

        public Process Process {
            get { return _schemaReader.Process;}
            set { _schemaReader.Process = value;}
        }

        public Schema Read(Process process) {
            Process = process;
            return Read();
        }

        public Schema Read() {
            if (_cache.ContainsKey(Process.Key)) {
                return _cache[Process.Key];
            }
            var schema = _schemaReader.Read();
            _cache[Process.Key] = schema;
            return schema;
        }

        public Schema Read(Entity entity) {
            if (_cache.ContainsKey(entity.Key)) {
                return _cache[entity.Key];
            }
            var schema = _schemaReader.Read(entity);
            _cache[entity.Key] = schema;
            return schema;
        }


    }
}