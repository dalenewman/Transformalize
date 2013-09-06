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

using System;
using System.Linq;
using Transformalize.Core;
using Transformalize.Core.Entity_;
using Transformalize.Core.Process_;
using Transformalize.Libs.NLog;
using Transformalize.Libs.Rhino.Etl.Core;
using Transformalize.Operations;

namespace Transformalize.Processes
{
    public class EntityKeysProcess : EtlProcess
    {
        private readonly Process _process;
        private readonly Entity _entity;

        public EntityKeysProcess(Process process, Entity entity, IEntityBatchReader entityBatchReader) : base(process.Name)
        {
            GlobalDiagnosticsContext.Set("entity", Common.LogLength(entity.Alias,20));
            _process = process;
            _entity = entity;
            _entity.TflBatchId = entityBatchReader.ReadNext(_entity);
        }

        protected override void Initialize()
        {
            if (_process.OutputRecordsExist && _process.Options.UseBeginVersion)
            {
                var operation = new EntityInputKeysExtractDelta(_process, _entity);
                if(operation.NeedsToRun())
                    Register(operation);
            }
            else
            {
                Register(new EntityInputKeysExtractAll(_process, _entity));
            }

            Register(new EntityInputKeysStore(_process, _entity));

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
