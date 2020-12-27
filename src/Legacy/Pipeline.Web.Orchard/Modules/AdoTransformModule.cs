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
using Transformalize.Context;
using Transformalize.Contracts;
using Transformalize.Providers.Ado;
using Transformalize.Transforms.Ado;

namespace Pipeline.Web.Orchard.Modules {

    public class AdoTransformModule : Module {
        private readonly Process _process;

        public AdoTransformModule() {
        }

        public AdoTransformModule(Process process) {
            _process = process;
        }

        protected override void Load(ContainerBuilder builder) {

            if (_process == null)
                return;

            var adoProviders = new HashSet<string>(new[] { "sqlserver", "mysql", "postgresql", "sqlite", "sqlce", "access" }, StringComparer.OrdinalIgnoreCase);
            var adoMethods = new HashSet<string>(new[] { "fromquery", "run" });

            // only register by transform key if provider is ado, this allows other providers to implement fromquery and run methods
            foreach (var entity in _process.Entities) {
                foreach (var field in entity.GetAllFields().Where(f => f.Transforms.Any())) {
                    foreach (var transform in field.Transforms.Where(t => adoMethods.Contains(t.Method))) {
                        if (transform.Connection == string.Empty) {
                            transform.Connection = field.Connection;
                        }

                        var connection = _process.Connections.FirstOrDefault(c => c.Name == transform.Connection);
                        if (connection != null && adoProviders.Contains(connection.Provider)) {
                            if (transform.Method == "fromquery") {
                                // ado from query
                                builder.Register<ITransform>(ctx => {
                                    var context = new PipelineContext(ctx.Resolve<IPipelineLogger>(), _process, entity, field, transform);
                                    return new AdoFromQueryTransform(context, ctx.ResolveNamed<IConnectionFactory>(connection.Key));
                                }).Named<ITransform>(transform.Key);
                            } else {
                                // ado run
                                builder.Register<ITransform>(ctx => {
                                    var context = new PipelineContext(ctx.Resolve<IPipelineLogger>(), _process, entity, field, transform);
                                    return new AdoRunTransform(context, ctx.ResolveNamed<IConnectionFactory>(connection.Key));
                                }).Named<ITransform>(transform.Key);
                            }
                        }
                    }
                }
            }
        }
    }
}
