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
using Transformalize.Libs.Rhino.Etl.Core;
using Transformalize.Libs.Rhino.Etl.Core.Operations;
using Transformalize.Model;

namespace Transformalize.Operations
{
    public class EntityJoinAction : JoinOperation {
        private readonly string[] _keys;
        private readonly string _firstKey;
        private readonly int _tflBatchId;

        public EntityJoinAction(Entity entity) {
            _keys = entity.PrimaryKey.Select(e => e.Value.Alias).ToArray();
            _firstKey = _keys[0];
            _tflBatchId = entity.TflBatchId;
        }

        protected override Row MergeRows(Row leftRow, Row rightRow) {

            if (rightRow.ContainsKey(_firstKey)) {
                leftRow["a"] = EntityAction.Update;
                leftRow["TflKey"] = rightRow["TflKey"];
            } else {
                leftRow["a"] = EntityAction.Insert;
                leftRow["TflBatchId"] = _tflBatchId;
            }

            return leftRow;
        }

        protected override void SetupJoinConditions() {
            LeftJoin.Left(_keys).Right(_keys);
        }
    }
}