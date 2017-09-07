#region license
// Transformalize
// Configurable Extract, Transform, and Load
// Copyright 2013-2017 Dale Newman
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

using System.Collections.Generic;
using System.Linq;
using Autofac;
using Transformalize.Configuration;
using Transformalize.Context;
using Transformalize.Contracts;
using Transformalize.Extensions;
using Transformalize.Nulls;
using Transformalize.Providers.Ado;
using Transformalize.Providers.MySql;
using Transformalize.Providers.PostgreSql;
using Transformalize.Providers.SqlServer;
using Transformalize.Providers.SQLite;
using Transformalize.Transforms.System;
using Pipeline.Web.Orchard.Impl;
using Transformalize;
using Transformalize.Providers.SqlCe;

namespace Pipeline.Web.Orchard.Modules {
    public class AdoModule : Module {
        private readonly Process _process;
        private readonly HashSet<string> _ado = Constants.AdoProviderSet();

        public AdoModule() { }

        public AdoModule(Process process) {
            _process = process;
        }

        protected override void Load(ContainerBuilder builder) {

            if (_process == null)
                return;

            // connections
            foreach (var connection in _process.Connections.Where(c => _ado.Contains(c.Provider))) {

                var cn = connection;
                // Connection Factory
                builder.Register<IConnectionFactory>(ctx => {
                    switch (cn.Provider) {
                        case "sqlserver":
                            return new SqlServerConnectionFactory(cn);
                        case "mysql":
                            return new MySqlConnectionFactory(cn);
                        case "postgresql":
                            return new PostgreSqlConnectionFactory(cn);
                        case "sqlite":
                            return new SqLiteConnectionFactory(cn);
                        case "sqlce":
                            return new SqlCeConnectionFactory(cn);
                        default:
                            return new NullConnectionFactory();
                    }
                }).Named<IConnectionFactory>(connection.Key).InstancePerLifetimeScope();

                // Schema Reader
                builder.Register<ISchemaReader>(ctx => {
                    var factory = ctx.ResolveNamed<IConnectionFactory>(cn.Key);
                    return new AdoSchemaReader(ctx.ResolveNamed<IConnectionContext>(cn.Key), factory);
                }).Named<ISchemaReader>(connection.Key);

            }

            //ISchemaReader
            //IOutputController
            //IRead (Process for Calculated Columns)
            //IWrite (Process for Calculated Columns)
            //IInitializer (Process)

            // Per Entity
            // IInputVersionDetector
            // IRead (Input, per Entity)
            // IOutputController
            // -- ITakeAndReturnRows (for matching)
            // -- IWriteMasterUpdateQuery (for updating)
            // IUpdate
            // IWrite
            // IEntityDeleteHandler

            // entitiy input
            foreach (var entity in _process.Entities.Where(e => _ado.Contains(_process.Connections.First(c => c.Name == e.Connection).Provider))) {

                // INPUT READER
                builder.Register<IRead>(ctx => {
                    var input = ctx.ResolveNamed<InputContext>(entity.Key);
                    var rowFactory = ctx.ResolveNamed<IRowFactory>(entity.Key, new NamedParameter("capacity", input.RowCapacity));

                    switch (input.Connection.Provider) {
                        case "mysql":
                        case "postgresql":
                        case "sqlite":
                        case "sqlce":
                        case "sqlserver":
                            return new AdoInputReader(
                                input,
                                input.InputFields,
                                ctx.ResolveNamed<IConnectionFactory>(input.Connection.Key),
                                rowFactory
                            );
                        default:
                            return new NullReader(input, false);
                    }
                }).Named<IRead>(entity.Key);

                // INPUT VERSION DETECTOR
                builder.Register<IInputProvider>(ctx => {
                    var input = ctx.ResolveNamed<InputContext>(entity.Key);
                    switch (input.Connection.Provider) {
                        case "mysql":
                        case "postgresql":
                        case "sqlite":
                        case "sqlce":
                        case "sqlserver":
                            return new AdoInputProvider(input, ctx.ResolveNamed<IConnectionFactory>(input.Connection.Key));
                        default:
                            return new NullInputProvider();
                    }
                }).Named<IInputProvider>(entity.Key);

            }

            // entity output
            if (_ado.Contains(_process.Output().Provider)) {

                var calc = _process.ToCalculatedFieldsProcess();

                // PROCESS OUTPUT CONTROLLER
                builder.Register<IOutputController>(ctx => {
                    var output = ctx.Resolve<OutputContext>();
                    if (_process.Mode != "init")
                        return new NullOutputController();

                    switch (output.Connection.Provider) {
                        case "mysql":
                        case "postgresql":
                        case "sqlite":
                        case "sqlce":
                        case "sqlserver":
                            var actions = new List<IAction> { new AdoStarViewCreator(output, ctx.ResolveNamed<IConnectionFactory>(output.Connection.Key)) };
                            if (_process.Flatten) {
                                actions.Add(new AdoFlatTableCreator(output, ctx.ResolveNamed<IConnectionFactory>(output.Connection.Key)));
                            }
                            return new AdoStarController(output, actions);
                        default:
                            return new NullOutputController();
                    }
                }).As<IOutputController>();

                // PROCESS CALCULATED READER
                builder.Register<IRead>(ctx => {
                    var calcContext = new PipelineContext(ctx.Resolve<IPipelineLogger>(), calc, calc.Entities.First());
                    var outputContext = new OutputContext(calcContext, new Incrementer(calcContext));
                    var cf = ctx.ResolveNamed<IConnectionFactory>(outputContext.Connection.Key);
                    var capacity = outputContext.Entity.Fields.Count + outputContext.Entity.CalculatedFields.Count;
                    var rowFactory = new RowFactory(capacity, false, false);
                    return new AdoStarParametersReader(outputContext, _process, cf, rowFactory);
                }).As<IRead>();

                // PROCESS CALCULATED FIELD WRITER
                builder.Register<IWrite>(ctx => {
                    var calcContext = new PipelineContext(ctx.Resolve<IPipelineLogger>(), calc, calc.Entities.First());
                    var outputContext = new OutputContext(calcContext, new Incrementer(calcContext));
                    var cf = ctx.ResolveNamed<IConnectionFactory>(outputContext.Connection.Key);
                    return new AdoCalculatedFieldUpdater(outputContext, _process, cf);
                }).As<IWrite>();

                // PROCESS INITIALIZER
                builder.Register<IInitializer>(ctx => {
                    var output = ctx.Resolve<OutputContext>();
                    return new AdoInitializer(output, ctx.ResolveNamed<IConnectionFactory>(output.Connection.Key));
                }).As<IInitializer>();

                // ENTITIES
                foreach (var e in _process.Entities) {

                    var entity = e;

                    // WRITER
                    builder.Register<IWrite>(ctx => {
                        var output = ctx.ResolveNamed<OutputContext>(entity.Key);
                        var cf = ctx.ResolveNamed<IConnectionFactory>(output.Connection.Key);

                        switch (output.Connection.Provider) {
                            case "sqlserver":
                                return new SqlServerWriter(
                                    output,
                                    cf,
                                    ctx.ResolveNamed<ITakeAndReturnRows>(entity.Key),
                                    new AdoEntityUpdater(output, cf)
                                );
                            case "sqlce":
                                return new SqlCeWriter(
                                    output,
                                    cf,
                                    ctx.ResolveNamed<ITakeAndReturnRows>(entity.Key),
                                    new AdoEntityUpdater(output, cf)
                                );
                            case "mysql":
                            case "postgresql":
                            case "sqlite":
                                return new AdoEntityWriter(
                                    output,
                                    ctx.ResolveNamed<ITakeAndReturnRows>(entity.Key),
                                    new AdoEntityInserter(output, cf),
                                    entity.Update ? (IWrite)new AdoEntityUpdater(output, cf) : new NullWriter(output)
                                );
                            default:
                                return new NullWriter(output);
                        }
                    }).Named<IWrite>(entity.Key);

                    builder.Register<IOutputProvider>(ctx => {
                        var output = ctx.ResolveNamed<OutputContext>(entity.Key);
                        return new AdoOutputProvider(output, ctx.ResolveNamed<IConnectionFactory>(output.Connection.Key), ctx.ResolveNamed<IWrite>(entity.Key));
                    }).Named<IOutputProvider>(entity.Key);

                    // ENTITY OUTPUT CONTROLLER
                    builder.Register<IOutputController>(ctx => {

                        var output = ctx.ResolveNamed<OutputContext>(entity.Key);

                        switch (output.Connection.Provider) {
                            case "mysql":
                            case "postgresql":
                            case "sqlite":
                            case "sqlce":
                            case "sqlserver":
                                var initializer = _process.Mode == "init" ? (IAction)new AdoEntityInitializer(output, ctx.ResolveNamed<IConnectionFactory>(output.Connection.Key)) : new NullInitializer();
                                return new AdoOutputController(
                                    output,
                                    initializer,
                                    ctx.ResolveNamed<IInputProvider>(entity.Key),
                                    ctx.ResolveNamed<IOutputProvider>(entity.Key),
                                    ctx.ResolveNamed<IConnectionFactory>(output.Connection.Key)
                                );
                            default:
                                return new NullOutputController();
                        }

                    }).Named<IOutputController>(entity.Key);

                    // OUTPUT ROW MATCHER
                    builder.Register(ctx => {
                        if (!entity.Update)
                            return new NullTakeAndReturnRows();

                        var output = ctx.ResolveNamed<OutputContext>(entity.Key);
                        var rowFactory = ctx.ResolveNamed<IRowFactory>(entity.Key, new NamedParameter("capacity", output.GetAllEntityFields().Count()));
                        var cf = ctx.ResolveNamed<IConnectionFactory>(output.Connection.Key);
                        switch (output.Connection.Provider) {
                            case "sqlite":
                                return new TypedEntityMatchingKeysReader(new AdoEntityMatchingKeysReader(output, cf, rowFactory), output);
                            default:
                                return (ITakeAndReturnRows)new AdoEntityMatchingKeysReader(output, cf, rowFactory);
                        }
                    }).Named<ITakeAndReturnRows>(entity.Key);

                    // MASTER UPDATE QUERY
                    builder.Register<IWriteMasterUpdateQuery>(ctx => {
                        var output = ctx.ResolveNamed<OutputContext>(entity.Key);
                        var factory = ctx.ResolveNamed<IConnectionFactory>(output.Connection.Key);
                        switch (output.Connection.Provider) {
                            case "mysql":
                                return new MySqlUpdateMasterKeysQueryWriter(output, factory);
                            case "postgresql":
                                return new PostgreSqlUpdateMasterKeysQueryWriter(output, factory);
                            default:
                                return new SqlServerUpdateMasterKeysQueryWriter(output, factory);
                        }
                    }).Named<IWriteMasterUpdateQuery>(entity.Key + "MasterKeys");

                    // MASTER UPDATER
                    builder.Register<IUpdate>(ctx => {
                        var output = ctx.ResolveNamed<OutputContext>(entity.Key);
                        switch (output.Connection.Provider) {
                            case "mysql":
                            case "postgresql":
                            case "sqlserver":
                                return new AdoMasterUpdater(
                                    output,
                                    ctx.ResolveNamed<IConnectionFactory>(output.Connection.Key),
                                    ctx.ResolveNamed<IWriteMasterUpdateQuery>(entity.Key + "MasterKeys")
                                );
                            case "sqlite":
                            case "sqlce":
                                return new AdoTwoPartMasterUpdater(output, ctx.ResolveNamed<IConnectionFactory>(output.Connection.Key));
                            default:
                                return new NullMasterUpdater();
                        }
                    }).Named<IUpdate>(entity.Key);

                    // DELETE HANDLER
                    if (entity.Delete) {
                        builder.Register<IEntityDeleteHandler>(ctx => {
                            var context = ctx.ResolveNamed<IContext>(entity.Key);
                            var inputContext = ctx.ResolveNamed<InputContext>(entity.Key);
                            var rowCapacity = inputContext.Entity.GetPrimaryKey().Count();
                            var rowFactory = new RowFactory(rowCapacity, false, true);
                            IRead input = new NullReader(context);
                            var primaryKey = entity.GetPrimaryKey();

                            switch (inputContext.Connection.Provider) {
                                case "mysql":
                                case "postgresql":
                                case "sqlce":
                                case "sqlite":
                                case "sqlserver":
                                    input = new AdoReader(
                                        inputContext,
                                        primaryKey,
                                        ctx.ResolveNamed<IConnectionFactory>(inputContext.Connection.Key),
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
                                case "mysql":
                                case "postgresql":
                                case "sqlite":
                                case "sqlserver":
                                    var ocf = ctx.ResolveNamed<IConnectionFactory>(outputConnection.Key);
                                    output = new AdoReader(context, entity.GetPrimaryKey(), ocf, rowFactory, ReadFrom.Output);
                                    deleter = new AdoDeleter(outputContext, ocf);
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