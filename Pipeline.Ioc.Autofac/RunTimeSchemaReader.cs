using System.Linq;
using Autofac;
using Pipeline.Configuration;
using Pipeline.Contracts;
using Pipeline.Ioc.Autofac.Modules;

namespace Pipeline.Ioc.Autofac {
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