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
using System.Data.Common;
using Dapper;
using Transformalize.Context;
using Transformalize.Contracts;
using Transformalize.Provider.Ado.Ext;
using Transformalize.Actions;

namespace Transformalize.Provider.Ado {
    public class AdoInitializer : IInitializer {
        readonly OutputContext _context;
        private readonly IConnectionFactory _cf;

        public AdoInitializer(OutputContext context, IConnectionFactory cf) {
            _context = context;
            _cf = cf;
        }

        void Destroy(IDbConnection cn) {
            try {
                if (_context.Connection.DropControl) {
                    cn.Execute(_context.SqlDropControl(_cf));
                }
            } catch (DbException ex) {
                _context.Debug(() => ex.Message);
            }
        }

        void Create(IDbConnection cn) {
            try {
                var sql = _context.SqlCreateControl(_cf);
                cn.Execute(sql);
            } catch (DbException ex) {
                _context.Debug(() => ex.Message);
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
