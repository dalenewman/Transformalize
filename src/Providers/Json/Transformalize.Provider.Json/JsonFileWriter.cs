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
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Transformalize.Context;
using Transformalize.Contracts;

namespace Transformalize.Providers.Json {

   public class JsonFileWriter : IWrite {
      private readonly OutputContext _context;

      public JsonFileWriter(OutputContext context) {
         _context = context;
      }

      public void Write(IEnumerable<IRow> rows) {
         using (var fileStream = File.Create(_context.Connection.File))
         using (var streamWriter = new StreamWriter(fileStream)) {
            new JsonStreamWriter(_context, streamWriter).Write(rows);
         }
      }

      public async Task WriteAsync(IEnumerable<IRow> rows, CancellationToken token = default) {
         using (var fileStream = new FileStream(_context.Connection.File, FileMode.Create, FileAccess.Write, FileShare.None, 4096, FileOptions.Asynchronous))
         using (var streamWriter = new StreamWriter(fileStream)) {
            await new JsonStreamWriter(_context, streamWriter).WriteAsync(rows, token).ConfigureAwait(false);
            await streamWriter.FlushAsync().ConfigureAwait(false);
         }
      }
   }
}
