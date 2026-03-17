#region license
// Transformalize
// Configurable Extract, Transform, and Load
// Copyright 2013-2017 Dale Newman
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
using System.Linq;
using System.Text;
using System.Text.Json;
using Elastic.Transport;
using Transformalize.Configuration;
using Transformalize.Context;
using Transformalize.Contracts;
using Transformalize.Extensions;
using System.Threading;
using System.Threading.Tasks;

namespace Transformalize.Providers.Elasticsearch {

   public class ElasticWriter : IWrite {

      readonly OutputContext _context;
      readonly ITransport _client;
      readonly string _prefix;
      readonly AliasField[] _fields;
      private readonly JsonSerializerOptions _options = new JsonSerializerOptions {
         Converters = { new DecimalToDoubleConverter() }
      };

      private sealed class DecimalToDoubleConverter : System.Text.Json.Serialization.JsonConverter<decimal> {
         public override decimal Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) => reader.GetDecimal();
         public override void Write(Utf8JsonWriter writer, decimal value, JsonSerializerOptions options) {
            // Format with explicit decimal point so JSON parsers treat as floating-point (not integer)
            var s = value.ToString("0.0###########################", System.Globalization.CultureInfo.InvariantCulture);
            writer.WriteRawValue(s);
         }
      }

      private class AliasField {
         public string Alias { get; set; }
         public Field Field { get; set; }
      }

      public ElasticWriter(OutputContext context, ITransport client) {
         _context = context;
         _client = client;
         _prefix = "{\"index\": {\"_index\": \"" + context.Connection.Index + "\", \"_id\": \"";
         _fields = context.OutputFields.Select(f => new AliasField { Alias = f.Alias.ToLower(), Field = f }).ToArray();
      }

      public void Write(IEnumerable<IRow> rows) {
         var builder = new StringBuilder();
         var fullCount = 0;
         var batchCount = (uint)0;

         foreach (var part in rows.Partition(_context.Entity.InsertSize)) {
            foreach (var row in part) {
               batchCount++;
               fullCount++;
               foreach (var af in _fields) {

                  switch (af.Field.Type) {
                     case "guid":
                        row[af.Field] = ((Guid)row[af.Field]).ToString();
                        break;
                     case "datetime":
                        row[af.Field] = ((DateTime)row[af.Field]).ToString("o");
                        break;
                  }
                  if (af.Field.SearchType == "geo_point") {
                     var gp = row[af.Field].ToString();
                     row[af.Field] = new Dictionary<string, string> {
                        { "text", gp },
                        { "location", gp }
                     };
                  }
               }

               builder.Append(_prefix);
               foreach (var key in _fields.Where(af => af.Field.PrimaryKey)) {
                  builder.Append(row[key.Field]);
               }
               builder.AppendLine("\"}}");
               builder.AppendLine(JsonSerializer.Serialize(_fields.ToDictionary(af => af.Alias, af => row[af.Field]), _options));
            }

            var bulkPath = new EndpointPath(HttpMethod.POST, "/_bulk?refresh=true");
            var response = _client.Request<DynamicResponse>(in bulkPath, PostData.String(builder.ToString()));

            if (response.ApiCallDetails.HasSuccessfulStatusCode) {
               var count = batchCount;
               _context.Entity.Inserts += count;
               _context.Debug(() => $"{count} to output");
            } else {
               _context.Error(response.ApiCallDetails.DebugInformation.Replace("{", "{{").Replace("}", "}}"));
            }
            builder.Clear();
            batchCount = 0;
         }

         _context.Info($"{fullCount} to output");
      }

      public async Task WriteAsync(IEnumerable<IRow> rows, CancellationToken token = default) {
         var builder = new StringBuilder();
         var fullCount = 0;
         var batchCount = (uint)0;

         foreach (var part in rows.Partition(_context.Entity.InsertSize)) {
            foreach (var row in part) {
               batchCount++;
               fullCount++;
               foreach (var af in _fields) {

                  switch (af.Field.Type) {
                     case "guid":
                        row[af.Field] = ((Guid)row[af.Field]).ToString();
                        break;
                     case "datetime":
                        row[af.Field] = ((DateTime)row[af.Field]).ToString("o");
                        break;
                  }
                  if (af.Field.SearchType == "geo_point") {
                     var gp = row[af.Field].ToString();
                     row[af.Field] = new Dictionary<string, string> {
                        { "text", gp },
                        { "location", gp }
                     };
                  }
               }

               builder.Append(_prefix);
               foreach (var key in _fields.Where(af => af.Field.PrimaryKey)) {
                  builder.Append(row[key.Field]);
               }
               builder.AppendLine("\"}}");
               builder.AppendLine(JsonSerializer.Serialize(_fields.ToDictionary(af => af.Alias, af => row[af.Field]), _options));
            }

            var asyncBulkPath = new EndpointPath(HttpMethod.POST, "/_bulk?refresh=true");
            var response = await _client.RequestAsync<DynamicResponse>(in asyncBulkPath, PostData.String(builder.ToString()), token).ConfigureAwait(false);

            if (response.ApiCallDetails.HasSuccessfulStatusCode) {
               var count = batchCount;
               _context.Entity.Inserts += count;
               _context.Debug(() => $"{count} to output");
            } else {
               _context.Error(response.ApiCallDetails.DebugInformation.Replace("{", "{{").Replace("}", "}}"));
            }
            builder.Clear();
            batchCount = 0;
         }

         _context.Info($"{fullCount} to output");
      }
   }
}
