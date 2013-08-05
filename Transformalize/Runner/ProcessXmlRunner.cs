using System;
using System.IO;
using System.Xml.Linq;
using Transformalize.Configuration;
using Transformalize.Data;
using Transformalize.Extensions;
using Transformalize.Model;

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