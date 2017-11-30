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
using System.Web;
using Autofac;
using Transformalize.Configuration;
using Transformalize.Context;
using Transformalize.Contracts;
using Transformalize.Nulls;
using Transformalize.Providers.Ado;
using Transformalize.Providers.MySql;
using Transformalize.Providers.SqlServer;
using Transformalize;
using Transformalize.Impl;

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
            // -- IBatchReader (for matching)
            // -- IWriteMasterUpdateQuery (for updating)
            // IUpdate
            // IWrite
            // IEntityDeleteHandler

            // entitiy input
            foreach (var entity in _process.Entities.Where(e => _ado.Contains(_process.Connections.First(c => c.Name == e.Connection).Provider))) {

                // INPUT READER
                builder.Register(ctx => {

                    var input = ctx.ResolveNamed<InputContext>(entity.Key);
                    var rowFactory = ctx.ResolveNamed<IRowFactory>(entity.Key, new NamedParameter("capacity", input.RowCapacity));
                    IRead dataReader;
                    switch (input.Connection.Provider) {
                        case "mysql":
                        case "sqlserver":
                            dataReader = new AdoInputReader(
                                input,
                                input.InputFields,
                                ctx.ResolveNamed<IConnectionFactory>(input.Connection.Key),
                                rowFactory
                            );
                            break;
                        default:
                            dataReader = new NullReader(input, false);
                            break;
                    }

                    // if key filter exists
                    if (entity.GetPrimaryKey().Any() && entity.GetPrimaryKey().All(f => entity.Filter.Any(i => i.Field == f.Alias || i.Field == f.Name))) {
                        if (entity.GetPrimaryKey().All(pk => entity.Filter.First(i => i.Field == pk.Alias || i.Field == pk.Name).Value == (pk.Default == Constants.DefaultSetting ? Constants.StringDefaults()[pk.Type] : pk.Default))) {
                            // this is a form insert, create a new default row and apply parameters
                            return new ParameterRowReader(input, new DefaultRowReader(input, rowFactory));
                        }

                        // this is a form update, read the row and apply parameters if it's post
                        if (HttpContext.Current.Request.HttpMethod == "POST") {
                            return new ParameterRowReader(input, dataReader);
                        }
                    }
                     
                    return dataReader;
                }).Named<IRead>(entity.Key);

                // INPUT VERSION DETECTOR
                builder.Register<IInputProvider>(ctx => {
                    var input = ctx.ResolveNamed<InputContext>(entity.Key);
                    switch (input.Connection.Provider) {
                        case "mysql":
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
                                    ctx.ResolveNamed<IBatchReader>(entity.Key),
                                    new AdoEntityUpdater(output, cf)
                                );
                            case "mysql":
                                return new AdoEntityWriter(
                                    output,
                                    ctx.ResolveNamed<IBatchReader>(entity.Key),
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
                    builder.Register<IBatchReader>(ctx => {
                        if (!entity.Update)
                            return new NullBatchReader();
                        var output = ctx.ResolveNamed<OutputContext>(entity.Key);
                        var rowFactory = ctx.ResolveNamed<IRowFactory>(entity.Key, new NamedParameter("capacity", output.GetAllEntityFields().Count()));
                        var cf = ctx.ResolveNamed<IConnectionFactory>(output.Connection.Key);
                        return new AdoEntityMatchingKeysReader(output, cf, rowFactory);
                    }).Named<IBatchReader>(entity.Key);

                    // MASTER UPDATE QUERY
                    builder.Register<IWriteMasterUpdateQuery>(ctx => {
                        var output = ctx.ResolveNamed<OutputContext>(entity.Key);
                        var factory = ctx.ResolveNamed<IConnectionFactory>(output.Connection.Key);
                        switch (output.Connection.Provider) {
                            case "mysql":
                                return new MySqlUpdateMasterKeysQueryWriter(output, factory);
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
                        builder.Register<IEntityDeleteHandler>(ctx => new NullDeleteHandler()).Named<IEntityDeleteHandler>(entity.Key);
                    }

                }
            }
        }

    }
}