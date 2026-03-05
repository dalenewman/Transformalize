#region license
// Transformalize
// Configurable Extract, Transform, and Load
// Copyright 2013-2020 Dale Newman
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

using Autofac;
using Autofac.Core;
using Cfg.Net.Shorthand;
using System;
using System.Collections.Generic;
using System.Linq;
using Transformalize.Containers.Autofac.Modules;
using Transformalize.Context;
using Transformalize.Contracts;
using Transformalize.Impl;
using Transformalize.Nulls;
using Transformalize.Transforms.System;
using LogTransform = Transformalize.Transforms.System.LogTransform;
using Process = Transformalize.Configuration.Process;

namespace Transformalize.Containers.Autofac {
   public class Container {

      private readonly List<IModule> _modules = new List<IModule>();
      private readonly HashSet<string> _methods = new HashSet<string>();
      private readonly ShorthandRoot _shortHand = new ShorthandRoot();
      private readonly List<TransformHolder> _transforms = new List<TransformHolder>();
      private readonly List<ValidatorHolder> _validators = new List<ValidatorHolder>();

      /// <summary>
      /// Initializes a new instance of the execution <see cref="Container"/> class with modules.
      /// </summary>
      /// <param name="modules">Autofac modules providing provider-specific implementations.</param>
      public Container(params IModule[] modules) {
         _modules.AddRange(modules);
      }

      /// <summary>
      /// Initializes a new instance of the execution <see cref="Container"/> class with custom transforms.
      /// </summary>
      /// <param name="transforms">Custom transforms to register.</param>
      public Container(params TransformHolder[] transforms) {
         _transforms.AddRange(transforms);
      }

      /// <summary>
      /// Initializes a new instance of the execution <see cref="Container"/> class with custom validators.
      /// </summary>
      /// <param name="validators">Custom validators to register.</param>
      public Container(params ValidatorHolder[] validators) {
         _validators.AddRange(validators);
      }

      /// <summary>
      /// Initializes a new instance of the execution <see cref="Container"/> class.
      /// </summary>
      public Container() { }

