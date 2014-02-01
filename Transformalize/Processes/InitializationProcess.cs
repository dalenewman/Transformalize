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
using Transformalize.Libs.NLog;
using Transformalize.Libs.Rhino.Etl;
using Transformalize.Main;
using Transformalize.Operations;

namespace Transformalize.Processes {
    public class InitializationProcess : EtlProcess {
        private readonly Process _process;

        public InitializationProcess(Process process) {
            GlobalDiagnosticsContext.Set("entity", Common.LogLength("All"));

            _process = process;

            process.OutputConnection.TflWriter.Initialize(process);
            process.OutputConnection.ViewWriter.Drop(process);
        }

        protected override void Initialize() {
            foreach (var entity in _process.Entities) {
                Register(new EntityDrop(_process, entity));
                Register(new EntityCreate(entity, _process));
            }
        }

        protected override void PostProcessing() {
            var errors = GetAllErrors().ToArray();
            if (errors.Any()) {
                foreach (var error in errors) {
                    Error(error.InnerException, "Message: {0}\r\nStackTrace:{1}\r\n", error.Message, error.StackTrace);
                }
                Environment.Exit(1);
            }

            if (_process.Entities.Count > 0) {
                _process.OutputConnection.ViewWriter.Create(_process);
            }
            base.PostProcessing();
        }
    }
}