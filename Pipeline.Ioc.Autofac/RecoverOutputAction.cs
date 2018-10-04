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
using System.Collections.Generic;
using System.Linq;
using Autofac;
using Transformalize.Actions;
using Transformalize.Configuration;
using Transformalize.Contracts;

namespace Transformalize.Ioc.Autofac {
    public class RecoverOutputAction : IAction {
        private readonly IContext _context;

        public RecoverOutputAction(IContext context) {
            _context = context;
        }

        public ActionResponse Execute() {

            var process = _context.Process;
            var originalOutput = process.Connections.First(c => c.Name == Constants.OriginalOutput);
            process.Connections.Remove(originalOutput);
            originalOutput.Name = "output";

            var threshold = process.Entities.Min(e => e.BatchId) - 1;
            var readFirstTable = process.InternalProvider == "sqlce" && process.Entities.Count == 1;
            var firstEntity = process.Entities.First();
            var firstTable = firstEntity.OutputTableName(process.Name);
            var counter = (short)0;

            var reversed = new Process {
                Name = process.Name,
                ReadOnly = true,
                Connections = new List<Connection>(2){
                    new Connection { Name = "input", Provider = process.InternalProvider, File = process.Output().File},
                    originalOutput
                },
                Entities = new List<Entity>(1) {
                    new Entity {
                        Name = process.InternalProvider == "sqlce" ? (readFirstTable ? firstTable : process.Name + process.FlatSuffix) : process.Name + process.StarSuffix,
                        Connection = "input",
                        Fields = process.GetStarFields().SelectMany(f => f).Select(field => new Field {
                            Index = counter++,
                            MasterIndex = field.Index,
                            Name = readFirstTable ? field.FieldName() : field.Alias,
                            Alias = field.Alias,
                            Type = field.Type,
                            Input = true,
                            PrimaryKey = field.Name == Constants.TflKey
                        }).ToList(),
                        Filter = new List<Filter> {
                            new Filter {
                                Field = readFirstTable ? firstEntity.TflBatchId().FieldName() : Constants.TflBatchId,
                                Operator = "greaterthan",
                                Value = threshold.ToString()
                            }
                        }
                    }
                }
            };

            reversed.Check();

            if (reversed.Errors().Any()) {
                foreach (var error in reversed.Errors()) {
                    _context.Error(error);
                }
                return new ActionResponse(500, "See error log");
            }

            using (var scope = DefaultContainer.Create(reversed, _context.Logger, "@[]")) {
                scope.Resolve<IProcessController>().Execute();
                if (originalOutput.Provider == "internal") {
                    process.Rows = reversed.Entities.First().Rows;
                }
            }

            return new ActionResponse(200, string.Empty);
        }
    }
}
