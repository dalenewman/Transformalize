using System;
using System.Data;
using System.Linq;
using Transformalize.Libs.Rhino.Etl;
using Transformalize.Libs.Rhino.Etl.Operations;
using Transformalize.Operations;
using Transformalize.Runner;

namespace Transformalize.Main.Providers.Sql {
    public class SqlOverrideOperation : InputCommandOperation {

        private readonly Entity _entity;
        private readonly NameAlias[] _fields;
        private const StringComparison IC = StringComparison.OrdinalIgnoreCase;

        public SqlOverrideOperation(Entity entity, AbstractConnection connection)
            : base(connection) {
            CommandBehavior = CommandBehavior.Default;
            EntityName = entity.Name;
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

            if (!string.IsNullOrEmpty(_entity.SqlScriptOverride)) {
                var reader = _entity.SqlScriptOverride.StartsWith("http", IC) ? (ContentsReader)new ContentsWebReader(Logger) : new ContentsFileReader(Logger);
                var contents = reader.Read(_entity.SqlScriptOverride);
                _entity.SqlOverride = contents.Content.TrimEnd();

                if (_entity.SqlOverride.EndsWith("UNION ALL", IC)) {
                     _entity.SqlOverride = _entity.SqlOverride.Remove(_entity.SqlOverride.Length - 9);
                }
            }

            Debug("SqlOverride: " + _entity.SqlOverride);
            cmd.CommandText = _entity.SqlOverride;
            cmd.CommandTimeout = 0;
            cmd.CommandType = CommandType.Text;
        }
    }
}