using System;
using System.Collections.Generic;
using Autofac;
using Humanizer;
using Humanizer.Bytes;
using Quartz;
using Transformalize.Configuration;
using Transformalize.Context;
using Transformalize.Contracts;
using Transformalize.Desktop.Transforms;
using Transformalize.Extensions;
using Transformalize.Ioc.Autofac.Modules;
using Transformalize.Logging.NLog;
using Environment = System.Environment;

namespace Transformalize.Command {
    [DisallowConcurrentExecution]
    public class BaseExecutor : IRunTimeExecute {

        public string Cfg { get; set; }
        public string Format { get; set; }
        public string Mode { get; set; }

        public BaseExecutor(string cfg, string mode, string format) {
            Cfg = cfg;
            Mode = mode;
            Format = format;
        }

        public void Execute(Process process) {

            // Since we're in a Console app, honor output format
            if (process.Output().Provider.In("internal", "console")) {
                process.Output().Provider = "console";
                process.Output().Format = Format;
            }

            var logger = new NLogPipelineLogger(SlugifyTransform.Slugify(Cfg));

            CheckMemory(process, logger);

            var builder = new ContainerBuilder();
            builder.RegisterInstance(logger).As<IPipelineLogger>().SingleInstance();
            builder.RegisterCallback(new RootModule(process.Shorthand).Configure);
            builder.RegisterCallback(new ContextModule(process).Configure);

            // providers
            builder.RegisterCallback(new AdoModule(process).Configure);
            builder.RegisterCallback(new LuceneModule(process).Configure);
            builder.RegisterCallback(new SolrModule(process).Configure);
            builder.RegisterCallback(new ElasticModule(process).Configure);
            builder.RegisterCallback(new InternalModule(process).Configure);
            builder.RegisterCallback(new FileModule(process).Configure);
            builder.RegisterCallback(new GeoJsonModule(process).Configure);
            // builder.RegisterCallback(new NumlModule(process).Configure);
            builder.RegisterCallback(new FolderModule(process).Configure);
            builder.RegisterCallback(new DirectoryModule(process).Configure);
            builder.RegisterCallback(new ExcelModule(process).Configure);
            builder.RegisterCallback(new WebModule(process).Configure);

            builder.RegisterCallback(new MapModule(process).Configure);
            builder.RegisterCallback(new TemplateModule(process).Configure);
            builder.RegisterCallback(new ActionModule(process).Configure);

            builder.RegisterCallback(new EntityPipelineModule(process).Configure);
            builder.RegisterCallback(new ProcessPipelineModule(process).Configure);
            builder.RegisterCallback(new ProcessControlModule(process).Configure);

            using (var scope = builder.Build().BeginLifetimeScope()) {
                try {
                    scope.Resolve<IProcessController>().Execute();
                } catch (Exception ex) {
                    scope.Resolve<IContext>().Error(ex.Message);
                }
            }
        }

        private static void CheckMemory(Process process, IPipelineLogger logger) {
            if (!string.IsNullOrEmpty(process.MaxMemory)) {
                var context = new PipelineContext(logger, process);

                GC.Collect();
                GC.WaitForPendingFinalizers();

                var currentBytes = System.Diagnostics.Process.GetCurrentProcess().WorkingSet64.Bytes();
                var maxMemory = ByteSize.Parse(process.MaxMemory);

                if (maxMemory.CompareTo(currentBytes) < 0) {
                    context.Error(
                        $"Process exceeded {maxMemory.Megabytes.ToString("#.0")} Mb. Current memory is {currentBytes.Megabytes.ToString("#.0")} Mb!");
                    Environment.Exit(1);
                } else {
                    context.Info(
                        $"The process is using {currentBytes.Megabytes.ToString("#.0")} Mb of it's max {maxMemory.Megabytes.ToString("#.0")} Mb allowed.");
                }
            }
        }

        public void Execute(string cfg, string shorthand, Dictionary<string, string> parameters) {
            Process process;
            if (ProcessFactory.TryCreate(cfg, shorthand, parameters, out process)) {
                process.Mode = Mode;
                Execute(process);
            }
        }

    }
}