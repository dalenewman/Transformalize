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
using System.Configuration;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using Transformalize.Configuration;
using Transformalize.Libs.NanoXml;
using Transformalize.Logging;
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
            XElement transformalize;

            try {
                transformalize = XDocument.Parse(contents.Content).Element("transformalize");
                if (transformalize == null)
                    throw new TransformalizeException(string.Empty, string.Empty, "Can't find the <transformalize/> element in {0}.", string.IsNullOrEmpty(contents.Name) ? "the configuration" : contents.Name);

                // The Transformalize.Orchard API returns these elements, but .NET Configuration doesn't allow them.
                var apiElements = new[] { "request", "status", "message", "time", "response", "log" };
                foreach (var element in apiElements.Where(element => transformalize.Elements(element).Any())) {
                    transformalize.Elements(element).Remove();
                }
            } catch (Exception e) {
                throw new TransformalizeException("Couldn't parse {0}.  Make sure it is valid XML and try again. {1}", contents.Name, e.Message);
            }

            try {
                section.Deserialize(
                    DefaultParameters(transformalize.ToString())
                );
                return section.Processes;
            } catch (ConfigurationErrorsException e) {
                throw new TransformalizeException("Couldn't parse {0}.  {1}", contents.Name, e.Message);
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

            if (environments.Length > 0) {
                var parameters = environmentDefault.Equals(string.Empty) ?
                    environments.First().SubNodes.First().SubNodes.ToArray() :
                    environments.First(e => e.GetAttribute("name").Value.Equals(environmentDefault)).SubNodes.First().SubNodes.ToArray();

                foreach (var process in processes) {
                    TflLogger.Info(string.Empty, string.Empty, "Environment: {0}", environmentDefault.Equals(string.Empty) ? "first" : environmentDefault);
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
            var result = xml;
            foreach (var parameter in parameters) {
                var name = parameter.GetAttribute("name").Value.Trim("@".ToCharArray());
                var placeHolder = "@(" + name + ")";
                if (result.Contains(placeHolder)) {
                    var value = parameter.GetAttribute("value").Value;
                    result = result.Replace(placeHolder, value);
                    TflLogger.Info(string.Empty, string.Empty, "{0} replaced with \"{1}\"", placeHolder, value);
                } else {
                    TflLogger.Debug(string.Empty, string.Empty, "{0} not found.", placeHolder);
                }
            }
            return result;
        }

    }
}