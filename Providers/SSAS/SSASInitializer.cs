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
using Humanizer;
using Microsoft.AnalysisServices;
using System.Data;
using System.Data.OleDb;
using System.Data.SqlClient;
using System.Linq;
using Transformalize.Actions;
using Transformalize.Context;
using Transformalize.Contracts;
using Transformalize.Provider.Ado;
using System;
using System.Collections.Generic;
using Transformalize.Configuration;
using Transformalize.Provider.Ado.Ext;

namespace Transformalize.Provider.SSAS {

    public class SSASInitializer : IInitializer {

        readonly InputContext _input;
        readonly OutputContext _output;
        readonly Microsoft.AnalysisServices.Server _server;
        readonly IConnectionFactory _connectionFactory;

        public SSASInitializer(InputContext input, OutputContext output, IConnectionFactory connectionFactory) {
            _input = input;
            _output = output;
            _server = new Microsoft.AnalysisServices.Server();
            _connectionFactory = connectionFactory;
        }

        public ActionResponse Execute() {

            var ids = new SSASIdentifiers(_input, _output);

            using (_server) {
                Database database;
                _server.Connect($"Data Source={_output.Connection.Server};Catalog=;");

                if (_server.Databases.Contains(ids.DatabaseId)) {
                    database = _server.Databases.Find(ids.DatabaseId);
                    _output.Info($"Updating existing OLAP database: {ids.DatabaseId}");
                } else {
                    database = new Database() {
                        Name = ids.DatabaseId,
                        ID = ids.DatabaseId,
                        DataSourceImpersonationInfo = new ImpersonationInfo { ImpersonationMode = ImpersonationMode.Default }
                    };
                    _server.Databases.Add(database);
                    _output.Info($"Creating new OLAP database: {ids.DatabaseId}");
                    if (!SSAS.Save(_server, _output, database)) {
                        return new ActionResponse(500, $"Could not save OLAP database: {ids.DatabaseId}");
                    }
                }

                DataSource dataSource;
                if (database.DataSources.Contains(ids.DataSourceId)) {
                    dataSource = database.DataSources.Find(ids.DataSourceId);
                    _output.Info($"Updating existing data source: {ids.DataSourceId}");
                } else {
                    dataSource = database.DataSources.AddNew(ids.DataSourceId);
                    _output.Info($"Creating new data source: {ids.DataSourceId}");
                }

                dataSource.ConnectionString = $"Provider=SQLOLEDB.1;Data Source={_input.Connection.Server};Integrated Security=SSPI;Initial Catalog={_input.Connection.Database}";
                dataSource.ImpersonationInfo = new ImpersonationInfo { ImpersonationMode = ImpersonationMode.ImpersonateServiceAccount };
                dataSource.Timeout = System.TimeSpan.FromSeconds(_input.Connection.RequestTimeout);

                if (!SSAS.Save(_server, _output, dataSource)) {
                    return new ActionResponse(500, $"Could not save data source: {ids.DataSourceId}");
                }

                // view
                var entity = _output.Entity;
                var fields = SSAS.GetFields(_output);
                DataSourceView dataSourceView;
                if (database.DataSourceViews.Contains(ids.DataSourceViewId)) {
                    dataSourceView = database.DataSourceViews.Find(ids.DataSourceViewId);
                    if (dataSourceView.Schema.Tables.Contains(entity.Alias)) {
                        dataSourceView.Schema.Tables.Remove(entity.Alias);
                    }
                    dataSourceView.Schema.Tables.Add(CreateDataTable(_output, fields));
                    _output.Info($"Updating existing data source view: {ids.DataSourceViewId}");
                } else {
                    _output.Info($"Creating new data source view: {ids.DataSourceViewId}");

                    dataSourceView = database.DataSourceViews.AddNew(ids.DataSourceViewId);
                    dataSourceView.DataSourceID = ids.DataSourceId;
                    dataSourceView.Schema = new DataSet("Schema");
                    dataSourceView.Schema.Tables.Add(CreateDataTable(_output, fields));

                    if (!SSAS.Save(_server, _output, dataSourceView)) {
                        return new ActionResponse(500, $"Could not save data source view: {ids.DataSourceViewId}");
                    }

                }

                Dimension dimension;
                if (database.Dimensions.Contains(entity.Alias)) {
                    dimension = database.Dimensions.Find(entity.Alias);
                    _output.Info($"Updating existing dimension: {entity.Alias}");
                } else {
                    dimension = database.Dimensions.AddNew(entity.Alias, entity.Alias);
                    dimension.WriteEnabled = false;
                    dimension.StorageMode = DimensionStorageMode.Molap;
                    _output.Info($"Creating new dimension: {entity.Alias}");
                }
                dimension.Source = new DataSourceViewBinding(ids.DataSourceViewId);

                foreach (var field in fields) {
                    DimensionAttribute dimensionAttribute;
                    if (dimension.Attributes.Contains(field.Alias)) {
                        dimensionAttribute = dimension.Attributes.Find(field.Alias);
                    } else {
                        dimensionAttribute = dimension.Attributes.Add(field.Label, field.Alias);
                        dimensionAttribute.OrderBy = OrderBy.Key; // for now
                    }

                    var optimize = field.PrimaryKey || field.Dimension == "true" || field.Dimension == "default" && !field.IsNumeric();
                    dimensionAttribute.AttributeHierarchyEnabled = optimize;
                    dimensionAttribute.IsAggregatable = optimize;
                    dimensionAttribute.Usage = field.PrimaryKey ? AttributeUsage.Key : AttributeUsage.Regular;

                    dimensionAttribute.KeyColumns.Clear();
                    DataItem keyColumn;
                    if (field.Type == "string") {
                        var length = field.Length == "max" ? int.MaxValue : Convert.ToInt32(field.Length);
                        keyColumn = new DataItem(entity.Alias, field.Alias, SSAS.GetOleDbType(field), length) { Trimming = Trimming.None };
                    } else {
                        keyColumn = new DataItem(entity.Alias, field.Alias, SSAS.GetOleDbType(field));
                    }

                    dimensionAttribute.KeyColumns.Add(keyColumn);
                }

                if (!SSAS.Save(_server, _output, dimension)) {
                    return new ActionResponse(500, $"Could not save dimension: {entity.Alias}");
                }

                Cube cube;
                if (database.Cubes.Contains(ids.CubeId)) {
                    cube = database.Cubes.Find(ids.CubeId);
                    _output.Info($"Updating existing cube: {ids.CubeId}");
                } else {
                    cube = database.Cubes.AddNew(ids.CubeId, ids.CubeId);
                    _output.Info($"Creating new cube: {ids.CubeId}");
                }

                cube.Source = new DataSourceViewBinding(ids.DataSourceViewId);

                CubeDimension cubeDimension;
                if (cube.Dimensions.Contains(entity.Alias)) {
                    cubeDimension = cube.Dimensions.Find(entity.Alias);
                } else {
                    cubeDimension = cube.Dimensions.Add(entity.Alias);
                }
                cubeDimension.Attributes.Clear();
                foreach (DimensionAttribute attribute in database.Dimensions.Find(entity.Alias).Attributes) {
                    cubeDimension.Attributes.Add(attribute.ID);
                }

                var normalMeasureGroup = GetMeasureGroup(cube, entity, ids.NormalMeasureGroupId, ids.DataSourceViewId);

                // standard count measure
                Measure countMeasure;
                if (normalMeasureGroup.Measures.Contains("Count")) {
                    countMeasure = normalMeasureGroup.Measures.Find("Count");
                } else {
                    countMeasure = normalMeasureGroup.Measures.Add("Count", "Count");
                }
                countMeasure.AggregateFunction = AggregationFunction.Count;
                countMeasure.Source = new DataItem(new RowBinding(entity.Alias), OleDbType.Integer);

                // version measures
                var versionField = entity.GetVersionField();
                if (versionField != null) {
                    Measure versionMeasure;
                    if (normalMeasureGroup.Measures.Contains(ids.VersionId)) {
                        versionMeasure = normalMeasureGroup.Measures.Find(ids.VersionId);
                    } else {
                        versionMeasure = normalMeasureGroup.Measures.Add(ids.VersionId);
                    }
                    versionMeasure.AggregateFunction = AggregationFunction.Max;
                    versionMeasure.Source = new DataItem(entity.Alias, versionField.Alias, SSAS.GetOleDbType(versionField));
                    versionMeasure.Visible = false;
                }

                // defined measures
                var measureFields = entity.GetAllOutputFields().Where(f => f.Measure && f.Type != "byte[]");

                // normal measures
                foreach (var field in measureFields.Where(f => f.AggregateFunction != "distinctcount")) {
                    Measure measure;
                    var function = (AggregationFunction)Enum.Parse(typeof(AggregationFunction), field.AggregateFunction, true);
                    if (normalMeasureGroup.Measures.Contains(field.Alias)) {
                        measure = normalMeasureGroup.Measures.Find(field.Alias);
                    } else {
                        measure = normalMeasureGroup.Measures.Add(field.Label, field.Alias);
                    }
                    measure.AggregateFunction = function;
                    measure.FormatString = field.Format;
                    measure.Source = new DataItem(entity.Alias, field.Alias, SSAS.GetOleDbType(field));
                }

                // distinct measures
                foreach (var field in measureFields.Where(f => f.AggregateFunction == "distinctcount")) {
                    var type = SSAS.GetOleDbType(field);
                    if (SSAS.CanDistinctCount(type)) {
                        Measure measure;
                        var distinctMeasureGroup = GetMeasureGroup(cube, entity, ids.DistinctMeasureGroupId, ids.DataSourceViewId);
                        if (distinctMeasureGroup.Measures.Contains(field.Alias)) {
                            measure = distinctMeasureGroup.Measures.Find(field.Alias);
                        } else {
                            measure = distinctMeasureGroup.Measures.Add(field.Label, field.Alias);
                        }
                        measure.AggregateFunction = AggregationFunction.DistinctCount;
                        measure.FormatString = field.Format;
                        measure.Source = new DataItem(entity.Alias, field.Alias, SSAS.GetOleDbType(field));
                    } else {
                        _output.Warn($"Can not distinct count {field.Label} using {field.Type} type.");
                    }
                }

                if (!SSAS.Save(_server, _output, cube)) {
                    return new ActionResponse(500, $"Could not save cube: {ids.CubeId}");
                }

                _server.Disconnect();
            }

            return new ActionResponse();
        }

