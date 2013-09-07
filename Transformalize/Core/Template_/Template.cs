using System.Collections.Generic;
using System.Dynamic;
using Transformalize.Configuration;
using Transformalize.Libs.RazorEngine.Core;
using Transformalize.Libs.RazorEngine.Core.Configuration.Fluent;
using Transformalize.Libs.RazorEngine.Core.Templating;

namespace Transformalize.Core.Template_
{
    public class Template
    {

        public Dictionary<string, object> Settings = new Dictionary<string, object>();
        public List<TemplateAction> Actions = new List<TemplateAction>();
        private readonly ProcessConfigurationElement _process;
        public string Content { get; private set; }
        public string Name { get; private set; }
        public string File { get; private set; }
        public Encoding ContentType { get; private set; }

        public Template(string name, string content, string file, string contentType, ProcessConfigurationElement process)
        {
            File = file;
            Name = name;
            Content = content;
            ContentType = contentType.Equals("raw") ? Encoding.Raw : Encoding.Html;
            _process = process;

        }

        public string Render()
        {
            var config = new FluentTemplateServiceConfiguration(c => c.WithEncoding(ContentType));
            var templateService = new TemplateService(config);
            Razor.SetTemplateService(templateService);

            var settings = new ExpandoObject();
            foreach (var setting in Settings)
            {
                ((IDictionary<string, object>)settings).Add(setting.Key, setting.Value);
            }
            ((IDictionary<string, object>)settings).Add("Process", _process);

            return Razor.Parse(Content, new {Process = _process, Settings = settings});
        }

    }
}