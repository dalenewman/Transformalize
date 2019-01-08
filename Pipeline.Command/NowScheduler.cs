#region license
// Transformalize
// Configurable Extract, Transform, and Load
// Copyright 2013-2019 Dale Newman
//  
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//   
//       http://www.apache.org/licenses/LICENSE-2.0
//   
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
#endregion
using System;
using System.Linq;
using Autofac;
using Transformalize.Configuration;
using Transformalize.Context;
using Transformalize.Contracts;
using Transformalize.Ioc.Autofac;

namespace Transformalize.Command {
    public class NowScheduler : IScheduler {

        private readonly Options _options;
        private readonly ISchemaHelper _schemaHelper;
        private readonly IPipelineLogger _logger;

        public NowScheduler(Options options, ISchemaHelper schemaHelper, IPipelineLogger logger) {
            _options = options;
            _schemaHelper = schemaHelper;
            _logger = logger;
        }

        public void Start() {

            using (var outer = ConfigurationContainer.Create(_options.Arrangement, _logger, null, _options.PlaceHolderStyle)) {
                var process = outer.Resolve<Configuration.Process>();
                var log = new PipelineContext(_logger, process);

                if (process.Entities.Any(e => process.Connections.First(c => c.Name == e.Connection).Provider != Constants.DefaultSetting && !e.Fields.Any(f => f.Input))) {
                    log.Debug(() => "Detecting schema...");
                    if (_schemaHelper.Help(process)) {
                        process.Check();
                    }
                }

                if (_options.Mode != null && _options.Mode.ToLower() == "check") {
                    SimplifyForOutput(process);
                    Console.WriteLine(process.Serialize());
                    return;
                }

                if (_options.Mode != "default") {
                    process.Mode = _options.Mode;
                }

                foreach (var warning in process.Warnings()) {
                    log.Debug(() => warning);
                }

                if (process.Errors().Any()) {
                    foreach (var error in process.Errors()) {
                        log.Error(error);
                    }
                    log.Error("The configuration errors must be fixed before this job will run.");
                    log.Logger.Clear();
                    return;
                }

                using (var inner = DefaultContainer.Create(process, _logger, _options.PlaceHolderStyle)) {
                    inner.Resolve<IProcessController>().Execute();
                }
            }
        }

        private static void SimplifyForOutput(Process process) {
            foreach (var connection in process.Connections) {
                connection.Delimiters.Clear();
            }
            foreach (var entity in process.Entities) {
                if (entity.Name == entity.Alias) {
                    entity.Alias = null;
                }
                entity.Fields.RemoveAll(f => f.System);
            }
            foreach (var field in process.GetAllFields().Where(f => !string.IsNullOrEmpty(f.T))) {
                field.T = string.Empty;
            }
            foreach (var field in process.GetAllFields()) {
                if (field.Name == field.Alias) {
                    field.Alias = null;
                } else {
                    if (field.Alias != null && field.Alias == field.Label) {
                        field.Label = null;
                    }
                }
                if (field.Name == field.Label) {
                    field.Label = string.Empty;
                }
                field.SortField = string.Empty;
                field.Sortable = Constants.DefaultSetting;
            }
        }

        public void Stop() { }
    }
}