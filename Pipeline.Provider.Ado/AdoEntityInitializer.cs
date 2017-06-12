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
using System.Data;
using Dapper;
using Transformalize.Actions;
using Transformalize.Context;
using Transformalize.Contracts;
using Transformalize.Provider.Ado.Ext;

namespace Transformalize.Provider.Ado {

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
                _context.Error($"Could not drop output view {_context.Entity.OutputViewName(_context.Process.Name)}");
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
                _context.Error($"Could not drop output {_context.Entity.OutputTableName(_context.Process.Name)}");
                _context.Debug(() => ex.Message);
            }
        }

        void Create(IDbConnection cn) {
            var createSql = _context.SqlCreateOutput(_cf);
            try {
                cn.Execute(createSql);
            } catch (System.Data.Common.DbException ex) {
                _context.Error($"Could not create output {_context.Entity.OutputTableName(_context.Process.Name)}");
                _context.Error(ex, ex.Message);
            }

            try {
                var createIndex = _context.SqlCreateOutputUniqueIndex(_cf);
                cn.Execute(createIndex);
            } catch (System.Data.Common.DbException ex) {
                _context.Error($"Could not create unique index on output {_context.Entity.OutputTableName(_context.Process.Name)}");
                _context.Error(ex, ex.Message);
            }

            if (_cf.AdoProvider == AdoProvider.SqlCe)
                return;

            try {
                var createView = _context.SqlCreateOutputView(_cf);
                cn.Execute(createView);
            } catch (System.Data.Common.DbException ex) {
                _context.Error($"Could not create output view {_context.Entity.OutputViewName(_context.Process.Name)}");
                _context.Error(ex, ex.Message);
            }
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
