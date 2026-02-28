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
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Transformalize.Contracts;

namespace Transformalize.Nulls {

    public class NullDeleteHandler : IEntityDeleteHandler {
        public IEnumerable<IRow> DetermineDeletes() {
            return Enumerable.Empty<IRow>();
        }

        public void Delete() { }

        public async IAsyncEnumerable<IRow> DetermineDeletesAsync([EnumeratorCancellation] CancellationToken cancellationToken = default) {
            yield break;
        }

        public Task DeleteAsync(CancellationToken cancellationToken = default) => Task.CompletedTask;
    }
}