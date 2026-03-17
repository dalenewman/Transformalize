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

using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using Elastic.Transport;
using Transformalize.Actions;
using Transformalize.Context;
using Transformalize.Contracts;
using System.Threading;
using System.Threading.Tasks;

namespace Transformalize.Providers.Elasticsearch {
   public class ElasticEntityInitializer : IAction {

      private readonly OutputContext _context;
      private readonly ITransport _client;

      private static string TranslateType(string type) {
         switch (type) {
            case "int64":
               return "long";
            case "int16":
               return "short";
            case "int":
            case "int32":
               return "integer";
            case "datetime":
            case "time":
               return "date";
            case "bool":
               return "boolean";
            case "decimal":
               return "double";
            case "single":
               return "float";
            case "byte[]":
               return "binary";
            case "guid":
            case "char":
               return "string"; // dealt with later
            default:
               return type;
         }
      }

      private readonly List<string> _analyzers = new List<string> {
            "standard",
            "simple",
            "whitespace",
            "keyword",
            "pattern",
            "snowball",
            "arabic",
            "armenian",
            "basque",
            "brazilian",
            "bulgarian",
            "catalan",
            "chinese",
            "cjk",
            "czech",
            "danish",
            "dutch",
            "english",
            "finnish",
            "french",
            "galician",
            "german",
            "greek",
            "hindi",
            "hungarian",
            "indonesian",
            "italian",
            "norwegian",
            "persian",
            "portuguese",
            "romanian",
            "russian",
            "spanish",
            "swedish",
            "turkish",
            "thai",
            string.Empty
        };


      public ElasticEntityInitializer(OutputContext context, ITransport client) {
         _context = context;
         _client = client;
      }

      public ActionResponse Execute() {
         _context.Warn("Initializing");

         var version = ElasticVersionParser.ParseVersion(_context);

         var properties = new Dictionary<string, object> { { "properties", GetFields() } };
         var typeName = _context.Entity.Alias.ToLower();
         var json = JsonSerializer.Serialize(properties);

         DynamicResponse elasticResponse;

         var putMappingPath = new EndpointPath(HttpMethod.PUT, $"/{_context.Connection.Index}/_mapping");
         elasticResponse = _client.Request<DynamicResponse>(in putMappingPath, PostData.String(json));

         var response = new ActionResponse(
            (int?)elasticResponse.ApiCallDetails?.HttpStatusCode ?? 500,
            elasticResponse.ApiCallDetails?.OriginalException == null ? string.Empty : (elasticResponse.ApiCallDetails?.DebugInformation ?? string.Empty).Replace("{", "{{").Replace("}", "}}")
         ) {
            Action = new Configuration.Action() {
               Type = "internal",
               ErrorMode = "continue",
               Description = $"Initialize {typeName} entity."
            }
         };

         return response;
      }

      private Dictionary<string, object> GetFields() {

         var version = ElasticVersionParser.ParseVersion(_context);

         var fields = new Dictionary<string, object>();
         foreach (var field in _context.OutputFields) {

            var alias = field.Alias.ToLower();
            var searchType = _context.Process.SearchTypes.First(st => st.Name == field.SearchType);
            var analyzer = searchType.Analyzer;

            var type = TranslateType(field.Type);

            if (field.Type.Equals("string")) {

               // by default, searchType.Type defers, but on occassion (e.g. geo_point), it takes over
               type = searchType.Type == "defer" ? type : searchType.Type;

               if (_analyzers.Contains(analyzer)) {

                  // handle things that are not analyzed
                  if (analyzer.Equals(string.Empty)) {

                     if (type.Equals("geo_point")) {
                        fields[alias] = new Dictionary<string, object> {
                           { "properties", new Dictionary<string,object> {{ "location", new Dictionary<string,object> { {"type","geo_point"} } } }}
                        };
                     } else {
                        fields[alias] = new Dictionary<string, object> {
                           { "type", version.Major >= 5 ? "keyword" : "string" },
                           { "store", searchType.Store }
                        };
                     }

                  } else {

                     if (version.Major >= 5) {

                        // version 5+ use keyword and text types instead of string
                        if (type == "string") {
                           if (analyzer == "keyword") {
                              fields[alias] = new Dictionary<string, object> {
                                 { "type", "keyword" }
                              };
                           } else {
                              fields[alias] = new Dictionary<string, object> {
                                 { "type", "text" },
                                 { "analyzer", analyzer },
                                 { "store", searchType.Store }
                              };
                           }
                        } else {
                           fields[alias] = new Dictionary<string, object> {
                              { "type", type },
                              { "analyzer", analyzer }
                           };
                        }

                     } else {  // versions prior to 5 uses string types and keyword analyzer
                        fields[alias] = new Dictionary<string, object> {
                           { "type", type },
                           { "analyzer", analyzer },
                           { "store", searchType.Store }
                        };
                     }
                  }

               } else {
                  //TODO: MOVE THIS INTO VALIDATION
                  _context.Warn("Analyzer '{0}' specified in search type '{1}' is not supported.  Please use a built-in analyzer for Elasticsearch.", analyzer, field.SearchType);
                  if (!fields.ContainsKey(alias)) {
                     fields[alias] = new Dictionary<string, object> {
                                { "type", type }
                            };
                  }
               }
            } else {
               fields[alias] = new Dictionary<string, object> { { "type", type } };
            }
         }

         if (!fields.ContainsKey("tflbatchid")) {
            fields.Add("tflbatchid", new Dictionary<string, object> { { "type", "integer" } });
         }

         return fields;
      }


   public async Task<ActionResponse> ExecuteAsync(CancellationToken token = default) {
         _context.Warn("Initializing");

         var version = ElasticVersionParser.ParseVersion(_context);

         var properties = new Dictionary<string, object> { { "properties", GetFields() } };
         var typeName = _context.Entity.Alias.ToLower();
         var json = JsonSerializer.Serialize(properties);

         var putMappingPath = new EndpointPath(HttpMethod.PUT, $"/{_context.Connection.Index}/_mapping");
         var elasticResponse = await _client.RequestAsync<DynamicResponse>(in putMappingPath, PostData.String(json), token).ConfigureAwait(false);

         var response = new ActionResponse(
            (int?)elasticResponse.ApiCallDetails?.HttpStatusCode ?? 500,
            elasticResponse.ApiCallDetails?.OriginalException == null ? string.Empty : (elasticResponse.ApiCallDetails?.DebugInformation ?? string.Empty).Replace("{", "{{").Replace("}", "}}")
         ) {
            Action = new Configuration.Action() {
               Type = "internal",
               ErrorMode = "continue",
               Description = $"Initialize {typeName} entity."
            }
         };

         return response;
      }
   }
}
