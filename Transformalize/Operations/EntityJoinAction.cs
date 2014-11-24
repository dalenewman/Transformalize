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
using Transformalize.Libs.Nest.DSL.Repository;
using Transformalize.Libs.Rhino.Etl;
using Transformalize.Libs.Rhino.Etl.Operations;
using Transformalize.Logging;
using Transformalize.Main;

namespace Transformalize.Operations {
    public class EntityJoinAction : JoinOperation {
        private readonly string _firstKey;
        private readonly bool _hasVersion;
        private readonly string _versionAlias;
        private readonly string _versionSimpleType;
        private readonly bool _entityDelete;
        private readonly int _tflBatchId;
        private readonly string[] _keys;
        private static readonly string[] Bytes = new[] { "byte[]", "rowversion" };

        public EntityJoinAction(Process process, Entity entity)
            : base(process) {
            _hasVersion = entity.Version != null;
            _versionAlias = _hasVersion ? entity.Version.Alias : string.Empty;
            _versionSimpleType = _hasVersion ? entity.Version.SimpleType : string.Empty;
            _entityDelete = entity.Delete;
            _tflBatchId = entity.TflBatchId;
            _keys = entity.PrimaryKey.Aliases().ToArray();
            _firstKey = _keys[0];
        }

        protected override Row MergeRows(Row leftRow, Row rightRow) {

            if (rightRow.ContainsKey(_firstKey)) {
                var wasDeleted = _entityDelete && rightRow["TflDeleted"] != null && (bool)rightRow["TflDeleted"];
                if (wasDeleted || !_hasVersion || UpdateIsNecessary(ref leftRow, ref rightRow, _versionAlias, _versionSimpleType)) {
                    leftRow["TflAction"] = EntityAction.Update;
                    leftRow["TflKey"] = rightRow["TflKey"];
                    leftRow["TflBatchId"] = _tflBatchId;
                    leftRow["TflDeleted"] = false;
                } else {
                    leftRow["TflAction"] = EntityAction.None;
                    leftRow["TflDeleted"] = false;
                }
            } else {
                leftRow["TflAction"] = EntityAction.Insert;
                leftRow["TflBatchId"] = _tflBatchId;
                leftRow["TflDeleted"] = false;
            }

            return leftRow;
        }

        protected override void SetupJoinConditions() {
            LeftJoin.Left(_keys).Right(_keys);
        }

        private static bool UpdateIsNecessary(ref Row leftRow, ref Row rightRow, string versionAlias, string versionSimpleType) {
            if (Bytes.Any(t => t == versionSimpleType)) {
                var beginBytes = (byte[])leftRow[versionAlias];
                var endBytes = (byte[])rightRow[versionAlias];
                return !beginBytes.SequenceEqual(endBytes);
            }
            return !leftRow[versionAlias].Equals(rightRow[versionAlias]);
        }
    }
}