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

using System.Collections.Generic;
using System.Linq;
using Transformalize.Libs.Rhino.Etl;
using Transformalize.Libs.Rhino.Etl.Operations;
using Transformalize.Main;

namespace Transformalize.Operations {
    public class EntityActionFilter : AbstractOperation {
        private readonly Process _process;
        private readonly Entity _entity;
        private readonly EntityAction _entityAction;

        public EntityActionFilter(ref Process process, ref Entity entity, EntityAction entityAction) {
            _process = process;
            _entity = entity;
            _entityAction = entityAction;
        }

        public override IEnumerable<Row> Execute(IEnumerable<Row> rows) {
            OnFinishedProcessing += EntityActionFilter_OnFinishedProcessing;
            return rows.Where(row => row["TflAction"].Equals(_entityAction));
        }

        private void EntityActionFilter_OnFinishedProcessing(IOperation obj) {
            switch (_entityAction) {
                case EntityAction.Insert:
                    _entity.Inserts += obj.Statistics.OutputtedRows;
                    _process.Anything += _entity.Inserts;
                    break;
                case EntityAction.Update:
                    _entity.Updates += obj.Statistics.OutputtedRows;
                    _process.Anything += _entity.Updates;
                    break;
                case EntityAction.Delete:
                    _entity.Deletes += obj.Statistics.OutputtedRows;
                    _process.Anything += _entity.Deletes;
                    break;
            }
        }
    }
}