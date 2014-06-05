using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Microsoft.AnalysisServices;
using Transformalize.Libs.Rhino.Etl;
using Transformalize.Libs.Rhino.Etl.Operations;
using Transformalize.Main;
using Transformalize.Main.Providers;

namespace Transformalize.Operations {
    public class SqlOverrideOperation : InputCommandOperation {

        private readonly Entity _entity;
        private readonly NameAlias[] _fields;

        public SqlOverrideOperation(Entity entity, AbstractConnection connection)
            : base(connection) {
            CommandBehavior = CommandBehavior.Default;
            _entity = entity;
            _fields = entity.Fields.WithInput().NameAliases().ToArray();
        }

        protected override Row CreateRowFromReader(IDataReader reader) {
            var row = new Row();
            foreach (var field in _fields) {
                row[field.Alias] = reader[field.Name];
            }
            return row;
        }

        protected override void PrepareCommand(IDbCommand cmd) {
            Debug("SqlOverride: " + _entity.SqlOverride);
            cmd.CommandText = _entity.SqlOverride;
            cmd.CommandTimeout = 0;
            cmd.CommandType = CommandType.Text;
        }
    }
}