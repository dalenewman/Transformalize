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
using System.Collections.Generic;
using System.Linq;
using Autofac;
using Autofac.Core;
using Pipeline.Configuration;
using Pipeline.Context;
using Pipeline.Contracts;
using Pipeline.Extensions;
using Pipeline.Nulls;
using Pipeline.Provider.Solr;
using Pipeline.Provider.Solr.Ext;
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

namespace Pipeline.Ioc.Autofac.Modules {
    public class SolrModule : Module {

        private readonly Process _process;

        public SolrModule() { }

        public SolrModule(Process process) {
            _process = process;
        }

        protected override void Load(ContainerBuilder builder) {

            if (_process == null)
                return;

            // SolrNet
            var mapper = new MemoizingMappingManager(new AttributesMappingManager());
            builder.RegisterInstance(mapper).As<IReadOnlyMappingManager>();
            builder.RegisterType<NullCache>().As<ISolrCache>();
            builder.RegisterType<DefaultDocumentVisitor>().As<ISolrDocumentPropertyVisitor>();
            builder.RegisterType<DefaultFieldParser>().As<ISolrFieldParser>();
            builder.RegisterGeneric(typeof(SolrDocumentActivator<>)).As(typeof(ISolrDocumentActivator<>));
            builder.RegisterGeneric(typeof(SolrDocumentResponseParser<>)).As(typeof(ISolrDocumentResponseParser<>));
            builder.RegisterType<DefaultFieldSerializer>().As<ISolrFieldSerializer>();
            builder.RegisterType<DefaultQuerySerializer>().As<ISolrQuerySerializer>();
            builder.RegisterType<DefaultFacetQuerySerializer>().As<ISolrFacetQuerySerializer>();
            builder.RegisterGeneric(typeof(DefaultResponseParser<>)).As(typeof(ISolrAbstractResponseParser<>));

            builder.RegisterType<HeaderResponseParser<string>>().As<ISolrHeaderResponseParser>();
            builder.RegisterType<ExtractResponseParser>().As<ISolrExtractResponseParser>();

            builder.RegisterType(typeof(MappedPropertiesIsInSolrSchemaRule)).As<IValidationRule>();
            builder.RegisterType(typeof(RequiredFieldsAreMappedRule)).As<IValidationRule>();
            builder.RegisterType(typeof(UniqueKeyMatchesMappingRule)).As<IValidationRule>();
            builder.RegisterType(typeof(MultivaluedMappedToCollectionRule)).As<IValidationRule>();

            builder.RegisterType<SolrSchemaParser>().As<ISolrSchemaParser>();
            builder.RegisterGeneric(typeof(SolrMoreLikeThisHandlerQueryResultsParser<>)).As(typeof(ISolrMoreLikeThisHandlerQueryResultsParser<>));
            builder.RegisterGeneric(typeof(SolrQueryExecuter<>)).As(typeof(ISolrQueryExecuter<>));
            builder.RegisterGeneric(typeof(SolrDocumentSerializer<>)).As(typeof(ISolrDocumentSerializer<>));
            builder.RegisterType<SolrDIHStatusParser>().As<ISolrDIHStatusParser>();
            builder.RegisterType<MappingValidator>().As<IMappingValidator>();
            builder.RegisterType<SolrDictionarySerializer>().As<ISolrDocumentSerializer<Dictionary<string, object>>>();
            builder.RegisterType<SolrDictionaryDocumentResponseParser>().As<ISolrDocumentResponseParser<Dictionary<string, object>>>();


            // connections
            foreach (var connection in _process.Connections.Where(c => c.Provider.In("solr"))) {

                connection.Url = connection.BuildSolrUrl();
                RegisterCore(builder, connection);

                builder.Register<ISchemaReader>(ctx => {
                    Startup.Init<Dictionary<string, object>>(connection.Url);
                    var solr = ctx.ResolveNamed<ISolrReadOnlyOperations<Dictionary<string, object>>>(connection.Key);
                    return new SolrSchemaReader(connection, solr);
                }).Named<ISchemaReader>(connection.Key);
            }

            // entity input
            foreach (var entity in _process.Entities.Where(e => _process.Connections.First(c => c.Name == e.Connection).Provider == "solr")) {

                builder.Register<IInputVersionDetector>(ctx => {
                    var input = ctx.ResolveNamed<InputContext>(entity.Key);
                    switch (input.Connection.Provider) {
                        case "solr":
                            return new SolrInputVersionDetector(input, ctx.ResolveNamed<ISolrReadOnlyOperations<Dictionary<string, object>>>(input.Connection.Key));
                        default:
                            return new NullVersionDetector();
                    }
                }).Named<IInputVersionDetector>(entity.Key);

                // INPUT READER
                builder.Register<IRead>(ctx => {
                    var input = ctx.ResolveNamed<InputContext>(entity.Key);
                    var rowFactory = ctx.ResolveNamed<IRowFactory>(entity.Key, new NamedParameter("capacity", input.RowCapacity));

                    switch (input.Connection.Provider) {
                        case "solr":
                            var solr = ctx.ResolveNamed<ISolrReadOnlyOperations<Dictionary<string, object>>>(input.Connection.Key);
                            return new SolrInputReader(solr, input, input.InputFields, rowFactory);
                        default:
                            return new NullReader(input, false);
                    }
                }).Named<IRead>(entity.Key);

            }

            // entity output
            if (_process.Output().Provider == "solr") {

                // PROCESS OUTPUT CONTROLLER
                builder.Register<IOutputController>(ctx => new NullOutputController()).Named<IOutputController>(_process.Key);

                foreach (var entity in _process.Entities) {

                    // INPUT VALIDATOR
                    builder.Register<IInputValidator>(ctx => {
                        var input = ctx.ResolveNamed<InputContext>(entity.Key);
                        return new SolrInputValidator(
                            input,
                            ctx.ResolveNamed<ISolrReadOnlyOperations<Dictionary<string, object>>>(input.Connection.Key)
                        );
                    }).Named<IInputValidator>(entity.Key);

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
                            case "solr":
                                var solr = ctx.ResolveNamed<ISolrReadOnlyOperations<Dictionary<string, object>>>(output.Connection.Key);
                                return new SolrOutputController(
                                    output,
                                    new NullInitializer(),
                                    ctx.ResolveNamed<IInputVersionDetector>(entity.Key),
                                    new SolrOutputVersionDetector(output, solr),
                                    solr
                                );
                            default:
                                return new NullOutputController();
                        }

                    }).Named<IOutputController>(entity.Key);

