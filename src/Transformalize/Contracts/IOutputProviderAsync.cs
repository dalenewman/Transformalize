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
    public interface IOutputProviderAsync : IDisposable {
        /// <summary>
        /// Initialize the output:
        /// * destroy existing structures
        /// * create existing structures
        /// </summary>
        Task InitializeAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Get the maximum output version that is not marked as deleted, or null if no version defined
        /// </summary>
        Task<object> GetMaxVersionAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Get the maximum TflBatchId in the output, or null if init mode
        /// </summary>
        Task<int> GetNextTflBatchIdAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Get the maximum TflKey in the output, or null if init mode
        /// </summary>
        Task<int> GetMaxTflKeyAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Provider specific start actions
        /// </summary>
        Task StartAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Provider specific end actions
        /// </summary>
        Task EndAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Write all or just what is necessary to the output; determine inserts vs. updates
        /// </summary>
        Task WriteAsync(IAsyncEnumerable<IRow> rows, CancellationToken cancellationToken = default);

        /// <summary>
        /// When delete is enabled, determine what needs to be deleted, and mark them as deleted (TflDeleted = true)
        /// </summary>
        Task DeleteAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Read all primary key, TflHashCode, and TflDeleted (support for delete)
        /// </summary>
        IAsyncEnumerable<IRow> ReadKeysAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Given the input (or a batch of input), find matching primary keys with TflDeleted and TflHashCode
        /// </summary>
        IAsyncEnumerable<IRow> MatchAsync(IAsyncEnumerable<IRow> rows, CancellationToken cancellationToken = default);
    }
}
