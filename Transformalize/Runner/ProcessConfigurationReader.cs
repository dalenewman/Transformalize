using System;
using System.Configuration;
using Transformalize.Configuration;
using Transformalize.Core;
using Transformalize.Libs.NLog;

namespace Transformalize.Runner
{
    public class ProcessConfigurationReader : IReader<ProcessConfigurationElement>
    {
        private readonly string _name;
        private readonly Logger _log = LogManager.GetCurrentClassLogger();

        public ProcessConfigurationReader(string name)
        {
            _name = name;
        }

        public ProcessConfigurationElement Read()
        {
            var config = ((TransformalizeConfiguration)ConfigurationManager.GetSection("transformalize")).Processes.Get(_name);
            if (config == null)
            {
                _log.Warn("Sorry.  I can't find a process named {0}.", _name);
                Environment.Exit(1);
            }
            return config;
        }
    }
}