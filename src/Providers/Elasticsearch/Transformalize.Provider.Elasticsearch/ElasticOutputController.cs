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

using System.Diagnostics;
using Elasticsearch.Net;
using Transformalize.Context;
using Transformalize.Contracts;
using Transformalize.Impl;

namespace Transformalize.Providers.Elasticsearch {

    public class ElasticOutputController : BaseOutputController {

        private readonly IElasticLowLevelClient _client;
        private readonly Stopwatch _stopWatch;

        public ElasticOutputController(
            OutputContext context,
            IAction initializer,
            IInputProvider inputProvider,
            IOutputProvider outputProvider,
            IElasticLowLevelClient client
            ) : base(
                context,
                initializer,
                inputProvider,
                outputProvider
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
        }

    }
}
