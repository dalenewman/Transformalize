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
using Transformalize.Context;
using Transformalize.Contracts;
using Transformalize.Extensions;

namespace Transformalize.Providers.Ado {
    public class AdoEntityWriter : IWrite {

        private readonly OutputContext _output;
        private readonly IBatchReader _matcher;
        private readonly IWrite _updater;
        private readonly IWrite _inserter;

        public AdoEntityWriter(OutputContext output, IBatchReader matcher, IWrite inserter, IWrite updater) {
            _output = output;
            _matcher = matcher;
            _inserter = inserter;
            _updater = updater;
        }

        public void Write(IEnumerable<IRow> rows) {
            var tflHashCode = _output.Entity.TflHashCode();
            var tflDeleted = _output.Entity.TflDeleted();

            foreach (var part in rows.Partition(_output.Entity.InsertSize)) {

                var inserts = new List<IRow>(_output.Entity.InsertSize);
                var updates = new List<IRow>(_output.Entity.InsertSize);
                var batchCount = (uint)0;

                if (_output.Process.Mode == "init" || (_output.Entity.Insert && !_output.Entity.Update)) {
                    foreach (var row in part) {
                        inserts.Add(row);
                        batchCount++;
                    }
                } else {
                    var newRows = part.ToArray();
                    var oldRows = _matcher.Read(newRows);
                    for (int i = 0, batchLength = newRows.Length; i < batchLength; i++) {
                        var row = newRows[i];
                        if (oldRows.Contains(i)) {
                            if (oldRows[i][tflDeleted].Equals(true) || !oldRows[i][tflHashCode].Equals(row[tflHashCode])) {
                                updates.Add(row);
                            }
                        } else {
                            inserts.Add(row);
                        }
                        batchCount++;
                    }
                }

                if (inserts.Any()) {
                    _inserter.Write(inserts);
                }

                if (updates.Any()) {
                    _updater.Write(updates);
                }

            }

            if (_output.Entity.Inserts > 0) {
                _output.Info("{0} inserts into {1}", _output.Entity.Inserts, _output.Connection.Name);
            }

            if (_output.Entity.Updates > 0) {
                _output.Info("{0} updates to {1}", _output.Entity.Updates, _output.Connection.Name);
            }

        }
    }
}
