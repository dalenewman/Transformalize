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

using System.Linq;
using Transformalize.Libs.Rhino.Etl;
using Transformalize.Libs.Rhino.Etl.Operations;
using Transformalize.Main;

namespace Transformalize.Operations {
    public class EntityJoinAction : JoinOperation {
        private readonly Entity _entity;
        private readonly string _firstKey;
        private readonly string[] _keys;
        private readonly string[] _bytes = new[] { "byte[]", "rowversion" };

        public EntityJoinAction(Entity entity) {
            _entity = entity;
            _keys = entity.PrimaryKey.Aliases().ToArray();
            _firstKey = _keys[0];
        }

        protected override Row MergeRows(Row leftRow, Row rightRow) {
            if (rightRow.ContainsKey(_firstKey)) {
                if (_entity.Version == null || UpdateIsNecessary(ref leftRow, ref rightRow)) {
                    leftRow["TflAction"] = EntityAction.Update;
                    leftRow["TflKey"] = rightRow["TflKey"];
                    leftRow["TflBatchId"] = _entity.TflBatchId;
                } else {
                    leftRow["TflAction"] = EntityAction.None;
                }
            } else {
                leftRow["TflAction"] = EntityAction.Insert;
                leftRow["TflBatchId"] = _entity.TflBatchId;
            }

            return leftRow;
        }

        protected override void SetupJoinConditions() {
            LeftJoin.Left(_keys).Right(_keys);
        }

        private bool UpdateIsNecessary(ref Row leftRow, ref Row rightRow) {
            if (_bytes.Any(t => t == _entity.Version.SimpleType)) {
                var beginBytes = (byte[])leftRow[_entity.Version.Alias];
                var endBytes = (byte[])rightRow[_entity.Version.Alias];
                return !beginBytes.SequenceEqual(endBytes);
            }
            return !leftRow[_entity.Version.Alias].Equals(rightRow[_entity.Version.Alias]);
        }
    }
}