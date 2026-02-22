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
using Transformalize.Extensions;

namespace Transformalize.Providers.Json {

   public class JsonStreamReader : IRead {

      private readonly InputContext _context;
      private readonly Stream _stream;
      private readonly Field[] _fields;
      private readonly IRowFactory _rowFactory;
      private readonly Dictionary<string, Field> _fieldLookup;
      public JsonStreamReader(InputContext context, Stream stream, IRowFactory rowFactory) {
         _context = context;
         _stream = stream;
         _fields = context.GetAllEntityFields().Where(f => f.Input && !f.System).ToArray();
         _fieldLookup = _fields.ToDictionary(f => f.Name, f => f);
         _rowFactory = rowFactory;
      }

      public IEnumerable<IRow> Read() {

         IRow row = null;
         Field field = null;
         var types = Constants.TypeSystem();
         var current = 0;
         var start = 0;
         var end = 0;

         if (_context.Entity.IsPageRequest()) {
            start += (_context.Entity.Page * _context.Entity.Size) - _context.Entity.Size;
            end = start + _context.Entity.Size;
         }

         var textReader = new StreamReader(_stream);
         var reader = new JsonTextReader(textReader);

         var textWriter = new StringWriter();
         var jsonWriter = new JsonTextWriter(textWriter);

         while (reader.Read()) {

            switch (reader.TokenType) {
               case JsonToken.StartObject:
                  if (reader.Depth == 1) {
                     row = _rowFactory.Create();
                  }
                  if (reader.Depth > 1) {
                     jsonWriter.WriteStartObject();
                  }
                  break;
               case JsonToken.EndObject:
                  if (reader.Depth == 1) {
                     if (end == 0 || current.Between(start, end)) {
                        yield return row;
                     }
                     ++current;
                     if (current == end) {
                        yield break;
                     }
                  }
                  if (reader.Depth > 1 && field != null) {
                     jsonWriter.WriteEndObject();
                     jsonWriter.Flush();
                     row[field] = textWriter.ToString();
                     field = null;
                     textWriter = new StringWriter();
                     jsonWriter = new JsonTextWriter(textWriter);
                  }
                  break;
               case JsonToken.StartArray:
                  if(reader.Depth > 0) {
                     jsonWriter.WriteStartArray();
                  }
                  break;
               case JsonToken.EndArray:
                  if(reader.Depth > 0) {
                     jsonWriter.WriteEndArray();
                  }
                  break;
               case JsonToken.PropertyName:
                  var name = (string)reader.Value;
                  if (_fieldLookup.ContainsKey(name)) {
                     if (reader.Depth == 2) {
                        field = _fieldLookup[name];
                     }
                  }
                  if (reader.Depth > 2) {
                     jsonWriter.WritePropertyName(name);
                  }
                  break;
               default:
                  if (reader.Depth == 2 && reader.Value != null && field != null) {
                     if (types[field.Type] == reader.ValueType) {
                        row[field] = reader.Value;
                     } else {
                        row[field] = field.Convert(reader.Value);
                     }
                     field = null;
                  }
                  if (reader.Depth > 2 && reader.Value != null) {
                     jsonWriter.WriteValue(reader.Value);
                  }
                  break;
            }
         }

         _context.Entity.Hits = current;
      }
   }
}
