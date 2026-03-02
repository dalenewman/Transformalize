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

using System;
using System.Collections.Generic;
using System.Linq;
using Autofac;
using Autofac.Core;
using Cfg.Net.Reader;
using SolrNet;
using SolrNet.Impl;
using SolrNet.Impl.DocumentPropertyVisitors;
using SolrNet.Impl.FacetQuerySerializers;
using SolrNet.Impl.FieldParsers;
using SolrNet.Impl.FieldSerializers;
using SolrNet.Impl.QuerySerializers;
using SolrNet.Impl.ResponseParsers;
using SolrNet.Mapping;
using SolrNet.Mapping.Validation;
using SolrNet.Mapping.Validation.Rules;
using SolrNet.Schema;
using Transformalize.Configuration;
using Transformalize.Context;
using Transformalize.Contracts;
using Transformalize.Extensions;
using Transformalize.Nulls;
using Transformalize.Providers.Solr.Ext;

namespace Transformalize.Providers.Solr.Autofac {

   public class SolrBuilder {

      private const string Solr = "solr";
      private readonly Process _process;
      private readonly ContainerBuilder _builder;

      public SolrBuilder(Process process, ContainerBuilder builder) {
         _process = process ?? throw new ArgumentException("SolrBuilder's constructor must be provided with a non-null process.", nameof(process));
         _builder = builder ?? throw new ArgumentException("SolrBuilder's constructor must be provided with a non-null builder.", nameof(builder));
      }

