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
using System.IO;
using System.Linq;
using Cfg.Net.Contracts;
using RazorEngine;
using RazorEngine.Configuration;
using RazorEngine.Templating;
using RazorEngine.Text;
using Transformalize.Contracts;

namespace Transformalize.Providers.Razor {

    public class RazorWriter : IWrite {

        private readonly IConnectionContext _output;
        private readonly IReader _templateReader;
        private readonly TemplateServiceConfiguration _config;

        public RazorWriter(IConnectionContext output, IReader templateReader) {
            _templateReader = templateReader;
            _output = output;
            _config = new TemplateServiceConfiguration {
                EncodedStringFactory = output.Connection.ContentType == "html" ? (IEncodedStringFactory)new HtmlEncodedStringFactory() : new RawStringFactory(),
                Language = Language.CSharp,
                CachingProvider = new DefaultCachingProvider(t => { })
            };
        }

        public void Write(IEnumerable<IRow> rows) {
            var l = new Cfg.Net.Loggers.MemoryLogger();
            _output.Debug(()=>$"Loading template {_output.Connection.Template}");
            var template = _templateReader.Read(_output.Connection.Template, new Dictionary<string, string>(), l);

            if (l.Errors().Any()) {
                foreach (var error in l.Errors()) {
                    _output.Error(error);
                }
            } else {
                using (var service = RazorEngineService.Create(_config)) {
                    File.WriteAllText(_output.Connection.File, service.RunCompile(template, _output.Connection.Name, typeof(RazorModel), new RazorModel(_output.Entity, rows)));
                }
            }
        }

    }
}
