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

namespace Transformalize.Processes
{
    public class TransformProcess : EtlProcess
    {
        private readonly Process _process;

        public TransformProcess(Process process) : base(process.Name)
        {
            GlobalDiagnosticsContext.Set("entity", Common.LogLength("All", 20));
            _process = process;
        }

        protected override void Initialize()
        {
            Register(new ParametersExtract(_process));
            Register(new ApplyDefaults(_process.CalculatedFields));
            Register(new TransformFields(_process.CalculatedFields));
            RegisterLast(new ResultsLoad(_process));
        }

        protected override void PostProcessing()
        {
            var errors = GetAllErrors().ToArray();
            if (errors.Any())
            {
                foreach (var error in errors)
                {
                    Error(error.InnerException, "Message: {0}\r\nStackTrace:{1}\r\n", error.Message, error.StackTrace);
                }
                Environment.Exit(1);
            }

            base.PostProcessing();
        }
    }
}