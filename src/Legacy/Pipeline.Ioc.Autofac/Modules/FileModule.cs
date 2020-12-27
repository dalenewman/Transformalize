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
using System.Collections.Generic;
using System.Linq;
using Autofac;
using Transformalize.Actions;
using Transformalize.Configuration;
using Transformalize.Context;
using Transformalize.Contracts;
using Transformalize.Providers.File;
using Transformalize.Providers.File.Actions;

namespace Transformalize.Ioc.Autofac.Modules {
   public class FileModule : Module {
      private readonly Process _process;
      private readonly HashSet<string> _fileActions = new HashSet<string> { "copy", "move", "archive", "replace", "open" };

      public FileModule() { }

      public FileModule(Process process) {
         _process = process;
      }

      protected override void Load(ContainerBuilder builder) {

         if (_process == null)
            return;

         foreach (var action in _process.Templates.Where(t => t.Enabled).SelectMany(t => t.Actions).Where(a => a.GetModes().Any(m => m == _process.Mode || m == "*"))) {
            if (_fileActions.Contains(action.Type)) {

               builder.Register(ctx => {
                  return SwitchAction(ctx, action);
               }).Named<IAction>(action.Key);
            }
         }
         foreach (var action in _process.Actions.Where(a => a.GetModes().Any(m => m == _process.Mode || m == "*"))) {
            if (_fileActions.Contains(action.Type)) {
               builder.Register(ctx => {
                  return SwitchAction(ctx, action);
               }).Named<IAction>(action.Key);
            }
         }
      }

      private IAction SwitchAction(IComponentContext ctx, Action action) {
         var context = new PipelineContext(ctx.Resolve<IPipelineLogger>(), _process);
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
            case "open":
               return new OpenAction(action);
            default:
               context.Error($"Attempting to register unsupported file sytem action: {action.Type}");
               return new NullAction();
         }
      }
   }
}