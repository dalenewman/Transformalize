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
using Elasticsearch.Net;
using Newtonsoft.Json;
using Transformalize.Actions;
using Transformalize.Context;
using Transformalize.Contracts;

namespace Transformalize.Providers.Elastic {
    public class ElasticEntityInitializer : IAction {

        readonly OutputContext _context;
        readonly IElasticLowLevelClient _client;

        readonly Dictionary<string, string> _types = new Dictionary<string, string>() {
            {"int64", "long"},
            {"int16","integer"},
            {"int","integer"},
            {"int32","integer"},
            {"datetime","date"},
            {"time","date"},
            {"bool","boolean"},
            {"decimal","float"},
            {"single","double"},
            {"guid","string"},
            {"byte[]","binary"},
            {"char", "string" }
        };

        readonly List<string> _analyzers = new List<string> {
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


        public ElasticEntityInitializer(OutputContext context, IElasticLowLevelClient client) {
            _context = context;
            _client = client;
        }

        public ActionResponse Execute() {
            _context.Warn("Initializing");

            var properties = new Dictionary<string, object> { { "properties", GetFields() } };
            var typeName = _context.Entity.Alias.ToLower();
            var body = new Dictionary<string, object> { { typeName, properties } };
            var json = JsonConvert.SerializeObject(body);

            var elasticResponse = _client.IndicesPutMapping<DynamicResponse>(_context.Connection.Index, typeName, json);
            return new ActionResponse {
                Code = elasticResponse.HttpStatusCode ?? 500,
                Message = elasticResponse.ServerError == null ? string.Empty : elasticResponse.ServerError.Error.Reason ?? string.Empty
            };
        }

        private Dictionary<string, object> GetFields() {
            var fields = new Dictionary<string, object>();
            foreach (var field in _context.OutputFields) {

                var alias = field.Alias.ToLower();
                var type = _types.ContainsKey(field.Type) ? _types[field.Type] : field.Type;
                if (field.Type.Equals("string")) {

                    var searchType = _context.Process.SearchTypes.First(st => st.Name == field.SearchType);
                    var analyzer = searchType.Analyzer;
                    type = searchType.Type == "defer" ? type : searchType.Type;

                    if (_analyzers.Contains(analyzer)) {
                        if (analyzer.Equals(string.Empty)) {
                            if (type.Equals("geo_point")) {
                                fields[alias] = new Dictionary<string, object> {
                                    { "properties", new Dictionary<string,object> {{ "location", new Dictionary<string,object> { {"type","geo_point"} } } }}
                                };
                            } else {
                                fields[alias] = new Dictionary<string, object> {
                                    { "type", type }
                                };
                            }
                        } else {
                            if (_context.Connection.Version.StartsWith("5", System.StringComparison.Ordinal) && analyzer == "keyword" && type == "string") {
                                fields[alias] = new Dictionary<string, object> {
                                    { "type", "keyword" }
                                };
                            } else {
                                fields[alias] = new Dictionary<string, object> {
                                    { "type", type },
                                    { "analyzer", analyzer }
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

    }
}
