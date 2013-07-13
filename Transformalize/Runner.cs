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
using Transformalize.Readers;

namespace Transformalize {
    public class Runner {
        private readonly string _mode;
        private Process _process;

        public Runner(string process, string mode) {
            _mode = mode.ToLower();
            _process = new ProcessReader(process).Read();
        }

        public void Run() {
            if (!_process.IsReady()) return;

            switch (_mode) {
                case "init":
                    new InitializationProcess(_process).Execute();
                    break;
                default:
                    new EntityRecordsExist(ref _process).Check();
                    foreach (var entity in _process.Entities) {
                        new EntityProcess(ref _process, entity.Value).Execute();
                    }
                    new UpdateMasterProcess(ref _process).Execute();
                    new TransformProcess(_process).Execute();
                    break;
            }
        }


    }
}
