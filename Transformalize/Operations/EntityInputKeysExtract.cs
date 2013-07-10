using System.Collections.Generic;
using System.Data.SqlClient;
using Transformalize.Data;
using Transformalize.Model;
using Transformalize.Rhino.Etl.Core;
using Transformalize.Rhino.Etl.Core.Operations;

namespace Transformalize.Operations {

    public class EntityInputKeysExtract : AbstractOperation {
        private readonly Entity _entity;
        private readonly IEntityVersion _entityVersion;

        public EntityInputKeysExtract(Entity entity, IEntityVersion entityVersion = null) {
            _entity = entity;
            _entityVersion = entityVersion ?? new SqlServerEntityVersion(_entity);
            _entity.Begin = _entityVersion.GetBeginVersion();
            _entity.End = _entityVersion.GetEndVersion();
        }

        public override IEnumerable<Row> Execute(IEnumerable<Row> rows) {

            if (!_entityVersion.HasRows) {
                Info("{0} | No data detected in {1}.", _entity.ProcessName, _entity.Name);
                yield break;
            }

            if (_entityVersion.IsRange) {
                if (_entityVersion.BeginAndEndAreEqual()) {
                    Info("{0} | No changes detected in {1}.", _entity.ProcessName, _entity.Name);
                    yield break;
                }
            }

            using (var cn = new SqlConnection(_entity.InputConnection.ConnectionString)) {
                cn.Open();
                var cmd = new SqlCommand(_entity.EntitySqlWriter.SelectKeys(_entityVersion.IsRange), cn);

                if (_entityVersion.IsRange)
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
    }
}