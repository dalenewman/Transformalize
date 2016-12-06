#region license
// Transformalize
// Configurable Extract, Transform, and Load
// Copyright 2013-2016 Dale Newman
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

using System;
using System.Linq;
using Dapper;
using Transformalize.Context;
using Transformalize.Contracts;
using Transformalize.Provider.Ado.Ext;

namespace Transformalize.Provider.Ado {
    public class AdoInputVersionDetector : IInputVersionDetector {
        private readonly InputContext _context;
        private readonly IConnectionFactory _cf;

        public AdoInputVersionDetector(InputContext context, IConnectionFactory connectionFactory) {
            _context = context;
            _cf = connectionFactory;
        }

        public object Detect() {

            if (string.IsNullOrEmpty(_context.Entity.Version))
                return null;

            var version = _context.Entity.GetVersionField();

            var schema = _context.Entity.Schema == string.Empty ? string.Empty : _cf.Enclose(_context.Entity.Schema) + ".";

            var filter = string.Empty;
            if (_context.Entity.Filter.Any()) {
                filter = _context.ResolveFilter(_cf);
            }

            string sql;
            if (_context.Connection.Provider == "sqlserver" && version.Type == "byte[]" && version.Length == "8") {
                sql = $"SELECT MAX({_cf.Enclose(version.Name)}) FROM {schema}{_cf.Enclose(_context.Entity.Name)} WHERE {_cf.Enclose(version.Name)} < MIN_ACTIVE_ROWVERSION() {(filter == string.Empty ? string.Empty : " AND " + filter)}";
            } else {
                sql = $"SELECT MAX({_cf.Enclose(version.Name)}) FROM {schema}{_cf.Enclose(_context.Entity.Name)} {(filter == string.Empty ? string.Empty : " WHERE " + filter)}";
            }

            _context.Debug(()=>$"Loading Input Version: {sql}");

            try {
                using (var cn = _cf.GetConnection()) {
                    cn.Open();
                    return cn.ExecuteScalar(sql);
                }
            } catch (Exception ex) {
                _context.Error(ex, ex.Message + " " + sql);
                throw;
            }
        }
    }
}