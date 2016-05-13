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