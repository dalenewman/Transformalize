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
using Transformalize.Libs.Rhino.Etl;
using Transformalize.Libs.Rhino.Etl.Operations;
using Transformalize.Main;

namespace Transformalize.Operations {
    public class EntityAddTflFields : AbstractOperation {
        private readonly Process _process;
        private readonly Entity _entity;

        public EntityAddTflFields(Process process, Entity entity) {
            _process = process;
            _entity = entity;
            UseTransaction = false;
        }

        public override IEnumerable<Row> Execute(IEnumerable<Row> rows) {
            OnFinishedProcessing += EntityAddTflFields_OnFinishedProcessing;
            foreach (var row in rows) {
                row["TflBatchId"] = _entity.TflBatchId;
                row["TflDeleted"] = false;
                yield return row;
            }
        }

        private void EntityAddTflFields_OnFinishedProcessing(IOperation obj) {
            _entity.Inserts = obj.Statistics.OutputtedRows;
            _process.Anything += _entity.Inserts;
        }
    }
}