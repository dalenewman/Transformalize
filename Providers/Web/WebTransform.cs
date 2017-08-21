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
using Transformalize.Actions;
using Transformalize.Configuration;
using Transformalize.Contracts;
using Transformalize.Transforms;

namespace Transformalize.Providers.Web {
    public class WebTransform : BaseTransform {
        private readonly Func<IRow, Configuration.Action> _getAction;
        private readonly Func<Configuration.Action, ActionResponse> _getResponse;
        private readonly Field _input;

        public WebTransform(IContext context) : base(context, "string") {
            Field url;
            _input = SingleInput();

            if (_input.Equals(Context.Field)) {
                // the url in this field will be transformed inline a web response
                _getAction = row => new Configuration.Action { Type = "web", Url = row[Context.Field] as string, Method = context.Transform.WebMethod, Body = context.Transform.Body };
            } else {
                if (context.Entity.TryGetField(context.Transform.Url, out url)) {
                    // url referenced a field
                    _getAction = row => new Configuration.Action { Type = "web", Url = row[url] as string, Method = context.Transform.WebMethod, Body = context.Transform.Body };
                } else {
                    if (context.Transform.Url != string.Empty) {
                        // url is literal url
                        _getAction = row => new Configuration.Action { Type = "web", Url = context.Transform.Url, Method = context.Transform.WebMethod, Body = context.Transform.Body };
                    } else {
                        // the url as copied in via a parameter (aka copy(url))
                        _getAction = row => new Configuration.Action { Type = "web", Url = row[_input] as string, Method = context.Transform.WebMethod, Body = context.Transform.Body };
                    }
                }
            }

            if (context.Transform.WebMethod == "GET") {
                _getResponse = WebAction.Get;
            } else {
                _getResponse = WebAction.Post;
            }
        }

        public override IRow Transform(IRow row) {
            var response = _getResponse(_getAction(row));
            Increment();
            row[Context.Field] = response.Message;
            return row;
        }
    }
}
