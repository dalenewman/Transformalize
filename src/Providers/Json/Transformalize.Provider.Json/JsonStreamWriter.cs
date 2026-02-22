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
using Newtonsoft.Json;
using Transformalize.Configuration;
using Transformalize.Context;
using Transformalize.Contracts;

namespace Transformalize.Providers.Json {

   public class JsonStreamWriter : IWrite {

      private readonly StreamWriter _streamWriter;
      private readonly Field[] _fields;
      private readonly string[] _formats;
      private readonly OutputContext _context;

      public JsonStreamWriter(OutputContext context, StreamWriter streamWriter) {
         _context = context;
         _streamWriter = streamWriter;
         _fields = context.GetAllEntityOutputFields().ToArray();
         _formats = new string[_fields.Count()];
         for (int i = 0; i < _fields.Length; i++) {
            _formats[i] = _fields[i].Format == string.Empty ? string.Empty : string.Concat("{0:", _fields[i].Format, "}");
         }
      }

      public void Write(IEnumerable<IRow> rows) {

         var jw = new JsonTextWriter(_streamWriter) {
            Formatting = _context.Connection.Format == "json" ? Formatting.Indented : Formatting.None
         };

         jw.WriteStartArrayAsync().ConfigureAwait(false);

         foreach (var row in rows) {

            jw.WriteStartObjectAsync().ConfigureAwait(false);

            for (int i = 0; i < _fields.Length; i++) {
               jw.WritePropertyNameAsync(_fields[i].Alias).ConfigureAwait(false);
               if (_formats[i] == string.Empty) {
                  jw.WriteValueAsync(row[_fields[i]]).ConfigureAwait(false);
               } else {
                  jw.WriteValueAsync(string.Format(_formats[i], row[_fields[i]])).ConfigureAwait(false);
               }
            }
            jw.WriteEndObjectAsync().ConfigureAwait(false);
            _context.Entity.Inserts++;

            jw.FlushAsync().ConfigureAwait(false);
         }

         jw.WriteEndArrayAsync().ConfigureAwait(false);

         jw.FlushAsync().ConfigureAwait(false);
      }
   }
}
