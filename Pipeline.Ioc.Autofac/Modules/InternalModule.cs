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
using Transformalize.Logging.NLog;
using Transformalize.Nulls;
using System.Collections.Generic;
using Transformalize.Providers.File;
using Transformalize.Providers.Internal;
using Transformalize.Providers.Trace;
using Transformalize.Impl;
using Transformalize.Actions;
using Transformalize.Extensions;

namespace Transformalize.Ioc.Autofac.Modules {

   public class InternalModule : Module {
      private readonly Process _process;
      private readonly HashSet<string> _internalActions = new HashSet<string> { "log", "web", "wait", "sleep", "tfl", "exit" };
      private readonly HashSet<string> _internalProviders = new HashSet<string>(new[] { "internal", "trace", "log", "text", Constants.DefaultSetting });

      public InternalModule() { }

      public InternalModule(Process process) {
         _process = process;
      }

      protected override void Load(ContainerBuilder builder) {

         if (_process == null)
            return;

         foreach (var action in _process.Templates.Where(t => t.Enabled).SelectMany(t => t.Actions).Where(a => a.GetModes().Any(m => m == _process.Mode || m == "*"))) {
            if (_internalActions.Contains(action.Type)) {
               builder.Register(ctx => SwitchAction(ctx, _process, action)).Named<IAction>(action.Key);
            }
         }
         foreach (var action in _process.Actions.Where(a => a.GetModes().Any(m => m == _process.Mode || m == "*"))) {
            if (_internalActions.Contains(action.Type)) {
               builder.Register(ctx => SwitchAction(ctx, _process, action)).Named<IAction>(action.Key);
            }
         }

         // Connections
         foreach (var connection in _process.Connections.Where(c => c.Provider == "internal")) {
            builder.RegisterType<NullSchemaReader>().Named<ISchemaReader>(connection.Key);
         }

         // Entity input
         foreach (var entity in _process.Entities.Where(e => _internalProviders.Contains(_process.Connections.First(c => c.Name == e.Connection).Provider))) {

            builder.RegisterType<NullInputProvider>().Named<IInputProvider>(entity.Key);

            // Another provider's delete handler may need this if the consumer has swapped moved the input to internal rows for editing (e.g. HandsOnTable in Orchard CMS Module)
            if (entity.Delete) {
               builder.Register<IReadInputKeysAndHashCodes>(ctx => {
                  // note: i tried to just load keys but had a lot of troubles, this works (for now).
                  var inputContext = ctx.ResolveNamed<InputContext>(entity.Key);
                  var rowFactory = new RowFactory(inputContext.RowCapacity, entity.IsMaster, false);
                  return new InternalKeysReader(new InternalReader(inputContext, rowFactory));
               }).Named<IReadInputKeysAndHashCodes>(entity.Key);
            }

            // READER
            builder.Register<IRead>(ctx => {
               var input = ctx.ResolveNamed<InputContext>(entity.Key);
               var rowFactory = ctx.ResolveNamed<IRowFactory>(entity.Key, new NamedParameter("capacity", input.RowCapacity));

               switch (input.Connection.Provider) {
                  case Constants.DefaultSetting:
                  case "internal":
                     return new InternalReader(input, rowFactory);
                  default:
                     return new NullReader(input, false);
               }
            }).Named<IRead>(entity.Key);

         }

         // Entity Output
         if (_internalProviders.Contains(_process.Output().Provider)) {

            // PROCESS OUTPUT CONTROLLER
            builder.Register<IOutputController>(ctx => new NullOutputController()).As<IOutputController>();

            foreach (var entity in _process.Entities) {

               builder.Register<IOutputController>(ctx => new NullOutputController()).Named<IOutputController>(entity.Key);
               builder.Register<IOutputProvider>(ctx => new InternalOutputProvider(ctx.ResolveNamed<OutputContext>(entity.Key), ctx.ResolveNamed<IWrite>(entity.Key))).Named<IOutputProvider>(entity.Key);

               // WRITER
               builder.Register<IWrite>(ctx => {
                  var output = ctx.ResolveNamed<OutputContext>(entity.Key);

                  switch (output.Connection.Provider) {
                     case "trace":
                        return new TraceWriter(new JsonNetSerializer(output));
                     case Constants.DefaultSetting:
                     case "internal":
                        return new InternalWriter(output);
                     case "text":
                        return new FileStreamWriter(output, Console.OpenStandardOutput());
                     case "log":
                        return new NLogWriter(output);
                     default:
                        return new NullWriter(output);
                  }
               }).Named<IWrite>(entity.Key);
            }
         }
      }

      private static IAction SwitchAction(IComponentContext ctx, Process process, Configuration.Action action) {

         var context = new PipelineContext(ctx.Resolve<IPipelineLogger>(), process);
         switch (action.Type) {
            case "log":
               return new LogAction(context, action);
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

            case "exit":
               return new ExitAction(context, action);
            default:
               context.Error("{0} action is not registered.", action.Type);
               return new NullAction();
         }
      }
   }
}