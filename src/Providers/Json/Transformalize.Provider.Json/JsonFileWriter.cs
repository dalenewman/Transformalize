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
using Transformalize.Context;
using Transformalize.Contracts;

namespace Transformalize.Providers.Json {

   public class JsonFileWriter : IWrite {
      private readonly OutputContext _context;
      private readonly IWrite _jsonStreamWriter;
      private readonly StreamWriter _streamWriter;

      public JsonFileWriter(OutputContext context) {
         _context = context;
         _streamWriter = new StreamWriter(new MemoryStream());
         _jsonStreamWriter = new JsonStreamWriter(context, _streamWriter);
      }

      public void Write(IEnumerable<IRow> rows) {
         using (var fileStream = File.Create(_context.Connection.File)) {
            _jsonStreamWriter.Write(rows);
            _streamWriter.BaseStream.Seek(0, SeekOrigin.Begin);
            _streamWriter.BaseStream.CopyTo(fileStream);
         }
      }
   }
}