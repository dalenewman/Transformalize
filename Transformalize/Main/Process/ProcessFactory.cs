using System.Collections.Generic;
using System.IO;
using Transformalize.Configuration;
using Transformalize.Libs.NLog;

namespace Transformalize.Main {

    public static class ProcessFactory {

        private static readonly Logger Log = LogManager.GetLogger("tfl");

        public static Process[] Create(string resource, Options options = null) {
            InitializeLogger("All");
            var element = new ConfigurationFactory(resource).Create();
            return Create(element, options);
        }

        public static Process[] Create(ProcessConfigurationElement element, Options options = null) {
            InitializeLogger(element.Name);
            return Create(new ProcessElementCollection() { element }, options);
        }

        public static Process[] Create(ProcessElementCollection elements, Options options = null) {

            var processes = new List<Process>();

            foreach (ProcessConfigurationElement element in elements) {
                InitializeLogger(element.Name);

                options = options ?? new Options();
                var collected = Collect(element);

                processes.Add(new ProcessReader(collected, options).Read());
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

        private static void InitializeLogger(string name) {
            GlobalDiagnosticsContext.Set("process", name);
            GlobalDiagnosticsContext.Set("entity", Common.LogLength("All"));
        }

        public static Process CreateSingle(ProcessConfigurationElement process, Options options = null) {
            return Create(process, options)[0];
        }
    }
}
