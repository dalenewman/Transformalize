#region license
// Transformalize
// Configurable Extract, Transform, and Load
// Copyright 2013-2017 Dale Newman
//  
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//   
//       http://www.apache.org/licenses/LICENSE-2.0
//   
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
#endregion
using System;
using System.Collections.Generic;
using Transformalize.Context;
using Transformalize.Contracts;

namespace Transformalize.Provider.Internal {

    public class InternalOutputProvider : IOutputProvider {
        private readonly OutputContext _context;
        private readonly IWrite _writer;

        public InternalOutputProvider(OutputContext context, IWrite writer) {
            _writer = writer;
            _context = context;
        }

        public void Dispose() {
        }

        public void Initialize() {
            _context.Process.Rows.Clear();
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
            _writer.Write(rows);
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
