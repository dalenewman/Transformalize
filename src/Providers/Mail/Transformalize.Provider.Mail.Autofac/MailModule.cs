#region license
// Transformalize
// Configurable Extract, Transform, and Load
// Copyright 2013-2024 Dale Newman
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
using Transformalize.Nulls;

namespace Transformalize.Providers.Mail.Autofac {
   public class MailModule : Module {

      public const string ProviderName = "mail";
      private readonly Process _process;

      public MailModule() {
         _process = null;
      }

      public MailModule(Process process) {
         _process = process;
      }

      protected override void Load(ContainerBuilder builder) {

         if (_process == null && !builder.Properties.ContainsKey("Process")) {
            return;
         }

         var process = _process ?? (Process)builder.Properties["Process"];

         // connections
         foreach (var connection in process.Connections.Where(c => c.Provider == ProviderName)) {
            builder.Register<ISchemaReader>(ctx => new NullSchemaReader()).Named<ISchemaReader>(connection.Key);
         }

         // Entity input
         foreach (var entity in process.Entities.Where(e => process.Connections.First(c => c.Name == e.Input).Provider == ProviderName)) {

            // input version detector
            builder.RegisterType<NullInputProvider>().Named<IInputProvider>(entity.Key);

            // input reader
            builder.Register<IRead>(ctx => {
               var input = ctx.ResolveNamed<InputContext>(entity.Key);
               return new NullReader(input);
            }).Named<IRead>(entity.Key);
         }

         if (process.GetOutputConnection().Provider == ProviderName) {
            // PROCESS OUTPUT CONTROLLER
            builder.Register<IOutputController>(ctx => new NullOutputController()).As<IOutputController>();

            foreach (var entity in process.Entities) {
               builder.Register<IOutputController>(ctx => new NullOutputController()).Named<IOutputController>(entity.Key);

               // ENTITY WRITER
               builder.Register<IWrite>(ctx => new MailWriter(ctx.ResolveNamed<OutputContext>(entity.Key))).Named<IWrite>(entity.Key);
            }
         }

      }
   }
}