#region license
// Transformalize
// Configurable Extract, Transform, and Load
// Copyright 2013-2016 Dale Newman
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
using Transformalize.Contracts;
using Transformalize.Ioc.Autofac.Modules;

namespace Transformalize.Ioc.Autofac {
    public class RunTimeSchemaReader : IRunTimeSchemaReader {

        public Process Process { get; set; }
        public Schema Read(Process process) {
            Process = process;
            return Read();
        }

        private readonly IContext _host;

        public RunTimeSchemaReader(IContext host) {
            _host = host;
        }

        public RunTimeSchemaReader(Process process, IContext host) {
            Process = process;
            _host = host;
        }

        public Schema Read(Entity entity) {
            if (Process == null) {
                _host.Error("RunTimeSchemaReader executed without a Process");
                return new Schema();
            }

            var container = new ContainerBuilder();
            container.RegisterInstance(_host.Logger).SingleInstance();
            container.RegisterCallback(new ContextModule(Process).Configure);

            container.RegisterCallback(new AdoModule(Process).Configure);
            container.RegisterCallback(new LuceneModule(Process).Configure);
            container.RegisterCallback(new SolrModule(Process).Configure);
            container.RegisterCallback(new InternalModule(Process).Configure);
            container.RegisterCallback(new ElasticModule(Process).Configure);
            container.RegisterCallback(new FileModule(Process).Configure);
            container.RegisterCallback(new WebModule(Process).Configure);
            container.RegisterCallback(new ExcelModule(Process).Configure);

            using (var scope = container.Build().BeginLifetimeScope()) {
                var reader = scope.ResolveNamed<ISchemaReader>(Process.Connections.First().Key);
                return reader.Read(entity);
            }
        }

        public Schema Read() {

            if (Process == null) {
                _host.Error("RunTimeSchemaReader executed without a Process");
                return new Schema();
            }

            var container = new ContainerBuilder();
            container.RegisterInstance(_host.Logger).SingleInstance();
            container.RegisterCallback(new ContextModule(Process).Configure);

            container.RegisterCallback(new AdoModule(Process).Configure);
            container.RegisterCallback(new LuceneModule(Process).Configure);
            container.RegisterCallback(new SolrModule(Process).Configure);
            container.RegisterCallback(new ElasticModule(Process).Configure);
            container.RegisterCallback(new FileModule(Process).Configure);
            container.RegisterCallback(new ExcelModule(Process).Configure);

            using (var scope = container.Build().BeginLifetimeScope()) {
                var reader = scope.ResolveNamed<ISchemaReader>(Process.Connections.First().Key);
                return Process.Entities.Count == 1 ? reader.Read(Process.Entities.First()) : reader.Read();
            }

        }
    }
}