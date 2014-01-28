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
using System.Linq;
using System.Xml.Linq;
using Transformalize.Configuration;
using Transformalize.Extensions;
using Transformalize.Libs.NLog;
using Transformalize.Main;

namespace Transformalize.Runner {
    public class ProcessXmlConfigurationReader : IReader<ProcessConfigurationElement> {

        private readonly string _file;
        private readonly IContentsReader _contentsReader;
        private readonly Logger _log = LogManager.GetLogger(string.Empty);

        public ProcessXmlConfigurationReader(string file, IContentsReader contentsReader) {
            _file = file;
            _contentsReader = contentsReader;
        }

        public ProcessConfigurationElement Read() {
            var contents = _contentsReader.Read(_file);
            var section = new TransformalizeConfiguration();

            try {
                var doc = XDocument.Parse(contents.Content);

                var process = doc.Element("process");
                if (process == null) {
                    var transformalize = doc.Element("transformalize");
                    if (transformalize == null) {
                        _log.Error("Sorry.  I can't find the <process/> or <transformalize/> element in {0}.", _file);
                        Environment.Exit(1);
                    } else {
                        section.Deserialize(transformalize.ToString());
                    }
                } else {
                    var xml = string.Format(@"
                    <transformalize>
                        <processes>
                            <add name=""{0}"" enabled=""{1}"" inherit=""{2}"" time-zone=""{3}"">{4}</add>
                        </processes>
                    </transformalize>",
                        contents.Name,
                        SafeAttribute(process, "enabled", true),
                        SafeAttribute(process, "inherit", string.Empty),
                        SafeAttribute(process, "time-zone", string.Empty),
                        process.InnerXml()
                    );
                    section.Deserialize(xml);
                }

                return section.Processes[0];
            } catch (Exception e) {
                _log.Error("Sorry.  I couldn't parse the file {0}.  Make sure it is valid XML and try again. {1}", contents.Name, e.Message);
                Environment.Exit(1);
            }

            return null;
        }

        private object SafeAttribute(XElement element, string attribute, object defaultValue) {
            if (element.HasAttributes && element.Attributes().Any(a => a.Name.ToString().Equals(attribute))) {
                return Convert.ChangeType(element.Attribute(attribute).Value, defaultValue.GetType());
            }
            return defaultValue;
        }

    }
}