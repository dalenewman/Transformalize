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
using Transformalize.Extensions;
using Transformalize.Libs.NLog;
using Transformalize.Libs.Rhino.Etl;
using Transformalize.Main;
using Transformalize.Operations;

namespace Transformalize.Processes {
    public class TransformProcess : EtlProcess {

        public TransformProcess(Process process)
            : base(process) {
        }

        protected override void Initialize() {

            GlobalDiagnosticsContext.Set("entity", Common.LogLength("All"));

            Register(new ParametersExtract(Process));
            Register(new ApplyDefaults(true, Process.CalculatedFields));

            foreach (var transform in Process.TransformOperations) {
                Register(transform);
            }

            Register(new TruncateOperation(Process.CalculatedFields));
            Register(new GatherOperation());
            //RegisterLast(new ResultsLoad(Process));
            RegisterLast(new DapperResultsLoad(Process));
        }

        protected override void PostProcessing() {
            var errors = GetAllErrors().ToArray();
            if (errors.Any()) {
                foreach (var error in errors) {
                    foreach (var e in error.FlattenHierarchy()) {
                        Error(e.Message);
                        Debug(e.StackTrace);
                    }
                }
                throw new TransformalizeException("Transform process failed for {0}.", Process.Name);
            }

            base.PostProcessing();
        }
    }
}