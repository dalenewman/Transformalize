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

using System;
using System.Collections.Generic;
using System.Linq;
using Autofac;
using Elasticsearch.Net;
using Transformalize.Configuration;
using Transformalize.Context;
using Transformalize.Contracts;
using Transformalize.Desktop;
using Transformalize.Extensions;
using Transformalize.Nulls;
using Transformalize.Provider.Elastic;
using Transformalize.Provider.Elastic.Ext;
using Transformalize.Transforms.System;
using Pipeline.Web.Orchard.Impl;
using Transformalize;

namespace Pipeline.Web.Orchard.Modules {
    public class ElasticModule : Module {
        private readonly Process _process;

        public ElasticModule() { }

        public ElasticModule(Process process) {
            _process = process;
        }

        protected override void Load(ContainerBuilder builder) {

            if (_process == null)
                return;

            //CONNECTIONS
            foreach (var connection in _process.Connections.Where(c => c.Provider.In("elasticsearch"))) {


                if (connection.Servers.Any()) {
                    var uris = new List<Uri>();
                    foreach (var server in connection.Servers) {
                        server.Url = server.GetElasticUrl();
                        uris.Add(new Uri(server.Url));
                    }
                    // for now, just use static connection pool, there are 2 other types...
                    builder.Register<IConnectionPool>(ctx => new StaticConnectionPool(uris)).Named<IConnectionPool>(connection.Key);
                } else {
                    connection.Url = connection.GetElasticUrl();
                    builder.Register<IConnectionPool>(ctx => new SingleNodeConnectionPool(new Uri(connection.Url))).Named<IConnectionPool>(connection.Key);
                }

                // Elasticsearch.Net
                builder.Register(ctx => {
                    var settings = new ConnectionConfiguration(ctx.ResolveNamed<IConnectionPool>(connection.Key));
                    if (!string.IsNullOrEmpty(connection.User)) {
                        settings.BasicAuthentication(connection.User, connection.Password);
                    }

                    if (_process.Mode != "init" && connection.RequestTimeout >= 0) {
                        settings.RequestTimeout(new TimeSpan(0, 0, 0, connection.RequestTimeout * 1000));
                    }
                    if (connection.Timeout > 0) {
                        settings.PingTimeout(new TimeSpan(0, 0, connection.Timeout));
                    }
                    return new ElasticLowLevelClient(settings);
                }).Named<IElasticLowLevelClient>(connection.Key);

                // Process-Level Schema Reader
                builder.Register<ISchemaReader>(ctx => new ElasticSchemaReader(ctx.ResolveNamed<IConnectionContext>(connection.Key), ctx.ResolveNamed<IElasticLowLevelClient>(connection.Key))).Named<ISchemaReader>(connection.Key);

                // Entity Level Schema Readers
                foreach (var entity in _process.Entities.Where(e => e.Connection == connection.Name)) {
                    builder.Register<ISchemaReader>(ctx => new ElasticSchemaReader(ctx.ResolveNamed<IConnectionContext>(entity.Key), ctx.ResolveNamed<IElasticLowLevelClient>(connection.Key))).Named<ISchemaReader>(entity.Key);
                }

            }

            // Entity Input
            foreach (var entity in _process.Entities.Where(e => _process.Connections.First(c => c.Name == e.Connection).Provider == "elasticsearch")) {

                builder.Register<IInputVersionDetector>(ctx => {
                    var input = ctx.ResolveNamed<InputContext>(entity.Key);
                    switch (input.Connection.Provider) {
                        case "elasticsearch":
                            return new ElasticInputVersionDetector(input, ctx.ResolveNamed<IElasticLowLevelClient>(input.Connection.Key));
                        default:
                            return new NullVersionDetector();
                    }
                }).Named<IInputVersionDetector>(entity.Key);

                // INPUT READER
                builder.Register<IRead>(ctx => {
                    var input = ctx.ResolveNamed<InputContext>(entity.Key);
                    var rowFactory = ctx.ResolveNamed<IRowFactory>(entity.Key, new NamedParameter("capacity", input.RowCapacity));

                    switch (input.Connection.Provider) {
                        case "elasticsearch":
                            if (input.Entity.Query == string.Empty) {
                                return new ElasticReader(input, input.InputFields, ctx.ResolveNamed<IElasticLowLevelClient>(input.Connection.Key), rowFactory, ReadFrom.Input);
                            }
                            return new ElasticQueryReader(input, ctx.ResolveNamed<IElasticLowLevelClient>(input.Connection.Key), rowFactory);
                        default:
                            return new NullReader(input, false);
                    }
                }).Named<IRead>(entity.Key);


            }

            // Entity Output
            if (_process.Output().Provider == "elasticsearch") {

                // PROCESS OUTPUT CONTROLLER
                builder.Register<IOutputController>(ctx => new NullOutputController()).As<IOutputController>();

                // PROCESS INITIALIZER
                builder.Register<IInitializer>(ctx => {
                    var output = ctx.Resolve<OutputContext>();
                    return new ElasticInitializer(output, ctx.ResolveNamed<IElasticLowLevelClient>(output.Connection.Key));
                }).As<IInitializer>();

                foreach (var entity in _process.Entities) {

                    // UPDATER
                    builder.Register<IUpdate>(ctx => {
                        var output = ctx.ResolveNamed<OutputContext>(entity.Key);
                        output.Warn($"{output.Connection.Provider} does not denormalize.");
                        return new NullMasterUpdater();
                    }).Named<IUpdate>(entity.Key);

                    // OUTPUT
                    builder.Register<IOutputController>(ctx => {

                        var output = ctx.ResolveNamed<OutputContext>(entity.Key);
                        switch (output.Connection.Provider) {
                            case "elasticsearch":
                                var initializer = _process.Mode == "init" ? (IAction)new ElasticEntityInitializer(output, ctx.ResolveNamed<IElasticLowLevelClient>(output.Connection.Key)) : new NullInitializer();
                                return new ElasticOutputController(
                                    output,
                                    initializer,
                                    ctx.ResolveNamed<IInputVersionDetector>(entity.Key),
                                    new ElasticOutputVersionDetector(output, ctx.ResolveNamed<IElasticLowLevelClient>(output.Connection.Key)),
                                    ctx.ResolveNamed<IElasticLowLevelClient>(output.Connection.Key)
                                );
                            default:
                                return new NullOutputController();
                        }

                    }).Named<IOutputController>(entity.Key);

                    // WRITER
                    builder.Register<IWrite>(ctx => {
                        var output = ctx.ResolveNamed<OutputContext>(entity.Key);

                        switch (output.Connection.Provider) {
                            case "elasticsearch":
                                return new ElasticWriter(output, ctx.ResolveNamed<IElasticLowLevelClient>(output.Connection.Key));
                            default:
                                return new NullWriter(output);
                        }
                    }).Named<IWrite>(entity.Key);

                    // DELETE HANDLER
                    if (entity.Delete) {
                        builder.Register<IEntityDeleteHandler>(ctx => {

                            var context = ctx.ResolveNamed<IContext>(entity.Key);
                            var inputContext = ctx.ResolveNamed<InputContext>(entity.Key);
                            var rowFactory = ctx.ResolveNamed<IRowFactory>(entity.Key, new NamedParameter("capacity", inputContext.RowCapacity));
                            IRead input = new NullReader(context);
                            var primaryKey = entity.GetPrimaryKey();

                            switch (inputContext.Connection.Provider) {
                                case "elasticsearch":
                                    input = new ElasticReader(
                                        inputContext,
                                        primaryKey,
                                        ctx.ResolveNamed<IElasticLowLevelClient>(inputContext.Connection.Key),
                                        rowFactory,
                                        ReadFrom.Input
                                    );
                                    break;
                            }

                            IRead output = new NullReader(context);
                            IDelete deleter = new NullDeleter(context);
                            var outputConnection = _process.Output();
                            var outputContext = ctx.ResolveNamed<OutputContext>(entity.Key);

                            switch (outputConnection.Provider) {
                                case "elasticsearch":
                                    output = new ElasticReader(
                                        outputContext,
                                        primaryKey,
                                        ctx.ResolveNamed<IElasticLowLevelClient>(inputContext.Connection.Key),
                                        rowFactory,
                                        ReadFrom.Output
                                    );
                                    deleter = new ElasticPartialUpdater(
                                        outputContext,
                                        new[] { context.Entity.TflDeleted() },
                                        ctx.ResolveNamed<IElasticLowLevelClient>(inputContext.Connection.Key)
                                    );
                                    break;
                            }

                            var handler = new DefaultDeleteHandler(context, input, output, deleter);

                            // since the primary keys from the input may have been transformed into the output, you have to transform before comparing
                            // feels a lot like entity pipeline on just the primary keys... may look at consolidating
                            handler.Register(new DefaultTransform(context, entity.GetPrimaryKey().ToArray()));
                            handler.Register(TransformFactory.GetTransforms(ctx, _process, entity, primaryKey));
                            handler.Register(new StringTruncateTransfom(context, primaryKey));

                            return new ParallelDeleteHandler(handler);
                        }).Named<IEntityDeleteHandler>(entity.Key);
                    }

                }
            }


        }
    }
}