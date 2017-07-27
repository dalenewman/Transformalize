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
using System.Linq;
using Autofac;
using Transformalize.Configuration;
using Transformalize.Context;
using Transformalize.Contracts;
using Transformalize.Nulls;
using Transformalize.Provider.RethinkDB;
using RethinkDb;
using RethinkDb.ConnectionFactories;
using System;
using System.Net;
using System.Collections.Generic;

namespace Transformalize.Ioc.Autofac {

    public class RethinkDBModule : Module {

        private readonly Process _process;
        private const string PROVIDER = "rethinkdb";

        public RethinkDBModule() { }

        public RethinkDBModule(Process process) {
            _process = process;
        }

        protected override void Load(ContainerBuilder builder) {
            if (_process == null)
                return;

            // connections
            foreach (var connection in _process.Connections.Where(c => c.Provider == PROVIDER)) {
                builder.Register<ISchemaReader>(ctx => {
                    return new NullSchemaReader();
                }).Named<ISchemaReader>(connection.Key);

                builder.Register<IConnectionFactory>(ctx => {
                    if (connection.Servers.Any()) {
                        var endPoints = new List<EndPoint>();
                        foreach (var server in connection.Servers) {
                            endPoints.Add(GetEndPoint(server.Name, server.Port));
                        }
                        return new ConnectionPoolingConnectionFactory(new ReliableConnectionFactory(new DefaultConnectionFactory(endPoints)), TimeSpan.FromSeconds(connection.RequestTimeout));
                    } else {
                        return new ReliableConnectionFactory(new DefaultConnectionFactory(new List<EndPoint> { GetEndPoint(connection.Server, connection.Port) }));
                    }
                }).Named<IConnectionFactory>(connection.Key).SingleInstance();
            }

            // Entity input
            foreach (var entity in _process.Entities.Where(e => _process.Connections.First(c => c.Name == e.Connection).Provider == PROVIDER)) {

                // input version detector
                builder.RegisterType<NullInputProvider>().Named<IInputProvider>(entity.Key);

                // input reader
                builder.Register<IRead>(ctx => {
                    return new NullReader(ctx.ResolveNamed<InputContext>(entity.Key), false);
                }).Named<IRead>(entity.Key);
            }

            if (_process.Output().Provider == PROVIDER) {
                // PROCESS OUTPUT CONTROLLER
                builder.Register<IOutputController>(ctx => new NullOutputController()).As<IOutputController>();

                foreach (var entity in _process.Entities) {
                    builder.Register<IOutputController>(ctx => {
                        var input = ctx.ResolveNamed<InputContext>(entity.Key);
                        var output = ctx.ResolveNamed<OutputContext>(entity.Key);
                        var factory = ctx.ResolveNamed<IConnectionFactory>(output.Connection.Key);
                        var initializer = _process.Mode == "init" ? (IAction)new RethinkDbInitializer(input, output, factory) : new NullInitializer();
                        return new RethinkDbOutputController(
                            output,
                            initializer,
                            ctx.ResolveNamed<IInputProvider>(entity.Key),
                            _process.Mode == "init" ? (IOutputProvider) new NullOutputProvider() : new RethinkDbOutputProvider(input, output, factory),
                            factory
                        );
                    }
                    ).Named<IOutputController>(entity.Key);

                    // ENTITY WRITER
                    builder.Register<IWrite>(ctx => {
                        var output = ctx.ResolveNamed<OutputContext>(entity.Key);
                        return new RethinkDbWriter(
                            ctx.ResolveNamed<InputContext>(entity.Key), 
                            output,
                            ctx.ResolveNamed<IConnectionFactory>(output.Connection.Key)
                        );
                    }).Named<IWrite>(entity.Key);
                }
            }

        }

        private EndPoint GetEndPoint(string nameOrAddress, int port) {
            IPAddress ip;
            if (IPAddress.TryParse(nameOrAddress, out ip))
                return new IPEndPoint(ip, port);
            else
                return new DnsEndPoint(nameOrAddress, port);
        }
    }
}