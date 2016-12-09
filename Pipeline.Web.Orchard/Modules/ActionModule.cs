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

using System.Linq;
using Autofac;
using Orchard.FileSystems.AppData;
using Orchard.Templates.Services;
using Orchard.UI.Notify;
using Transformalize.Actions;
using Transformalize.Configuration;
using Transformalize.Context;
using Transformalize.Contracts;
using Transformalize.Desktop.Actions;
using Transformalize.Extensions;
using Transformalize.Provider.Ado;
using Transformalize.Provider.Ado.Actions;
using Pipeline.Web.Orchard.Impl;
using OpenAction = Pipeline.Web.Orchard.Impl.OpenAction;

namespace Pipeline.Web.Orchard.Modules {
    /// <summary>
    /// The `ActionModule` is only for actions embedded in a host process:
    /// 
    /// * copy
    /// * web
    /// * tfl
    /// * run
    /// * open
    /// 
    /// </summary>
    public class ActionModule : Module {
        readonly Process _process;

        public ActionModule() {

        }

        public ActionModule(Process process) {
            _process = process;
        }

        protected override void Load(ContainerBuilder builder) {
            if (_process == null)
                return;

            foreach (var action in _process.Templates.Where(t => t.Enabled).SelectMany(t => t.Actions).Where(a => a.GetModes().Any(m => m == _process.Mode || m == "*"))) {
                builder.Register(ctx => SwitchAction(ctx, _process, action)).Named<IAction>(action.Key);
            }
            foreach (var action in _process.Actions.Where(a => a.GetModes().Any(m => m == _process.Mode || m == "*"))) {
                builder.Register(ctx => SwitchAction(ctx, _process, action)).Named<IAction>(action.Key);
            }
        }

        private static IAction SwitchAction(IComponentContext ctx, Process process, Action action) {
            var context = new PipelineContext(ctx.Resolve<IPipelineLogger>(), process);
            switch (action.Type) {
                case "copy":
                    return action.InTemplate ? (IAction)
                        new ContentToFileAction(context, action) :
                        new FileCopyAction(context, action);
                case "move":
                    return new FileMoveAction(context, action);
                case "print":
                    return new PrintAction(action);
                case "log":
                    return new LogAction(context, action);
                case "web":
                    return new WebAction(context, action);
                case "tfl":
                    var cfg = string.IsNullOrEmpty(action.Url) ? action.File : action.Url;
                    if (string.IsNullOrEmpty(cfg) && !string.IsNullOrEmpty(action.Body)) {
                        cfg = action.Body;
                    }

                    var root = ctx.Resolve<Process>(new NamedParameter("cfg", cfg));

                    foreach (var warning in root.Warnings()) {
                        context.Warn(warning);
                    }
                    if (root.Errors().Any()) {
                        context.Error($"TFL Pipeline Action '{cfg.Left(20)}' + ... has errors!");
                        foreach (var error in root.Errors()) {
                            context.Error(error);
                        }
                        return new NullAction();
                    }
                     
                    var builder = new ContainerBuilder();

                    // Register Orchard CMS Stuff
                    builder.RegisterInstance(ctx.Resolve<IAppDataFolder>()).As<IAppDataFolder>();
                    builder.RegisterInstance(ctx.Resolve<ITemplateProcessor>()).As<ITemplateProcessor>();
                    builder.RegisterInstance(ctx.Resolve<INotifier>()).As<INotifier>();

                    builder.RegisterInstance(context.Logger).As<IPipelineLogger>();
                    builder.RegisterCallback(new RootModule().Configure);
                    builder.RegisterCallback(new ContextModule(root).Configure);

                    // providers
                    builder.RegisterCallback(new AdoModule(root).Configure);
                    builder.RegisterCallback(new SolrModule(root).Configure);
                    builder.RegisterCallback(new InternalModule(root).Configure);
                    builder.RegisterCallback(new FileModule().Configure);
                    builder.RegisterCallback(new ExcelModule().Configure);

                    builder.RegisterCallback(new MapModule(root).Configure);
                    builder.RegisterCallback(new ActionModule(root).Configure);
                    builder.RegisterCallback(new EntityPipelineModule(root).Configure);
                    builder.RegisterCallback(new ProcessPipelineModule(root).Configure);
                    builder.RegisterCallback(new ProcessControlModule(root).Configure);

                    return new PipelineAction(builder.Build(), root);
                case "run":
                    var connection = process.Connections.First(c => c.Name == action.Connection);
                    switch (connection.Provider) {
                        case "mysql":
                        case "postgresql":
                        case "sqlite":
                        case "sqlserver":
                            return new AdoRunAction(context, action, ctx.ResolveNamed<IConnectionFactory>(connection.Key));
                        default:
                            context.Error("{0} provider is not registered for run action.", connection.Provider);
                            return new NullAction();
                    }
                case "open":
                    return new OpenAction(action);
                default:
                    context.Error("{0} action is not registered.", action.Type);
                    return new NullAction();
            }
        }

    }
}