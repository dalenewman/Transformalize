using System.Collections.Generic;
using System.Linq;
using Transformalize.Configuration;

namespace Transformalize.Main {

    public static class ProcessFactory {

        public static Process[] Create(string resource, Options options = null) {
            var element = new ConfigurationFactory(resource).Create();
            return Create(element, options);
        }

        public static Process[] Create(ProcessConfigurationElement element, Options options = null) {
            return Create(new ProcessElementCollection() { element }, options);
        }

        private static Process[] Create(ProcessElementCollection elements, Options options = null) {

            var host = System.Net.Dns.GetHostName();
            var ip = string.Join(", ", System.Net.Dns.GetHostEntry(host).AddressList.Select(a => a.ToString()));
            TflLogger.Info(string.Empty, string.Empty, "Host: {0}", host);
            TflLogger.Info(string.Empty, string.Empty, "IP(s): {0}", ip);

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

        public static Process CreateSingle(ProcessConfigurationElement process, Options options = null) {
            return Create(process, options)[0];
        }
    }
}
