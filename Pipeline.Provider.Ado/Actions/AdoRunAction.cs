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
using System;
using Dapper;
using Pipeline.Configuration;
using Pipeline.Contracts;
using Action = Pipeline.Configuration.Action;

namespace Pipeline.Provider.Ado.Actions {
    public class AdoRunAction : IAction {
        private readonly Action _node;
        private readonly IConnectionFactory _cf;

        public AdoRunAction(Action node, IConnectionFactory cf) {
            _node = node;
            _cf = cf;
        }

        public ActionResponse Execute() {
            var response = new ActionResponse();
            using (var cn = _cf.GetConnection()) {
                cn.Open();
                try {
                    response.Content = $"{cn.Execute(_node.Command,commandTimeout:_node.TimeOut)} rows affected.";
                } catch (Exception ex) {
                    response.Code = 500;
                    response.Content = ex.Message + " " + ex.StackTrace + " " + _node.Command;
                }
            }
            return response;
        }
    }
}
