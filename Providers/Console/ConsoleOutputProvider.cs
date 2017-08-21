using System;
using System.Collections.Generic;
using Transformalize.Context;
using Transformalize.Contracts;

namespace Transformalize.Providers.Console {
    public class ConsoleOutputProvider : IOutputProvider {
        private readonly OutputContext _outputContext;
        private readonly ISerialize _serializer;
        private readonly IWrite _writer;

        public ConsoleOutputProvider(OutputContext context, IWrite writer) {
            _writer = writer;
            _outputContext = context;
        }
        public void Delete() {
        }

        public void Dispose() {
        }

        public void End() {
        }

        public int GetMaxTflKey() {
            return 0;
        }

        public object GetMaxVersion() {
            return null;
        }

        public int GetNextTflBatchId() {
            return _outputContext.Entity.Index;
        }

        public void Initialize() {
        }

        public IEnumerable<IRow> Match(IEnumerable<IRow> rows) {
            throw new NotImplementedException();
        }

        public IEnumerable<IRow> ReadKeys() {
            throw new NotImplementedException();
        }

        public void Start() {
        }

        public void Write(IEnumerable<IRow> rows) {
            _writer.Write(rows);
        }
    }
}
