#region license
// Transformalize
// Configurable Extract, Transform, and Load
// Copyright 2013-2016 Dale Newman
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
using System.CodeDom.Compiler;
using System.Diagnostics;
using Elasticsearch.Net;
using Pipeline.Context;
using Pipeline.Contracts;
using Pipeline.Provider.Elastic.Ext;

namespace Pipeline.Provider.Elastic {
    public class ElasticOutputController : BaseOutputController {
        private readonly IElasticLowLevelClient _client;
        private readonly Stopwatch _stopWatch;

        public ElasticOutputController(
            OutputContext context,
            IAction initializer,
            IVersionDetector inputVersionDetector,
            IVersionDetector outputVersionDetector,
            IElasticLowLevelClient client
            ) : base(
                context,
                initializer,
                inputVersionDetector,
                outputVersionDetector
                ) {
            _client = client;
            _stopWatch = new Stopwatch();
        }

        public override void End() {
            _stopWatch.Stop();
            Context.Info("Ending {0}", _stopWatch.Elapsed);
        }

        public override void Start() {

            base.Start();

            var body = new {
                aggs = new {
                    b = new {
                        max = new {
                            field = "tflbatchid"
                        }
                    },
                    k = new {
                        max = new {
                            field = "tflkey"
                        }
                    }
                },
                size = 0
            };
            var result = _client.Search<DynamicResponse>(Context.Connection.Index, Context.TypeName(), new PostData<object>(body));

            if (result.Success) {
                var batchId = result.Body["aggregations"]["b"]["value"].Value;
                var key = result.Body["aggregations"]["k"]["value"].Value;
                Context.Entity.BatchId = (batchId == null ? 0 : (int)batchId) + 1;
                Context.Entity.Identity = (key == null ? 0 : (int)key);
                Context.Debug(() => $"Next TflBatchId: {Context.Entity.BatchId}.");
                Context.Debug(() => $"Last TflKey: {Context.Entity.Identity}.");

                var countBody = new { query = new { match_all = new { } } };
                Context.Entity.IsFirstRun = Context.Entity.MinVersion == null && _client.Count<DynamicResponse>(Context.Connection.Index, Context.TypeName(), new PostData<object>(countBody)).Body["count"].Value == (long)0;

            } else {
                Context.Error(result.ServerError.ToString());
                Context.Debug(() => result.DebugInformation);
            }

        }

    }
}
