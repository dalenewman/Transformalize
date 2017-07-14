using System.Collections.Generic;
using Transformalize.Contracts;
using Microsoft.AnalysisServices;
using Transformalize.Context;

namespace Transformalize.Provider.SSAS {
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
                            var versionField = _output.Entity.GetVersionField();
                            if (versionField == null) {
                                dimension.Process(ProcessType.ProcessUpdate, WriteBackTableCreation.UseExisting);
                            } else {
                                SSAS.Process(dimension, ProcessType.ProcessUpdate, _output);
                            }
                        } else {
                            _output.Error($"{_output.Entity.Alias} dimension does not exist!");
                        }
                        if (database.Cubes.Contains(ids.CubeId)) {
                            var cube = database.Cubes.Find(ids.CubeId);
                            if (cube.MeasureGroups.Contains(_output.Entity.Alias)) {
                                var measureGroup = cube.MeasureGroups.Find(_output.Entity.Alias);
                                if (measureGroup.Partitions.Contains(_output.Entity.Alias)) {
                                    var partition = measureGroup.Partitions.Find(_output.Entity.Alias);
                                    _output.Info($"Updating partition {_output.Entity.Alias}");
                                    if(SSAS.Process(partition, ProcessType.ProcessData, _output)) {
                                        SSAS.Process(partition, ProcessType.ProcessIndexes, _output);
                                    }
                                } else {
                                    _output.Error($"{_output.Entity.Alias} partition does not exist!");
                                }
                            } else {
                                _output.Error($"{_output.Entity.Alias} measure group does not exist!");
                            }
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
    }
}
