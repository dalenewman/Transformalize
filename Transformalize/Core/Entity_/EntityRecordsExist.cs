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

using Transformalize.Core.Process_;
using Transformalize.Libs.Rhino.Etl.Core;
using Transformalize.Providers.SqlServer;

namespace Transformalize.Core.Entity_ {
    public class EntityRecordsExist : WithLoggingMixin {
        private readonly Process _process;
        private readonly IEntityRecordsExist _entityRecordsExist;

        public EntityRecordsExist(ref Process process, IEntityRecordsExist entityRecordsExist = null) {
            _process = process;
            _entityRecordsExist = entityRecordsExist ?? new SqlServerEntityRecordsExist();
        }

        public void Check() {
            var entity = _process.MasterEntity;
            _process.OutputRecordsExist = _entityRecordsExist.OutputRecordsExist(entity);
            Debug(
                _process.OutputRecordsExist
                    ? "{0} | {1}.{2} has records; delta run."
                    : "{0} | {1}.{2} is empty; initial run.",
                Process.Name,
                entity.Schema,
                entity.OutputName()
            );
        }
    }
}
