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
using System;
using System.Collections.Generic;
using System.Linq;
using Transformalize.Configuration;
using Transformalize.Containers.Autofac;
using Transformalize.Context;
using Transformalize.Contracts;
using Transformalize.Impl;
using Transformalize.Nulls;
using Transformalize.Providers.Ado;
using Transformalize.Transforms.System;

namespace Transformalize.Providers.SqlServer.Autofac {
   public class SqlServerModule : Module {
      private readonly Process _process;

      private const string SqlServer = "sqlserver";

      /// <summary>
      /// Initializes a new instance of the <see cref="SqlServerModule"/> class.
      /// </summary>
      public SqlServerModule() { }

      /// <summary>
      /// Initializes a new instance of the <see cref="SqlServerModule"/> class with a specific process.
      /// </summary>
      /// <param name="process">The process being executed.</param>
      public SqlServerModule(Process process) {
         _process = process;
      }

      /// <summary>
      /// Gets or sets an optional factory function for creating connection factories.
      /// </summary>
      public Func<Connection,IConnectionFactory> ConnectionFactory { get; set; }

      /// <summary>
      /// Configures the Autofac container with SQL Server-specific components (readers, writers, controllers).
      /// </summary>
      /// <param name="builder">The container builder.</param>
      protected override void Load(ContainerBuilder builder) {

         if (_process == null && !builder.Properties.ContainsKey("Process")) {
            return;
         }

         var process = _process ?? (Process)builder.Properties["Process"];

         // connections
         foreach (var connection in process.Connections.Where(c => c.Provider == SqlServer)) {

            // Connection Factory: Manages creating and opening SQL connections.
            if(ConnectionFactory == null) {
               builder.Register<IConnectionFactory>(ctx => new SqlServerConnectionFactory(connection)).Named<IConnectionFactory>(connection.Key).InstancePerLifetimeScope();
            } else {
               builder.Register(ctx => ConnectionFactory(connection)).Named<IConnectionFactory>(connection.Key).InstancePerLifetimeScope();
            }            

            // Schema Reader: Inspects SQL Server schema to discover table structures.
            builder.Register<ISchemaReader>(ctx => {
               var factory = ctx.ResolveNamed<IConnectionFactory>(connection.Key);
               return new AdoSchemaReader(ctx.ResolveNamed<IConnectionContext>(connection.Key), factory);
            }).Named<ISchemaReader>(connection.Key);

         }

         // entitiy input
         foreach (var entity in process.Entities.Where(e => process.Connections.First(c => c.Name == e.Input).Provider == SqlServer)) {

            // INPUT READER: Reads raw data from SQL Server using ADO.NET.
            builder.Register<IRead>(ctx => {
               var input = ctx.ResolveNamed<InputContext>(entity.Key);
               var rowFactory = ctx.ResolveNamed<IRowFactory>(entity.Key, new NamedParameter("capacity", input.RowCapacity));
               return new AdoInputReader(
                   input,
                   input.InputFields,
                   ctx.ResolveNamed<IConnectionFactory>(input.Connection.Key),
                   rowFactory
               );
            }).Named<IRead>(entity.Key);

            // INPUT VERSION DETECTOR: Detects the max version of data in the input to support incremental loads.
            builder.Register<IInputProvider>(ctx => {
               var input = ctx.ResolveNamed<InputContext>(entity.Key);
               return new AdoInputProvider(input, ctx.ResolveNamed<IConnectionFactory>(input.Connection.Key));
            }).Named<IInputProvider>(entity.Key);

         }

         // entity output
         if (process.GetOutputConnection().Provider == SqlServer) {

            var calc = process.ToCalculatedFieldsProcess();

            // PROCESS OUTPUT CONTROLLER: Manages high-level output actions like creating star schema views or flattened tables.
            builder.Register<IOutputController>(ctx => {
               var output = ctx.Resolve<OutputContext>();
               if (process.Mode != "init")
                  return new NullOutputController();

               var actions = new List<IAction> { new AdoStarViewCreator(output, ctx.ResolveNamed<IConnectionFactory>(output.Connection.Key)) };
               if (process.Flatten) {
                  actions.Add(new AdoFlatTableCreator(output, ctx.ResolveNamed<IConnectionFactory>(output.Connection.Key)));
               }
               return new AdoStarController(output, actions);
            }).As<IOutputController>();

            // PROCESS CALCULATED READER: Reads data from the output table specifically for resolving process-level calculated fields.
            builder.Register<IRead>(ctx => {
               var calcContext = new PipelineContext(ctx.Resolve<IPipelineLogger>(), calc, calc.Entities.First());
               var outputContext = new OutputContext(calcContext);
               var cf = ctx.ResolveNamed<IConnectionFactory>(outputContext.Connection.Key);
               var capacity = outputContext.Entity.Fields.Count + outputContext.Entity.CalculatedFields.Count;
               var rowFactory = new RowFactory(capacity, false, false);
               return new AdoStarParametersReader(outputContext, process, cf, rowFactory);
            }).As<IRead>();

            // PROCESS CALCULATED FIELD WRITER: Updates the output table with values for process-level calculated fields.
            builder.Register<IWrite>(ctx => {
               var calcContext = new PipelineContext(ctx.Resolve<IPipelineLogger>(), calc, calc.Entities.First());
               var outputContext = new OutputContext(calcContext);
               var cf = ctx.ResolveNamed<IConnectionFactory>(outputContext.Connection.Key);
               return new AdoCalculatedFieldUpdater(outputContext, process, cf);
            }).As<IWrite>();

            // PROCESS INITIALIZER: Handles initialization of the output database/schema (e.g., creating the output tables).
            builder.Register<IInitializer>(ctx => {
               var output = ctx.Resolve<OutputContext>();
               return new AdoInitializer(output, ctx.ResolveNamed<IConnectionFactory>(output.Connection.Key));
            }).As<IInitializer>();

            // ENTITIES
            foreach (var entity in process.Entities) {

               // OUTPUT PROVIDER: Orchestrates writing data to the output, including change detection (matching keys).
               builder.Register<IOutputProvider>(ctx => {
                  var output = ctx.ResolveNamed<OutputContext>(entity.Key);
                  var cf = ctx.ResolveNamed<IConnectionFactory>(output.Connection.Key);
                  var rowFactory = ctx.ResolveNamed<IRowFactory>(entity.Key, new NamedParameter("capacity", output.GetAllEntityFields().Count()));

                  // matcher determines what's an update vs. and insert by reading existing keys/hashes from the output.
                  var matcher = entity.Update ? (IBatchReader)new AdoEntityMatchingKeysReader(output, cf, rowFactory) : new NullBatchReader();

                  var writer = new SqlServerWriter(
                      output,
                      cf,
                      matcher,
                      new AdoEntityUpdater(output, cf)
                  );

                  return new AdoOutputProvider(output, cf, writer);
               }).Named<IOutputProvider>(entity.Key);

               // ENTITY OUTPUT CONTROLLER: Handles per-entity output initialization and management.
               builder.Register<IOutputController>(ctx => {

                  var output = ctx.ResolveNamed<OutputContext>(entity.Key);
                  var initializer = process.Mode == "init" ? (IAction)new AdoEntityInitializer(output, ctx.ResolveNamed<IConnectionFactory>(output.Connection.Key)) : new NullInitializer();

                  return new AdoOutputController(
                      output,
                      initializer,
                      ctx.ResolveNamed<IInputProvider>(entity.Key),
                      ctx.ResolveNamed<IOutputProvider>(entity.Key),
                      ctx.ResolveNamed<IConnectionFactory>(output.Connection.Key)
                  );

               }).Named<IOutputController>(entity.Key);

               // MASTER UPDATE QUERY: Generates the SQL for updating the master output table when an entity changes.
               builder.Register<IWriteMasterUpdateQuery>(ctx => {
                  var output = ctx.ResolveNamed<OutputContext>(entity.Key);
                  var factory = ctx.ResolveNamed<IConnectionFactory>(output.Connection.Key);
                  return new AdoUpdateMasterKeysQueryWriter(output, factory);
               }).Named<IWriteMasterUpdateQuery>(entity.Key + "MasterKeys");

               // MASTER UPDATER: Performs the update of the master output table.
               builder.Register<IUpdate>(ctx => {
                  var output = ctx.ResolveNamed<OutputContext>(entity.Key);
                  return new AdoMasterUpdater(
                      output,
                      ctx.ResolveNamed<IConnectionFactory>(output.Connection.Key),
                      ctx.ResolveNamed<IWriteMasterUpdateQuery>(entity.Key + "MasterKeys")
                  );
               }).Named<IUpdate>(entity.Key);

               // DELETE HANDLER: Identifies and removes records from the output that are no longer present in the source.
               if (entity.Delete) {
                  // ... rest of delete handling logic ...

                  if (AdoCrossDatabaseEntityDeleteHandler.IsApplicable(process, entity)){

                     builder.Register<IEntityDeleteHandler>(ctx => {
                        var outputContext = ctx.ResolveNamed<OutputContext>(entity.Key);
                        var connectionFactory = ctx.ResolveNamed<IConnectionFactory>(process.GetOutputConnection().Key);
                        return new AdoCrossDatabaseEntityDeleteHandler(outputContext, connectionFactory);
                     }).Named<IEntityDeleteHandler>(entity.Key);

                  } else {

                     // register input keys and hashcode reader if necessary
                     var sqlInput = process.Connections.FirstOrDefault(c => c.Provider == SqlServer && c.Name == entity.Input);

                     if (sqlInput != null) {
                        builder.Register(ctx => {
                           var inputContext = ctx.ResolveNamed<InputContext>(entity.Key);
                           var rowCapacity = inputContext.Entity.GetPrimaryKey().Count();
                           var rowFactory = new RowFactory(rowCapacity, false, true);
                           return new AdoReader(
                               inputContext,
                               entity.GetPrimaryKey(),
                               ctx.ResolveNamed<IConnectionFactory>(inputContext.Connection.Key),
                               rowFactory,
                               ReadFrom.Input
                           );
                        }).Named<IReadInputKeysAndHashCodes>(entity.Key);
                     }

                     // register output keys and hash code reader if necessary
                     builder.Register(ctx => {
                        var context = ctx.ResolveNamed<OutputContext>(entity.Key);
                        var rowCapacity = context.Entity.GetPrimaryKey().Length;
                        var rowFactory = new RowFactory(rowCapacity, false, true);

                        var outputConnection = process.GetOutputConnection();
                        var ocf = ctx.ResolveNamed<IConnectionFactory>(outputConnection.Key);
                        return new AdoReader(context, entity.GetPrimaryKey(), ocf, rowFactory, ReadFrom.Output);

                     }).Named<IReadOutputKeysAndHashCodes>(entity.Key);

                     builder.Register((ctx) => {
                        var outputConnection = process.GetOutputConnection();
                        var outputContext = ctx.ResolveNamed<OutputContext>(entity.Key);
                        var ocf = ctx.ResolveNamed<IConnectionFactory>(outputConnection.Key);
                        return new AdoDeleter(outputContext, ocf);
                     }).Named<IDelete>(entity.Key);

                     builder.Register<IEntityDeleteHandler>(ctx => {
                        var context = ctx.ResolveNamed<IContext>(entity.Key);
                        var primaryKey = entity.GetPrimaryKey();

                        var handler = new DefaultDeleteHandler(
                            context,
                            ctx.ResolveNamed<IReadInputKeysAndHashCodes>(entity.Key),
                            ctx.ResolveNamed<IReadOutputKeysAndHashCodes>(entity.Key),
                            ctx.ResolveNamed<IDelete>(entity.Key)
                        );

                        // since the primary keys from the input may have been transformed into the output, you have to transform before comparing
                        // feels a lot like entity pipeline on just the primary keys... may look at consolidating
                        handler.Register(new DefaultTransform(context, entity.GetPrimaryKey().ToArray()));
                        handler.Register(TransformFactory.GetTransforms(ctx, context, primaryKey));
                        handler.Register(new StringTruncateTransform(context, primaryKey));

                        return handler;
                     }).Named<IEntityDeleteHandler>(entity.Key);

                  }

               }
            }
         }
      }
   }
}