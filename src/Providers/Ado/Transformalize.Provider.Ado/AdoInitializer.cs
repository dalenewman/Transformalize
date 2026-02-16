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
using System.Data;
using System.Data.Common;
using Transformalize.Actions;
using Transformalize.Context;
using Transformalize.Contracts;
using Transformalize.Providers.Ado.Ext;

namespace Transformalize.Providers.Ado {
   public class AdoInitializer : IInitializer {
      private readonly OutputContext _context;
      private readonly IConnectionFactory _cf;

      public AdoInitializer(OutputContext context, IConnectionFactory cf) {
         _context = context;
         _cf = cf;
      }

      private void Destroy(IDbConnection cn) {
         try {
            if (!_context.Connection.DropControl)
               return;

            var sql = _context.SqlDropControl(_cf);
            cn.Execute(sql);
         } catch (DbException ex) {
            _context.Debug(() => ex.Message);
         }
      }

      private void Create(IDbConnection cn) {
         try {
            var sql = _context.SqlCreateControl(_cf);
            cn.Execute(sql);
         } catch (DbException ex) {
            _context.Debug(() => ex.Message);
         }
      }

      public ActionResponse Execute() {
         using (var cn = _cf.GetConnection()) {
            try {
               cn.Open();
            } catch (DbException ex) {
               _context.Error($"Couldn't open {_context.Connection}.");
               _context.Error(ex.Message);
               return new ActionResponse(500, ex.Message) { Action = new Configuration.Action { Type = "internal", ErrorMode = "abort" } };
            }

            Destroy(cn);
            Create(cn);
         }
         return new ActionResponse();
      }
   }

}
