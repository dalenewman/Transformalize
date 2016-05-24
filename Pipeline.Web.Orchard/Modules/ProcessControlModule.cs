#region license
// Transformalize
// A Configurable ETL Solution Specializing in Incremental Denormalization.
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
using Pipeline.Actions;
using Pipeline.Context;
using Pipeline.Contracts;
using Process = Pipeline.Configuration.Process;

namespace Pipeline.Web.Orchard.Modules {
    public class ProcessControlModule : Module {
        private readonly Process _process;

        public ProcessControlModule() { }

        public ProcessControlModule(Process process) {
            _process = process;
        }

        protected override void Load(ContainerBuilder builder) {
            if (_process == null)
                return;

            if (!_process.Enabled)
                return;

            builder.Register<IProcessController>(ctx => {

                var pipelines = new List<IPipeline>();

                // entity-level pipelines
                foreach (var entity in _process.Entities) {
                    var pipeline = ctx.ResolveNamed<IPipeline>(entity.Key);

                    pipelines.Add(pipeline);
                    if (entity.Delete && _process.Mode != "init") {
                        pipeline.Register(ctx.ResolveNamed<IEntityDeleteHandler>(entity.Key));
                    }
                }

                // process-level pipeline for process level calculated fields
                if (ctx.IsRegisteredWithName<IPipeline>(_process.Key)) {
                    pipelines.Add(ctx.ResolveNamed<IPipeline>(_process.Key));
                }

                var outputConnection = _process.Output();
                var context = ctx.ResolveNamed<IContext>(_process.Key);

                var controller = new ProcessController(pipelines, context);

                // output initialization
                if (_process.Mode == "init") {
                    var output = ctx.ResolveNamed<OutputContext>(outputConnection.Key);
                    switch (outputConnection.Provider) {
                        case "mysql":
                        case "postgresql":
                        case "sqlite":
                        case "sqlserver":
                        case "elastic":
                        case "lucene":
                            controller.PreActions.Add(ctx.ResolveNamed<IInitializer>(_process.Key));
                            break;
                        default:
                            output.Warn("The {0} provider does not support initialization.", outputConnection.Provider);
                            break;
                    }

                }

                // input validation
                if (_process.Mode == "init") {
                    var providers = _process.Connections.Select(c => c.Provider).Distinct();

                    foreach (var provider in providers) {
                        switch (provider) {
                            case "solr":
                                foreach (var connection in _process.Connections.Where(c => c.Provider == "solr")) {
                                    foreach (var entity in _process.Entities.Where(e => e.Connection == connection.Name)) {
                                        controller.PreActions.Add(ctx.ResolveNamed<IInputValidator>(entity.Key));
                                    }
                                }
                                break;
                        }
                    }
                }

                // templates
                foreach (var template in _process.Templates.Where(t => t.Enabled).Where(t => t.Actions.Any(a => a.GetModes().Any(m => m == _process.Mode)))) {
                    controller.PreActions.Add(new RenderTemplateAction(template, ctx.ResolveNamed<ITemplateEngine>(template.Key)));
                    foreach (var action in template.Actions.Where(a => a.GetModes().Any(m => m == _process.Mode || m == "*"))) {
                        if (action.Before) {
                            controller.PreActions.Add(ctx.ResolveNamed<IAction>(action.Key));
                        }
                        if (action.After) {
                            controller.PostActions.Add(ctx.ResolveNamed<IAction>(action.Key));
                        }
                    }
                }

                // actions
                foreach (var action in _process.Actions.Where(a => a.GetModes().Any(m => m == _process.Mode || m == "*"))) {
                    if (action.Before) {
                        controller.PreActions.Add(ctx.ResolveNamed<IAction>(action.Key));
                    }
                    if (action.After) {
                        controller.PostActions.Add(ctx.ResolveNamed<IAction>(action.Key));
                    }
                }

                return controller;
            }).Named<IProcessController>(_process.Key);

        }

    }
}