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

using System.Data.Common;
using Dapper;
using Transformalize.Actions;
using Transformalize.Context;
using Transformalize.Contracts;
using Transformalize.Providers.Ado.Ext;

namespace Transformalize.Providers.Ado {
    public class AdoFlatTableCreator : IAction {
        private readonly OutputContext _output;
        private readonly IConnectionFactory _cf;

        public AdoFlatTableCreator(OutputContext output, IConnectionFactory cf) {
            _output = output;
            _cf = cf;
        }

        public ActionResponse Execute() {
            var drop = _output.SqlDropFlatTable(_cf);
            var create = _output.SqlCreateFlatTable(_cf);
            var createIndex = _output.SqlCreateFlatIndex(_cf);

            using (var cn = _cf.GetConnection()) {
                cn.Open();
                try {
                    cn.Execute(drop);
                } catch (DbException ex) {
                    _output.Debug(() => ex.Message);
                }
                cn.Execute(create);
                cn.Execute(createIndex);
            }
            return new ActionResponse();
        }
    }
}