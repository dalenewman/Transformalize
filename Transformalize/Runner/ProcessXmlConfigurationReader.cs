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
using System.IO;
using System.Xml.Linq;
using Transformalize.Configuration;
using Transformalize.Extensions;
using Transformalize.Libs.NLog;
using Transformalize.Main;

namespace Transformalize.Runner
{
    public class ProcessXmlConfigurationReader : IReader<ProcessConfigurationElement>
    {
        private readonly string _file;
        private readonly Logger _log = LogManager.GetCurrentClassLogger();

        public ProcessXmlConfigurationReader(string file)
        {
            _file = file;
        }

        public ProcessConfigurationElement Read()
        {
            var xmlFileInfo = new FileInfo(_file);

            if (!xmlFileInfo.Exists)
            {
                _log.Error("Sorry. I can't find file {0}.", xmlFileInfo.FullName);
                Environment.Exit(1);
            }

            var contents = File.ReadAllText(xmlFileInfo.FullName);
            try
            {
                var doc = XDocument.Parse(contents);

                var process = doc.Element("process");
                if (process == null)
                {
                    _log.Error("Sorry.  I can't find the <process/> element in {0}.", xmlFileInfo.Name);
                    Environment.Exit(1);
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
                return section.Processes[0];
            }
            catch (Exception e)
            {
                _log.Error("Sorry.  I couldn't parse the file {0}.  Make sure it is valid XML and try again. {1}", xmlFileInfo.Name, e.Message);
                Environment.Exit(1);
            }

            return null;
        }
    }
}