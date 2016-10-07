#region license
// Transformalize
// Copyright 2013 Dale Newman
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//  
//      http://www.apache.org/licenses/LICENSE-2.0
//  
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
#endregion

using System.Collections.Generic;
using System.Linq;
using Autofac;
using Orchard.FileSystems.AppData;
using Orchard.Templates.Services;
using Pipeline.Configuration;
using Pipeline.Contracts;
using Pipeline.Nulls;
using Pipeline.Web.Orchard.Modules;

namespace Pipeline.Web.Orchard.Impl {

    public class RunTimeDataReader : IRunTimeRun {
        private readonly IPipelineLogger _logger;
        private readonly IAppDataFolder _appDataFolder;
        private readonly ITemplateProcessor _templateProcessor;

        public RunTimeDataReader(IPipelineLogger logger, IAppDataFolder appDataFolder, ITemplateProcessor templateProcessor) {
            _logger = logger;
            _appDataFolder = appDataFolder;
            _templateProcessor = templateProcessor;
        }

        public IEnumerable<IRow> Run(Process process) {

            var nested = new ContainerBuilder();
            var entity = process.Entities.First();
            nested.RegisterInstance(_logger).As<IPipelineLogger>();

            nested.RegisterCallback(new ContextModule(process).Configure);

            // providers
            nested.RegisterCallback(new AdoModule(process).Configure);
            nested.RegisterCallback(new SolrModule(process).Configure);
            nested.RegisterCallback(new ElasticModule(process).Configure);
            nested.RegisterCallback(new InternalModule(process).Configure);
            nested.RegisterCallback(new FileModule(process, _appDataFolder, _templateProcessor).Configure);
            nested.RegisterCallback(new ExcelModule(process, _appDataFolder, _templateProcessor).Configure);
            nested.RegisterCallback(new WebModule(process).Configure);

            nested.RegisterCallback(new MapModule(process).Configure);
            nested.RegisterCallback(new TemplateModule(process, _templateProcessor).Configure);

            nested.RegisterType<NullOutputController>().Named<IOutputController>(entity.Key);
            nested.RegisterType<NullWriter>().Named<IWrite>(entity.Key);
            nested.RegisterType<NullUpdater>().Named<IUpdate>(entity.Key);
            nested.RegisterType<NullDeleteHandler>().Named<IEntityDeleteHandler>(entity.Key);
            nested.RegisterCallback(new EntityPipelineModule(process).Configure);

            using (var scope = nested.Build().BeginLifetimeScope()) {
                return scope.ResolveNamed<IPipeline>(entity.Key).Read();
            }
        }
    }
}