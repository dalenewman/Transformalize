#region license
// Transformalize
// Configurable Extract, Transform, and Load
// Copyright 2013-2017 Dale Newman
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
using Transformalize.Configuration;
using Transformalize.Context;
using Transformalize.Contracts;
using Transformalize.Impl;
using Transformalize.Providers.Console;
using Transformalize.Transforms.CSharp;

namespace Transformalize.Ioc.Autofac.Modules {

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
            builder.Register<IContext>((ctx, p) => new PipelineContext(ctx.Resolve<IPipelineLogger>(), _process)).As<IContext>();

            // Register CSharp Host
            builder.Register((ctx, p) => new CSharpHost(ctx.Resolve<IContext>(), new CSharpCodeWriter(ctx.Resolve<IContext>()))).Named<IHost>("cs");

            // Process Output Context
            builder.Register(ctx => {
                var context = ctx.Resolve<IContext>();
                return new OutputContext(context);
            }).As<OutputContext>();

            // Connection and Process Level Output Context
            foreach (var connection in _process.Connections) {

                builder.Register(ctx => new ConnectionContext(ctx.Resolve<IContext>(), connection)).Named<IConnectionContext>(connection.Key);

                if (connection.Name != "output")
                    continue;

                // register output for connection
                builder.Register(ctx => {
                    var context = ctx.ResolveNamed<IConnectionContext>(connection.Key);
                    return new OutputContext(context);
                }).Named<OutputContext>(connection.Key);

                //TODO, move JsonNetSerializer to JSON Provider, and Csv Serializer to Console or File Provider
                if (connection.Provider == "console") {
                    builder.Register(ctx => new ConsoleWriter(connection.Format == "json" ? new JsonNetSerializer(ctx.ResolveNamed<OutputContext>(connection.Key)) : new CsvSerializer(ctx.ResolveNamed<OutputContext>(connection.Key)) as ISerialize)).As<ConsoleWriter>();
                }
            }

            // Entity Context and RowFactory
            foreach (var entity in _process.Entities) {
                builder.Register<IContext>((ctx, p) => new PipelineContext(ctx.Resolve<IPipelineLogger>(), _process, entity)).Named<IContext>(entity.Key);

                builder.Register(ctx => {
                    var context = ctx.ResolveNamed<IContext>(entity.Key);
                    return new InputContext(context);
                }).Named<InputContext>(entity.Key);

                builder.Register<IRowFactory>((ctx, p) => new RowFactory(p.Named<int>("capacity"), entity.IsMaster, false)).Named<IRowFactory>(entity.Key);

                builder.Register(ctx => {
                    var context = ctx.ResolveNamed<IContext>(entity.Key);
                    return new OutputContext(context);
                }).Named<OutputContext>(entity.Key);

                var connection = _process.Connections.First(c => c.Name == entity.Connection);
                builder.Register(ctx => new ConnectionContext(ctx.Resolve<IContext>(), connection)).Named<IConnectionContext>(entity.Key);
               
            }
        }
    }
}