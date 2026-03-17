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
using System.IO;
using System.Linq;
using System.Text.Json;
using Transformalize.Configuration;
using Transformalize.Context;
using Transformalize.Contracts;
using System.Threading;
using System.Threading.Tasks;

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

         var options = new JsonWriterOptions { Indented = _context.Connection.Format == "json" };
         using var jw = new Utf8JsonWriter(_streamWriter.BaseStream, options);

         jw.WriteStartArray();

         foreach (var row in rows) {

            jw.WriteStartObject();

            for (int i = 0; i < _fields.Length; i++) {
               jw.WritePropertyName(_fields[i].Alias);
               if (_formats[i] == string.Empty) {
                  WriteValue(jw, row[_fields[i]]);
               } else {
                  jw.WriteStringValue(string.Format(_formats[i], row[_fields[i]]));
               }
            }
            jw.WriteEndObject();
            _context.Entity.Inserts++;

            jw.Flush();
         }

         jw.WriteEndArray();

         jw.Flush();
      }

      public async Task WriteAsync(IEnumerable<IRow> rows, CancellationToken token = default) {

         var options = new JsonWriterOptions { Indented = _context.Connection.Format == "json" };
         var jw = new Utf8JsonWriter(_streamWriter.BaseStream, options);

         token.ThrowIfCancellationRequested();
         jw.WriteStartArray();

         foreach (var row in rows) {
            token.ThrowIfCancellationRequested();
            jw.WriteStartObject();

            for (int i = 0; i < _fields.Length; i++) {
               jw.WritePropertyName(_fields[i].Alias);
               if (_formats[i] == string.Empty) {
                  WriteValue(jw, row[_fields[i]]);
               } else {
                  jw.WriteStringValue(string.Format(_formats[i], row[_fields[i]]));
               }
            }

            jw.WriteEndObject();
            _context.Entity.Inserts++;
            await jw.FlushAsync(token).ConfigureAwait(false);
         }

         jw.WriteEndArray();
         await jw.FlushAsync(token).ConfigureAwait(false);
      }

      private static void WriteValue(Utf8JsonWriter jw, object value) {
         switch (value) {
            case null: jw.WriteNullValue(); break;
            case bool b: jw.WriteBooleanValue(b); break;
            case int i: jw.WriteNumberValue(i); break;
            case long l: jw.WriteNumberValue(l); break;
            case float f: jw.WriteNumberValue(f); break;
            case double d: jw.WriteNumberValue(d); break;
            case decimal dec: jw.WriteNumberValue(dec); break;
            case DateTime dt: jw.WriteStringValue(dt.ToString("o")); break;
            case Guid g: jw.WriteStringValue(g.ToString()); break;
            default: jw.WriteStringValue(value.ToString()); break;
         }
      }
   }
}