        private MeasureGroup GetMeasureGroup(Cube cube, Entity entity, string id, string dataSourceViewId) {
            MeasureGroup measureGroup;
            if (cube.MeasureGroups.Contains(id)) {
                measureGroup = cube.MeasureGroups.Find(id);
            } else {
                measureGroup = cube.MeasureGroups.Add(id);
                measureGroup.StorageMode = StorageMode.Molap;
            }
            Partition partition;
            if (measureGroup.Partitions.Contains(id)) {
                partition = measureGroup.Partitions.Find(id);
            } else {
                partition = measureGroup.Partitions.Add(id);
            }
            partition.Source = new DsvTableBinding(dataSourceViewId, entity.Alias);
            partition.StorageMode = StorageMode.Molap;

            measureGroup.Dimensions.Clear();
            var measureGroupDimension = new DegenerateMeasureGroupDimension(entity.Alias);
            foreach (var key in entity.GetPrimaryKey()) {
                var attribute = measureGroupDimension.Attributes.Add(key.Alias);
                attribute.Type = MeasureGroupAttributeType.Granularity;
                attribute.KeyColumns.Add(entity.Alias, key.Alias, SSAS.GetOleDbType(key));
            }
            measureGroup.Dimensions.Add(measureGroupDimension);

            return measureGroup;
        }

