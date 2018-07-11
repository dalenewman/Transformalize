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
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using RazorEngine;
using RazorEngine.Configuration;
using RazorEngine.Templating;
using RazorEngine.Text;
using Transformalize.Configuration;
using Transformalize.Contracts;
using Transformalize.Transforms;

namespace Transformalize.Providers.Razor {

    public class RazorTransform : BaseTransform {

        private IRazorEngineService _service;
        private Field[] _input;
        private static readonly ConcurrentDictionary<int, IRazorEngineService> Cache = new ConcurrentDictionary<int, IRazorEngineService>();

        public RazorTransform(IContext context = null) : base(context, "object") {

            if (IsMissingContext()) {
                return;
            }

            Returns = Context.Field.Type;

            if (IsMissing(context.Operation.Template)) {
                return;
            }

            if (context.Operation.ContentType == string.Empty) {
                context.Operation.ContentType = "raw"; //other would be html
            }

        }

        public override IRow Operate(IRow row) {
            var output = _service.Run(Context.Key, typeof(ExpandoObject), row.ToFriendlyExpandoObject(_input));
            row[Context.Field] = Context.Field.Convert(output.Trim(' ', '\n', '\r'));
            
            return row;
        }

        public override IEnumerable<IRow> Operate(IEnumerable<IRow> rows) {
            var fileBasedTemplate = Context.Process.Templates.FirstOrDefault(t => t.Name == Context.Operation.Template);

            if (fileBasedTemplate != null) {
                Context.Operation.Template = fileBasedTemplate.Content;
            }

            _input = Context.Entity.GetFieldMatches(Context.Operation.Template).ToArray();
            var key = GetHashCode(Context.Operation.Template, _input);

            if (!Cache.TryGetValue(key, out _service)) {
                var config = new TemplateServiceConfiguration {
                    DisableTempFileLocking = true,
                    EncodedStringFactory = Context.Operation.ContentType == "html"
                        ? (IEncodedStringFactory)new HtmlEncodedStringFactory()
                        : new RawStringFactory(),
                    Language = Language.CSharp,
                    CachingProvider = new DefaultCachingProvider(t => { })
                };
                _service = RazorEngineService.Create(config);
            }

            try {
                RazorEngineServiceExtensions.Compile(_service, (string)Context.Operation.Template, (string)Context.Key, typeof(ExpandoObject));
                Cache[key] = _service;
            } catch (Exception ex) {
                Context.Error(Context.Operation.Template.Replace("{", "{{").Replace("}", "}}"));
                Context.Error(ex.Message);
                Run = false;
            }

            return base.Operate(rows);
        }

        /// <summary>
        /// Jon Skeet's Answer from Stack Overflow
        /// http://stackoverflow.com/questions/263400/what-is-the-best-algorithm-for-an-overridden-system-object-gethashcode
        /// http://eternallyconfuzzled.com/tuts/algorithms/jsw_tut_hashing.aspx
        /// </summary>
        /// <param name="template"></param>
        /// <param name="fields"></param>
        /// <returns></returns>
        private static int GetHashCode(string template, IEnumerable<Field> fields) {
            unchecked {
                var hash = (int)2166136261;
                hash = hash * 16777619 ^ template.GetHashCode();
                foreach (var field in fields) {
                    hash = hash * 16777619 ^ field.Type.GetHashCode();
                }
                return hash;
            }
        }

        public override IEnumerable<OperationSignature> GetSignatures() {
            yield return new OperationSignature("razor") {
                Parameters = new List<OperationParameter> {
                    new OperationParameter("template"),
                    new OperationParameter("content-type","raw")
                }
            };
        }
    }
}