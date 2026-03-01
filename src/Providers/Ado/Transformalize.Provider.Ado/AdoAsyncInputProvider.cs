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

using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Transformalize.Configuration;
using Transformalize.Context;
using Transformalize.Contracts;
using Transformalize.Providers.Ado.Ext;

namespace Transformalize.Providers.Ado {

   public class AdoAsyncInputProvider : IInputProviderAsync {

      private int _rowCount;
      private readonly InputContext _input;
      private readonly AdoRowCreator _rowCreator;
      private readonly Field[] _fields;
      private readonly IConnectionFactory _factory;

      public AdoAsyncInputProvider(InputContext input, Field[] fields, IConnectionFactory factory, IRowFactory rowFactory) {
         _input = input;
         _fields = fields;
         _factory = factory;
         _rowCreator = new AdoRowCreator(input, rowFactory);
      }

      public async IAsyncEnumerable<IRow> ReadAsync([EnumeratorCancellation] CancellationToken cancellationToken = default) {

         var cn = (DbConnection)_factory.GetConnection(Constants.ApplicationName);

         try {
            await cn.OpenAsync(cancellationToken).ConfigureAwait(false);
         } catch (DbException e) {
            _input.Error($"Can't open {_input.Connection} for reading.");
            _input.Error(e.Message);
            cn.Dispose();
            yield break;
         }

         using (cn) {

            var cmd = (DbCommand)cn.CreateCommand();

            if (string.IsNullOrEmpty(_input.Entity.Query)) {

               if (_input.Entity.MinVersion == null) {
                  cmd.CommandText = _input.SqlSelectInput(_fields, _factory);
                  _input.Debug(() => cmd.CommandText);
               } else {
                  cmd.CommandText = _input.SqlSelectInputWithMinVersion(_fields, _factory);
                  _input.Debug(() => cmd.CommandText);

                  var parameter = cmd.CreateParameter();
                  parameter.ParameterName = "@MinVersion";
                  parameter.Direction = ParameterDirection.Input;
                  parameter.Value = _input.Entity.MinVersion;
                  cmd.Parameters.Add(parameter);
               }

               if (_input.Entity.IsPageRequest()) {
                  var countCmd = (DbCommand)cn.CreateCommand();
                  var filter = _input.ResolveFilter(_factory);
                  countCmd.CommandText = $"SELECT COUNT(*) FROM {_input.SqlInputName(_factory)} {(_factory.AdoProvider == AdoProvider.SqlServer ? "WITH (NOLOCK)" : string.Empty)} {(filter == string.Empty ? string.Empty : " WHERE " + filter)}";
                  _input.Debug(() => countCmd.CommandText);
                  AddAdoParameters(countCmd);
                  try {
                     var countResult = await countCmd.ExecuteScalarAsync(cancellationToken).ConfigureAwait(false);
                     _input.Entity.Hits = Convert.ToInt32(countResult);
                  } catch (DbException ex) {
                     _input.Error($"Error counting {_input.Entity.Name} records.");
                     _input.Error(ex.Message);
                  }
               }

               _input.Entity.Query = cmd.CommandText;

            } else {
               if (_input.Entity.Query.Length <= 128 && _input.Process.Scripts.Any(s => s.Name == _input.Entity.Query)) {
                  var script = _input.Process.Scripts.First(s => s.Name == _input.Entity.Query);
                  if (script.Content != string.Empty) {
                     _input.Entity.Query = script.Content;
                  }
               }
               cmd.CommandText = _input.Entity.Query;
            }

            // automatic facet filter maps need connections and queries and map readers
            foreach (var filter in _input.Entity.Filter.Where(f => f.Type == "facet" && f.Map != string.Empty)) {
               var map = _input.Process.Maps.First(m => m.Name == filter.Map);
               if (!map.Items.Any() && map.Query == string.Empty) {
                  map.Connection = _input.Connection.Name;
                  map.Query = _input.SqlSelectFacetFromInput(filter, _factory);
                  foreach (var mapItem in new AdoMapReader(_input, cn, map.Name).Read(_input)) {
                     if (mapItem.To != null) {
                        var value = mapItem.To.ToString();
                        if (value.Contains("'")) {
                           mapItem.To = value.Replace("'", "''");
                        }
                     }
                     map.Items.Add(mapItem);
                  }
               }
            }

            AddAdoParameters(cmd);
            cmd.CommandType = CommandType.Text;
            cmd.CommandTimeout = _input.Connection.RequestTimeout;

            DbDataReader reader;

            try {
               reader = await cmd.ExecuteReaderAsync(CommandBehavior.SequentialAccess, cancellationToken).ConfigureAwait(false);
            } catch (DbException ex) {
               _input.Error(ex.Message);
               yield break;
            }

            using (reader) {
               if (_fields.Length < reader.FieldCount) {
                  _input.Warn($"The reader is returning {reader.FieldCount} fields, but the entity {_input.Entity.Alias} expects {_fields.Length}!");
               }

               while (await reader.ReadAsync(cancellationToken).ConfigureAwait(false)) {
                  cancellationToken.ThrowIfCancellationRequested();
                  _rowCount++;
                  yield return _rowCreator.Create(reader, _fields);
               }
            }
         }

         _input.Info("{0} from {1}", _rowCount, _input.Connection.Name);
      }

