#region license
// Transformalize
// A Configurable ETL solution specializing in incremental denormalization.
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
using System.Dynamic;
using Pipeline.Configuration;
using Pipeline.Contracts;
using Pipeline.Transforms;
using RazorEngine;
using RazorEngine.Configuration;
using RazorEngine.Templating;
using RazorEngine.Text;

namespace Pipeline.Template.Razor {
    public class RazorTransform : BaseTransform, ITransform {

        private readonly IRazorEngineService _service;
        private readonly Field[] _input;
        private readonly bool _compiled;

        public RazorTransform(IContext context) : base(context) {
            _input = MultipleInput();

            var config = new TemplateServiceConfiguration {
                DisableTempFileLocking = true,
                EncodedStringFactory = Context.Transform.ContentType == "html"
                    ? (IEncodedStringFactory)new HtmlEncodedStringFactory()
                    : new RawStringFactory(),
                Language = Language.CSharp,
                CachingProvider = new DefaultCachingProvider(t => { })
            };

            _service = RazorEngineService.Create(config);
            try {
                _service.Compile(Context.Transform.Template, Context.Key, typeof(ExpandoObject));
                _compiled = true;
            } catch (Exception ex) {
                Context.Warn(Context.Transform.Template.Replace("{", "{{").Replace("}", "}}"));
                Context.Warn(ex.Message);
                throw;
            }
        }

        public IRow Transform(IRow row) {
            if (_compiled)
                row[Context.Field] = Context.Field.Convert(_service.Run(Context.Key, typeof(ExpandoObject), row.ToFriendlyExpandoObject(_input)));
            Increment();
            return row;
        }
    }
}