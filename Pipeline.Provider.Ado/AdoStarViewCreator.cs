#region license
// Transformalize
// A Configurable ETL solution specializing in incremental denormalization.
// Copyright 2013 Dale Newman
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
using Pipeline.Context;
using Pipeline.Contracts;
using Pipeline.Provider.Ado.Ext;

namespace Pipeline.Provider.Ado {
    public class AdoStarViewCreator : IAction {
        private readonly OutputContext _output;
        private readonly IConnectionFactory _cf;

        public AdoStarViewCreator(OutputContext output, IConnectionFactory cf) {
            _output = output;
            _cf = cf;
        }

        public ActionResponse Execute() {
            using (var cn = _cf.GetConnection()) {
                cn.Open();
                try {
                    cn.Execute(_output.SqlDropStarView(_cf));
                } catch (DbException) {
                }
                cn.Execute(_output.SqlCreateStarView(_cf));
            }
            return new ActionResponse();
        }
    }
}