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

using Transformalize.Model;
using Transformalize.Rhino.Etl.Core;

namespace Transformalize.Data {
    public class EntityCounter : WithLoggingMixin {
        private readonly Process _process;
        private readonly IEntityCounter _entityCounter;

        public EntityCounter(ref Process process, IEntityCounter entityCounter = null) {
            _process = process;
            _entityCounter = entityCounter ?? new SqlServerEntityCounter(new SqlServerConnectionChecker(process.Name));
        }

        public void Count() {
            foreach (var entity in _process.Entities) {
                entity.Value.InputCount = _entityCounter.CountInput(entity.Value);
                Info("{0} | Entity {1} input has {2} records.", _process.Name, entity.Value.Name, entity.Value.InputCount);
                entity.Value.OutputCount = _entityCounter.CountOutput(entity.Value);
                Info("{0} | Entity {1} output has {2} records.", _process.Name, entity.Value.Name, entity.Value.OutputCount);
            }
        }
    }
}
