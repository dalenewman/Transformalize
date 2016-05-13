using System.Linq;
using Autofac;
using Pipeline.Configuration;
using Pipeline.Context;
using Pipeline.Contracts;
using Pipeline.Nulls;

namespace Pipeline.Ioc.Autofac.Modules {
    public class FolderModule : Module {
        private readonly Process _process;

        public FolderModule() { }

        public FolderModule(Process process) {
            _process = process;
        }

        protected override void Load(ContainerBuilder builder) {

            if (_process == null)
                return;

            // enitity input
            foreach (var entity in _process.Entities.Where(e => _process.Connections.First(c => c.Name == e.Connection).Provider == "folder")) {

                builder.Register<IRead>(ctx => {
                    var input = ctx.ResolveNamed<InputContext>(entity.Key);
                    var rowFactory = ctx.ResolveNamed<IRowFactory>(entity.Key, new NamedParameter("capacity", input.RowCapacity));

                    switch (input.Connection.Provider) {
                        case "folder":
                            return new FolderReader(input, rowFactory);
                        default:
                            return new NullReader(input, false);
                    }
                }).Named<IRead>(entity.Key);
            }

            if (_process.Output().Provider == "folder") {
                // PROCESS OUTPUT CONTROLLER
                builder.Register<IOutputController>(ctx => new NullOutputController()).Named<IOutputController>(_process.Key);

                foreach (var entity in _process.Entities) {
                    // todo
                }
            }
        }
    }
}