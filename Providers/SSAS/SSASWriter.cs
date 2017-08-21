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
using Microsoft.AnalysisServices;
using Transformalize.Context;
using Transformalize.Contracts;

namespace Transformalize.Providers.SSAS {
    public class SSASWriter : IWrite {

        readonly InputContext _input;
        readonly OutputContext _output;
        public SSASWriter(InputContext input, OutputContext output) {
            _input = input;
            _output = output;
        }

        public void Write(IEnumerable<IRow> rows) {

            if (!_output.Entity.NeedsUpdate())
                return;

            var ids = new SSASIdentifiers(_input, _output);

            using (var server = new Server()) {
                server.Connect($"Data Source={_output.Connection.Server};Catalog=;");

                if (server.Databases.Contains(ids.DatabaseId)) {
                    var database = server.Databases.Find(ids.DatabaseId);
                    if (_output.Process.Mode == "init") {
                        _output.Info($"Processing OLAP database {ids.DatabaseId}");
                        SSAS.Process(database, ProcessType.ProcessFull, _output);
                    } else {
                        if (database.Dimensions.Contains(_output.Entity.Alias)) {
                            _output.Info($"Updating dimension {_output.Entity.Alias}");
                            var dimension = database.Dimensions.Find(_output.Entity.Alias);
                            SSAS.Process(dimension, ProcessType.ProcessUpdate, _output);
                        } else {
                            _output.Error($"{_output.Entity.Alias} dimension does not exist!");
                        }
                        if (database.Cubes.Contains(ids.CubeId)) {
                            var cube = database.Cubes.Find(ids.CubeId);
                            ProcessPartition(cube, ids.NormalMeasureGroupId);
                            ProcessPartition(cube, ids.DistinctMeasureGroupId);
                        } else {
                            _output.Error($"{ids.CubeId} cube does not exist!");
                        }
                    }
                } else {
                    _output.Error($"{ids.DatabaseId} OLAP database does not exist!");
                }
                server.Disconnect();
            }

        }

        public void ProcessPartition(Cube cube, string partitionId) {
            if (cube.MeasureGroups.Contains(partitionId)) {
                var measureGroup = cube.MeasureGroups.Find(partitionId);
                if (measureGroup.Partitions.Contains(partitionId)) {
                    var partition = measureGroup.Partitions.Find(partitionId);
                    _output.Info($"Updating partition {partitionId}");
                    if (SSAS.Process(partition, ProcessType.ProcessData, _output)) {
                        SSAS.Process(partition, ProcessType.ProcessIndexes, _output);
                    }
                } else {
                    _output.Error($"{partitionId} partition does not exist!");
                }
            } else {
                _output.Error($"{partitionId} measure group does not exist!");
            }
        }
    }
}
