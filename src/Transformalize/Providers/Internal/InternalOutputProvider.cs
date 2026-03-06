#region license
// Transformalize
// Configurable Extract, Transform, and Load
// Copyright 2013-2025 Dale Newman
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
using System.Threading;
using System.Threading.Tasks;
using Transformalize.Context;
using Transformalize.Contracts;

namespace Transformalize.Providers.Internal {

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

        public Task InitializeAsync(CancellationToken token = default) {
            Initialize();
            return Task.CompletedTask;
        }

        public object GetMaxVersion() {
            return null;
        }

        public Task<object> GetMaxVersionAsync(CancellationToken token = default) {
            return Task.FromResult(GetMaxVersion());
        }

        public int GetNextTflBatchId() {
            return _context.Entity.Index;
        }

        public Task<int> GetNextTflBatchIdAsync(CancellationToken token = default) {
            return Task.FromResult(GetNextTflBatchId());
        }

        public int GetMaxTflKey() {
            return 0;
        }

        public Task<int> GetMaxTflKeyAsync(CancellationToken token = default) {
            return Task.FromResult(GetMaxTflKey());
        }

        public void Start() {

        }

        public Task StartAsync(CancellationToken token = default) {
            Start();
            return Task.CompletedTask;
        }

        public void End() {
        }

        public Task EndAsync(CancellationToken token = default) {
            End();
            return Task.CompletedTask;
        }

        public void Write(IEnumerable<IRow> rows) {
            _writer.Write(rows);
        }

        public Task WriteAsync(IEnumerable<IRow> rows, CancellationToken token = default) {
            Write(rows);
            return Task.CompletedTask;
        }

        public void Delete() {
            throw new NotImplementedException();
        }

        public Task DeleteAsync(CancellationToken token = default) {
            Delete();
            return Task.CompletedTask;
        }

        public IEnumerable<IRow> ReadKeys() {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<IRow>> ReadKeysAsync(CancellationToken token = default) {
            return Task.FromResult(ReadKeys());
        }

        public IEnumerable<IRow> Match(IEnumerable<IRow> rows) {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<IRow>> MatchAsync(IEnumerable<IRow> rows, CancellationToken token = default) {
            return Task.FromResult(Match(rows));
        }
    }
}