                    // WRITER
                    builder.Register<IWrite>(ctx => {
                        var output = ctx.ResolveNamed<OutputContext>(entity.Key);

                        switch (output.Connection.Provider) {
                            case "solr":
                                output.Warn($"The {output.Connection.Provider} does not support output yet.  Currently the author of this library is using Solr's Data Import Handler for loading SOLR.");
                                return new NullWriter(output);
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

            builder.Register((ctx => new SolrConnection(url))).Named<ISolrConnection>(key);

            builder.RegisterType<SolrQueryExecuter<Dictionary<string, object>>>()
                .Named<ISolrQueryExecuter<Dictionary<string, object>>>(key)
                .WithParameters(new[] {
                    new ResolvedParameter((p, c) => p.Name == "connection", (p, c) => c.ResolveNamed(key, typeof (ISolrConnection))),
            });

            builder.RegisterType<SolrBasicServer<Dictionary<string, object>>>()
                .Named<ISolrBasicOperations<Dictionary<string, object>>>(key)
                .WithParameters(new[] {
                    new ResolvedParameter((p, c) => p.Name == "connection", (p, c) => c.ResolveNamed<ISolrConnection>(key)),
                    new ResolvedParameter((p, c) => p.Name == "queryExecuter", (p, c) => c.ResolveNamed<ISolrQueryExecuter<Dictionary<string,object>>>(connection.Key))
                });

            builder.RegisterType<SolrBasicServer<Dictionary<string, object>>>()
                .Named<ISolrBasicReadOnlyOperations<Dictionary<string, object>>>(key)
                .WithParameters(new[] {
                    new ResolvedParameter((p, c) => p.Name == "connection", (p, c) => c.ResolveNamed<ISolrConnection>(key)),
                    new ResolvedParameter((p, c) => p.Name == "queryExecuter", (p, c) => c.ResolveNamed<ISolrQueryExecuter<Dictionary<string,object>>>(key))
                });

            builder.RegisterType<SolrServer<Dictionary<string, object>>>()
                .Named<ISolrOperations<Dictionary<string, object>>>(key)
                .WithParameters(new[] {
                    new ResolvedParameter((p, c) => p.Name == "basicServer", (p, c) => c.ResolveNamed<ISolrBasicOperations<Dictionary<string,object>>>(key)),
                });

            builder.RegisterType<SolrServer<Dictionary<string, object>>>()
                .Named<ISolrReadOnlyOperations<Dictionary<string, object>>>(key)
                .WithParameters(new[] {
                    new ResolvedParameter((p, c) => p.Name == "basicServer", (p, c) => c.ResolveNamed<ISolrBasicOperations<Dictionary<string,object>>>(key)),
                });
        }

    }
}