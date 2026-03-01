#region license
// Transformalize
// Configurable Extract, Transform, and Load
// Copyright 2013-2025 Dale Newman
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
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Transformalize.Context;
using Transformalize.Contracts;
using Transformalize.Providers.Ado.Ext;

namespace Transformalize.Providers.Ado {

   public class AdoAsyncOutputProvider : IOutputProviderAsync {

      private readonly OutputContext _context;
      private readonly IConnectionFactory _cf;
      private readonly IBatchReader _matcher;

      public AdoAsyncOutputProvider(OutputContext context, IConnectionFactory cf, IBatchReader matcher) {
         _context = context;
         _cf = cf;
         _matcher = matcher;
      }

      public async Task WriteAsync(IAsyncEnumerable<IRow> rows, CancellationToken cancellationToken = default) {

         var insertCmd = _context.SqlInsertIntoOutput(_cf);
         var updateCmd = _context.SqlUpdateOutput(_cf);
         var tflHashCode = _context.Entity.TflHashCode();
         var tflDeleted = _context.Entity.TflDeleted();
         var updateFields = _context.GetUpdateFields().ToArray();

         using (var cn = _cf.GetConnection()) {
            cn.Open();
            var trans = cn.BeginTransaction();
            try {
               await foreach (var batch in PartitionAsync(rows, _context.Entity.InsertSize, cancellationToken).ConfigureAwait(false)) {

                  var inserts = new List<IRow>(_context.Entity.InsertSize);
                  var updates = new List<IRow>(_context.Entity.InsertSize);

                  if (_context.Process.Mode == "init" || (_context.Entity.Insert && !_context.Entity.Update)) {
                     inserts.AddRange(batch);
                  } else {
                     var batchArray = batch.ToArray();
                     var oldRows = _matcher.Read(batchArray);
                     for (int i = 0, batchLength = batchArray.Length; i < batchLength; i++) {
                        var row = batchArray[i];
                        if (oldRows.Contains(i)) {
                           if (oldRows[i][tflDeleted].Equals(true) || !oldRows[i][tflHashCode].Equals(row[tflHashCode])) {
                              updates.Add(row);
                           }
                        } else {
                           inserts.Add(row);
                        }
                     }
                  }

                  if (inserts.Any()) {
                     var insertCount = Convert.ToUInt32(await cn.ExecuteAsync(
                        insertCmd,
                        inserts.Select(r => r.ToExpandoObject(_context.OutputFields)),
                        trans,
                        commandTimeout: 0,
                        CommandType.Text
                     ).ConfigureAwait(false));
                     _context.Entity.Inserts += insertCount;
                  }

                  if (updates.Any()) {
                     var updateCount = Convert.ToUInt32(await cn.ExecuteAsync(
                        updateCmd,
                        updates.Select(r => r.ToExpandoObject(updateFields)),
                        trans,
                        commandTimeout: 0,
                        CommandType.Text
                     ).ConfigureAwait(false));
                     _context.Entity.Updates += updateCount;
                  }
               }
               trans.Commit();
            } catch (Exception ex) {
               _context.Error(ex.Message);
               _context.Warn("Rolling back");
               trans.Rollback();
            }
         }

         if (_context.Entity.Inserts > 0) {
            _context.Info("{0} inserts into {1}", _context.Entity.Inserts, _context.Connection.Name);
         }
         if (_context.Entity.Updates > 0) {
            _context.Info("{0} updates to {1}", _context.Entity.Updates, _context.Connection.Name);
         }
      }

      public async Task<object> GetMaxVersionAsync(CancellationToken cancellationToken = default) {

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
            using (var cn = _cf.GetConnection()) {
               cn.Open();
               var result = await cn.ExecuteScalarAsync(sql, commandTimeout: _context.Connection.RequestTimeout).ConfigureAwait(false);
               return result == DBNull.Value ? null : result;
            }
         } catch (System.Data.Common.DbException ex) {
            _context.Error($"Error retrieving maximum version from {_context.Connection.Name}, {objectName}.");
            _context.Error(ex.Message);
            _context.Debug(() => ex.StackTrace);
            _context.Debug(() => sql);
            return null;
         }
      }

