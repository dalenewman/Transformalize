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
using System.Text;
using System.Text.Json;
using Transformalize.Configuration;
using Transformalize.Context;
using Transformalize.Contracts;
using Transformalize.Extensions;
using System.Threading;
using System.Threading.Tasks;

namespace Transformalize.Providers.Json {

   public class JsonLinesStreamReader : IRead {

      private readonly InputContext _context;
      private readonly Stream _stream;
      private readonly Field[] _fields;
      private readonly IRowFactory _rowFactory;
      private readonly Dictionary<string, Field> _fieldLookup;

      public JsonLinesStreamReader(InputContext context, Stream stream, IRowFactory rowFactory) {
         _context = context;
         _stream = stream;
         _fields = context.GetAllEntityFields().Where(f => f.Input && !f.System).ToArray();
         _fieldLookup = _fields.ToDictionary(f => f.Name, f => f);
         _rowFactory = rowFactory;
      }

      public IEnumerable<IRow> Read() {
         ResetStreamPosition();
         using (var lineReader = new StreamReader(_stream, Encoding.UTF8, true, 1024, true)) {
            return ReadRows(lineReader);
         }
      }

      public async Task<IEnumerable<IRow>> ReadAsync(CancellationToken token = default) {
         token.ThrowIfCancellationRequested();
         ResetStreamPosition();

         var rows = new List<IRow>();
         using (var lineReader = new StreamReader(_stream, Encoding.UTF8, true, 1024, true)) {
            string line;
            while ((line = await lineReader.ReadLineAsync().ConfigureAwait(false)) != null) {
               token.ThrowIfCancellationRequested();
               if (ParseLine(line.TrimEnd('\r'), rows)) {
                  break;
               }
            }
         }

         return rows;
      }

      private List<IRow> ReadRows(StreamReader lineReader) {
         var rows = new List<IRow>();
         string line;

         while ((line = lineReader.ReadLine()) != null) {
            if (ParseLine(line.TrimEnd('\r'), rows)) {
               break;
            }
         }

         return rows;
      }

      private bool ParseLine(string line, List<IRow> rows) {
         var current = _context.Entity.Hits;
         var start = 0;
         var end = 0;

         if (_context.Entity.IsPageRequest()) {
            start += (_context.Entity.Page * _context.Entity.Size) - _context.Entity.Size;
            end = start + _context.Entity.Size;
         }

         if (string.IsNullOrWhiteSpace(line)) {
            return false;
         }

         using var doc = JsonDocument.Parse(line);
         var element = doc.RootElement;

         if (element.ValueKind != JsonValueKind.Object) {
            return false;
         }

         if (end == 0 || current.Between(start, end)) {
            var row = _rowFactory.Create();
            foreach (var prop in element.EnumerateObject()) {
               if (_fieldLookup.TryGetValue(prop.Name, out var field)) {
                  row[field] = field.Convert(ConvertJsonElement(prop.Value));
               }
            }
            rows.Add(row);
         }

         ++current;
         _context.Entity.Hits = current;
         if (current == end) {
            return true;
         }

         return false;
      }

      private static object ConvertJsonElement(JsonElement el) {
         return el.ValueKind switch {
            JsonValueKind.String => el.GetString(),
            JsonValueKind.Number => el.TryGetInt64(out var l) ? (object)l : el.GetDouble(),
            JsonValueKind.True => true,
            JsonValueKind.False => false,
            JsonValueKind.Null => null,
            _ => el.GetRawText()
         };
      }

      private void ResetStreamPosition() {
         if (_stream.CanSeek) {
            _stream.Seek(0, SeekOrigin.Begin);
            _context.Entity.Hits = 0;
         }
      }
   }
}
