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
using System.Configuration;
using System.Linq;
using Transformalize.Configuration;
using Transformalize.Extensions;
using Transformalize.Libs.Ninject;
using Transformalize.Libs.NLog;
using Transformalize.Libs.NLog.Targets;
using Transformalize.Libs.RazorEngine;
using Transformalize.Main.Providers;
using Transformalize.Main.Providers.File;
using Transformalize.Main.Transform;

namespace Transformalize.Main {

    public class ProcessReader : IReader<Process> {

        private readonly ProcessConfigurationElement _element;
        private readonly Logger _log = LogManager.GetLogger("tfl");
        private readonly Options _options;
        private readonly string _processName = string.Empty;
        private Process _process;
        private readonly string[] _transformToFields = new[] { "fromxml", "fromregex", "fromjson", "fromsplit" };

        public ProcessReader(ProcessConfigurationElement process, Options options) {
            AddShortHandTransforms(process);
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
                Kernal = new StandardKernel(new NinjectBindings(_element))
            };

            // options mode overrides process node
            if (_options.Mode != Common.DefaultValue && _options.Mode != _process.Mode) {
                _process.Mode = _options.Mode;
            }
            _log.Info("Mode: {0}", _process.Mode);

            //shared across the process
            var connectionFactory = new ConnectionFactory(_process);
            foreach (ProviderConfigurationElement element in _element.Providers) {
                connectionFactory.Providers[element.Name.ToLower()] = element.Type;
            }
            _process.Connections = connectionFactory.Create(_element.Connections);
            if (!_process.Connections.ContainsKey("output")) {
                _log.Warn("No output connection detected.  Defaulting to internal.");
                _process.OutputConnection = connectionFactory.Create(new ConnectionConfigurationElement() { Name = "output", Provider = "internal" });
            } else {
                _process.OutputConnection = _process.Connections["output"];
            }

            _process.Scripts = new ScriptReader(_element.Scripts).Read();
            _process.Actions = new ActionReader(_process).Read(_element.Actions);
            _process.Templates = new TemplateReader(_process, _element.Templates).Read();
            _process.SearchTypes = new SearchTypeReader(_element.SearchTypes).Read();
            new MapLoader(ref _process, _element.Maps).Load();

            //these depend on the shared process properties
            new EntitiesLoader(ref _process, _element.Entities).Load();
            new OperationsLoader(ref _process, _element.Entities).Load();

            _process.Relationships = new RelationshipsReader(_process, _element.Relationships).Read();
            new ProcessOperationsLoader(ref _process, _element.CalculatedFields).Load();
            new EntityRelationshipLoader(ref _process).Load();

            LoadLogConfiguration(_process.Options);

            Summarize();

            return _process;
        }

        private void LoadLogConfiguration(Options options) {

            _process.LogRows = _element.Log.Rows;

            if (options.MemoryTarget != null) {
                _process.Log.Add(new Log {
                    Provider = ProviderType.Internal,
                    Name = "memory",
                    MemoryTarget = options.MemoryTarget,
                    Level = options.LogLevel
                });
            }

            if (_element.Log.Count <= 0)
                return;

            foreach (LogConfigurationElement element in _element.Log) {
                var log = new Log {
                    Name = element.Name,
                    Subject = element.Subject,
                    From = element.From,
                    To = element.To,
                    Layout = element.Layout,
                    File = element.File
                };

                if (element.Connection != Common.DefaultValue) {
                    if (_process.Connections.ContainsKey(element.Connection)) {
                        log.Connection = _process.Connections[element.Connection];
                        if (log.Connection.Type == ProviderType.File && log.File.Equals(Common.DefaultValue)) {
                            log.File = log.Connection.File;
                        }
                    } else {
                        throw new TransformalizeException("You are referencing an invalid connection name in your log configuration.  {0} is not confiured in <connections/>.", element.Connection);
                    }
                }

                try {
                    log.Level = LogLevel.FromString(element.LogLevel);
                    log.Provider = (ProviderType)Enum.Parse(typeof(ProviderType), element.Provider, true);
                } catch (Exception ex) {
                    throw new TransformalizeException("Log configuration invalid. {0}", ex.Message);
                }
                _process.Log.Add(log);

            }
        }

        private static ProcessConfigurationElement Adapt(ProcessConfigurationElement process, IEnumerable<string> transformToFields) {

            foreach (var field in transformToFields) {
                while (new TransformFieldsToParametersAdapter(process).Adapt(field) > 0) {
                    new TransformFieldsMoveAdapter(process).Adapt(field);
                };
            }

            return process;
        }

        /// <summary>
        /// Converts t attribute to configuration items for the whole process
        /// </summary>
        /// <param name="process"></param>
        private static void AddShortHandTransforms(ProcessConfigurationElement process) {
            foreach (EntityConfigurationElement entity in process.Entities) {
                foreach (FieldConfigurationElement field in entity.Fields) {
                    AddShortHandTransforms(field);
                }
                foreach (FieldConfigurationElement field in entity.CalculatedFields) {
                    AddShortHandTransforms(field);
                }
            }
            foreach (FieldConfigurationElement field in process.CalculatedFields) {
                AddShortHandTransforms(field);
            }
        }

        /// <summary>
        /// Converts t attribute to configuration items for one field.
        /// </summary>
        /// <param name="f">the field</param>
        private static void AddShortHandTransforms(FieldConfigurationElement f) {
            var transforms = new List<TransformConfigurationElement>(Common.Split(f.ShortHand, ';').Where(t => !string.IsNullOrEmpty(t)).Select(ShortHandFactory.Interpret));
            var collection = new TransformElementCollection();
            foreach (var transform in transforms) {
                foreach (FieldConfigurationElement field in transform.Fields) {
                    AddShortHandTransforms(field);
                }
                collection.Add(transform);
            }
            foreach (TransformConfigurationElement transform in f.Transforms) {
                foreach (FieldConfigurationElement field in transform.Fields) {
                    AddShortHandTransforms(field);
                }
                collection.Add(transform);
            }
            f.Transforms = collection;
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
            _log.Debug("{0} Log{0}", _process.Log.Count, _process.Log.Count.Plural());

            var mapCount = _process.MapStartsWith.Count + _process.MapEquals.Count + _process.MapEndsWith.Count;
            _log.Debug("{0} Map{1}.", mapCount, mapCount.Plural());
        }
    }
}