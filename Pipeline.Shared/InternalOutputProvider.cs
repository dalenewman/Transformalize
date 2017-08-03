using System;
using System.Collections.Generic;
using Transformalize.Context;
using Transformalize.Contracts;
using Transformalize.Writers;

namespace Transformalize.Provider.Internal {
    public class InternalOutputProvider : IOutputProvider {
        private readonly OutputContext _context;

        public InternalOutputProvider(OutputContext context) {
            _context = context;
        }

        public void Dispose() {
        }

        public void Initialize() {
            _context.Process.Rows.Clear();
            foreach (var entity in _context.Process.Entities) {
                entity.Rows.Clear();
            }
        }

        public object GetMaxVersion() {
            return null;
        }

        public int GetNextTflBatchId() {
            return _context.Entity.Index;
        }

        public int GetMaxTflKey() {
            return 0;
        }

        public void Start() {

        }

        public void End() {
        }

        public void Write(IEnumerable<IRow> rows) {
            new InternalWriter(_context).Write(rows);
        }

        public void Delete() {
            throw new NotImplementedException();
        }

        public IEnumerable<IRow> ReadKeys() {
            throw new NotImplementedException();
        }

        public IEnumerable<IRow> Match(IEnumerable<IRow> rows) {
            throw new NotImplementedException();
        }
    }
}
