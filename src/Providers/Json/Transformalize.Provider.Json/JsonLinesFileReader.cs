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
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Transformalize.Context;
using Transformalize.Contracts;

namespace Transformalize.Providers.Json {

   public class JsonLinesFileReader : IRead {

      private readonly InputContext _context;
      private readonly IRowFactory _rowFactory;

      public JsonLinesFileReader(InputContext context, IRowFactory rowFactory) {
         _context = context;
         _rowFactory = rowFactory;
      }

      public IEnumerable<IRow> Read() {
         var fileInfo = FileUtility.Find(_context.Connection.File);
         using (var stream = new FileStream(fileInfo.FullName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite)) {
            return new JsonLinesStreamReader(_context, stream, _rowFactory).Read().ToList();
         }
      }

      public async Task<IEnumerable<IRow>> ReadAsync(CancellationToken token = default) {
         token.ThrowIfCancellationRequested();
         var fileInfo = FileUtility.Find(_context.Connection.File);
         using (var stream = new FileStream(fileInfo.FullName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite, 4096, FileOptions.Asynchronous | FileOptions.SequentialScan)) {
            var rows = await new JsonLinesStreamReader(_context, stream, _rowFactory).ReadAsync(token).ConfigureAwait(false);
            return rows.ToList();
         }
      }
   }
}
