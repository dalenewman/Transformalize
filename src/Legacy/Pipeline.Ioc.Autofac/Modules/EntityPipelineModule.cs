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
using Autofac;
using System.Linq;
using Transformalize.Configuration;
using Transformalize.Context;
using Transformalize.Contracts;
using Transformalize.Impl;
using Transformalize.Nulls;
using Transformalize.Transforms.System;

namespace Transformalize.Ioc.Autofac.Modules {

   public class EntityPipelineModule : EntityModule {

      protected EntityPipelineModule() { }

      public EntityPipelineModule(Process process) : base(process) { }

      public override void LoadEntity(ContainerBuilder builder, Process process, Entity entity) {
         if (process == null)
            return;

         builder.Register(ctx => {
            var context = ctx.ResolveNamed<IContext>(entity.Key);
            IPipeline pipeline;
            var outputController = ctx.IsRegisteredWithName<IOutputController>(entity.Key) ? ctx.ResolveNamed<IOutputController>(entity.Key) : new NullOutputController();
            pipeline = new DefaultPipeline(outputController, context);

            // TODO: rely on IInputProvider's Read method instead (after every provider has one)
            pipeline.Register(ctx.IsRegisteredWithName(entity.Key, typeof(IRead)) ? ctx.ResolveNamed<IRead>(entity.Key) : null);
            pipeline.Register(ctx.IsRegisteredWithName(entity.Key, typeof(IInputProvider)) ? ctx.ResolveNamed<IInputProvider>(entity.Key) : null);

            // register transform and validator operations
            pipeline.Register(new IncrementTransform(context));
            pipeline.Register(new DefaultTransform(context, context.GetAllEntityFields().Where(f => !f.System)));
            pipeline.Register(new SystemHashcodeTransform(new PipelineContext(ctx.Resolve<IPipelineLogger>(), process, entity)));
            pipeline.Register(TransformFactory.GetTransforms(ctx, context, entity.GetAllFields().Where(f => f.Transforms.Any())));
            pipeline.Register(new SystemFieldsTransform(new PipelineContext(ctx.Resolve<IPipelineLogger>(), process, entity)));
            pipeline.Register(ValidateFactory.GetValidators(ctx, context, entity.GetAllFields().Where(f => f.Validators.Any())));
            pipeline.Register(new StringTruncateTransfom(new PipelineContext(ctx.Resolve<IPipelineLogger>(), process, entity)));
            pipeline.Register(new LogTransform(context));

            // writer, TODO: rely on IOutputProvider instead
            pipeline.Register(ctx.IsRegisteredWithName(entity.Key, typeof(IWrite)) ? ctx.ResolveNamed<IWrite>(entity.Key) : null);
            pipeline.Register(ctx.IsRegisteredWithName(entity.Key, typeof(IOutputProvider)) ? ctx.ResolveNamed<IOutputProvider>(entity.Key) : null);

            // updater
            pipeline.Register(process.ReadOnly || !ctx.IsRegisteredWithName(entity.Key, typeof(IUpdate)) ? new NullUpdater() : ctx.ResolveNamed<IUpdate>(entity.Key));

            return pipeline;

         }).Named<IPipeline>(entity.Key);
      }
   }
}