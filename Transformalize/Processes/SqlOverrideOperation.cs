using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Transformalize.Libs.Rhino.Etl;
using Transformalize.Libs.Rhino.Etl.Operations;
using Transformalize.Main;
using Transformalize.Main.Providers;
using Transformalize.Operations;

namespace Transformalize.Processes
{
    public class SqlOverrideOperation : InputCommandOperation {
        private readonly Entity _entity;
        private readonly Dictionary<string, Func<IDataReader, int, object, object>> _map = Common.GetReaderMap();
        private readonly FieldTypeDefault[] _fields;
        private readonly int _length;

        public SqlOverrideOperation(Entity entity, AbstractConnection connection)
            : base(connection) {
            _entity = entity;
            _fields = entity.Fields.Select(f => new FieldTypeDefault(f.Value.Alias, _map.ContainsKey(f.Value.SimpleType) && !f.Value.Transforms.Contains("map") ? f.Value.SimpleType : string.Empty, f.Value.Default)).ToArray();
            _length = _fields.Length;
        }

        protected override Row CreateRowFromReader(IDataReader reader) {
            var row = new Row();
            for (var i = 0; i < _length; i++) {
                //row[_fields[i].Alias] = _map[_fields[i].Type](reader, i, _fields[i].Default);
                row[_fields[i].Alias] = reader.GetValue(i);
            }
            return row;
        }

        protected override void PrepareCommand(IDbCommand cmd) {
            cmd.CommandText = _entity.SqlOverride;
            cmd.CommandTimeout = 0;
            cmd.CommandType = CommandType.Text;
        }
    }
}