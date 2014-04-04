using System.Collections.Generic;
using System.IO;
using Transformalize.Configuration;
using Transformalize.Libs.Mono.CSharp;
using Transformalize.Libs.NLog;

namespace Transformalize.Main {

    public static class ProcessFactory {

        private static readonly Logger Log = LogManager.GetLogger(string.Empty);

        public static Process[] Create(string resource, Options options = null) {
            var element = new ConfigurationFactory(resource).Create();
            return Create(element, options);
        }

        public static Process[] Create(ProcessConfigurationElement element, Options options = null) {
            return Create(new ProcessElementCollection() { element }, options);
        }

        public static Process[] Create(ProcessElementCollection elements, Options options = null) {

            var processes = new List<Process>();

            foreach (ProcessConfigurationElement element in elements) {
                options = options ?? new Options();
                var collected = Collect(element);
                GlobalDiagnosticsContext.Set("process", collected.Name);
                GlobalDiagnosticsContext.Set("entity", Common.LogLength("All"));
                DetectConfigurationUpdate(options, collected);

                processes.Add(new ProcessReader(collected, options).Read());
            }

            return processes.ToArray();
        }

        private static void DetectConfigurationUpdate(Options options, ProcessConfigurationElement collected) {
            var contents = collected.Serialize();
            var configFile = new FileInfo(Path.Combine(Common.GetTemporaryFolder(collected.Name), "Configuration.xml"));

            options.ConfigurationUpdated = !configFile.Exists || File.ReadAllText(configFile.FullName).GetHashCode() != contents.GetHashCode();
            if (options.ConfigurationUpdated) {
                Log.Info("Detected configuration update.");
            }

            if (collected.Enabled) {
                File.WriteAllText(configFile.FullName, contents);
            }
        }

        private static ProcessConfigurationElement Collect(ProcessConfigurationElement child) {
            while (!string.IsNullOrEmpty(child.Inherit)) {
                var parent = new ConfigurationFactory(child.Inherit).Create()[0];
                parent.Merge(child);
                child = parent;
            }
            return child;
        }

    }
}
