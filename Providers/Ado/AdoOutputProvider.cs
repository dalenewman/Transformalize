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
using System.Collections.Generic;
using System.Data;
using Dapper;
using Transformalize.Context;
using Transformalize.Contracts;
using Transformalize.Providers.Ado.Ext;

namespace Transformalize.Providers.Ado {
    public class AdoOutputProvider : IOutputProvider {

        private readonly OutputContext _context;
        private readonly IConnectionFactory _cf;
        private readonly IDbConnection _cn;
        private readonly IWrite _writer;

        public AdoOutputProvider(OutputContext context, IConnectionFactory cf, IWrite writer) {
            _writer = writer;
            _context = context;
            _cf = cf;
            _cn = cf.GetConnection();
        }

        public void Delete() {
            throw new NotImplementedException();
        }

        public object GetMaxVersion() {

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
                if (_cn.State != ConnectionState.Open) {
                    _cn.Open();
                }
                return _cn.ExecuteScalar(sql, commandTimeout: _context.Connection.RequestTimeout);
            } catch (Exception ex) {
                _context.Error(ex, ex.Message + " " + sql);
                throw;
            }
        }

        public void End() {
            throw new NotImplementedException();
        }

        public int GetNextTflBatchId() {
            if (_cn.State != ConnectionState.Open) {
                _cn.Open();
            }
            var sql = _context.SqlControlLastBatchId(_cf);
            return _cn.ExecuteScalar<int>(sql) + 1;
        }

        public int GetMaxTflKey() {
            if (_cn.State != ConnectionState.Open) {
                _cn.Open();
            }
            return _cn.ExecuteScalar<int>($"SELECT MAX({_cf.Enclose(_context.Entity.TflKey().FieldName())}) FROM {_cf.Enclose(_context.Entity.OutputTableName(_context.Process.Name))};");
        }

        public void Initialize() {
            throw new NotImplementedException();
        }

        public IEnumerable<IRow> Match(IEnumerable<IRow> rows) {
            throw new NotImplementedException();
        }

        public IEnumerable<IRow> ReadKeys() {
            throw new NotImplementedException();
        }

        public void Start() {
            throw new NotImplementedException();
        }

        public void Write(IEnumerable<IRow> rows) {
            _writer.Write(rows);
        }

        public void Dispose() {
            if (_cn != null) {
                if (_cn.State != ConnectionState.Closed) {
                    _cn.Close();
                }
                _cn.Dispose();
            }
        }
    }
}