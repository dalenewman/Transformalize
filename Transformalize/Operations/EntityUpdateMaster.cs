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
using System.Data;
using System.Linq;
using System.Text;
using Transformalize.Main;
using Transformalize.Main.Providers;
using Transformalize.Libs.NLog;
using Transformalize.Libs.Rhino.Etl;
using Transformalize.Libs.Rhino.Etl.Operations;

namespace Transformalize.Operations
{
    public class EntityUpdateMaster : AbstractOperation
    {
        private readonly Entity _entity;
        private readonly Process _process;

        public EntityUpdateMaster(Process process, Entity entity)
        {
            GlobalDiagnosticsContext.Set("entity", Common.LogLength(entity.Alias, 20));
            _process = process;
            _entity = entity;
            UseTransaction = false;
        }

        public override IEnumerable<Row> Execute(IEnumerable<Row> rows)
        {
            if (_entity.IsMaster())
                return rows;

            if (_entity.RecordsAffected == 0)
                return rows;

            if (_process.OutputRecordsExist || _entity.HasForeignKeys())
            {
                AbstractConnection connection = _process.MasterEntity.OutputConnection;
                using (IDbConnection cn = connection.GetConnection())
                {
                    cn.Open();
                    IDbCommand cmd = cn.CreateCommand();
                    cmd.CommandText = PrepareSql();
                    cmd.CommandTimeout = 0;

                    Debug(cmd.CommandText);

                    IDbDataParameter parameter = cmd.CreateParameter();
                    parameter.ParameterName = "@TflBatchId";
                    parameter.Value = _entity.TflBatchId;

                    cmd.Parameters.Add(parameter);
                    int records = cmd.ExecuteNonQuery();

                    Debug("TflBatchId = {0}.", _entity.TflBatchId);
                    Info("Processed {0} rows.  Updated {1} with {2}.", records, _process.MasterEntity.Alias, _entity.Alias);
                }
            }
            return rows;
        }

        private string PrepareSql()
        {
            var builder = new StringBuilder();
            Entity masterEntity = _process.MasterEntity;
            AbstractProvider provider = _entity.OutputConnection.Provider;

            string master = string.Format("[{0}]", masterEntity.OutputName());
            string source = string.Format("[{0}]", _entity.OutputName());
            string sets = _process.OutputRecordsExist ?
                              new FieldSqlWriter(_entity.Fields).FieldType(FieldType.ForeignKey).AddBatchId(false).Alias(provider).Set(master, source).Write(",\r\n    ") :
                              new FieldSqlWriter(_entity.Fields).FieldType(FieldType.ForeignKey).Alias(provider).Set(master, source).Write(",\r\n    ");


            builder.AppendFormat("UPDATE {0}\r\n", master);
            builder.AppendFormat("SET {0}\r\n", sets);
            builder.AppendFormat("FROM {0}\r\n", source);

            foreach (Relationship relationship in _entity.RelationshipToMaster)
            {
                string left = string.Format("[{0}]", relationship.LeftEntity.OutputName());
                string right = string.Format("[{0}]", relationship.RightEntity.OutputName());
                string join = string.Join(" AND ", relationship.Join.Select(j => string.Format("{0}.[{1}] = {2}.[{3}]", left, j.LeftField.Alias, right, j.RightField.Alias)));
                builder.AppendFormat("INNER JOIN {0} ON ({1})\r\n", left, join);
            }

            builder.AppendFormat("WHERE {0}.[TflBatchId] = @TflBatchId", source);
            return builder.ToString();
        }
    }
}