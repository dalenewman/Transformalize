using Humanizer;
using Microsoft.AnalysisServices;
using System.Data;
using System.Data.OleDb;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Xml;
using Transformalize.Actions;
using Transformalize.Context;
using Transformalize.Contracts;
using Transformalize.Provider.Ado;

namespace Transformalize.Provider.SSAS {

    public class SSASInitializer : IInitializer {

        readonly InputContext _input;
        readonly OutputContext _output;
        readonly Server _server;
        readonly IConnectionFactory _connectionFactory;

        public SSASInitializer(InputContext input, OutputContext output, IConnectionFactory connectionFactory) {
            _input = input;
            _output = output;
            _server = new Server();
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
                DataSourceView dataSourceView;
                if (database.DataSourceViews.Contains(ids.DataSourceViewId)) {
                    dataSourceView = database.DataSourceViews.Find(ids.DataSourceViewId);
                    _output.Info($"Updating existing data source view: {ids.DataSourceViewId}");
                } else {
                    dataSourceView = database.DataSourceViews.AddNew(ids.DataSourceViewId);
                    dataSourceView.DataSourceID = ids.DataSourceId;
                    _output.Info($"Creating new data source view: {ids.DataSourceViewId}");
                }

                var schema = new DataSet("Schema");
                var sql = SSAS.CreateQuery(_output);
                var adapter = new SqlDataAdapter(sql, new SqlConnection(_connectionFactory.GetConnectionString($"Transformalize ({_input.Process.Name})")));
                adapter.FillSchema(schema, SchemaType.Source);

                var table = schema.Tables[0];
                table.ExtendedProperties.Add("QueryDefinition", sql);
                table.ExtendedProperties.Add("DbTableName", entity.Name);
                table.ExtendedProperties.Add("FriendlyName", entity.Alias);
                table.ExtendedProperties.Add("TableType", "View");
                table.TableName = entity.Alias;

                var fields = SSAS.GetFields(_output);
                foreach (var field in fields) {
                    table.Columns[field.Alias].ExtendedProperties.Add("DbColumnName", field.Alias);
                    if (field.Label == string.Empty || field.Label == field.Alias) {
                        field.Label = field.Alias.Titleize();
                    }
                    table.Columns[field.Alias].ExtendedProperties.Add("FriendlyName", field.Label);
                }
                dataSourceView.Schema = schema;

                if (!SSAS.Save(_server, _output, dataSourceView)) {
                    return new ActionResponse(500, $"Could not save data source view: {ids.DataSourceViewId}");
                }

                Dimension dimension;
                if (database.Dimensions.Contains(entity.Alias)) {
                    dimension = database.Dimensions.Find(entity.Alias);
                    _output.Info($"Updating existing dimension: {entity.Alias}");
                } else {
                    dimension = database.Dimensions.AddNew(entity.Alias, entity.Alias);
                    _output.Info($"Creating new dimension: {entity.Alias}");
                }
                dimension.Source = new DataSourceViewBinding(ids.DataSourceViewId);
                dimension.CurrentStorageMode = DimensionStorageMode.Molap;

                foreach (var field in fields) {
                    DimensionAttribute dimensionAttribute;
                    if (dimension.Attributes.Contains(field.Alias)) {
                        dimensionAttribute = dimension.Attributes.Find(field.Alias);
                    } else {
                        dimensionAttribute = dimension.Attributes.Add(field.Label, field.Alias);
                        dimensionAttribute.OrderBy = OrderBy.Key; // for now
                    }
                    dimensionAttribute.Usage = field.PrimaryKey ? AttributeUsage.Key : AttributeUsage.Regular;
                    dimensionAttribute.AttributeHierarchyEnabled = field.PrimaryKey || field.Dimension == "true" || field.Dimension == "default" && !field.IsNumeric();

                    dimensionAttribute.KeyColumns.Clear();
                    DataItem keyColumn;
                    if (field.Type == "string") {
                        var length = field.Length == "max" ? int.MaxValue : System.Convert.ToInt32(field.Length);
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
                    cube.Annotations.Add("http://schemas.microsoft.com/DataWarehouse/Designer/1.0:ShowFriendlyNames", "true");
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

                MeasureGroup measureGroup;
                if (cube.MeasureGroups.Contains(entity.Alias)) {
                    measureGroup = cube.MeasureGroups.Find(entity.Alias);
                } else {
                    measureGroup = cube.MeasureGroups.Add(entity.Alias, entity.Alias);
                }
                measureGroup.StorageMode = StorageMode.Molap;

                // partitions
                Partition partition;
                if (measureGroup.Partitions.Contains(entity.Alias)) {
                    partition = measureGroup.Partitions.Find(entity.Alias);
                } else {
                    partition = measureGroup.Partitions.Add(entity.Alias, entity.Alias);
                }
                partition.Source = new DsvTableBinding(ids.DataSourceViewId, entity.Alias);
                partition.StorageMode = StorageMode.Molap;

                // standard count measure
                Measure countMeasure;
                if (measureGroup.Measures.Contains("Count")) {
                    countMeasure = measureGroup.Measures.Find("Count");
                } else {
                    countMeasure = measureGroup.Measures.Add("Count", "Count");
                }
                countMeasure.AggregateFunction = AggregationFunction.Count;
                countMeasure.Source = new DataItem(new RowBinding(entity.Alias), OleDbType.Integer);

                // version measures
                var versionField = entity.GetVersionField();
                if (versionField != null) {
                    Measure versionMeasure;
                    if (measureGroup.Measures.Contains(ids.VersionId)) {
                        versionMeasure = measureGroup.Measures.Find(ids.VersionId);
                    } else {
                        versionMeasure = measureGroup.Measures.Add(ids.VersionId);
                    }
                    versionMeasure.AggregateFunction = AggregationFunction.Max;
                    versionMeasure.Source = new DataItem(entity.Alias, versionField.Alias, SSAS.GetOleDbType(versionField));
                    versionMeasure.Visible = false;
                }

                // defined measures
                var measureFields = entity.GetAllOutputFields().Where(f => f.Measure && f.IsNumeric());
                foreach (var field in measureFields) {
                    Measure measure;
                    if (measureGroup.Measures.Contains(field.Alias)) {
                        measure = measureGroup.Measures.Find(field.Alias);
                    } else {
                        measure = measureGroup.Measures.Add(field.Label, field.Alias);
                    }
                    measure.AggregateFunction = (AggregationFunction)System.Enum.Parse(typeof(AggregationFunction), field.AggregateFunction, true);
                    measure.FormatString = field.Format;
                    measure.Source = new DataItem(entity.Alias, field.Alias, SSAS.GetOleDbType(field));
                }

                measureGroup.Dimensions.Clear();
                var measureGroupDimension = new DegenerateMeasureGroupDimension(entity.Alias);
                measureGroup.Dimensions.Add(measureGroupDimension);
                foreach (var key in entity.GetPrimaryKey()) {
                    var attribute = measureGroupDimension.Attributes.Add(key.Alias);
                    attribute.Type = MeasureGroupAttributeType.Granularity;
                    attribute.KeyColumns.Add(entity.Alias, key.Alias, SSAS.GetOleDbType(key));
                }


                if (!SSAS.Save(_server, _output, cube)) {
                    return new ActionResponse(500, $"Could not save cube: {ids.CubeId}");
                }

                _server.Disconnect();
            }

            return new ActionResponse();
        }

    }
}
