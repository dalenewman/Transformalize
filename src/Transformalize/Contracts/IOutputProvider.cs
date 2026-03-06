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

namespace Transformalize.Contracts {

    public interface IOutputProvider : IDisposable {
        /// <summary>
        /// Initialize the output:
        /// * destroy existing structures
        /// * create existing structures
        /// </summary>
        void Initialize();
        Task InitializeAsync(CancellationToken token = default);

        /// <summary>
        /// Get the maximum output version that is not marked as deleted, or null if no version defined
        /// </summary>
        /// <returns></returns>
        object GetMaxVersion();
        Task<object> GetMaxVersionAsync(CancellationToken token = default);

        /// <summary>
        /// Get the maximum TflBatchId in the output, or null if init mode
        /// </summary>
        /// <returns></returns>
        int GetNextTflBatchId();
        Task<int> GetNextTflBatchIdAsync(CancellationToken token = default);

        /// <summary>
        /// Get the maximum TflKey in the output, or null if init mode
        /// </summary>
        /// <returns></returns>
        int GetMaxTflKey();
        Task<int> GetMaxTflKeyAsync(CancellationToken token = default);

        /// <summary>
        /// provider specific start actions
        /// </summary>
        void Start();
        Task StartAsync(CancellationToken token = default);

        // provider specific end actions
        void End();
        Task EndAsync(CancellationToken token = default);

        /// <summary>
        /// write all or just what is necessary to the output
        /// determine inserts vs. updates
        /// </summary>
        /// <param name="rows"></param>
        void Write(IEnumerable<IRow> rows);
        Task WriteAsync(IEnumerable<IRow> rows, CancellationToken token = default);

        /// <summary>
        /// When delete is enabled, you must determine what needs to be deleted, and then MARK them as deleted, TflDeleted = true
        /// </summary>
        void Delete();
        Task DeleteAsync(CancellationToken token = default);

        // Read all primary key, TflHashCode, and TflDeleted (support for delete)
        IEnumerable<IRow> ReadKeys();
        Task<IEnumerable<IRow>> ReadKeysAsync(CancellationToken token = default);

        /// <summary>
        /// Given the input (or most likely a batch of input)
        /// Find the matching primary keys, along with TflDeleted, and TflHashCode (system fields every output has)
        /// TODO: make IBatchReader, and IReadInputKeysAndHashcodes obsolete
        /// </summary>
        /// <param name=""></param>
        /// <returns></returns>
        IEnumerable<IRow> Match(IEnumerable<IRow> rows);
        Task<IEnumerable<IRow>> MatchAsync(IEnumerable<IRow> rows, CancellationToken token = default);

    }
}
