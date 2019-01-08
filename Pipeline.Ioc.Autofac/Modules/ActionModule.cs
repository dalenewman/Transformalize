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
using System.Linq;
using Autofac;
using Quartz.Collection;
using Transformalize.Actions;
using Transformalize.Configuration;
using Transformalize.Context;
using Transformalize.Contracts;
using Transformalize.Extensions;
using Transformalize.Providers.Console;
using Transformalize.Providers.File;
using Transformalize.Providers.File.Actions;
using Transformalize.Providers.Web;

namespace Transformalize.Ioc.Autofac.Modules {

    /// <summary>
    /// Register native actions
    /// </summary>
    public class ActionModule : Module {

        private readonly Process _process;
        private readonly HashSet<string> _types = new HashSet<string> { "copy", "move", "archive", "replace", "print", "log", "web", "wait", "sleep", "tfl", "open", "exit" };

        public ActionModule() { }

        public ActionModule(Process process) {
            _process = process;
        }

        protected override void Load(ContainerBuilder builder) {
            if (_process == null)
                return;

            foreach (var action in _process.Templates.Where(t => t.Enabled).SelectMany(t => t.Actions).Where(a => a.GetModes().Any(m => m == _process.Mode || m == "*"))) {
                if (_types.Contains(action.Type)) {
                    builder.Register(ctx => SwitchAction(ctx, _process, action)).Named<IAction>(action.Key);
                }
            }
            foreach (var action in _process.Actions.Where(a => a.GetModes().Any(m => m == _process.Mode || m == "*"))) {
                if (_types.Contains(action.Type)) {
                    builder.Register(ctx => SwitchAction(ctx, _process, action)).Named<IAction>(action.Key);
                }
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
                case "archive":
                    return new FileArchiveAction(context, action);
                case "replace":
                    return new FileReplaceAction(context, action);
                case "print":
                    return new PrintAction(action);
                case "log":
                    return new LogAction(context, action);
                case "web":
                    return new WebAction(context, action);
                case "wait":
                case "sleep":
                    return new WaitAction(action);
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
                        context.Error($"TFL Pipeline Action '{cfg.Left(30)}'... has errors!");
                        foreach (var error in root.Errors()) {
                            context.Error(error);
                        }
                        return new NullAction();
                    }
                    return new PipelineAction(DefaultContainer.Create(root, ctx.Resolve<IPipelineLogger>(), action.PlaceHolderStyle));
                case "open":
                    return new OpenAction(action);
                case "exit":
                    return new ExitAction(context, action);
                default:
                    context.Error("{0} action is not registered.", action.Type);
                    return new NullAction();
            }
        }

    }
}