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

using System.Linq;
using System.Text;
using Transformalize.Extensions;
using Transformalize.Libs.Rhino.Etl;
using Transformalize.Logging;
using Transformalize.Main;
using Transformalize.Operations;

namespace Transformalize.Processes {

    public class InitializationProcess : EtlProcess {

        public InitializationProcess(Process process)
            : base(process) {
            process.OutputConnection.TflWriter.Initialize(process);
            foreach (var writer in process.OutputConnection.ViewWriters) {
                writer.Drop(process);
            }
        }

        protected override void Initialize() {
            foreach (var entity in Process.Entities) {
                Register(new EntityDrop(Process, entity));
                Register(new EntityCreate(Process, entity));
            }
            Register(new MasterEntityIndex(Process));
        }

        protected override void PostProcessing() {
            var errors = GetAllErrors().ToArray();
            if (errors.Any()) {
                var messageBuilder = new StringBuilder();
                foreach (var error in errors) {
                    foreach (var e in error.FlattenHierarchy()) {
                        TflLogger.Error(this.Process.Name, string.Empty, e.Message);
                        messageBuilder.AppendLine(e.Message);
                        TflLogger.Debug(this.Process.Name, string.Empty, e.StackTrace);
                        messageBuilder.AppendLine(e.StackTrace);
                    }
                }
                throw new TransformalizeException(this.Process.Name, string.Empty, "Initialization Process failed for {0}. {1}", Process.Name, messageBuilder.ToString());
            }

            foreach (var writer in Process.OutputConnection.ViewWriters) {
                writer.Create(Process);
            }

            base.PostProcessing();
        }
    }
}