using System;
using System.Collections.Generic;
using Transformalize.Contracts;

namespace Transformalize.Nulls {
    public class NullOutputProvider : IOutputProvider {

        public void Delete() {
            throw new NotImplementedException();
        }

        public void Dispose() {
            
        }

        public void End() {
            throw new NotImplementedException();
        }

        public int GetNextTflBatchId() {
            return 1;
        }

        public int GetMaxTflKey() {
            return 0;
        }

        public object GetMaxVersion() {
            return null;
        }

        public void Initialize() {
            throw new NotImplementedException();
        }

        public IEnumerable<IRow> Match(IEnumerable<IRow> rows) {
            throw new NotImplementedException();
        }

        public IEnumerable<IRow> ReadKeys() {
            throw new NotImplementedException();
        }

        public void Start() {
            throw new NotImplementedException();
        }

        public void Write(IEnumerable<IRow> rows) {
            throw new NotImplementedException();
        }
    }
}
