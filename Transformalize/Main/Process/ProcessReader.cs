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
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using System.Linq;
using Transformalize.Configuration;
using Transformalize.Extensions;
using Transformalize.Libs.Microsoft.System.Web.Razor.Text;
using Transformalize.Libs.Ninject;
using Transformalize.Libs.NVelocity.App;
using Transformalize.Libs.RazorEngine;
using Transformalize.Logging;
using Transformalize.Main.Providers;
using Transformalize.Main.Providers.File;
using Transformalize.Main.Transform;

namespace Transformalize.Main {

    public class ProcessReader : IReader<Process> {

        private readonly ProcessConfigurationElement _element;
        private readonly Options _options;
        private readonly string _processName = string.Empty;
        private Process _process;
        private readonly string[] _transformToFields = { "fromxml", "fromregex", "fromjson", "fromsplit" };

        public ProcessReader(ProcessConfigurationElement process, ref Options options) {
            ShortHandFactory.ExpandShortHandTransforms(process);
            _element = Adapt(process, _transformToFields);
            _processName = process.Name;
            _options = options;
        }

        public Process Read() {

            if (_element == null) {
                throw new TransformalizeException("Can't find a process named {0}.", _processName);
            }
            _process = new Process(_element.Name) {
                Options = _options,
                TemplateContentType = _element.TemplateContentType.Equals("raw") ? Encoding.Raw : Encoding.Html,
                Enabled = _element.Enabled,
                FileInspectionRequest = _element.FileInspection == null ? new FileInspectionRequest() : _element.FileInspection.GetInspectionRequest(),
                Star = _element.Star,
                View = _element.View,
                Mode = _element.Mode,
                StarEnabled = _element.StarEnabled,
                TimeZone = string.IsNullOrEmpty(_element.TimeZone) ? TimeZoneInfo.Local.Id : _element.TimeZone,
                PipelineThreading = (PipelineThreading)Enum.Parse(typeof(PipelineThreading), _element.PipelineThreading, true),
                Parallel =  _element.Parallel,
                Kernal = new StandardKernel(new NinjectBindings(_element))
            };

            // options mode overrides process node
            if (_options.Mode != Common.DefaultValue && _options.Mode != _process.Mode) {
                _process.Mode = _options.Mode;
            }
            TflLogger.Info(_processName, string.Empty, "Mode is set to {0}", _process.Mode);

            //shared across the process
            var connectionFactory = new ConnectionFactory(_process);
            foreach (ProviderConfigurationElement element in _element.Providers) {
                connectionFactory.Providers[element.Name.ToLower()] = element.Type;
            }
            _process.Connections = connectionFactory.Create(_element.Connections);
            if (!_process.Connections.ContainsKey("output")) {
                TflLogger.Warn(_processName, string.Empty, "No output connection detected.  Defaulting to internal.");
                _process.OutputConnection = connectionFactory.Create(new ConnectionConfigurationElement() { Name = "output", Provider = "internal" });
            } else {
                _process.OutputConnection = _process.Connections["output"];
            }

            //logs set after connections, because they may depend on them
            LoadLogConfiguration(_element, ref _process);

            _process.Scripts = new ScriptReader(_element.Scripts).Read();
            _process.Actions = new ActionReader(_process).Read(_element.Actions);
            _process.Templates = new TemplateReader(_process, _element.Templates).Read();
            _process.SearchTypes = new SearchTypeReader(_element.SearchTypes).Read();
            new MapLoader(ref _process, _element.Maps).Load();

            if (_process.Templates.Any(t => t.Value.Engine.Equals("velocity", StringComparison.OrdinalIgnoreCase))) {
                Velocity.Init();
                _process.VelocityInitialized = true;
            }

            //these depend on the shared process properties
            new EntitiesLoader(ref _process, _element.Entities).Load();
            new OperationsLoader(ref _process, _element.Entities).Load();

            _process.Relationships = new RelationshipsReader(_process, _element.Relationships).Read();
            _process.RelationshipsIndexMode = _element.Relationships.IndexMode;
            new ProcessOperationsLoader(ref _process, _element.CalculatedFields).Load();
            new EntityRelationshipLoader(ref _process).Load();

            return _process;
        }

        private static void LoadLogConfiguration(ProcessConfigurationElement element, ref Process process) {

            process.LogRows = element.Log.Rows;

            if (element.Log.Count == 0)
                return;

            var fileLogs = new List<Log>();

            foreach (LogConfigurationElement logElement in element.Log) {
                var log = MapLog(process, logElement);
                if (log.Provider == ProviderType.File) {
                    fileLogs.Add(log);
                } else {
                    process.Log.Add(log);
                }
            }

            process.Log.AddRange(fileLogs);
        }

        private static Log MapLog(Process process, LogConfigurationElement logElement) {
            var log = new Log {
                Name = logElement.Name,
                Subject = logElement.Subject,
                From = logElement.From,
                To = logElement.To,
                Layout = logElement.Layout,
                File = logElement.File,
                Folder = logElement.Folder,
                Async = logElement.Async
            };

            if (logElement.Connection != Common.DefaultValue) {
                if (process.Connections.ContainsKey(logElement.Connection)) {
                    log.Connection = process.Connections[logElement.Connection];
                    if (log.Connection.Type == ProviderType.File && log.File.Equals(Common.DefaultValue)) {
                        log.File = log.Connection.File;
                    }
                } else {
                    throw new TransformalizeException("You are referencing an invalid connection name in your log configuration.  {0} is not confiured in <connections/>.", logElement.Connection);
                }
            }

            try {
                EventLevel eventLevel;
                Enum.TryParse(logElement.LogLevel, out eventLevel);
                log.Level = eventLevel;
                log.Provider = (ProviderType)Enum.Parse(typeof(ProviderType), logElement.Provider, true);
            } catch (Exception ex) {
                throw new TransformalizeException("Log configuration invalid. {0}", ex.Message);
            }
            return log;
        }

        private static ProcessConfigurationElement Adapt(ProcessConfigurationElement process, IEnumerable<string> transformToFields) {

            foreach (var field in transformToFields) {
                while (new TransformFieldsToParametersAdapter(process).Adapt(field) > 0) {
                    new TransformFieldsMoveAdapter(process).Adapt(field);
                };
            }

            return process;
        }

    }
}