      public async Task<int> GetNextTflBatchIdAsync(CancellationToken cancellationToken = default) {
         var sql = _context.SqlControlLastBatchId(_cf);
         var result = 0;
         try {
            using (var cn = _cf.GetConnection()) {
               cn.Open();
               result = await cn.ExecuteScalarAsync<int>(sql).ConfigureAwait(false) + 1;
            }
         } catch (System.Data.Common.DbException e) {
            _context.Error(e.Message);
            _context.Debug(() => sql);
            _context.Debug(() => e.StackTrace);
         }
         return result;
      }

      public async Task<int> GetMaxTflKeyAsync(CancellationToken cancellationToken = default) {
         var tableName = _context.Entity.OutputTableName(_context.Process.Name);
         var sql = $"SELECT MAX({_cf.Enclose(_context.Entity.TflKey().FieldName())}) FROM {_cf.Enclose(tableName)};";
         var result = 0;
         try {
            using (var cn = _cf.GetConnection()) {
               cn.Open();
               result = await cn.ExecuteScalarAsync<int>(sql).ConfigureAwait(false);
            }
         } catch (System.Data.Common.DbException e) {
            _context.Error($"Error retrieving maximum surrogate key (TflKey) from {tableName}.");
            _context.Error(e.Message);
            _context.Debug(() => sql);
            _context.Debug(() => e.StackTrace);
         }
         return result;
      }

      public async Task EndAsync(CancellationToken cancellationToken = default) {
         if (_context.Process.Mode != "init" || _context.Entity == null)
            return;

         if (_context.Entity.Version != string.Empty) {
            try {
               using (var cn = _cf.GetConnection()) {
                  cn.Open();
                  var version = _context.Entity.GetVersionField();
                  await cn.ExecuteAsync(_context.SqlCreateVersionIndex(_cf, version), commandTimeout: _context.Connection.RequestTimeout).ConfigureAwait(false);
                  _context.Debug(() => $"Indexed version for {_context.Entity.Alias}.");

                  if (_context.Entity.IsMaster) {
                     await cn.ExecuteAsync(_context.SqlCreateBatchIndex(_cf), commandTimeout: _context.Connection.RequestTimeout).ConfigureAwait(false);
                     _context.Debug(() => $"Indexed batch for {_context.Entity.Alias}.");
                  }
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

      public Task InitializeAsync(CancellationToken cancellationToken = default) => throw new NotImplementedException();
      public Task StartAsync(CancellationToken cancellationToken = default) => throw new NotImplementedException();
      public Task DeleteAsync(CancellationToken cancellationToken = default) => throw new NotImplementedException();
      public IAsyncEnumerable<IRow> ReadKeysAsync(CancellationToken cancellationToken = default) => throw new NotImplementedException();
      public IAsyncEnumerable<IRow> MatchAsync(IAsyncEnumerable<IRow> rows, CancellationToken cancellationToken = default) => throw new NotImplementedException();

      public void Dispose() { }

      private static async IAsyncEnumerable<List<IRow>> PartitionAsync(
            IAsyncEnumerable<IRow> source,
            int size,
            [EnumeratorCancellation] CancellationToken cancellationToken) {
         var batch = new List<IRow>(size);
         await foreach (var row in source.WithCancellation(cancellationToken).ConfigureAwait(false)) {
            cancellationToken.ThrowIfCancellationRequested();
            batch.Add(row);
            if (batch.Count == size) {
               yield return batch;
               batch = new List<IRow>(size);
            }
         }
         if (batch.Count > 0) {
            yield return batch;
         }
      }
   }
}
