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