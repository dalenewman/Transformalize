#region license
// Transformalize
// A Configurable ETL Solution Specializing in Incremental Denormalization.
// Copyright 2013 Dale Newman
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
using Pipeline.Configuration;
using Pipeline.Context;
using Pipeline.Contracts;

namespace Pipeline.Ioc.Autofac.Modules {
    public class ContextModule : Module {
        private readonly Process _process;

        public ContextModule() { }

        public ContextModule(Process process) {
            _process = process;
        }

        protected override void Load(ContainerBuilder builder) {

            if (_process == null)
                return;

            // Process Context
            builder.Register<IContext>((ctx, p) => new PipelineContext(ctx.Resolve<IPipelineLogger>(), _process)).Named<IContext>(_process.Key);

            // Process Output Context
            builder.Register(ctx => {
                var context = ctx.ResolveNamed<IContext>(_process.Key);
                return new OutputContext(context, new Incrementer(context));
            }).Named<OutputContext>(_process.Key);

            // Connection and Process Level Output Context
            foreach (var connection in _process.Connections) {

                builder.Register(ctx => new ConnectionContext(ctx.ResolveNamed<IContext>(_process.Key), connection)).Named<IConnectionContext>(connection.Key);

                if (connection.Name != "output")
                    continue;

                // register output for connection
                builder.Register(ctx => {
                    var context = ctx.ResolveNamed<IContext>(_process.Key);
                    return new OutputContext(context, new Incrementer(context));
                }).Named<OutputContext>(connection.Key);

            }

            // Entity Context and RowFactory
            foreach (var entity in _process.Entities) {
                builder.Register<IContext>((ctx, p) => new PipelineContext(ctx.Resolve<IPipelineLogger>(), _process, entity)).Named<IContext>(entity.Key);

                builder.Register<IIncrement>(ctx => new Incrementer(ctx.ResolveNamed<IContext>(entity.Key))).Named<IIncrement>(entity.Key).InstancePerDependency();

                builder.Register(ctx => {
                    var context = ctx.ResolveNamed<IContext>(entity.Key);
                    return new InputContext(context, ctx.ResolveNamed<IIncrement>(entity.Key));
                }).Named<InputContext>(entity.Key);

                builder.Register<IRowFactory>((ctx, p) => new RowFactory(p.Named<int>("capacity"), entity.IsMaster, false)).Named<IRowFactory>(entity.Key);

                builder.Register(ctx => {
                    var context = ctx.ResolveNamed<IContext>(entity.Key);
                    return new OutputContext(context, ctx.ResolveNamed<IIncrement>(entity.Key));
                }).Named<OutputContext>(entity.Key);
            }
        }
    }
}