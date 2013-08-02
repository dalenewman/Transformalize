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

using Transformalize.Data;
using Transformalize.Model;
using Transformalize.Processes;

namespace Transformalize
{
    public class Runner
    {
        private Process _process;

        public Runner(string process)
        {
            _process = new ProcessReader(process).Read();
        }

        public Runner(Process process)
        {
            _process = process;
        }

        public Runner(string process, Options options)
        {
            _process = new ProcessReader(process).Read();
            _process.Options = options;
        }

        public Runner(Process process, Options options)
        {
            _process = process;
            _process.Options = options;
        }

        public void Run()
        {
            if (!_process.IsReady()) return;

            switch (_process.Options.Mode)
            {
                case Modes.Initialize:
                    using (var process = new InitializationProcess(_process))
                    {
                        process.Execute();
                    }
                    break;
                default:
                    new EntityRecordsExist(ref _process).Check();

                    foreach (var entity in _process.Entities)
                    {
                        using (var entityProcess = new EntityProcess(ref _process, entity))
                        {
                            entityProcess.Execute();
                        }
                    }

                    using (var masterProcess = new UpdateMasterProcess(ref _process))
                    {
                        masterProcess.Execute();
                    }

                    if (_process.Transforms.Length > 0)
                    {
                        using (var transformProcess = new TransformProcess(_process))
                        {
                            transformProcess.Execute();
                        }
                    }

                    if (_process.Options.RenderTemplates)
                    {
                        new TemplateManager(_process).Manage();    
                    }
                    
                    break;
            }
        }

    }
}
