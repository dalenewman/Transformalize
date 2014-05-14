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

        private struct NameAlias {
            public string Name;
            public string Alias;
        }

        private readonly Entity _entity;
        private readonly Dictionary<string, Func<IDataReader, int, object, object>> _map = Common.GetReaderMap();
        private readonly NameAlias[] _fields;

        public SqlOverrideOperation(Entity entity, AbstractConnection connection)
            : base(connection) {
            CommandBehavior = CommandBehavior.Default;
            _entity = entity;
            _fields = entity.Fields.Where(f => f.Value.Input).Select(f => new NameAlias() { Name = f.Value.Name, Alias = f.Value.Alias }).ToArray();
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