#region license
// Transformalize
// A Configurable ETL Solution Specializing in Incremental Denormalization.
// Copyright 2013 Dale Newman
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
using System.Runtime.InteropServices;
using Pipeline.Configuration;
using Pipeline.Contracts;
using Pipeline.Transforms;
using RazorEngine;
using RazorEngine.Configuration;
using RazorEngine.Templating;
using RazorEngine.Text;

namespace Pipeline.Template.Razor {

    public class RazorTransform : BaseTransform {

        private readonly IRazorEngineService _service;
        private readonly Field[] _input;
        private static readonly ConcurrentDictionary<int, IRazorEngineService> Cache = new ConcurrentDictionary<int, IRazorEngineService>();

        public RazorTransform(IContext context) : base(context, context.Field.Type) {
            _input = MultipleInput();

            var key = GetHashCode(context.Transform.Template, _input);

            if (!Cache.TryGetValue(key, out _service)) {
                var config = new TemplateServiceConfiguration {
                    DisableTempFileLocking = true,
                    EncodedStringFactory = Context.Transform.ContentType == "html"
                        ? (IEncodedStringFactory)new HtmlEncodedStringFactory()
                        : new RawStringFactory(),
                    Language = Language.CSharp,
                    CachingProvider = new DefaultCachingProvider(t => { })
                };
                _service = RazorEngineService.Create(config);
            }

            try {
                _service.Compile(Context.Transform.Template, Context.Key, typeof(ExpandoObject));
                Cache[key] = _service;
            } catch (Exception ex) {
                Context.Warn(Context.Transform.Template.Replace("{", "{{").Replace("}", "}}"));
                Context.Warn(ex.Message);
                throw;
            }
        }

        public override IRow Transform(IRow row) {
            var output = _service.Run(Context.Key, typeof(ExpandoObject), row.ToFriendlyExpandoObject(_input));
            row[Context.Field] = Context.Field.Convert(output.Trim(' ', '\n', '\r'));
            Increment();
            return row;
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
    }
}