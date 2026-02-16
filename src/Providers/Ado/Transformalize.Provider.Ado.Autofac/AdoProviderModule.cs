// Transformalize.Provider.Ado
// An Alternative .NET Configuration Handler
// Copyright 2015-2018 Dale Newman
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

using Autofac;
using Cfg.Net.Reader;
using System;
using System.Collections.Generic;
using System.Linq;
using Transformalize.Configuration;
using Transformalize.Context;
using Transformalize.Contracts;
using Transformalize.Providers.Ado.Actions;
using Transformalize.Providers.Ado.Ext;

namespace Transformalize.Providers.Ado.Autofac {
   public class AdoProviderModule : Module {

      protected override void Load(ContainerBuilder builder) {

         if (!builder.Properties.ContainsKey("Process")) {
            return;
         }

         var adoProviders = new HashSet<string>(new[] { "sqlserver", "mysql", "postgresql", "sqlite", "sqlce", "access" }, StringComparer.OrdinalIgnoreCase);

         var process = (Process)builder.Properties["Process"];

         // actions
         foreach (var action in process.Actions.Where(a => a.GetModes().Any(m => m == process.Mode || m == "*"))) {
            switch (action.Type) {
               case "form-commands":
                  builder.Register<IAction>(ctx => {
                     var connection = process.Connections.First(c => c.Name == action.Connection);
                     var context = new PipelineContext(ctx.Resolve<IPipelineLogger>(), process);
                     return new AdoEntityFormCommands(context, action, ctx.ResolveNamed<IConnectionFactory>(connection.Key));
                  }).Named<IAction>(action.Key);
                  break;
               case "run":
                  builder.Register<IAction>(ctx => {
                     var connection = process.Connections.First(c => c.Name == action.Connection);
                     var context = new PipelineContext(ctx.Resolve<IPipelineLogger>(), process);
                     return new AdoRunAction(context, action, ctx.ResolveNamed<IConnectionFactory>(connection.Key), new DefaultReader(new FileReader(), new WebReader()));
                  }).Named<IAction>(action.Key);
                  break;
            }
         }

         // maps
         foreach (var m in process.Maps.Where(m => !string.IsNullOrEmpty(m.Connection) && !string.IsNullOrEmpty(m.Query))) {
            var map = m;
            var connection = process.Connections.First(cn => cn.Name == map.Connection);
            if (adoProviders.Contains(connection.Provider)) {
               builder.Register<IMapReader>(ctx => new AdoMapReader(ctx.Resolve<IContext>(), ctx.ResolveNamed<IConnectionFactory>(connection.Key), map.Name)).Named<IMapReader>(map.Name);
            }
         }

         // flatten output action
         if (process.Flatten) {
            foreach (var connection in process.Connections.Where(c => adoProviders.Contains(c.Provider))) {
               builder.Register<IAction>(ctx => {
                  var o = ctx.ResolveNamed<OutputContext>(connection.Key);
                  return new AdoFlattenAction(o, ctx.ResolveNamed<IConnectionFactory>(connection.Key));
               }).Named<IAction>(connection.Key);
            }
         }


      }
   }
}