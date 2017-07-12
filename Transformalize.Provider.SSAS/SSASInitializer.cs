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
        private readonly InputContext _input;
        private readonly OutputContext _output;
        private readonly Server _server;
        private readonly IConnectionFactory _connectionFactory;

        public SSASInitializer(InputContext input, OutputContext output, IConnectionFactory connectionFactory) {
            _input = input;
            _output = output;
            _server = new Server();
            _connectionFactory = connectionFactory;
        }

        public ActionResponse Execute() {

            var dataSourceIdentifier = _input.Connection.Database;
            var viewIdentifier = _input.Process.Name;
            var cubeIdentifier = _output.Process.Name;

            Database database;
            _server.Connect($"Data Source={_output.Connection.Server};Catalog=;");

            if (_server.Databases.ContainsName(_output.Connection.Database)) {
                database = _server.Databases.FindByName(_output.Connection.Database);
            } else {
                database = new Database() {
                    Name = _output.Connection.Database,
                    ID = _output.Connection.Database,
                    DataSourceImpersonationInfo = new ImpersonationInfo { ImpersonationMode = ImpersonationMode.Default }
                };
                _server.Databases.Add(database);
                Persist(database);
            }

            DataSource dataSource;
            if (database.DataSources.ContainsName(dataSourceIdentifier)) {
                dataSource = database.DataSources.FindByName(dataSourceIdentifier);
            } else {
                dataSource = database.DataSources.AddNew(dataSourceIdentifier, dataSourceIdentifier);
            }

            dataSource.ConnectionString = $"Provider=SQLOLEDB.1;Data Source={_input.Connection.Server};Integrated Security=SSPI;Initial Catalog={_input.Connection.Database}";
            dataSource.ImpersonationInfo = new ImpersonationInfo { ImpersonationMode = ImpersonationMode.ImpersonateServiceAccount };
            dataSource.Timeout = System.TimeSpan.FromSeconds(_input.Connection.RequestTimeout);
            Persist(dataSource);

            DataSourceView dataSourceView;
            if (database.DataSourceViews.ContainsName(viewIdentifier)) {
                dataSourceView = database.DataSourceViews.FindByName(viewIdentifier);
            } else {
                dataSourceView = database.DataSourceViews.AddNew(viewIdentifier, viewIdentifier);
                dataSourceView.DataSourceID = dataSourceIdentifier;
            }

            var schema = new DataSet("Schema");
            for (int i = 0; i < _output.Process.Entities.Count; i++) {

                var entity = _output.Process.Entities[i];
                var fields = entity.GetAllOutputFields().Where(f => f.Type != "byte[]" && f.Type != "guid" && f.Type != "object" && !f.System);
                var names = string.Join(",", fields.Select(f => f.Name == f.Alias ? $"[{f.Name}]" : $"[{f.Name}] AS [{f.Alias}]"));
                var sql = $"SELECT {names} FROM [{(entity.Schema == string.Empty ? "dbo" : entity.Schema)}].[{entity.Name}] WITH (NOLOCK)";

                var adapter = new SqlDataAdapter(sql, new SqlConnection(_connectionFactory.GetConnectionString($"Transformalize ({_input.Process.Name})")));
                adapter.FillSchema(schema, SchemaType.Source);

                var table = schema.Tables[i];
                table.ExtendedProperties.Add("QueryDefinition", sql);
                table.ExtendedProperties.Add("DbTableName", entity.Name);
                table.ExtendedProperties.Add("FriendlyName", entity.Alias);
                table.ExtendedProperties.Add("TableType", "View");
                table.TableName = entity.Alias;

                foreach (var field in fields) {
                    table.Columns[field.Alias].ExtendedProperties.Add("DbColumnName", field.Alias);
                    var name = field.Label == string.Empty ? field.Alias.Titleize() : field.Label;
                    table.Columns[field.Alias].ExtendedProperties.Add("FriendlyName", name);
                }
            }
            dataSourceView.Schema = schema;
            Persist(dataSourceView);

            foreach (var entity in _input.Process.Entities) {
                Dimension dimension;
                if (database.Dimensions.ContainsName(entity.Alias)) {
                    dimension = database.Dimensions.FindByName(entity.Alias);
                } else {
                    dimension = database.Dimensions.AddNew(entity.Alias, entity.Alias);
                }
                dimension.Source = new DataSourceViewBinding(viewIdentifier);
                dimension.CurrentStorageMode = DimensionStorageMode.Molap;  // configure per entity (molap, rolap, memory)

                var fields = entity.GetAllOutputFields().Where(f => f.Type != "byte[]" && f.Type != "guid" && f.Type != "object" && !f.System && (f.PrimaryKey || f.Dimension == "true" || f.Dimension == "default" && !f.IsNumeric()));
                foreach (var field in fields) {
                    DimensionAttribute dimensionAttribute;
                    if (dimension.Attributes.ContainsName(field.Alias)) {
                        dimensionAttribute = dimension.Attributes.FindByName(field.Alias);
                    } else {
                        dimensionAttribute = dimension.Attributes.Add(field.Alias, field.Alias);
                        dimensionAttribute.OrderBy = OrderBy.Key; // for now
                    }
                    dimensionAttribute.Usage = field.PrimaryKey ? AttributeUsage.Key : AttributeUsage.Regular;

                    dimensionAttribute.KeyColumns.Clear();
                    DataItem keyColumn;
                    if (field.Type == "string") {
                        var length = field.Length == "max" ? int.MaxValue : System.Convert.ToInt32(field.Length);
                        keyColumn = new DataItem(entity.Alias, field.Alias, GetOleDbType(field), length) { Trimming = Trimming.None };
                    } else {
                        keyColumn = new DataItem(entity.Alias, field.Alias, GetOleDbType(field));
                    }
                    dimensionAttribute.KeyColumns.Add(keyColumn);
                }
                Persist(dimension);
            }

            Cube cube;
            if (database.Cubes.ContainsName(cubeIdentifier)) {
                cube = database.Cubes.FindByName(cubeIdentifier);
            } else {
                cube = database.Cubes.AddNew(cubeIdentifier, cubeIdentifier);
                cube.Annotations.Add("http://schemas.microsoft.com/DataWarehouse/Designer/1.0:ShowFriendlyNames", "true");
            }

            cube.Source = new DataSourceViewBinding(viewIdentifier);

            foreach (var entity in _input.Process.Entities) {
                CubeDimension cubeDimension;
                if (cube.Dimensions.ContainsName(entity.Alias)) {
                    cubeDimension = cube.Dimensions.FindByName(entity.Alias);
                } else {
                    cubeDimension = cube.Dimensions.Add(entity.Alias);
                }
                cubeDimension.Attributes.Clear();
                foreach (DimensionAttribute attribute in database.Dimensions.FindByName(entity.Alias).Attributes) {
                    cubeDimension.Attributes.Add(attribute.ID);
                }
                MeasureGroup measureGroup;
                if (cube.MeasureGroups.ContainsName(entity.Alias)) {
                    measureGroup = cube.MeasureGroups.FindByName(entity.Alias);
                } else {
                    measureGroup = cube.MeasureGroups.Add(entity.Alias, entity.Alias);
                }
                measureGroup.StorageMode = StorageMode.Molap;

                // partitions
                Partition partition;
                if (measureGroup.Partitions.ContainsName(entity.Alias)) {
                    partition = measureGroup.Partitions.FindByName(entity.Alias);
                } else {
                    partition = measureGroup.Partitions.Add(entity.Alias, entity.Alias);
                }
                partition.Source = new DsvTableBinding(viewIdentifier, entity.Alias);
                partition.StorageMode = StorageMode.Molap;

                // standard count measure
                Measure countMeasure;
                if (measureGroup.Measures.ContainsName("Count")) {
                    countMeasure = measureGroup.Measures.FindByName("Count");
                } else {
                    countMeasure = measureGroup.Measures.Add("Count", "Count");
                }
                countMeasure.AggregateFunction = AggregationFunction.Count;
                countMeasure.Source = new DataItem(new RowBinding(entity.Alias), OleDbType.Integer, 4);

                // defined measures
                var measureFields = entity.GetAllOutputFields().Where(f => f.Measure && f.IsNumeric());
                foreach (var field in measureFields) {
                    Measure measure;
                    if (measureGroup.Measures.ContainsName(field.Alias)) {
                        measure = measureGroup.Measures.FindByName(field.Alias);
                    } else {
                        measure = measureGroup.Measures.Add(field.Alias, field.Alias);
                    }
                    measure.AggregateFunction = (AggregationFunction)System.Enum.Parse(typeof(AggregationFunction), field.AggregateFunction, true);
                    measure.Visible = true;

                    measure.Source = new DataItem(entity.Alias, field.Alias, GetOleDbType(field));
                }

                measureGroup.Dimensions.Clear();
                var measureGroupDimension = new DegenerateMeasureGroupDimension(entity.Alias);
                measureGroup.Dimensions.Add(measureGroupDimension);
                foreach (var key in entity.GetPrimaryKey()) {
                    var attribute = measureGroupDimension.Attributes.Add(key.Alias);
                    attribute.Type = MeasureGroupAttributeType.Granularity;  // i guess
                    attribute.KeyColumns.Add(entity.Alias, key.Alias, GetOleDbType(key));
                }
            }

            Persist(cube);

            return new ActionResponse();
        }

        private void Persist(IMajorObject obj) {

            var builder = new StringBuilder();
            using (var xmlWriter = XmlWriter.Create(builder, new XmlWriterSettings { OmitXmlDeclaration = true })) {
                Scripter.WriteAlter(xmlWriter, obj, true, true);
                xmlWriter.Flush();
            }

            var command = builder.ToString();
            _output.Debug(() => command);
            var results = _server.Execute(command);

            if (results.Count > 0) {
                foreach (XmlaResult result in results) {
                    if (result.Messages.Count > 0) {
                        foreach (XmlaMessage message in result.Messages) {
                            _output.Error(message.Description);
                        }
                    }
                }
            }
        }

        OleDbType GetOleDbType(Transformalize.Configuration.Field field) {
            var sysType = Constants.TypeSystem()[field.Type];
            if (ReferenceEquals(sysType, typeof(string))) {
                return field.Unicode ? OleDbType.WChar : OleDbType.Char;
            } else if (ReferenceEquals(sysType, typeof(int))) {
                return OleDbType.Integer;
            } else if (ReferenceEquals(sysType, typeof(long))) {
                return OleDbType.BigInt;
            } else if (ReferenceEquals(sysType, typeof(bool))) {
                return OleDbType.Boolean;
            } else if (ReferenceEquals(sysType, typeof(System.DateTime))) {
                return OleDbType.Date;
            } else if (ReferenceEquals(sysType, typeof(char))) {
                return OleDbType.Char;
            } else if (ReferenceEquals(sysType, typeof(decimal))) {
                return OleDbType.Decimal;
            } else if (ReferenceEquals(sysType, typeof(double))) {
                return OleDbType.Double;
            } else if (ReferenceEquals(sysType, typeof(float))) {
                return OleDbType.Single;
            } else if (ReferenceEquals(sysType, typeof(byte[]))) {
                return OleDbType.Binary;
            } else if (ReferenceEquals(sysType, typeof(System.Guid))) {
                return OleDbType.Guid;
            } else if (ReferenceEquals(sysType, typeof(uint))) {
                return OleDbType.UnsignedInt;
            } else if (ReferenceEquals(sysType, typeof(ulong))) {
                return OleDbType.UnsignedBigInt;
            } else if (ReferenceEquals(sysType, typeof(ushort))) {
                return OleDbType.UnsignedSmallInt;
            } else {
                return OleDbType.WChar;
            }
        }

    }
}
