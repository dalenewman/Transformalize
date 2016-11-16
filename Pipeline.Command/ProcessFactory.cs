using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autofac;
using Pipeline.Configuration;
using Pipeline.Context;
using Pipeline.Contracts;
using Pipeline.Desktop.Transforms;
using Pipeline.Ioc.Autofac.Modules;
using Pipeline.Logging.NLog;

namespace Pipeline.Command {
    public static class ProcessFactory {
        public static Process Create(string cfg, string shorthand) {
            var builder = new ContainerBuilder();
            builder.RegisterModule(new RootModule(shorthand));
            builder.Register<IPipelineLogger>(c => new NLogPipelineLogger(SlugifyTransform.Slugify(cfg))).As<IPipelineLogger>().SingleInstance();
            builder.Register<IContext>(c => new PipelineContext(c.Resolve<IPipelineLogger>())).As<IContext>();

            using (var scope = builder.Build().BeginLifetimeScope()) {
                return scope.Resolve<Process>(new NamedParameter("cfg", cfg));
            }
        }
    }
}
