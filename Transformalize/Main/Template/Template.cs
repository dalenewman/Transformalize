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
using System.IO;
using Transformalize.Configuration;
using Transformalize.Libs.NLog;
using Transformalize.Libs.RazorEngine;
using Transformalize.Libs.RazorEngine.Configuration.Fluent;
using Transformalize.Libs.RazorEngine.Templating;

namespace Transformalize.Main {
    public class Template
    {

        private readonly Logger _log = LogManager.GetCurrentClassLogger();
        private readonly Process _process;
        private readonly string _cacheFolder;
        private readonly string _cacheFile;
        private readonly string _cacheContent;

        public List<TemplateAction> Actions = new List<TemplateAction>();
        public Dictionary<string, object> Settings = new Dictionary<string, object>();
        public string Content { get; private set; }
        public string Name { get; private set; }
        public string File { get; private set; }
        public bool Cache { get; private set; }
        public Encoding ContentType { get; private set; }
        
        public Template(Process process, TemplateConfigurationElement element, string content, string file) {
            File = file;
            Cache = element.Cache;
            Name = element.Name;
            Content = content;
            ContentType = element.ContentType.Equals("raw") ? Encoding.Raw : Encoding.Html;
            _process = process;
            _cacheFolder = Path.Combine(Common.GetTemporaryFolder(_process.Name), "CachedTemplates");
            _cacheFile = new FileInfo(Path.Combine(_cacheFolder, Name + ".txt")).FullName;
            
            if (Cache) {
                if (!Directory.Exists(_cacheFolder)) {
                    Directory.CreateDirectory(_cacheFolder);
                }
                if (System.IO.File.Exists(_cacheFile))
                {
                    _cacheContent = System.IO.File.ReadAllText(_cacheFile);
                }
            }

        }

        public string Render() {

            if (Cache && !string.IsNullOrEmpty(_cacheContent))
            {
                _log.Debug("Returning {0} template output from cache.", Name);
                return _cacheContent;
            }

            var config = new FluentTemplateServiceConfiguration(c => c.WithEncoding(ContentType));
            var templateService = new TemplateService(config);
            Razor.SetTemplateService(templateService);

            var settings = new ExpandoObject();
            foreach (var setting in Settings) {
                ((IDictionary<string, object>)settings).Add(setting.Key, setting.Value);
            }
            ((IDictionary<string, object>)settings).Add("Process", _process);

            var content = Razor.Parse(Content, new {
                Process = _process,
                Settings = settings
            });

            _log.Debug("Rendered {0} template.", Name);
            if (Cache && !string.IsNullOrEmpty(content))
            {
                System.IO.File.WriteAllText(_cacheFile, content);
                _log.Debug("Cached {0} template output.", Name);
            }

            return content;
        }
    }
}