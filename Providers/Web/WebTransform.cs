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
using Transformalize.Actions;
using Transformalize.Contracts;
using Transformalize.Transforms;

namespace Transformalize.Providers.Web {
    public class WebTransform : BaseTransform {

        private readonly Func<IRow, Configuration.Action> _getAction;
        private readonly Func<Configuration.Action, ActionResponse> _getResponse;

        public WebTransform(IContext context = null) : base(context, "object") {

            //TODO: see geocode for making more http requests concurrently and rate gate

            if (IsMissingContext()) {
                return;
            }

            var input = SingleInput();

            Returns = Context.Field.Type;

            if (input.Equals(Context.Field)) {
                // the url in this field will be transformed inline a web response
                _getAction = row => new Configuration.Action { Type = "web", Url = row[Context.Field] as string, Method = Context.Operation.WebMethod, Body = Context.Operation.Body };
            } else {
                if (Context.Entity.TryGetField(Context.Operation.Url, out var url)) {
                    // url referenced a field
                    _getAction = row => new Configuration.Action { Type = "web", Url = row[url] as string, Method = Context.Operation.WebMethod, Body = Context.Operation.Body };
                } else {
                    if (Context.Operation.Url != string.Empty) {
                        // url is literal url
                        _getAction = row => new Configuration.Action { Type = "web", Url = Context.Operation.Url, Method = Context.Operation.WebMethod, Body = Context.Operation.Body };
                    } else {
                        // the url as copied in via a parameter (aka copy(url))
                        _getAction = row => new Configuration.Action { Type = "web", Url = row[input] as string, Method = Context.Operation.WebMethod, Body = Context.Operation.Body };
                    }
                }
            }

            if (Context.Operation.WebMethod == "GET") {
                if (Context.Field.Type == "byte[]") {
                    _getResponse = WebAction.GetData;
                } else {
                    _getResponse = WebAction.Get;
                }

            } else {
                _getResponse = WebAction.Post;
            }
        }

        public override IRow Operate(IRow row) {
            var response = _getResponse(_getAction(row));
            var value = Context.Field.Type == "byte[]" ? (object)response.Data : response.Message;

            row[Context.Field] = value ?? Context.Field.DefaultValue();
            return row;
        }

        public override IEnumerable<OperationSignature> GetSignatures() {
            yield return new OperationSignature("web") {
                Parameters = new List<OperationParameter>(3) {
                    new OperationParameter("url",""),
                    new OperationParameter("web-method", "GET"),
                    new OperationParameter("body", "")
                }
            };
        }
    }
}
