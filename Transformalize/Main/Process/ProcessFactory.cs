using System;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using System.Linq;
using Transformalize.Configuration;
using Transformalize.Libs.EnterpriseLibrary.SemanticLogging;
using Transformalize.Libs.EnterpriseLibrary.SemanticLogging.Formatters;
using Transformalize.Libs.EnterpriseLibrary.SemanticLogging.Sinks;
using Transformalize.Logging;

namespace Transformalize.Main {

    public static class ProcessFactory {

        /// <summary>
        /// Create process(es) from a resource.
        /// </summary>
        /// <param name="resource">resource may be a file name, a web address, or an XML configuration.</param>
        /// <param name="options"></param>
        /// <returns>A collection of processes that are ready to execute.</returns>
        public static Process[] Create(string resource, Options options = null) {
            TflLogger.Info(string.Empty, string.Empty, "Process Requested from {0}", ConfigurationFactory.DetermineConfigurationSource(resource).ToString());
            return Create(new ConfigurationFactory(resource).Create(), options);
        }

        /// <summary>
        /// Create process(es) from a configuration element.
        /// </summary>
        /// <param name="element"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        public static Process[] Create(ProcessConfigurationElement element, Options options = null) {
            TflLogger.Debug(element.Name, string.Empty, "Process Requested from element.");
            return Create(new ProcessElementCollection() { element }, options);
        }

        private static Process[] Create(ProcessElementCollection elements, Options options = null) {

            var host = System.Net.Dns.GetHostName();
            var process = string.Empty;
            if (elements != null && elements.Count > 0) {
                process = string.Join(", ", elements.Cast<ProcessConfigurationElement>().Select(p => p.Name));
            }
            TflLogger.Info(process, string.Empty, "Host is {0} ({1})", host, string.Join(", ", System.Net.Dns.GetHostEntry(host).AddressList.Select(a => a)));

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