      public void Build() {

         // SolrNet
         var mapper = new MemoizingMappingManager(new AttributesMappingManager());
         _builder.RegisterInstance(mapper).As<IReadOnlyMappingManager>();
         _builder.RegisterType<NullCache>().As<ISolrCache>();
         _builder.RegisterType<DefaultDocumentVisitor>().As<ISolrDocumentPropertyVisitor>();
         _builder.RegisterType<DefaultFieldParser>().As<ISolrFieldParser>();
         _builder.RegisterGeneric(typeof(SolrDocumentActivator<>)).As(typeof(ISolrDocumentActivator<>));
         _builder.RegisterGeneric(typeof(SolrDocumentResponseParser<>)).As(typeof(ISolrDocumentResponseParser<>));
         _builder.RegisterType<DefaultFieldSerializer>().As<ISolrFieldSerializer>();
         _builder.RegisterType<DefaultQuerySerializer>().As<ISolrQuerySerializer>();
         _builder.RegisterType<DefaultFacetQuerySerializer>().As<ISolrFacetQuerySerializer>();
         _builder.RegisterGeneric(typeof(DefaultResponseParser<>)).As(typeof(ISolrAbstractResponseParser<>));

         _builder.RegisterType<HeaderResponseParser<string>>().As<ISolrHeaderResponseParser>();
         _builder.RegisterType<ExtractResponseParser>().As<ISolrExtractResponseParser>();

         _builder.RegisterType(typeof(MappedPropertiesIsInSolrSchemaRule)).As<IValidationRule>();
         _builder.RegisterType(typeof(RequiredFieldsAreMappedRule)).As<IValidationRule>();
         _builder.RegisterType(typeof(UniqueKeyMatchesMappingRule)).As<IValidationRule>();
         _builder.RegisterType(typeof(MultivaluedMappedToCollectionRule)).As<IValidationRule>();

         _builder.RegisterType<SolrSchemaParser>().As<ISolrSchemaParser>();
         _builder.RegisterGeneric(typeof(SolrMoreLikeThisHandlerQueryResultsParser<>)).As(typeof(ISolrMoreLikeThisHandlerQueryResultsParser<>));
         _builder.RegisterGeneric(typeof(SolrQueryExecuter<>)).As(typeof(ISolrQueryExecuter<>));
         _builder.RegisterGeneric(typeof(SolrDocumentSerializer<>)).As(typeof(ISolrDocumentSerializer<>));
         _builder.RegisterType<SolrDIHStatusParser>().As<ISolrDIHStatusParser>();
         _builder.RegisterType<MappingValidator>().As<IMappingValidator>();
         _builder.RegisterType<SolrDictionarySerializer>().As<ISolrDocumentSerializer<Dictionary<string, object>>>();
         _builder.RegisterType<SolrDictionaryDocumentResponseParser>().As<ISolrDocumentResponseParser<Dictionary<string, object>>>();

         //MAPS
         foreach (var map in _process.Maps.Where(m => m.Connection != string.Empty && m.Query != string.Empty)) {
            var connection = _process.Connections.First(c => c.Name == map.Connection);
            if (connection != null && connection.Provider == Solr) {
               _builder.Register<IMapReader>(ctx => new DefaultMapReader()).Named<IMapReader>(map.Name);
            }
         }

         // connections
         foreach (var connection in _process.Connections.Where(c => c.Provider.In(Solr))) {

            connection.Url = connection.BuildSolrUrl();
            RegisterCore(_builder, connection);

            _builder.Register<ISchemaReader>(ctx => {

               var solr = ctx.ResolveNamed<ISolrReadOnlyOperations<Dictionary<string, object>>>(connection.Key);
               return new SolrSchemaReader(connection, solr);
            }).Named<ISchemaReader>(connection.Key);
         }

         // entity input
         foreach (var entity in _process.Entities.Where(e => _process.Connections.First(c => c.Name == e.Input).Provider == Solr)) {

            _builder.Register<IInputProvider>(ctx => {
               var input = ctx.ResolveNamed<InputContext>(entity.Key);
               switch (input.Connection.Provider) {
                  case Solr:
                     return new SolrInputProvider(input, ctx.ResolveNamed<ISolrReadOnlyOperations<Dictionary<string, object>>>(input.Connection.Key));
                  default:
                     return new NullInputProvider();
               }
            }).Named<IInputProvider>(entity.Key);

            // INPUT READER
            _builder.Register<IRead>(ctx => {
               var input = ctx.ResolveNamed<InputContext>(entity.Key);
               var rowFactory = ctx.ResolveNamed<IRowFactory>(entity.Key, new NamedParameter("capacity", input.RowCapacity));

               switch (input.Connection.Provider) {
                  case Solr:
                     var solr = ctx.ResolveNamed<ISolrReadOnlyOperations<Dictionary<string, object>>>(input.Connection.Key);
                     return new SolrInputReader(solr, input, input.InputFields, rowFactory);
                  default:
                     return new NullReader(input, false);
               }
            }).Named<IRead>(entity.Key);

         }

         // entity output
         if (_process.GetOutputConnection().Provider == Solr) {

            // PROCESS OUTPUT CONTROLLER
            _builder.Register<IOutputController>(ctx => new NullOutputController()).As<IOutputController>();

            foreach (var entity in _process.Entities) {

               // INPUT VALIDATOR
               _builder.Register<IInputValidator>(ctx => {
                  var input = ctx.ResolveNamed<InputContext>(entity.Key);
                  return new SolrInputValidator(
                      input,
                      ctx.ResolveNamed<ISolrReadOnlyOperations<Dictionary<string, object>>>(input.Connection.Key)
                  );
               }).Named<IInputValidator>(entity.Key);

               // UPDATER
               _builder.Register<IUpdate>(ctx => {
                  var output = ctx.ResolveNamed<OutputContext>(entity.Key);
                  output.Debug(() => $"{output.Connection.Provider} does not denormalize.");
                  return new NullMasterUpdater();
               }).Named<IUpdate>(entity.Key);

               // OUTPUT
               _builder.Register<IOutputProvider>((ctx) => {
                  var output = ctx.ResolveNamed<OutputContext>(entity.Key);
                  var solr = ctx.ResolveNamed<ISolrReadOnlyOperations<Dictionary<string, object>>>(output.Connection.Key);
                  return new SolrOutputProvider(output, solr);
               }).Named<IOutputProvider>(entity.Key);

               _builder.Register<IOutputController>(ctx => {

                  var output = ctx.ResolveNamed<OutputContext>(entity.Key);

                  switch (output.Connection.Provider) {
                     case Solr:
                        var solr = ctx.ResolveNamed<ISolrReadOnlyOperations<Dictionary<string, object>>>(output.Connection.Key);

                        var initializer = _process.Mode == "init" ? (IInitializer)new SolrInitializer(
                                output,
                                ctx.ResolveNamed<ISolrCoreAdmin>(output.Connection.Key),
                                ctx.ResolveNamed<ISolrOperations<Dictionary<string, object>>>(output.Connection.Key),
                                new RazorTemplateEngine(ctx.ResolveNamed<OutputContext>(entity.Key), new Template { Name = output.Connection.Key, File = "files\\solr\\schema.cshtml" }, new FileReader()),
                                new RazorTemplateEngine(ctx.ResolveNamed<OutputContext>(entity.Key), new Template { Name = output.Connection.Key, File = "files\\solr\\solrconfig.cshtml" }, new FileReader())
                            ) : new NullInitializer();

                        return new SolrOutputController(
                            output,
                            initializer,
                            ctx.ResolveNamed<IInputProvider>(entity.Key),
                            ctx.ResolveNamed<IOutputProvider>(entity.Key),
                            solr
                        );
                     default:
                        return new NullOutputController();
                  }

               }).Named<IOutputController>(entity.Key);

               // WRITER
               _builder.Register<IWrite>(ctx => {
                  var output = ctx.ResolveNamed<OutputContext>(entity.Key);
                  switch (output.Connection.Provider) {
                     case Solr:
                        if (output.Connection.MaxDegreeOfParallelism > 1) {
                           return new ParallelSolrWriter(output, ctx.ResolveNamed<ISolrOperations<Dictionary<string, object>>>(output.Connection.Key));
                        } else {
                           return new SolrWriter2(output, ctx.ResolveNamed<ISolrOperations<Dictionary<string, object>>>(output.Connection.Key));
                        }
                     default:
                        return new NullWriter(output);
                  }
               }).Named<IWrite>(entity.Key);
            }
         }
      }

