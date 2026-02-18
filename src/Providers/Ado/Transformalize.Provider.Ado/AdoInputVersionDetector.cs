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
using System.Linq;
using Transformalize.Configuration;
using Transformalize.Context;
using Transformalize.Contracts;
using Transformalize.Providers.Ado.Ext;

namespace Transformalize.Providers.Ado {
   public class AdoInputProvider : IInputProvider {
      private readonly InputContext _context;
      private readonly IConnectionFactory _cf;

      public AdoInputProvider(InputContext context, IConnectionFactory connectionFactory) {
         _context = context;
         _cf = connectionFactory;
      }

      public object GetMaxVersion() {

         if (string.IsNullOrEmpty(_context.Entity.Version))
            return null;

         var version = _context.Entity.GetVersionField();

         var schema = _context.Entity.Schema == string.Empty ? string.Empty : _cf.Enclose(_context.Entity.Schema) + ".";

         var filter = string.Empty;
         if (_context.Entity.Filter.Any()) {
            filter = _context.ResolveFilter(_cf);
         }

         string sql;
         var versionName = _context.Connection.Provider == "postgresql" && version.Name == "xmin" ? "xmin::text" : _cf.Enclose(version.Name);
         if (_context.Connection.Provider == "sqlserver" && version.Type == "byte[]" && version.Length == "8") {
            sql = $"SELECT MAX({versionName}) FROM {schema}{_cf.Enclose(_context.Entity.Name)} WHERE {versionName} < MIN_ACTIVE_ROWVERSION() {(filter == string.Empty ? string.Empty : " AND " + filter)}";
         } else {
            sql = $"SELECT MAX({versionName}) FROM {schema}{_cf.Enclose(_context.Entity.Name)} {(filter == string.Empty ? string.Empty : " WHERE " + filter)}";
         }

         _context.Debug(() => $"Loading Input Version: {sql}");

         try {
            using (var cn = _cf.GetConnection()) {
               cn.Open();

               var cmd = cn.CreateCommand();
               cmd.CommandText = sql;
               cmd.CommandType = CommandType.Text;
               cmd.CommandTimeout = _context.Connection.RequestTimeout;

               // handle ado parameters
               if (cmd.CommandText.Contains("@")) {
                  var active = _context.Process.Parameters;
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

               var result = cmd.ExecuteScalar();
               return result == DBNull.Value ? null : result;
            }
         } catch (System.Data.Common.DbException ex) {
            _context.Error($"Error retrieving max version from {_context.Connection.Name}, {_context.Entity.Alias}.");
            _context.Error(ex.Message);
            _context.Debug(() => ex.StackTrace);
            _context.Debug(() => sql);
            return null;
         }
      }

      public Schema GetSchema(Entity entity = null) {
         throw new NotImplementedException();
      }

      public IEnumerable<IRow> Read() {
         throw new NotImplementedException();
      }
   }
}