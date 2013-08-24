/*
Transformalize - Replicate, Transform, and Denormalize Your Data...
Copyright (C) 2013 Dale Newman

This program is free software: you can redistribute it and/or modify
it under the terms of the GNU General Public License as published by
the Free Software Foundation, either version 3 of the License, or
(at your option) any later version.

This program is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU General Public License for more details.

You should have received a copy of the GNU General Public License
along with this program.  If not, see <http://www.gnu.org/licenses/>.
*/

using System;
using System.IO;
using System.Xml.Linq;
using Transformalize.Configuration;
using Transformalize.Core;
using Transformalize.Core.Process_;
using Transformalize.Extensions;
using Transformalize.Libs.NLog;

namespace Transformalize.Runner
{
    public class ProcessXmlRunner : AbstractProcessRunner
    {
        private readonly string _xmlFile;
        private readonly Options _options;

        public ProcessXmlRunner(string xmlFile)
        {
            _xmlFile = xmlFile;
        }

        public ProcessXmlRunner(string xmlFile, Options options)
        {
            _xmlFile = xmlFile;
            _options = options;
        }

        public new void Run()
        {
            ProcessConfigurationElement config;
            var xmlFileInfo = new FileInfo(_xmlFile);

            if (!xmlFileInfo.Exists)
            {
                Log.Error("Sorry. I can't find file {0}.", xmlFileInfo.FullName);
                return;
            }

            var contents = File.ReadAllText(xmlFileInfo.FullName);
            try
            {
                var doc = XDocument.Parse(contents);

                var process = doc.Element("process");
                if (process == null)
                {
                    Log.Error("Sorry.  I can't find the <process/> element in {0}.", xmlFileInfo.Name);
                    return;
                }

                var section = new TransformalizeConfiguration();
                var name = xmlFileInfo.Name.Remove(xmlFileInfo.Name.Length - xmlFileInfo.Extension.Length);
                var xml = string.Format(@"
                    <transformalize>
                        <processes>
                            <add name=""{0}"">{1}</add>
                        </processes>
                    </transformalize>", name, process.InnerXml());
                section.Deserialize(xml);
                config = section.Processes[0];
            }
            catch (Exception e)
            {
                 Log.Error("Sorry.  I couldn't parse the file {0}.  Make sure it is valid XML and try again. {1}", xmlFileInfo.Name, e.Message);
                return;
            }

            Process = new ProcessReader(config).Read();

            if (_options != null)
            {
                Process.Options = _options;
            }

            base.Run();
        }
    }
}