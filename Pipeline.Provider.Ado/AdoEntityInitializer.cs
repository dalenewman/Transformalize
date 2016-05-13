#region license
// Transformalize
// Copyright 2013 Dale Newman
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//  
//      http://www.apache.org/licenses/LICENSE-2.0
//  
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
#endregion
using System.Data;
using Dapper;
using Pipeline.Context;
using Pipeline.Contracts;
using Pipeline.Provider.Ado.Ext;

namespace Pipeline.Provider.Ado {

    public class AdoEntityInitializer : IAction {

        readonly OutputContext _context;
        readonly IConnectionFactory _cf;

        public AdoEntityInitializer(OutputContext context, IConnectionFactory cf) {
            _context = context;
            _cf = cf;
        }

        void Destroy(IDbConnection cn) {

            _context.Warn("Initializing");

            if (!_context.Connection.DropControl) {
                try {
                    cn.Execute(_context.SqlDeleteEntityFromControl(_cf), new { Entity = _context.Entity.Alias });
                } catch (System.Data.Common.DbException ex) {
                    _context.Debug(() => ex.Message);
                }
            }

            try {
                cn.Execute(_context.SqlDropOutputView(_cf));
            } catch (System.Data.Common.DbException ex) {
                _context.Debug(() => ex.Message);
            }

            try {
                cn.Execute(_context.SqlDropOutputViewAsTable(_cf));
            } catch (System.Data.Common.DbException ex) {
                _context.Debug(() => ex.Message);
            }

            try {
                cn.Execute(_context.SqlDropOutput(_cf));
            } catch (System.Data.Common.DbException ex) {
                _context.Debug(() => ex.Message);
            }
        }

        void Create(IDbConnection cn) {
            var sql = _context.SqlCreateOutput(_cf);
            cn.Execute(sql);
            cn.Execute(_context.SqlCreateOutputUniqueIndex(_cf));
            cn.Execute(_context.SqlCreateOutputView(_cf));
        }

        public ActionResponse Execute() {
            using (var cn = _cf.GetConnection()) {
                cn.Open();
                Destroy(cn);
                Create(cn);
            }
            return new ActionResponse();
        }
    }
}
