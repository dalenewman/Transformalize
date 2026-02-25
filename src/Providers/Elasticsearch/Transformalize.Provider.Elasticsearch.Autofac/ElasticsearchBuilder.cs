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

using Autofac;
using Elasticsearch.Net;
using System;
using System.Collections.Generic;
using System.Linq;
using Transformalize.Configuration;
using Transformalize.Containers.Autofac;
using Transformalize.Context;
using Transformalize.Contracts;
using Transformalize.Impl;
using Transformalize.Nulls;
using Transformalize.Providers.Elasticsearch.Ext;
using Transformalize.Transforms.System;

namespace Transformalize.Providers.Elasticsearch.Autofac {

   public class ElasticsearchBuilder {

      private readonly ContainerBuilder _builder;
      private readonly Process _process;

      public ElasticsearchBuilder(Process process, ContainerBuilder builder) {
         _process = process ?? throw new ArgumentException("ElasticsearchBuilder's constructor must be provided with a non-null process.", nameof(process));
         _builder = builder ?? throw new ArgumentException("ElasticsearchBuilder's constructor must be provided with a non-null builder.", nameof(builder));
      }

      public void Build() {

         //MAPS
         foreach (var map in _process.Maps.Where(m => m.Connection != string.Empty && m.Query != string.Empty)) {
            var connection = _process.Connections.First(c => c.Name == map.Connection);
            if (connection != null && connection.Provider == "elasticsearch") {
               _builder.Register<IMapReader>(ctx => new DefaultMapReader()).Named<IMapReader>(map.Name);
            }
         }

         //CONNECTIONS
         foreach (var connection in _process.Connections.Where(c => c.Provider == "elasticsearch")) {

            if (connection.Servers.Any(s=>s.Url != "None")) {
               var uris = new List<Uri>();
               foreach (var server in connection.Servers.Where(s=>s.Url != "None")) {
                  server.Url = server.GetElasticUrl();
                  uris.Add(new Uri(server.Url));
               }
               // for now, just use static connection pool, there are 2 other types...
               _builder.Register<IConnectionPool>(ctx => new StaticConnectionPool(uris)).Named<IConnectionPool>(connection.Key);
            } else {
               connection.Url = connection.GetElasticUrl();
               _builder.Register<IConnectionPool>(ctx => new SingleNodeConnectionPool(new Uri(connection.Url))).Named<IConnectionPool>(connection.Key);
            }

            // Elasticsearch.Net
            _builder.Register(ctx => {

               var pool = ctx.ResolveNamed<IConnectionPool>(connection.Key);
               var settings = new ConnectionConfiguration(pool);

               var version = ElasticVersionParser.ParseVersion(ctx.ResolveNamed<IConnectionContext>(connection.Key));

               if (!string.IsNullOrEmpty(connection.User)) {
                  settings = settings.BasicAuthentication(connection.User, connection.Password);
               }

               if(version.Major > 7 || (version.Major == 7 && version.Minor >= 11)) {
                  settings.EnableApiVersioningHeader(true);
               }

               if (string.IsNullOrEmpty(connection.CertificateFingerprint)) {
                  settings.ServerCertificateValidationCallback(CertificateValidations.AllowAll);    
               } else {
                  settings = settings.CertificateFingerprint(connection.CertificateFingerprint);
               }

               if (_process.Mode != "init" && connection.RequestTimeout >= 0) {
                  settings = settings.RequestTimeout(new TimeSpan(0, 0, 0, connection.RequestTimeout * 1000));
               }
               if (connection.Timeout > 0) {
                  settings = settings.PingTimeout(new TimeSpan(0, 0, connection.Timeout));
               }

               return new ElasticLowLevelClient(settings);
            }).Named<IElasticLowLevelClient>(connection.Key);

            // Process-Level Schema Reader
            _builder.Register<ISchemaReader>(ctx => new ElasticSchemaReader(ctx.ResolveNamed<IConnectionContext>(connection.Key), ctx.ResolveNamed<IElasticLowLevelClient>(connection.Key))).Named<ISchemaReader>(connection.Key);

            // Entity Level Schema Readers
            foreach (var entity in _process.Entities.Where(e => e.Input == connection.Name)) {
               _builder.Register<ISchemaReader>(ctx => new ElasticSchemaReader(ctx.ResolveNamed<IConnectionContext>(entity.Key), ctx.ResolveNamed<IElasticLowLevelClient>(connection.Key))).Named<ISchemaReader>(entity.Key);
            }

         }

         // Entity Input
         foreach (var entity in _process.Entities.Where(e => _process.Connections.First(c => c.Name == e.Input).Provider == "elasticsearch")) {

            _builder.Register<IInputProvider>(ctx => {
               var input = ctx.ResolveNamed<InputContext>(entity.Key);
               return new ElasticInputProvider(input, ctx.ResolveNamed<IElasticLowLevelClient>(input.Connection.Key));
            }).Named<IInputProvider>(entity.Key);

            // INPUT READER
            _builder.Register<IRead>(ctx => {
               var input = ctx.ResolveNamed<InputContext>(entity.Key);
               var rowFactory = ctx.ResolveNamed<IRowFactory>(entity.Key, new NamedParameter("capacity", input.RowCapacity));

               if (entity.Query == string.Empty) {
                  return new ElasticReader(input, input.InputFields, ctx.ResolveNamed<IElasticLowLevelClient>(input.Connection.Key), rowFactory, ReadFrom.Input);
               }
               return new ElasticQueryReader(input, ctx.ResolveNamed<IElasticLowLevelClient>(input.Connection.Key), rowFactory);
            }).Named<IRead>(entity.Key);

         }

         // Entity Output
         if (_process.GetOutputConnection().Provider == "elasticsearch") {

            // PROCESS OUTPUT CONTROLLER
            _builder.Register<IOutputController>(ctx => new NullOutputController()).As<IOutputController>();

            // PROCESS INITIALIZER
            _builder.Register<IInitializer>(ctx => {
               var output = ctx.Resolve<OutputContext>();
               return new ElasticInitializer(output, ctx.ResolveNamed<IElasticLowLevelClient>(output.Connection.Key));
            }).As<IInitializer>();

            foreach (var entity in _process.Entities) {

               // UPDATER
               _builder.Register<IUpdate>(ctx => {
                  var output = ctx.ResolveNamed<OutputContext>(entity.Key);
                  output.Debug(() => $"{output.Connection.Provider} does not denormalize.");
                  return new NullMasterUpdater();
               }).Named<IUpdate>(entity.Key);

               // OUTPUT
               _builder.Register<IOutputController>(ctx => {

                  var output = ctx.ResolveNamed<OutputContext>(entity.Key);
                  switch (output.Connection.Provider) {
                     case "elasticsearch":
                        var initializer = _process.Mode == "init" ? (IAction)new ElasticEntityInitializer(output, ctx.ResolveNamed<IElasticLowLevelClient>(output.Connection.Key)) : new NullInitializer();
                        return new ElasticOutputController(
                            output,
                            initializer,
                            ctx.ResolveNamed<IInputProvider>(entity.Key),
                            new ElasticOutputProvider(output, ctx.ResolveNamed<IElasticLowLevelClient>(output.Connection.Key)),
                            ctx.ResolveNamed<IElasticLowLevelClient>(output.Connection.Key)
                        );
                     default:
                        return new NullOutputController();
                  }

               }).Named<IOutputController>(entity.Key);

               // WRITER
               _builder.Register<IWrite>(ctx => {
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
                  _builder.Register<IEntityDeleteHandler>(ctx => {

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
                     var outputConnection = _process.GetOutputConnection();
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
                     handler.Register(TransformFactory.GetTransforms(ctx, context, primaryKey));
                     handler.Register(new StringTruncateTransfom(context, primaryKey));

                     return handler;
                  }).Named<IEntityDeleteHandler>(entity.Key);
               }

            }
         }
      }
   }
}