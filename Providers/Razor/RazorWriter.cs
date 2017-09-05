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
