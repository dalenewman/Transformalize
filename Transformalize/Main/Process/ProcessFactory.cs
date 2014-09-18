using System.Collections.Generic;
using System.IO;
using Transformalize.Configuration;
using Transformalize.Libs.NLog;
using Transformalize.Libs.NLog.Config;

namespace Transformalize.Main {

    public static class ProcessFactory {

        public static Process[] Create(string resource, Options options = null) {
            InitializeLogger("All", options);
            var element = new ConfigurationFactory(resource).Create();
            return Create(element, options);
        }

        public static Process[] Create(ProcessConfigurationElement element, Options options = null) {
            InitializeLogger(element.Name, options);
            return Create(new ProcessElementCollection() { element }, options);
        }

        private static Process[] Create(ProcessElementCollection elements, Options options = null) {

            var processes = new List<Process>();
            if (options == null) {
                options = new Options();
            }

            foreach (ProcessConfigurationElement element in elements) {
                var collected = Collect(element);
                processes.Add(new ProcessReader(collected, ref options).Read());
            }

            return processes.ToArray();
        }

        private static ProcessConfigurationElement Collect(ProcessConfigurationElement child) {
            while (!string.IsNullOrEmpty(child.Inherit)) {
                var parent = new ConfigurationFactory(child.Inherit).Create()[0];
                parent.Merge(child);
                child = parent;
            }
            return child;
        }

        private static void InitializeLogger(string name, Options options) {
            if (options != null && options.MemoryTarget != null) {
                SimpleConfigurator.ConfigureForTargetLogging(options.MemoryTarget, options.LogLevel);
            }
            GlobalDiagnosticsContext.Set("process", Common.LogLength(name));
            GlobalDiagnosticsContext.Set("entity", Common.LogLength("All"));
        }

        public static Process CreateSingle(ProcessConfigurationElement process, Options options = null) {
            return Create(process, options)[0];
        }
    }
}
