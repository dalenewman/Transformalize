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

using System.Linq;
using Transformalize.Core;
using Transformalize.Core.Process_;
using Transformalize.Libs.Rhino.Etl.Core;
using Transformalize.Operations;
using Transformalize.Providers;
using Transformalize.Providers.SqlServer;

namespace Transformalize.Processes
{

    public class InitializationProcess : EtlProcess
    {

        private readonly Process _process;
        private readonly ITflWriter _tflWriter;
        private readonly IViewWriter _viewWriter;

        public InitializationProcess(Process process, ITflWriter tflWriter = null, IViewWriter viewWriter = null) : base(process.Name)
        {
            _process = process;
            _tflWriter = tflWriter ?? new SqlServerTflWriter(ref process);
            _viewWriter = viewWriter ?? new SqlServerViewWriter(process);

            _tflWriter.Initialize();
            _viewWriter.Drop();
        }

        protected override void Initialize()
        {
            foreach (var entity in Process.Entities)
            {
                Register(new EntityDrop(entity));
                Register(new EntityCreate(entity, _process));
            }
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
                throw new TransformalizeException("Initialization Error!");
            }

            _viewWriter.Create();
            base.PostProcessing();
        }

    }

}