      private static void RegisterCore(ContainerBuilder builder, Connection connection) {
         var url = connection.Url;
         var key = connection.Key;

         builder.Register(ctx => new SolrConnection(url) { Timeout = connection.RequestTimeout * 1000 }).Named<ISolrConnection>(key).InstancePerLifetimeScope();

         builder.RegisterType<SolrQueryExecuter<Dictionary<string, object>>>()
             .Named<ISolrQueryExecuter<Dictionary<string, object>>>(key)
             .WithParameters(new[] {
                    new ResolvedParameter((p, c) => p.Name == "connection", (p, c) => c.ResolveNamed(key, typeof (ISolrConnection))),
         }).InstancePerLifetimeScope();

         builder.RegisterType<SolrBasicServer<Dictionary<string, object>>>()
             .Named<ISolrBasicOperations<Dictionary<string, object>>>(key)
             .WithParameters(new[] {
                    new ResolvedParameter((p, c) => p.Name == "connection", (p, c) => c.ResolveNamed<ISolrConnection>(key)),
                    new ResolvedParameter((p, c) => p.Name == "queryExecuter", (p, c) => c.ResolveNamed<ISolrQueryExecuter<Dictionary<string,object>>>(connection.Key))
             }).InstancePerLifetimeScope();

         builder.RegisterType<SolrBasicServer<Dictionary<string, object>>>()
             .Named<ISolrBasicReadOnlyOperations<Dictionary<string, object>>>(key)
             .WithParameters(new[] {
                    new ResolvedParameter((p, c) => p.Name == "connection", (p, c) => c.ResolveNamed<ISolrConnection>(key)),
                    new ResolvedParameter((p, c) => p.Name == "queryExecuter", (p, c) => c.ResolveNamed<ISolrQueryExecuter<Dictionary<string,object>>>(key))
             }).InstancePerLifetimeScope();

         builder.RegisterType<SolrServer<Dictionary<string, object>>>()
             .Named<ISolrOperations<Dictionary<string, object>>>(key)
             .WithParameters(new[] {
                    new ResolvedParameter((p, c) => p.Name == "basicServer", (p, c) => c.ResolveNamed<ISolrBasicOperations<Dictionary<string,object>>>(key)),
             }).InstancePerLifetimeScope();

         builder.RegisterType<SolrServer<Dictionary<string, object>>>()
             .Named<ISolrReadOnlyOperations<Dictionary<string, object>>>(key)
             .WithParameters(new[] {
                    new ResolvedParameter((p, c) => p.Name == "basicServer", (p, c) => c.ResolveNamed<ISolrBasicOperations<Dictionary<string,object>>>(key)),
             }).InstancePerLifetimeScope();

         // modified url to not include the core
         builder.RegisterType<SolrCoreAdmin>()
             .Named<ISolrCoreAdmin>(key)
             .WithParameters(new[] {
                    new ResolvedParameter((p, c)=> p.Name == "connection", (p, c) => new SolrConnection(url.Substring(0, url.Length - connection.Core.Length - 1)){ Timeout = connection.Timeout * 1000 }),
                    new ResolvedParameter((p, c)=> p.Name == "headerParser", (p, c) => c.Resolve<ISolrHeaderResponseParser>()),
                    new ResolvedParameter((p, c)=> p.Name == "resultParser", (p, c) => new SolrStatusResponseParser())
             })
             .As<ISolrCoreAdmin>().InstancePerLifetimeScope();
      }
   }
}