using System.Collections.Generic;
using System.Dynamic;
using Transformalize.Configuration;
using Transformalize.Libs.RazorEngine.Core;

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

        public Template(string name, string content, string file, ProcessConfigurationElement process)
        {
            File = file;
            Name = name;
            Content = content;

            _process = process;
        }

        public string Render()
        {
            var settings = new ExpandoObject();
            foreach (var setting in Settings)
            {
                ((IDictionary<string, object>)settings).Add(setting.Key, setting.Value);
            }

            return Razor.Parse(Content, new {Process = _process, Settings = settings});
        }

    }
}