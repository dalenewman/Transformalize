using System.Collections.ObjectModel;
using Transformalize.Configuration;

namespace Transformalize.Main {

    public static class ProcessFactory {

        public static Process Create(string resource, Options options = null) {
            var element = new ConfigurationFactory(resource).Create();
            return Create(element, options);
        }

        public static Process Create(ProcessConfigurationElement element, Options options = null) {
            return new ProcessReader(Collect(element), options ?? new Options()).Read();
        }

        private static ProcessConfigurationElement Collect(ProcessConfigurationElement child) {
            while (!string.IsNullOrEmpty(child.Inherit)) {
                var parent = new ConfigurationFactory(child.Inherit).Create();
                parent.Merge(child);
                child = parent;
            }
            return child;
        }

    }
}
