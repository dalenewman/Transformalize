using System;
using System.IO;
using System.Linq;
using Autofac;
using Pipeline.Configuration;
using Pipeline.Context;
using Pipeline.Contracts;
using Pipeline.Desktop;
using Pipeline.Nulls;
using Pipeline.Provider.Excel;

namespace Pipeline.Ioc.Autofac.Modules {
    public class ExcelModule : Module {
        private readonly Process _process;

        public ExcelModule() { }

        public ExcelModule(Process process) {
            _process = process;
        }

        protected override void Load(ContainerBuilder builder) {
            if (_process == null)
                return;

            // connections
            foreach (var connection in _process.Connections.Where(c => c.Provider == "excel")) {
                builder.Register<ISchemaReader>(ctx => {
                    /* file and excel are different, have to load the content and check it to determine schema */
                    var fileInfo = new FileInfo(Path.IsPathRooted(connection.File) ? connection.File : Path.Combine(AppDomain.CurrentDomain.BaseDirectory, connection.File));
                    var context = ctx.ResolveNamed<IConnectionContext>(connection.Key);
                    var cfg = new ExcelInspection(context, fileInfo, 100).Create();
                    var process = ctx.Resolve<Process>();
                    process.Load(cfg);

                    foreach (var warning in process.Warnings()) {
                        context.Warn(warning);
                    }

                    if (process.Errors().Any()) {
                        foreach (var error in process.Errors()) {
                            context.Error(error);
                        }
                        return new NullSchemaReader();
                    }

                    return new SchemaReader(context, new RunTimeRunner(context), process);

                }).Named<ISchemaReader>(connection.Key);
            }

            // Entity input
            foreach (var entity in _process.Entities.Where(e => _process.Connections.First(c => c.Name == e.Connection).Provider == "excel")) {

                // input version detector
                builder.RegisterType<NullVersionDetector>().Named<IInputVersionDetector>(entity.Key);

                // input reader
                builder.Register<IRead>(ctx => {
                    var input = ctx.ResolveNamed<InputContext>(entity.Key);
                    var rowFactory = ctx.ResolveNamed<IRowFactory>(entity.Key, new NamedParameter("capacity", input.RowCapacity));
                    switch (input.Connection.Provider) {
                        case "excel":
                            return new ExcelReader(input, rowFactory);
                        default:
                            return new NullReader(input, false);
                    }
                }).Named<IRead>(entity.Key);
            }

            if (_process.Output().Provider == "excel") {
                // PROCESS OUTPUT CONTROLLER
                builder.Register<IOutputController>(ctx => new NullOutputController()).Named<IOutputController>(_process.Key);

                foreach (var entity in _process.Entities) {
                    // todo
                }
            }

        }
    }
}