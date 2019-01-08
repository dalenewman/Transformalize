#region license
// Transformalize
// Configurable Extract, Transform, and Load
// Copyright 2013-2019 Dale Newman
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
using Autofac;
using Transformalize.Actions;
using Transformalize.Configuration;
using Transformalize.Ioc.Autofac;
using Transformalize.Logging;

namespace Tests {
    public class TestBase {

        public static IDictionary<string, string> InitMode() {
            return new Dictionary<string, string> { { "Mode", "init" } };
        }

        public static Process ResolveRoot(IContainer container, string cfg, IDictionary<string, string> parameters = null) {
            if (parameters == null) {
                parameters = new Dictionary<string, string>();
            }
            return container.Resolve<Process>(new NamedParameter("cfg", cfg), new NamedParameter("parameters", parameters));
        }

        public static ActionResponse Execute(Process process, string placeHolderStyle = "@()") {
            var container = DefaultContainer.Create(process, new DebugLogger(), placeHolderStyle);
            return new PipelineAction(container).Execute();
        }
    }
}