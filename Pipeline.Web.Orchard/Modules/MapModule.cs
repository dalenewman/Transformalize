#region license
// Transformalize
// A Configurable ETL Solution Specializing in Incremental Denormalization.
// Copyright 2013 Dale Newman
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

using System.Linq;
using Autofac;
using Transformalize.Configuration;
using Transformalize.Context;
using Transformalize.Contracts;
using Transformalize.Provider.Ado;

namespace Pipeline.Web.Orchard.Modules {
    public class MapModule : Module {
        private readonly Process _process;

        public MapModule() { }

        public MapModule(Process process) {
            _process = process;
        }

        protected override void Load(ContainerBuilder builder) {
            if (_process == null)
                return;

            foreach (var m in _process.Maps) {
                var map = m;
                builder.Register<IMapReader>(ctx => {
                    if (map.Query == string.Empty) {
                        return new DefaultMapReader();
                    }
                    var connection = _process.Connections.FirstOrDefault(cn => cn.Name == map.Connection);
                    var provider = connection == null ? string.Empty : connection.Provider;
                    switch (provider) {
                        case "solr":
                        case "lucene":
                        case "elasticsearch":
                            return new DefaultMapReader();
                        case "mysql":
                        case "postgresql":
                        case "sqlite":
                        case "sqlserver":
                            if (connection != null)
                                return new AdoMapReader(ctx.ResolveNamed<IConnectionFactory>(connection.Key), map.Name);
                            return new DefaultMapReader();
                        default:
                            return new DefaultMapReader();
                    }
                }).Named<IMapReader>(map.Name);
            }

        }
    }
}