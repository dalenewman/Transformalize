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
using System;
using Dapper;
using Transformalize.Context;
using Transformalize.Contracts;

namespace Transformalize.Provider.Ado {
    public class AdoOutputVersionDetector : IVersionDetector {
        private readonly OutputContext _context;
        private readonly IConnectionFactory _cf;

        public AdoOutputVersionDetector(OutputContext context, IConnectionFactory cf) {
            _context = context;
            _cf = cf;
        }

        public object Detect() {

            if (string.IsNullOrEmpty(_context.Entity.Version))
                return null;

            var version = _context.Entity.GetVersionField();
            string sql;

            switch (_cf.AdoProvider) {
                case AdoProvider.PostgreSql:
                    sql = $"SELECT {_cf.Enclose(version.Alias)} FROM {_cf.Enclose(_context.Entity.OutputViewName(_context.Process.Name))} WHERE {_cf.Enclose(Constants.TflDeleted)} = false ORDER BY {_cf.Enclose(version.Alias)} DESC LIMIT 1;";
                    break;
                case AdoProvider.SqlCe:
                    sql = $"SELECT MAX({_cf.Enclose(version.FieldName())}) FROM {_cf.Enclose(_context.Entity.OutputTableName(_context.Process.Name))} WHERE {_cf.Enclose(_context.Entity.TflDeleted().FieldName())} = 0;";
                    break;
                default:
                    sql = $"SELECT MAX({_cf.Enclose(version.Alias)}) FROM {_cf.Enclose(_context.Entity.OutputViewName(_context.Process.Name))} WHERE {_cf.Enclose(Constants.TflDeleted)} = 0;";
                    break;
            }

            _context.Debug(() => $"Loading Output Version: {sql}");

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