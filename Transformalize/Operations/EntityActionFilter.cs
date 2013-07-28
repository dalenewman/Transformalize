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

using System.Collections.Generic;
using System.Linq;
using Transformalize.Libs.Rhino.Etl.Core;
using Transformalize.Libs.Rhino.Etl.Core.Operations;
using Transformalize.Model;

namespace Transformalize.Operations {
    public class EntityActionFilter : AbstractOperation {
        private readonly Entity _entity;
        private readonly EntityAction _entityAction;

        public EntityActionFilter(ref Entity entity, EntityAction entityAction) {
            _entity = entity;
            _entityAction = entityAction;
        }

        public override IEnumerable<Row> Execute(IEnumerable<Row> rows) {
            OnFinishedProcessing += EntityActionFilter_OnFinishedProcessing;
            return rows.Where(r => r["a"].Equals(_entityAction));
        }

        void EntityActionFilter_OnFinishedProcessing(IOperation obj) {
            _entity.RecordsAffected += obj.Statistics.OutputtedRows;
        }

    }
}