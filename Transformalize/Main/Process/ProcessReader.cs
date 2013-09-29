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

using System;
using System.Linq;
using Transformalize.Configuration;
using Transformalize.Extensions;
using Transformalize.Libs.NLog;
using Transformalize.Libs.RazorEngine;
using Transformalize.Main.Providers;

namespace Transformalize.Main {

    public class ProcessReader : IReader<Process> {
        private readonly ProcessConfigurationElement _config;
        private readonly Logger _log = LogManager.GetCurrentClassLogger();
        private readonly Options _options;
        private readonly string _processName = string.Empty;
        private Process _process;

        public ProcessReader(ProcessConfigurationElement process, Options options) {
            _config = Adapt(process);
            _options = options;
        }

        public Process Read() {
            if (_config == null) {
                _log.Error("Sorry.  I can't find a process named {0}.", _processName);
                Environment.Exit(1);
            }

            _process = new Process(_config.Name) {
                Options = _options,
                TemplateContentType = _config.TemplateContentType.Equals("raw") ? Encoding.Raw : Encoding.Html,
                Providers = new ProviderReader(_config.Providers).Read()
            };

            //shared across the process
            _process.Connections = new ConnectionFactory(_process, _config.Connections).Create();
            _process.OutputConnection = _process.Connections["output"];

            _process.Scripts = new ScriptReader(_config.Scripts).Read();
            _process.Templates = new TemplateReader(_process, _config.Templates).Read();
            _process.SearchTypes = new SearchTypeReader(_config.SearchTypes).Read();
            new MapLoader(ref _process, _config.Maps).Load();

            //depend on the shared process properties
            new EntitiesLoader(ref _process, _config.Entities).Load();
            _process.Relationships = new RelationshipsReader(_process, _config.Relationships).Read();
            new ProcessCalculatedFieldLoader(ref _process, _config.CalculatedFields).Load();
            _process.Star = string.IsNullOrEmpty(_config.Star) ? _process.Name + "Star" : _config.Star;

            new EntityRelationshipLoader(ref _process).Load();

            Summarize();

            return _process;
        }

        private static ProcessConfigurationElement Adapt(ProcessConfigurationElement process) {
            new TransformFieldsToParametersAdapter(process).Adapt("fromxml");
            new TransformFieldsMoveAdapter(process).Adapt("fromxml");
            new TransformFieldsToParametersAdapter(process).Adapt("fromregex");
            new TransformFieldsMoveAdapter(process).Adapt("fromregex");
            return process;
        }

        private void Summarize() {

            if (!_log.IsDebugEnabled)
                return;

            _log.Debug("Process Loaded.");
            _log.Debug("{0} Provider{1}.", _process.Providers.Count, _process.Providers.Count.Plural());
            _log.Debug("{0} Connection{1}.", _process.Connections.Count, _process.Connections.Count.Plural());
            _log.Debug("{0} Entit{1}.", _process.Entities.Count, _process.Entities.Count.Pluralize());
            _log.Debug("{0} Relationship{1}.", _process.Relationships.Count, _process.Relationships.Count.Plural());
            _log.Debug("{0} Script{1}.", _process.Scripts.Count, _process.Scripts.Count.Plural());
            _log.Debug("{0} Template{1}.", _process.Templates.Count, _process.Templates.Count.Plural());
            _log.Debug("{0} SearchType{1}.", _process.SearchTypes.Count, _process.SearchTypes.Count.Plural());

            var mapCount = _process.MapStartsWith.Count + _process.MapEquals.Count + _process.MapEndsWith.Count;
            _log.Debug("{0} Map{1}.", mapCount, mapCount.Plural());

            var transformCount = _process.CalculatedFields.ToEnumerable().Sum(f => f.Transforms.Count) + _process.Entities.Sum(e => e.CalculatedFields.ToEnumerable().Sum(f => f.Transforms.Count) + e.Fields.ToEnumerable().Sum(f => f.Transforms.Count));
            _log.Debug("{0} Transform{1}.", transformCount, transformCount.Plural());
        }

    }
}