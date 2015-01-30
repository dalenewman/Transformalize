using System.Collections.Generic;
using System.Linq;
using Transformalize.Configuration;
using Transformalize.Logging;

namespace Transformalize.Main {

    public static class ProcessFactory {

        public static Process[] Create(string resource, Options options = null, Dictionary<string, string> parameters = null) {
            var source = ConfigurationFactory.DetermineConfigurationSource(resource);
            CombineParameters(source, resource, parameters);
            TflLogger.Info(string.Empty, string.Empty, "Process Requested from {0}", source.ToString());
            return Create(new ConfigurationFactory(resource, parameters).Create(), options);
        }

        public static Process CreateSingle(string resource, Options options = null, Dictionary<string, string> parameters = null) {
            var source = ConfigurationFactory.DetermineConfigurationSource(resource);
            CombineParameters(source, resource, parameters);
            TflLogger.Info(string.Empty, string.Empty, "Process Requested from {0}", source.ToString());
            return CreateSingle(new ConfigurationFactory(resource, parameters).Create()[0], options);
        }
        /// <summary>
        /// Create process(es) from a configuration element.
        /// </summary>
        /// <param name="element"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        public static Process[] Create(TflProcess element, Options options = null) {
            TflLogger.Debug(element.Name, string.Empty, "Process Requested from element.");
            return Create(new List<TflProcess>() { element }, options);
        }

        private static Process[] Create(List<TflProcess> elements, Options options = null) {

            var host = System.Net.Dns.GetHostName();
            var process = string.Empty;
            if (elements != null && elements.Count > 0) {
                process = string.Join(", ", elements.Select(p => p.Name));
            }
            var ip4 = System.Net.Dns.GetHostEntry(host).AddressList.Where(a => a.ToString().Length > 4 && a.ToString()[4] != ':').Select(a => a.ToString()).ToArray();
            TflLogger.Info(process, string.Empty, "Host is {0} {1}", host, string.Join(", ", ip4.Any() ? ip4 : new[] { string.Empty }));

            var processes = new List<Process>();
            if (options == null) {
                options = new Options();
            }

            foreach (var element in elements) {
                processes.Add(new ProcessReader(element, ref options).Read());
            }

            return processes.ToArray();
        }

        public static Process CreateSingle(TflProcess process, Options options = null) {
            return Create(process, options)[0];
        }

        private static void CombineParameters(ConfigurationSource source, string resource, Dictionary<string, string> parameters) {
            if (source == ConfigurationSource.Xml || resource.IndexOf('?') <= 0)
                return;

            if (parameters == null) {
                parameters = new Dictionary<string, string>();
            }
            foreach (var pair in Common.ParseQueryString(resource.Substring(resource.IndexOf('?')))) {
                parameters[pair.Key] = pair.Value;
            }
        }

    }
}
