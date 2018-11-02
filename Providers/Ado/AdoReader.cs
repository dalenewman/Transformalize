#region license
// Transformalize
// Configurable Extract, Transform, and Load
// Copyright 2013-2017 Dale Newman
//  
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//   
//       http://www.apache.org/licenses/LICENSE-2.0
//   
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
#endregion

using System.Collections.Generic;
using System.Data;
using System.Linq;
using Transformalize.Configuration;
using Transformalize.Contracts;

namespace Transformalize.Providers.Ado {
    /// <summary>
    /// A reader for an entity's input (source) or output (destination).
    /// </summary>
    public class AdoReader : IReadInputKeysAndHashCodes, IReadOutputKeysAndHashCodes {

        private int _rowCount;
        private readonly IContext _context;
        private readonly Connection _connection;
        private readonly string _tableOrView;
        private readonly Field[] _fields;
        private readonly IConnectionFactory _cf;
        private readonly ReadFrom _readFrom;
        private readonly AdoRowCreator _rowCreator;
        private readonly string _filter;
        private readonly string _schemaPrefix;

        public AdoReader(IConnectionContext context, Field[] fields, IConnectionFactory cf, IRowFactory rowFactory, ReadFrom readFrom) {
            _context = context;
            _cf = cf;
            _connection = readFrom == ReadFrom.Output
                ? context.Process.Connections.First(c => c.Name == "output")
                : context.Process.Connections.First(c => c.Name == context.Entity.Connection);
            _tableOrView = readFrom == ReadFrom.Output ? context.Entity.OutputTableName(context.Process.Name) : context.Entity.Name;
            _schemaPrefix = readFrom == ReadFrom.Output ? string.Empty : (context.Entity.Schema == string.Empty ? string.Empty : cf.Enclose(context.Entity.Schema) + ".");
            _filter = readFrom == ReadFrom.Output ? $"WHERE {cf.Enclose(_context.Entity.TflDeleted().FieldName())} != 1" : string.Empty;
            _fields = fields;
            _readFrom = readFrom;
            _rowCreator = new AdoRowCreator(context, rowFactory);
        }

        public IEnumerable<IRow> Read() {

            using (var cn = _cf.GetConnection()) {
                cn.Open();
                var cmd = cn.CreateCommand();

                cmd.CommandTimeout = 0;
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = $@"
                    SELECT {string.Join(",", _fields.Select(f => _readFrom == ReadFrom.Output ? _cf.Enclose(f.FieldName()) : _cf.Enclose(f.Name)))} 
                    FROM {_schemaPrefix}{_cf.Enclose(_tableOrView)} {(_connection.Provider == "sqlserver" && _context.Entity.NoLock ? "WITH (NOLOCK)" : string.Empty)}
                    {_filter};";
                _context.Debug(() => cmd.CommandText);

                IDataReader reader;
                try {
                    reader = cmd.ExecuteReader(CommandBehavior.SequentialAccess);
                } catch (System.Data.Common.DbException e) {
                    _context.Error($"Error reading data from {_connection.Name}, {_tableOrView}.");
                    _context.Error(e.Message);
                    yield break;
                }

                while (reader.Read()) {
                    _rowCount++;
                    yield return _rowCreator.Create(reader, _fields);
                }
                _context.Info("{0} from {1}", _rowCount, _connection.Name);
            }
        }
    }
}