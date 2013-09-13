#region License

// /*
// Transformalize - Replicate, Transform, and Denormalize Your Data...
// Copyright (C) 2013 Dale Newman
// 
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with this program.  If not, see <http://www.gnu.org/licenses/>.
// */

#endregion

using System.Collections.Generic;
using System.Dynamic;
using Transformalize.Libs.RazorEngine;
using Transformalize.Libs.RazorEngine.Configuration.Fluent;
using Transformalize.Libs.RazorEngine.Templating;
using Transformalize.Main.Template_;

namespace Transformalize.Main
{
    public class Template
    {
        private readonly Process _process;
        public List<TemplateAction> Actions = new List<TemplateAction>();
        public Dictionary<string, object> Settings = new Dictionary<string, object>();

        public Template(string name, string content, string file, string contentType, Process process)
        {
            File = file;
            Name = name;
            Content = content;
            ContentType = contentType.Equals("raw") ? Encoding.Raw : Encoding.Html;
            _process = process;
        }

        public string Content { get; private set; }
        public string Name { get; private set; }
        public string File { get; private set; }
        public Encoding ContentType { get; private set; }

        public string Render()
        {
            var config = new FluentTemplateServiceConfiguration(c => c.WithEncoding(ContentType));
            var templateService = new TemplateService(config);
            Razor.SetTemplateService(templateService);

            var settings = new ExpandoObject();
            foreach (var setting in Settings)
            {
                ((IDictionary<string, object>) settings).Add(setting.Key, setting.Value);
            }
            ((IDictionary<string, object>) settings).Add("Process", _process);

            return Razor.Parse(Content, new
                                            {
                                                Process = _process,
                                                Settings = settings
                                            });
        }
    }
}