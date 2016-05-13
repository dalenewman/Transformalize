using System.Linq;
using Autofac;
using Pipeline.Configuration;
using Pipeline.Context;
using Pipeline.Contracts;
using Pipeline.Desktop;
using Pipeline.Desktop.Writers;
using Pipeline.Extensions;
using Pipeline.Nulls;

namespace Pipeline.Ioc.Autofac.Modules {
    public class InternalModule : Module {
        private readonly Process _process;
        private readonly string[] _internal = { "internal", "console", "trace" };

        public InternalModule() { }

        public InternalModule(Process process) {
            _process = process;
        }

        protected override void Load(ContainerBuilder builder) {

            if (_process == null)
                return;

            // Entity input
            foreach (var entity in _process.Entities.Where(e => _process.Connections.First(c => c.Name == e.Connection).Provider.In(_internal))) {

                // READER
                builder.Register<IRead>(ctx => {
                    var input = ctx.ResolveNamed<InputContext>(entity.Key);
                    var rowFactory = ctx.ResolveNamed<IRowFactory>(entity.Key, new NamedParameter("capacity", input.RowCapacity));

                    switch (input.Connection.Provider) {
                        case "internal":
                            return new DataSetEntityReader(input, rowFactory);
                        case "console":
                            // todo: take standard input
                            return new NullReader(input);
                        default:
                            return new NullReader(input, false);
                    }
                }).Named<IRead>(entity.Key);

            }

            // Entity Output
            if (_process.Output().Provider.In(_internal)) {

                // PROCESS OUTPUT CONTROLLER
                builder.Register<IOutputController>(ctx => new NullOutputController()).Named<IOutputController>(_process.Key);

                foreach (var entity in _process.Entities) {

                    builder.Register<IOutputController>(ctx => new NullOutputController()).Named<IOutputController>(entity.Key);

                    // WRITER
                    builder.Register<IWrite>(ctx => {
                        var output = ctx.ResolveNamed<OutputContext>(entity.Key);

                        switch (output.Connection.Provider) {
                            case "console":
                                return new ConsoleWriter(new JsonNetSerializer(output));
                            case "trace":
                                return new TraceWriter(new JsonNetSerializer(output));
                            case "internal":
                                return new InternalWriter(entity);
                            default:
                                return new NullWriter(output);
                        }
                    }).Named<IWrite>(entity.Key);
                }
            }
        }
    }
}