      /// <summary>
      /// Creates an Autofac lifetime scope configured for execution of the specified process.
      /// This includes registering contexts, entity pipelines, and the process controller.
      /// </summary>
      /// <param name="process">The hydrated and validated process to execute.</param>
      /// <param name="logger">The pipeline logger.</param>
      /// <returns>An Autofac <see cref="ILifetimeScope"/> containing the executable components.</returns>
      public ILifetimeScope CreateScope(Process process, IPipelineLogger logger) {

         var builder = new ContainerBuilder();
         builder.Properties["Process"] = process;
         builder.Register(ctx => process).As<Process>();
         builder.RegisterInstance(logger).As<IPipelineLogger>().SingleInstance();

         // register short-hand for t attribute, allowing for additional transforms
         var transformModule = new TransformModule(process, _methods, _shortHand, logger);
         foreach (var t in _transforms) {
            transformModule.AddTransform(t);
         }
         builder.RegisterModule(transformModule);

         // register short-hand for v attribute, allowing for additional validators
         var validateModule = new ValidateModule(process, _methods, _shortHand, logger);
         foreach (var v in _validators) {
            validateModule.AddValidator(v);
         }
         builder.RegisterModule(validateModule);

         builder.RegisterModule(new InternalModule(process));

         // allowing transform modules added via AddModule() to access the shared shorthand objects
         builder.Properties["ShortHand"] = _shortHand;
         builder.Properties["Methods"] = _methods;

         // allowing for additional modules
         foreach (var module in _modules) {
            builder.RegisterModule(module);
         }

         // Process Context
         builder.Register<IContext>((ctx, p) => new PipelineContext(logger, process)).As<IContext>();

         // Process Output Context
         builder.Register(ctx => {
            var context = ctx.Resolve<IContext>();
            return new OutputContext(context);
         }).As<OutputContext>();

         // Connection and Process Level Output Context
         foreach (var connection in process.Connections) {

            builder.Register(ctx => new ConnectionContext(ctx.Resolve<IContext>(), connection)).Named<IConnectionContext>(connection.Key);

            if (connection.Name != process.Output)
               continue;

            // register output for connection
            builder.Register(ctx => {
               var context = ctx.ResolveNamed<IConnectionContext>(connection.Key);
               return new OutputContext(context);
            }).Named<OutputContext>(connection.Key);

         }

         // Entity Context and RowFactory
         foreach (var entity in process.Entities) {
            builder.Register<IContext>((ctx, p) => new PipelineContext(ctx.Resolve<IPipelineLogger>(), process, entity)).Named<IContext>(entity.Key);

            builder.Register(ctx => {
               var context = ctx.ResolveNamed<IContext>(entity.Key);
               return new InputContext(context);
            }).Named<InputContext>(entity.Key);

            builder.Register<IRowFactory>((ctx, p) => new RowFactory(p.Named<int>("capacity"), entity.IsMaster, false)).Named<IRowFactory>(entity.Key);

            builder.Register(ctx => {
               var context = ctx.ResolveNamed<IContext>(entity.Key);
               return new OutputContext(context);
            }).Named<OutputContext>(entity.Key);

            var connection = process.Connections.First(c => c.Name == entity.Input);
            builder.Register(ctx => new ConnectionContext(ctx.Resolve<IContext>(), connection)).Named<IConnectionContext>(entity.Key);

         }

         // entity pipelines are where the data transformation logic resides
         foreach (var entity in process.Entities) {
            builder.Register(ctx => {

               var context = ctx.ResolveNamed<IContext>(entity.Key);
               var outputController = ctx.IsRegisteredWithName<IOutputController>(entity.Key) ? ctx.ResolveNamed<IOutputController>(entity.Key) : new NullOutputController();
               var pipeline = new DefaultPipeline(outputController, context);

               // Inputs: Providers like SqlServer register their entity readers with the entity key.
               pipeline.Register(ctx.IsRegisteredWithName(entity.Key, typeof(IRead)) ? ctx.ResolveNamed<IRead>(entity.Key) : null);
               pipeline.Register(ctx.IsRegisteredWithName(entity.Key, typeof(IInputProvider)) ? ctx.ResolveNamed<IInputProvider>(entity.Key) : null);

               // System Transforms:
               // Increment: Tracks row number.
               pipeline.Register(new IncrementTransform(context));
               // Default: Applies default field values if they are missing.
               pipeline.Register(new DefaultTransform(context, context.GetAllEntityFields().Where(f => !f.System)));
               // Hashcode: Calculates a deterministic hash of fields to detect changes.
               pipeline.Register(new SystemHashcodeTransform(new PipelineContext(ctx.Resolve<IPipelineLogger>(), process, entity)));
               // User-defined Transforms: Resolved from the shorthand expansion.
               pipeline.Register(TransformFactory.GetTransforms(ctx, context, entity.GetAllFields().Where(f => f.Transforms.Any())));
               // System Fields: Adds tfl-prefixed management fields like TflKey, TflDeleted, and TflBatchId.
               pipeline.Register(new SystemFieldsTransform(new PipelineContext(ctx.Resolve<IPipelineLogger>(), process, entity)));
               // Validators: Resolved from shorthand expansion.
               pipeline.Register(ValidateFactory.GetValidators(ctx, context, entity.GetAllFields().Where(f => f.Validators.Any())));
               // Truncate: Ensures string fields don't exceed their defined length.
               pipeline.Register(new StringTruncateTransform(new PipelineContext(ctx.Resolve<IPipelineLogger>(), process, entity)));
               // Log: Allows logging specific row data if configured.
               pipeline.Register(new LogTransform(context));

               // Output: Providers like SqlServer register their entity writers with the entity key.
               pipeline.Register(ctx.IsRegisteredWithName(entity.Key, typeof(IWrite)) ? ctx.ResolveNamed<IWrite>(entity.Key) : null);
               pipeline.Register(ctx.IsRegisteredWithName(entity.Key, typeof(IOutputProvider)) ? ctx.ResolveNamed<IOutputProvider>(entity.Key) : null);

               // Updater: Handles updating records in the output based on change detection.
               pipeline.Register(process.ReadOnly || !ctx.IsRegisteredWithName(entity.Key, typeof(IUpdate)) ? new NullUpdater() : ctx.ResolveNamed<IUpdate>(entity.Key));

               return pipeline;

            }).Named<IPipeline>(entity.Key);
         }

         // process pipeline
         builder.Register(ctx => {

            var calc = process.ToCalculatedFieldsProcess();
            var entity = calc.Entities.First();

            var context = new PipelineContext(ctx.Resolve<IPipelineLogger>(), calc, entity);
            var outputContext = new OutputContext(context);

            context.Debug(() => $"Registering {process.Pipeline} pipeline.");
            var outputController = ctx.IsRegistered<IOutputController>() ? ctx.Resolve<IOutputController>() : new NullOutputController();
            var pipeline = new DefaultPipeline(outputController, context);

            // no updater necessary
            pipeline.Register(new NullUpdater(context, false));

            if (!process.CalculatedFields.Any()) {
               pipeline.Register(new NullReader(context, false));
               pipeline.Register(new NullWriter(context, false));
               return pipeline;
            }

            // register transform and validator operations
            pipeline.Register(new IncrementTransform(context));
            pipeline.Register(new LogTransform(context));
            pipeline.Register(new DefaultTransform(new PipelineContext(ctx.Resolve<IPipelineLogger>(), calc, entity), entity.CalculatedFields));
            pipeline.Register(TransformFactory.GetTransforms(ctx, context, entity.CalculatedFields));
            pipeline.Register(ValidateFactory.GetValidators(ctx, context, entity.GetAllFields().Where(f => f.Validators.Any())));
            pipeline.Register(new StringTruncateTransform(new PipelineContext(ctx.Resolve<IPipelineLogger>(), calc, entity)));

            // register input and output
            pipeline.Register(ctx.IsRegistered<IRead>() ? ctx.Resolve<IRead>() : new NullReader(context));
            pipeline.Register(ctx.IsRegistered<IWrite>() ? ctx.Resolve<IWrite>() : new NullWriter(context));

            if (outputContext.Connection.Provider == "sqlserver") {
               pipeline.Register(new MinDateTransform(new PipelineContext(ctx.Resolve<IPipelineLogger>(), calc, entity), new DateTime(1753, 1, 1)));
            }

            return pipeline;
         }).As<IPipeline>();

         // process controller
         builder.Register<IProcessController>(ctx => {

            var pipelines = new List<IPipeline>();

            // entity-level pipelines
            foreach (var entity in process.Entities) {
               var pipeline = ctx.ResolveNamed<IPipeline>(entity.Key);

               pipelines.Add(pipeline);
               if (entity.Delete && process.Mode != "init") {
                  pipeline.Register(ctx.ResolveNamed<IEntityDeleteHandler>(entity.Key));
               }
            }

            // process-level pipeline for process level calculated fields
            if (ctx.IsRegistered<IPipeline>()) {
               pipelines.Add(ctx.Resolve<IPipeline>());
            }

            var context = ctx.Resolve<IContext>();
            var controller = new ProcessController(pipelines, context);

            // output initialization
            if (process.Mode == "init" && ctx.IsRegistered<IInitializer>()) {
               controller.PreActions.Add(ctx.Resolve<IInitializer>());
            }

            // flatten(ing) is first post-action
            var isAdo = Constants.AdoProviderSet().Contains(process.GetOutputConnection().Provider);
            if (process.Flatten && isAdo) {
               if (ctx.IsRegisteredWithName<IAction>(process.GetOutputConnection().Key)) {
                  controller.PostActions.Add(ctx.ResolveNamed<IAction>(process.GetOutputConnection().Key));
               } else {
                  context.Error($"Could not find ADO Flatten Action for provider {process.GetOutputConnection().Provider}.");
               }
            }

            // actions
            foreach (var action in process.Actions.Where(a => a.GetModes().Any(m => m == process.Mode || m == "*"))) {
               if (action.Before) {
                  controller.PreActions.Add(ctx.ResolveNamed<IAction>(action.Key));
               }
               if (action.After) {
                  controller.PostActions.Add(ctx.ResolveNamed<IAction>(action.Key));
               }
            }

            return controller;
         }).As<IProcessController>();

         var build = builder.Build();

         return build.BeginLifetimeScope();

      }

      /// <summary>
      /// Manually adds a validator and its shorthand signatures to the execution container.
      /// </summary>
      /// <param name="getValidator">A factory function to create the validator.</param>
      /// <param name="signatures">The shorthand signatures for the validator.</param>
      public void AddValidator(Func<IContext, IValidate> getValidator, IEnumerable<OperationSignature> signatures) {
         _validators.Add(new ValidatorHolder(getValidator, signatures));
      }

      /// <summary>
      /// Manually adds a transform and its shorthand signatures to the execution container.
      /// </summary>
      /// <param name="getTransform">A factory function to create the transform.</param>
      /// <param name="signatures">The shorthand signatures for the transform.</param>
      public void AddTransform(Func<IContext, ITransform> getTransform, IEnumerable<OperationSignature> signatures) {
         _transforms.Add(new TransformHolder(getTransform, signatures));
      }

      /// <summary>
      /// Registers an Autofac module that will be included in the execution scope.
      /// </summary>
      /// <param name="module">The module to add.</param>
      public void AddModule(IModule module) {
         _modules.Add(module);
      }

   }
}