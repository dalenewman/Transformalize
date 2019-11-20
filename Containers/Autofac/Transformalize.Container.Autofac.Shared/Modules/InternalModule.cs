#region license
// Transformalize
// Configurable Extract, Transform, and Load
// Copyright 2013-2019 Dale Newman
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
using System.Linq;
using Transformalize.Configuration;
using Transformalize.Context;
using Transformalize.Contracts;
using Transformalize.Impl;
using Transformalize.Nulls;
using Transformalize.Providers.Internal;
using Module = Autofac.Module;

namespace Transformalize.Containers.Autofac.Modules {

   /// <inheritdoc />
   /// <summary>
   /// Registers all the built-in transforms
   /// </summary>
   public class InternalModule : Module {

      private const string Internal = "internal";
      private readonly Process _process;

      public InternalModule(Process process) {
         _process = process;
      }

      protected override void Load(ContainerBuilder builder) {

         if (_process == null)
            return;

         if(_process.Connections.All(c=>c.Provider != Internal)) {
            return;
         }

         // add null schema reader for each internal connection
         foreach (var connection in _process.Connections.Where(c => c.Provider == Internal)) {
            builder.RegisterType<NullSchemaReader>().Named<ISchemaReader>(connection.Key);
         }

         // PROCESS AND ENTITY OUTPUT
         // if output is internal, setup internal output controllers for the process and each entity
         if (_process.Output().Provider == "internal") {

            // PROCESS OUTPUT CONTROLLER
            builder.Register<IOutputController>(ctx => new NullOutputController()).As<IOutputController>();

            foreach (var entity in _process.Entities) {

               builder.Register<IOutputController>(ctx => new NullOutputController()).Named<IOutputController>(entity.Key);
               builder.Register<IOutputProvider>(ctx => new InternalOutputProvider(ctx.ResolveNamed<OutputContext>(entity.Key), ctx.ResolveNamed<IWrite>(entity.Key))).Named<IOutputProvider>(entity.Key);

               // WRITER
               builder.Register<IWrite>(ctx => new InternalWriter(ctx.ResolveNamed<OutputContext>(entity.Key))).Named<IWrite>(entity.Key);
            }
         }

         // ENTITY INPUT
         // setup internal input readers for each entity if necessary
         foreach (var entity in _process.Entities.Where(e => _process.Connections.First(c => c.Name == e.Connection).Provider == Internal)) {

            builder.RegisterType<NullInputProvider>().Named<IInputProvider>(entity.Key);

            // handling internal deletes
            if (entity.Delete) {
               builder.Register<IReadInputKeysAndHashCodes>(ctx => {
                  // note: i tried to just load keys but had a lot of troubles, this works (for now).
                  var inputContext = ctx.ResolveNamed<InputContext>(entity.Key);
                  var rowFactory = new RowFactory(inputContext.RowCapacity, entity.IsMaster, false);
                  return new InternalKeysReader(new InternalReader(inputContext, rowFactory));
               }).Named<IReadInputKeysAndHashCodes>(entity.Key);
            }

            // READER
            builder.Register<IRead>(ctx => {
               var input = ctx.ResolveNamed<InputContext>(entity.Key);
               var rowFactory = ctx.ResolveNamed<IRowFactory>(entity.Key, new NamedParameter("capacity", input.RowCapacity));

               return new InternalReader(input, rowFactory);
            }).Named<IRead>(entity.Key);

         }
      }
   }
}
