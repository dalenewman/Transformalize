#region license
// Transformalize
// Configurable Extract, Transform, and Load
// Copyright 2013-2016 Dale Newman
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

using System;
using System.Linq;
using Autofac;
using Transformalize.Configuration;
using Transformalize.Context;
using Transformalize.Contracts;
using Transformalize.Desktop;
using Transformalize.Nulls;
using Transformalize.Transforms.System;
using Pipeline.Web.Orchard.Impl;
using Transformalize;

namespace Pipeline.Web.Orchard.Modules {

    public class EntityPipelineModule : EntityModule {

        public EntityPipelineModule() : base(null) {}

        public EntityPipelineModule(Process process) : base(process) { }

        public override void LoadEntity(ContainerBuilder builder, Process process, Entity entity) {
            if (process == null)
                return;

            var type = process.Pipeline == "defer" ? entity.Pipeline : process.Pipeline;

            builder.Register(ctx => {
                var context = ctx.ResolveNamed<IContext>(entity.Key);
                IPipeline pipeline;
                context.Debug(() => string.Format("Registering {0} for entity {1}.", type, entity.Alias));
                var outputController = ctx.IsRegisteredWithName<IOutputController>(entity.Key) ? ctx.ResolveNamed<IOutputController>(entity.Key) : new NullOutputController();
                switch (type) {
                    case "parallel.linq":
                        pipeline = new ParallelPipeline(new DefaultPipeline(outputController, context));
                        break;
                    default:
                        pipeline = new DefaultPipeline(outputController, context);
                        break;
                }

                var provider = process.Output().Provider;

                // extract
                pipeline.Register(ctx.ResolveNamed<IRead>(entity.Key));

                // transform
                if (!process.ReadOnly) {
                    pipeline.Register(new SetBatchId(new PipelineContext(ctx.Resolve<IPipelineLogger>(), process, entity, entity.TflBatchId())));
                    pipeline.Register(new SetKey(new PipelineContext(ctx.Resolve<IPipelineLogger>(), process, entity, entity.TflKey())));
                }

                pipeline.Register(new DefaultTransform(new PipelineContext(ctx.Resolve<IPipelineLogger>(), process, entity), context.GetAllEntityFields().Where(f => !f.System)));
                pipeline.Register(TransformFactory.GetTransforms(ctx, process, entity, entity.GetAllFields().Where(f => f.Transforms.Any())));

                if (!process.ReadOnly) {
                    pipeline.Register(new StringTruncateTransfom(new PipelineContext(ctx.Resolve<IPipelineLogger>(), process, entity)));
                    if (provider == "sqlserver") {
                        pipeline.Register(new MinDateTransform(new PipelineContext(ctx.Resolve<IPipelineLogger>(), process, entity), new DateTime(1753, 1, 1)));
                    }
                }

                // writer
                pipeline.Register(ctx.IsRegisteredWithName(entity.Key, typeof(IWrite)) ? ctx.ResolveNamed<IWrite>(entity.Key) : new NullWriter());

                // updater
                pipeline.Register(process.ReadOnly || !ctx.IsRegisteredWithName(entity.Key, typeof(IUpdate)) ? new NullUpdater() : ctx.ResolveNamed<IUpdate>(entity.Key));

                return pipeline;

            }).Named<IPipeline>(entity.Key);
        }
    }
}