#region License

// /*
// Transformalize - Replicate, Transform, and Denormalize Your Data...
// Copyright (C) 2013 Dale Newman
// 
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with this program.  If not, see <http://www.gnu.org/licenses/>.
// */

#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using Transformalize.Configuration;
using Transformalize.Extensions;
using Transformalize.Libs.NanoXml;
using Transformalize.Libs.NLog;
using Transformalize.Main;

namespace Transformalize.Runner {
    public class ProcessXmlConfigurationReader : IReader<ProcessElementCollection> {

        private readonly string _resource;
        private readonly ContentsReader _contentsReader;

        public ProcessXmlConfigurationReader(string resource, ContentsReader contentsReader) {
            _resource = resource;
            _contentsReader = contentsReader;
        }

        public ProcessElementCollection Read() {
            var contents = _contentsReader.Read(_resource);

            var section = new TransformalizeConfiguration();

            try {
                var doc = XDocument.Parse(contents.Content);
                var process = doc.Element("process");
                string xml;

                if (process == null) {
                    var transformalize = doc.Element("transformalize");
                    if (transformalize == null)
                        throw new TransformalizeException("Can't find the <process/> or <transformalize/> element in {0}.", _resource);
                    xml = transformalize.ToString();
                } else {
                    xml = string.Format(@"
                    <transformalize>
                        <processes>
                            <add name=""{0}"" enabled=""{1}"" inherit=""{2}"" time-zone=""{3}"" star=""{4}"" star-enabled=""{5}"" view=""{6}"">{7}</add>
                        </processes>
                    </transformalize>",
                        contents.Name,
                        SafeAttribute(process, "enabled", true),
                        SafeAttribute(process, "inherit", string.Empty),
                        SafeAttribute(process, "time-zone", string.Empty),
                        SafeAttribute(process, "star", Common.DefaultValue),
                        SafeAttribute(process, "star-enabled", true),
                        SafeAttribute(process, "view", Common.DefaultValue),
                        process.InnerXml()
                    );
                }

                var updated = DefaultParameters(xml);
                section.Deserialize(updated);
                return section.Processes;
            } catch (Exception e) {
                throw new TransformalizeException("Sorry.  I couldn't parse the file {0}.  Make sure it is valid XML and try again. {1}", contents.Name, e.Message);
            }
        }

        public static string DefaultParameters(string xml) {

            if (!xml.Contains("@"))
                return xml;

            var doc = new NanoXmlDocument(xml);

            var transformalize = doc.RootNode;
            var processes = new NanoXmlNode[0];
            var environments = new NanoXmlNode[0];
            var environmentDefault = string.Empty;

            foreach (var node in transformalize.SubNodes) {
                switch (node.Name) {
                    case "environments":
                        environmentDefault = node.Attributes.Any() ? node.Attributes.First().Value : string.Empty;
                        environments = node.SubNodes.ToArray();
                        break;
                    case "processes":
                        processes = node.SubNodes.ToArray();
                        break;
                }
            }

            var newXml = new StringBuilder("<transformalize><processes>");

            GlobalDiagnosticsContext.Set("process", processes.Count() > 1 ? "All" : processes.First().GetAttribute("name").Value);
            GlobalDiagnosticsContext.Set("entity", Common.LogLength("All"));

            if (environments.Length > 0) {
                var parameters = environmentDefault.Equals(string.Empty) ?
                    environments.First().SubNodes.First().SubNodes.ToArray() :
                    environments.First(e => e.GetAttribute("name").Value.Equals(environmentDefault)).SubNodes.First().SubNodes.ToArray();

                LogManager.GetLogger("tfl").Info("Environment: {0}", environmentDefault.Equals(string.Empty) ? "first" : environmentDefault);

                foreach (var process in processes) {
                    newXml.AppendLine(ApplyParameters(process.ToString(), parameters));
                }
            } else {
                foreach (var process in processes) {
                    var processXml = process.ToString();
                    if (process.SubNodes.Any(n => n.Name.Equals("parameters"))) {
                        processXml = ApplyParameters(processXml, process.SubNodes.First(n => n.Name.Equals("parameters")).SubNodes);
                    }
                    newXml.AppendLine(processXml);
                }
            }

            newXml.AppendLine("</processes></transformalize>");
            return newXml.ToString();
        }

        private static string ApplyParameters(string xml, IEnumerable<NanoXmlNode> parameters) {
            var log = LogManager.GetLogger("tfl");
            var result = xml;
            foreach (var parameter in parameters) {
                var name = parameter.GetAttribute("name").Value.Trim("@".ToCharArray());
                var placeHolder = "@(" + name + ")";
                if (result.Contains(placeHolder)) {
                    var value = parameter.GetAttribute("value").Value;
                    result = result.Replace(placeHolder, value);
                    log.Info("{0} replaced with \"{1}\"", placeHolder, value);
                } else {
                    log.Debug("{0} not found.", placeHolder);
                }
            }
            return result;
        }

        private static object SafeAttribute(XElement element, string attribute, object defaultValue) {
            if (element.HasAttributes && element.Attributes().Any(a => a.Name.ToString().Equals(attribute))) {
                return Convert.ChangeType(element.Attribute(attribute).Value, defaultValue.GetType());
            }
            return defaultValue;
        }
    }
}