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

using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
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
         string objectName;

         switch (_cf.AdoProvider) {
            case AdoProvider.SqlCe:
               objectName = _cf.Enclose(_context.Entity.OutputTableName(_context.Process.Name));
               break;
            default:
               objectName = _cf.Enclose(_context.Entity.OutputViewName(_context.Process.Name));
               break;
         }

         switch (_cf.AdoProvider) {
            case AdoProvider.PostgreSql:
               sql = _context.Entity.Delete ? $"SELECT {_cf.Enclose(version.Alias)} FROM {objectName} WHERE {_cf.Enclose(Constants.TflDeleted)} = false ORDER BY {_cf.Enclose(version.Alias)} DESC LIMIT 1;" : $"SELECT {_cf.Enclose(version.Alias)} FROM {objectName} ORDER BY {_cf.Enclose(version.Alias)} DESC LIMIT 1;";
               break;
            case AdoProvider.SqlCe:
               sql = _context.Entity.Delete ? $"SELECT MAX({_cf.Enclose(version.FieldName())}) FROM {objectName} WHERE {_cf.Enclose(_context.Entity.TflDeleted().FieldName())} = 0;" : $"SELECT MAX({_cf.Enclose(version.FieldName())}) FROM {objectName};";
               break;
            default:
               sql = _context.Entity.Delete ? $"SELECT MAX({_cf.Enclose(version.Alias)}) FROM {objectName} WHERE {_cf.Enclose(Constants.TflDeleted)} = 0;" : $"SELECT MAX({_cf.Enclose(version.Alias)}) FROM {objectName};";
               break;
         }

         _context.Debug(() => $"Loading Output Version: {sql}");

         try {
            if (_cn.State != ConnectionState.Open) {
               _cn.Open();
            }
            var result = _cn.ExecuteScalar(sql, commandTimeout: _context.Connection.RequestTimeout);
            return result == DBNull.Value ? null : result;
         } catch (System.Data.Common.DbException ex) {
            _context.Error($"Error retrieving maximum version from {_context.Connection.Name}, {objectName}.");
            _context.Error(ex.Message);
            _context.Debug(() => ex.StackTrace);
            _context.Debug(() => sql);
            return null;
         }
      }

      public void End() {
         if (_context.Process.Mode != "init" || _context.Entity == null)
            return;

         if (_context.Entity.Version != string.Empty) {

            try {
               if (_cn.State != ConnectionState.Open) {
                  _cn.Open();
               }

               var version = _context.Entity.GetVersionField();
               _cn.Execute(_context.SqlCreateVersionIndex(_cf, version), commandTimeout: _context.Connection.RequestTimeout);
               _context.Debug(() => $"Indexed version for {_context.Entity.Alias}.");
               _context.Debug(() => _context.SqlCreateVersionIndex(_cf, version));

               if (_context.Entity.IsMaster) {
                  _cn.Execute(_context.SqlCreateBatchIndex(_cf), commandTimeout: _context.Connection.RequestTimeout);
                  _context.Debug(() => $"Indexed batch for {_context.Entity.Alias}.");
                  _context.Debug(() => _context.SqlCreateBatchIndex(_cf));
               }

            } catch (System.Data.Common.DbException ex) {
               _context.Warn($"Error creating index: {ex.Message}");
               _context.Debug(() => ex.StackTrace);

            }
         }

         if (_context.Entity.RelationshipToMaster.Any()) {
            var response = new AdoEntityDepthChecker(_context, _cf).Execute();
            if (response.Code != 200) {
               _context.Warn(response.Message);
            }
         }

      }

      public int GetNextTflBatchId() {
         if (_cn.State != ConnectionState.Open) {
            _cn.Open();
         }
         var sql = _context.SqlControlLastBatchId(_cf);

         var result = 0;
         try {
            result = _cn.ExecuteScalar<int>(sql) + 1;
         } catch (System.Data.Common.DbException e) {
            _context.Error(e.Message);
            _context.Debug(() => sql);
            _context.Debug(() => e.StackTrace);
         }

         return result;

      }

      public int GetMaxTflKey() {
         if (_cn.State != ConnectionState.Open) {
            _cn.Open();
         }

         var tableName = _context.Entity.OutputTableName(_context.Process.Name);
         var sql = $"SELECT MAX({_cf.Enclose(_context.Entity.TflKey().FieldName())}) FROM {_cf.Enclose(tableName)};";
         var result = 0;
         try {
            result = _cn.ExecuteScalar<int>(sql);
         } catch (System.Data.Common.DbException e) {
            _context.Error($"Error retrieving maximum surrogate key (TflKey) from {tableName}.");
            _context.Error(e.Message);
            _context.Debug(() => sql);
            _context.Debug(() => e.StackTrace);
         }

         return result;
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