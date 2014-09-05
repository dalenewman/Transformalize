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
using System.IO;
using Transformalize.Configuration;
using Transformalize.Libs.NLog;
using Transformalize.Libs.RazorEngine;
using Transformalize.Libs.RazorEngine.Configuration.Fluent;
using Transformalize.Libs.RazorEngine.Templating;
using Transformalize.Runner;

namespace Transformalize.Main {

    public class Template {
        private const string TEMPLATE_CACHE_FOLDER = "TemplateCache";
        private const string RENDERED_TEMPLATE_CACHE_FOLDER = "RenderedTemplateCache";

        private readonly Logger _log = LogManager.GetLogger("tfl");
        private readonly Process _process;

        private readonly string _renderedTemplateFile;
        private readonly string _renderedTemplateContent;
        private readonly bool _renderedTemplateContentExists;

        private readonly string _templateFile;
        private readonly string _templateContent;
        private readonly bool _templateContentExists;

        public List<TemplateAction> Actions = new List<TemplateAction>();
        public IParameters Parameters = new Parameters.Parameters();
        public Contents Contents { get; private set; }
        public string Name { get; private set; }

        public bool Cache { get; private set; }
        public bool Enabled { get; private set; }
        public Encoding ContentType { get; private set; }
        public bool IsUsedInPipeline { get; set; }
        public bool Conditional { get; set; }

        public Template(Process process, TemplateConfigurationElement element, Contents contents) {

            Cache = element.Cache;
            Enabled = element.Enabled;
            Conditional = element.Conditional;
            Name = element.Name;
            ContentType = element.ContentType.Equals("raw") ? Encoding.Raw : Encoding.Html;
            Contents = contents;

            _process = process;

            _renderedTemplateFile = GetFileName(RENDERED_TEMPLATE_CACHE_FOLDER);
            _renderedTemplateContentExists = TryRead(_renderedTemplateFile, out _renderedTemplateContent);

            _templateFile = GetFileName(TEMPLATE_CACHE_FOLDER);
            _templateContentExists = TryRead(_templateFile, out _templateContent);

        }

        private string GetFileName(string folderName) {
            var folder = Common.GetTemporarySubFolder(_process.Name, folderName);
            return new FileInfo(Path.Combine(folder, Name + ".txt")).FullName;
        }

        private static bool TryRead(string fileName, out string contents) {
            var exists = File.Exists(fileName);
            contents = exists ? File.ReadAllText(fileName) : string.Empty;
            return exists;
        }

        public string Render() {
            if (!CacheIsUsable())
                return CacheContent(RenderContent());

            _log.Debug("Returning {0} template output from cache.", Name);
            return _renderedTemplateContent;
        }

        private bool CacheIsUsable() {
            return Cache &&
                !_process.Mode.StartsWith("init") &&
                _renderedTemplateContentExists &&
                _templateContentExists &&
                _templateContent.Equals(Contents.Content);
        }

        private string RenderContent() {

            if (Contents.Content.Equals(string.Empty)) {
                _log.Warn("Template {0} is empty.", Name);
                return string.Empty;
            }

            var config = new FluentTemplateServiceConfiguration(c => c.WithEncoding(ContentType));
            var templateService = new TemplateService(config);
            Razor.SetTemplateService(templateService);

            var renderedContent = Razor.Parse(Contents.Content, new {
                Process = _process,
                Parameters = Parameters.ToExpandoObject()
            });

            _log.Debug("Rendered {0} template.", Name);
            return renderedContent;
        }

        private string CacheContent(string renderedContent) {
            if (Cache && !string.IsNullOrEmpty(renderedContent)) {
                File.WriteAllText(_renderedTemplateFile, renderedContent);
                File.WriteAllText(_templateFile, Contents.Content);
                _log.Debug("Cached {0} template output.", Name);
            }
            return renderedContent;
        }

    }
}