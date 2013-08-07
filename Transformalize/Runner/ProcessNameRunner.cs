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

using System.Configuration;
using Transformalize.Configuration;
using Transformalize.Core;
using Transformalize.Core.Process_;

namespace Transformalize.Runner
{
    public class ProcessNameRunner : AbstractProcessRunner
    {
        private readonly string _processName;
        private readonly Options _options;

        public ProcessNameRunner(string processName)
        {
            _processName = processName;
        }

        public ProcessNameRunner(string processName, Options options)
        {
            _processName = processName;
            _options = options;
        }

        public new void Run()
        {
            var config = ((TransformalizeConfiguration)ConfigurationManager.GetSection("transformalize")).Processes.Get(_processName);
            if (config == null)
            {
                Log.Warn("Sorry.  I can't find a process named {0}.", _processName);
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