        private DataTable CreateDataTable(IContext context, IEnumerable<Field> fields) {
            var schema = new DataSet("Schema");

            var sql = SSAS.CreateQuery(_output);
            if (context.Entity.Filter.Any()) {
                sql += " WHERE " + context.ResolveFilter(_connectionFactory);
            }
            var adapter = new SqlDataAdapter(sql, new SqlConnection(_connectionFactory.GetConnectionString($"Transformalize ({_input.Process.Name})")));
            adapter.FillSchema(schema, SchemaType.Source);

            var table = schema.Tables[0];
            table.ExtendedProperties.Add("QueryDefinition", sql);
            table.ExtendedProperties.Add("DbTableName", context.Entity.Name);
            table.ExtendedProperties.Add("FriendlyName", context.Entity.Alias);
            table.ExtendedProperties.Add("TableType", "View");
            table.TableName = context.Entity.Alias;

            foreach (var field in fields) {
                table.Columns[field.Alias].ExtendedProperties.Add("DbColumnName", field.Alias);
                if (field.Label == string.Empty || field.Label == field.Alias) {
                    field.Label = field.Alias.Titleize();
                }
                table.Columns[field.Alias].ExtendedProperties.Add("FriendlyName", field.Label);
            }
            return table.Clone();
        }
    }
}
