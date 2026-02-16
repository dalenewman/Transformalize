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

using Cfg.Net.Contracts;
using Dapper;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Dynamic;
using System.Linq;
using Transformalize.Contracts;
using Transformalize.Extensions;

namespace Transformalize.Providers.Ado.Actions {
    public class AdoRunAction : IAction {
        private readonly IContext _context;
        private readonly Configuration.Action _node;
        private readonly IConnectionFactory _cf;
        private readonly IReader _commandReader;

        public AdoRunAction(IContext context, Configuration.Action node, IConnectionFactory cf, IReader commandReader) {
            _context = context;
            _node = node;
            _cf = cf;
            _commandReader = commandReader;
        }

        public ActionResponse Execute() {
            var response = new ActionResponse { Action = _node };
            using (var cn = _cf.GetConnection()) {

                try {
                    cn.Open();
                } catch (DbException e) {
                    _context.Error($"Can't open {_cf.AdoProvider} connection specified in {_node.Type} action.");
                    response.Message = e.Message;
                    response.Code = 500;
                    return response;
                }

                try {

                    if (_node.Command == string.Empty) {
                        var logger = new Cfg.Net.Loggers.MemoryLogger();
                        _node.Command = _commandReader.Read(_node.Url == string.Empty ? _node.File : _node.Url, new Dictionary<string, string>(), logger);
                        foreach (var warning in logger.Warnings()) {
                            _context.Warn(warning);
                        }
                        foreach (var error in logger.Errors()) {
                            _context.Error(error);
                        }
                    }

                    if (_node.Command.Contains("@")) {
                        var parameters = new ExpandoObject();
                        var editor = (IDictionary<string, object>)parameters;
                       foreach (var parameter in _context.Process.Parameters) {
                            if (parameter.Name.Contains(".")) {
                                parameter.Name = parameter.Name.Replace(".", "_");
                            }
                        }
                        foreach (var name in new AdoParameterFinder().Find(_node.Command).Distinct().ToList()) {
                            var match = _context.Process.Parameters.FirstOrDefault(p => p.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
                            if (match != null) {
                                editor[match.Name] = match.Convert(match.Value);
                            }
                        }
                        _node.RowCount = cn.Execute(_node.Command, parameters, commandTimeout: _node.TimeOut);
                    } else {
                        _node.RowCount = cn.Execute(_node.Command, commandTimeout: _node.TimeOut);
                    }
                    var message = $"{(_node.Description == string.Empty ? _node.Type + " action" : "'" + _node.Description + "'")} affected {(_node.RowCount == -1 ? 0 : _node.RowCount)} row{_node.RowCount.Plural()}.";
                    response.Message = message;
                    _context.Info(message);
                } catch (DbException ex) {
                    response.Code = 500;
                    response.Message = ex.Message + Environment.NewLine + _node.Command.Replace("{", "{{").Replace("}", "}}");
                }
            }
            return response;
        }
    }
}