      public async Task<object> GetMaxVersionAsync(CancellationToken cancellationToken = default) {

         if (string.IsNullOrEmpty(_input.Entity.Version))
            return null;

         var version = _input.Entity.GetVersionField();
         var schema = _input.Entity.Schema == string.Empty ? string.Empty : _factory.Enclose(_input.Entity.Schema) + ".";

         var filter = string.Empty;
         if (_input.Entity.Filter.Any()) {
            filter = _input.ResolveFilter(_factory);
         }

         string sql;
         var versionName = _input.Connection.Provider == "postgresql" && version.Name == "xmin" ? "xmin::text" : _factory.Enclose(version.Name);
         if (_input.Connection.Provider == "sqlserver" && version.Type == "byte[]" && version.Length == "8") {
            sql = $"SELECT MAX({versionName}) FROM {schema}{_factory.Enclose(_input.Entity.Name)} WHERE {versionName} < MIN_ACTIVE_ROWVERSION() {(filter == string.Empty ? string.Empty : " AND " + filter)}";
         } else {
            sql = $"SELECT MAX({versionName}) FROM {schema}{_factory.Enclose(_input.Entity.Name)} {(filter == string.Empty ? string.Empty : " WHERE " + filter)}";
         }

         _input.Debug(() => $"Loading Input Version: {sql}");

         try {
            using (var cn = (DbConnection)_factory.GetConnection(Constants.ApplicationName)) {
               await cn.OpenAsync(cancellationToken).ConfigureAwait(false);

               var cmd = (DbCommand)cn.CreateCommand();
               cmd.CommandText = sql;
               cmd.CommandType = CommandType.Text;
               cmd.CommandTimeout = _input.Connection.RequestTimeout;

               if (cmd.CommandText.Contains("@")) {
                  var active = _input.Process.Parameters;
                  foreach (var name in new AdoParameterFinder().Find(cmd.CommandText).Distinct().ToList()) {
                     var match = active.FirstOrDefault(p => p.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
                     if (match != null) {
                        var parameter = cmd.CreateParameter();
                        parameter.ParameterName = match.Name;
                        parameter.Value = match.Convert(match.Value);
                        cmd.Parameters.Add(parameter);
                     }
                  }
               }

               var result = await cmd.ExecuteScalarAsync(cancellationToken).ConfigureAwait(false);
               return result == DBNull.Value ? null : result;
            }
         } catch (DbException ex) {
            _input.Error($"Error retrieving max version from {_input.Connection.Name}, {_input.Entity.Alias}.");
            _input.Error(ex.Message);
            _input.Debug(() => ex.StackTrace);
            _input.Debug(() => sql);
            return null;
         }
      }

      private void AddAdoParameters(DbCommand cmd) {
         if (cmd.CommandText.Contains("@")) {
            var active = _input.Process.Parameters;
            foreach (var name in new AdoParameterFinder().Find(cmd.CommandText).Distinct().ToList()) {
               var match = active.FirstOrDefault(p => p.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
               if (match != null) {
                  var parameter = cmd.CreateParameter();
                  parameter.ParameterName = match.Name;
                  parameter.Value = match.Convert(match.Value);
                  cmd.Parameters.Add(parameter);
               }
            }
         }
      }
   }
}
