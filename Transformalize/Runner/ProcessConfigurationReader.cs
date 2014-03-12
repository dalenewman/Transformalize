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
using System.Configuration;
using Transformalize.Configuration;
using Transformalize.Libs.NLog;
using Transformalize.Main;

namespace Transformalize.Runner
{
    public class ProcessConfigurationReader : IReader<ProcessConfigurationElement>
    {
        private readonly Logger _log = LogManager.GetLogger(string.Empty);
        private readonly string _name;

        public ProcessConfigurationReader(string name)
        {
            _name = name;
        }

        public ProcessConfigurationElement Read()
        {
            var config = ((TransformalizeConfiguration) ConfigurationManager.GetSection("transformalize")).Processes.Get(_name);
            if (config == null)
            {
                _log.Warn("Sorry.  I can't find a process named {0}.", _name);
                LogManager.Flush();
                Environment.Exit(1);
            }
            return config;
        }
    }
}