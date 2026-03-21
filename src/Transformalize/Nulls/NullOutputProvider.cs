#region license
// Transformalize
// Configurable Extract, Transform, and Load
// Copyright 2013-2026 Dale Newman
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
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
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

        public Task InitializeAsync(CancellationToken token = default) {
            Initialize();
            return Task.CompletedTask;
        }

        public Task<object> GetMaxVersionAsync(CancellationToken token = default) {
            return Task.FromResult(GetMaxVersion());
        }

        public Task<int> GetNextTflBatchIdAsync(CancellationToken token = default) {
            return Task.FromResult(GetNextTflBatchId());
        }

        public Task<int> GetMaxTflKeyAsync(CancellationToken token = default) {
            return Task.FromResult(GetMaxTflKey());
        }

        public Task StartAsync(CancellationToken token = default) {
            Start();
            return Task.CompletedTask;
        }

        public Task EndAsync(CancellationToken token = default) {
            End();
            return Task.CompletedTask;
        }

        public Task WriteAsync(IEnumerable<IRow> rows, CancellationToken token = default) {
            Write(rows);
            return Task.CompletedTask;
        }

        public Task DeleteAsync(CancellationToken token = default) {
            Delete();
            return Task.CompletedTask;
        }

        public Task<IEnumerable<IRow>> ReadKeysAsync(CancellationToken token = default) {
            return Task.FromResult(ReadKeys());
        }

        public Task<IEnumerable<IRow>> MatchAsync(IEnumerable<IRow> rows, CancellationToken token = default) {
            return Task.FromResult(Match(rows));
        }
    }
}
