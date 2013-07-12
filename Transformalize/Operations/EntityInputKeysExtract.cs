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
using System.Data.SqlClient;
using Transformalize.Data;
using Transformalize.Model;
using Transformalize.Rhino.Etl.Core;
using Transformalize.Rhino.Etl.Core.Operations;

namespace Transformalize.Operations {

    public class EntityInputKeysExtract : AbstractOperation {
        private readonly Entity _entity;
        private readonly IEntityVersionReader _entityVersionReader;

        public EntityInputKeysExtract(Entity entity, IEntityVersionReader entityVersionReader = null) {
            _entity = entity;
            _entityVersionReader = entityVersionReader ?? new SqlServerEntityVersionReader(_entity);
            _entity.Begin = _entityVersionReader.GetBeginVersion();
            _entity.End = _entityVersionReader.GetEndVersion();
        }

        public override IEnumerable<Row> Execute(IEnumerable<Row> rows) {

            if (!_entityVersionReader.HasRows) {
                Info("{0} | No data detected in {1}.", _entity.ProcessName, _entity.Name);
                yield break;
            }

            if (_entityVersionReader.IsRange) {
                if (_entityVersionReader.BeginAndEndAreEqual()) {
                    Info("{0} | No changes detected in {1}.", _entity.ProcessName, _entity.Name);
                    yield break;
                }
            }

            using (var cn = new SqlConnection(_entity.InputConnection.ConnectionString)) {
                cn.Open();
                var cmd = new SqlCommand(SelectKeys(_entityVersionReader.IsRange), cn);

                if (_entityVersionReader.IsRange)
                    cmd.Parameters.Add(new SqlParameter("@Begin", _entity.Begin));
                cmd.Parameters.Add(new SqlParameter("@End", _entity.End));

                using (var reader = cmd.ExecuteReader()) {

                    while (reader.Read()) {
                        var row = new Row();
                        foreach (var pk in _entity.PrimaryKey) {
                            row[pk.Key] = reader.GetValue(pk.Value.Index);
                        }
                        yield return row;
                    }
                }
            }


        }

        public string SelectKeys(bool isRange) {
            const string sqlPattern = @"SELECT {0} FROM [{1}].[{2}] WITH (NOLOCK) WHERE {3} ORDER BY {4};";

            var criteria = string.Format(isRange ? "[{0}] BETWEEN @Begin AND @End" : "[{0}] <= @End", _entity.Version.Name);
            var orderByKeys = new List<string>();
            var selectKeys = new List<string>();

            foreach (var pair in _entity.PrimaryKey) {
                selectKeys.Add(pair.Value.Alias.Equals(pair.Value.Name) ? string.Concat("[", pair.Value.Name, "]") : string.Format("{0} = [{1}]", pair.Value.Alias, pair.Value.Name));
                orderByKeys.Add(string.Concat("[", pair.Value.Name, "]"));
            }

            return string.Format(sqlPattern, string.Join(", ", selectKeys), _entity.Schema, _entity.Name, criteria, string.Join(", ", orderByKeys));
        }

    }
}