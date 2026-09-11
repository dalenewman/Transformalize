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
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Transformalize.Contracts {
   public interface IRead {
      IEnumerable<IRow> Read();
      /// <summary>
      /// Read asynchronously, returning the complete result.
      /// </summary>
      /// <remarks>
      /// This is the buffered asynchronous read backing <c>ExecuteAsync</c>, and it remains fully
      /// supported. For incremental enumeration, prefer the <c>ReadStreamAsync</c> extension in
      /// <c>Transformalize.Extensions</c>; implement <c>IReadStream</c> to make it stream natively,
      /// otherwise it adapts this method.
      /// </remarks>
      Task<IEnumerable<IRow>> ReadAsync(CancellationToken token = default);
   }
}