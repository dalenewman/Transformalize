#region license
// Transformalize
// Configurable Extract, Transform, and Load
// Copyright 2013-2016 Dale Newman
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
using Transformalize.Desktop;
using Transformalize.Nulls;
using Transformalize.Transforms.System;

namespace Transformalize.Ioc.Autofac.Modules {

    public class ProcessPipelineModule : Module {
        private readonly Process _process;

        public ProcessPipelineModule() { }

        public ProcessPipelineModule(Process process) {
            _process = process;
        }

        protected override void Load(ContainerBuilder builder) {

            if (_process == null)
                return;

            var calc = _process.ToCalculatedFieldsProcess();
            var entity = calc.Entities.First();

            // I need a process keyed pipeline
            builder.Register(ctx => {

                var context = new PipelineContext(ctx.Resolve<IPipelineLogger>(), calc, entity);
                var outputContext = new OutputContext(context, new Incrementer(context));

                IPipeline pipeline;
                context.Debug(() => $"Registering {_process.Pipeline} pipeline.");
                switch (_process.Pipeline) {
                    case "parallel.linq":
                        pipeline = new ParallelPipeline(new DefaultPipeline(ctx.Resolve<IOutputController>(), context));
                        break;
                    default:
                        pipeline = new DefaultPipeline(ctx.Resolve<IOutputController>(), context);
                        break;
                }

                // no updater necessary
                pipeline.Register(new NullUpdater(context, false));

                if (!_process.CalculatedFields.Any()) {
                    pipeline.Register(new NullReader(context, false));
                    pipeline.Register(new NullWriter(context, false));
                    return pipeline;
                }

                // register transforms
                pipeline.Register(new DefaultTransform(new PipelineContext(ctx.Resolve<IPipelineLogger>(), calc, entity), entity.CalculatedFields));
                pipeline.Register(TransformFactory.GetTransforms(ctx, calc, entity, entity.CalculatedFields));
                pipeline.Register(new StringTruncateTransfom(new PipelineContext(ctx.Resolve<IPipelineLogger>(), calc, entity)));

                // register input and output
                switch (outputContext.Connection.Provider) {
                    case "sqlserver":
                        pipeline.Register(ctx.Resolve<IRead>());
                        pipeline.Register(ctx.Resolve<IWrite>());
                        pipeline.Register(new MinDateTransform(new PipelineContext(ctx.Resolve<IPipelineLogger>(), calc, entity), new DateTime(1753, 1, 1)));
                        break;
                    case "mysql":
                    case "postgresql":
                    case "sqlite":
                        pipeline.Register(ctx.Resolve<IRead>());
                        pipeline.Register(ctx.Resolve<IWrite>());
                        break;
                    default:
                        pipeline.Register(new NullReader(context));
                        pipeline.Register(new NullWriter(context));
                        break;
                }

                return pipeline;
            }).As<IPipeline>();

        }
    }
}