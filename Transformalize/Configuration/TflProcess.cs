using System;
using System.Collections.Generic;
using System.Linq;
using Cfg.Net;
using Cfg.Net.Ext;
using Transformalize.Libs.Ninject;
using Transformalize.Libs.Ninject.Parameters;
using Transformalize.Libs.Ninject.Syntax;
using Transformalize.Libs.NVelocity.App;
using Transformalize.Libs.NVelocity.Runtime;
using Transformalize.Libs.RazorEngine;
using Transformalize.Libs.RazorEngine.Configuration.Fluent;
using Transformalize.Libs.RazorEngine.Templating;
using Transformalize.Libs.SolrNet;
using Transformalize.Logging;
using Transformalize.Main;
using Transformalize.Main.Providers;
using Transformalize.Main.Providers.Solr;
using Transformalize.Main.Transform;
using Transformalize.Operations.Transform;

namespace Transformalize.Configuration {

    public class TflProcess : CfgNode {

        private const string ALL = "*";

        private readonly Dictionary<string, string> _providerTypes = new Dictionary<string, string>();

        public TflProcess() {

            _providerTypes.Add("sqlserver", "System.Data.SqlClient.SqlConnection, System.Data, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089");
            _providerTypes.Add("sqlce", "System.Data.SqlServerCe.SqlCeConnection, System.Data.SqlServerCe");
            _providerTypes.Add("mysql", "MySql.Data.MySqlClient.MySqlConnection, MySql.Data");
            _providerTypes.Add("postgresql", "Npgsql.NpgsqlConnection, Npgsql");
            var empties = new[] {
                "analysisservices",
                "file",
                "folder",
                "internal",
                "console",
                "log",
                "html",
                "elasticsearch",
                "solr",
                "lucene",
                "mail",
                "web"
            };
            foreach (var empty in empties) {
                _providerTypes.Add(empty, empty);
            }
        }

        /// <summary>
        /// A name (of your choosing) to identify the process.
        /// </summary>
        [Cfg(value = "", required = true, unique = true)]
        public string Name { get; set; }

        /// <summary>
        /// Optional.
        /// 
        /// `True` by default.
        /// 
        /// Indicates the process is enabled.  The included executable (e.g. `tfl.exe`) 
        /// respects this setting and does not run the process if disabled (or `False`).
        /// </summary>
        [Cfg(value = true)]
        public bool Enabled { get; set; }

        /// <summary>
        /// Optional. 
        /// 
        /// A mode reflects the intent of running the process.
        ///  
        /// * `init` wipes everything out
        /// * <strong>`default`</strong> moves data through the pipeline, from input to output.
        /// 
        /// Aside from these, you may use any mode (of your choosing).  Then, you can control
        /// whether or not templates and/or actions run by setting their modes.
        /// </summary>
        [Cfg(value = "", toLower = true)]
        public string Mode { get; set; }

        /// <summary>
        /// Optional.  Default is `false`
        /// 
        /// If true, process entities in parallel.  If false, process them one by one in their configuration order.
        /// 
        /// Parallel *on* allows you to process all the entities at the same time, potentially faster.
        /// Parallel *off* allows you to have one entity depend on a previous entity's data.
        /// </summary>
        [Cfg(value = false)]
        public bool Parallel { get; set; }

        /// <summary>
        /// Optional.
        /// 
        /// A choice between `MultiThreaded`, `SingleThreaded`, and <strong>`Default`</strong>.
        /// 
        /// `Default` defers this decision to the entity's PipelineThreading setting.
        /// </summary>
        [Cfg(value = "Default", domain = "SingleThreaded,MultiThreaded,Default")]
        public string PipelineThreading { get; set; }

        /// <summary>
        /// Optional.
        /// 
        /// If your output is a relational database that supports views and `StarEnabled` is `True`,
        /// this is the name of a view that projects fields from all the entities in the
        /// star-schema as a single flat projection.
        /// 
        /// If not set, it is the combination of the process name, and "Star." 
        /// </summary>
        [Cfg(value = "")]
        public string Star { get; set; }

        /// <summary>
        /// Optional.
        /// 
        /// `True` by default.
        /// 
        /// Star refers to star-schema Transformalize is creating.  You can turn this off 
        /// if your intention is not to create a star-schema.  A `False` setting here may
        /// speed things up.
        /// </summary>
        [Cfg(value = true)]
        public bool StarEnabled { get; set; }

        /// <summary>
        /// Optional.
        /// 
        /// Choices are `html` and <strong>`raw`</strong>.
        /// 
        /// This refers to the razor templating engine's content type.  If you're rendering HTML 
        /// markup, use `html`, if not, using `raw` may inprove performance.
        /// </summary>
        [Cfg(value = "raw", domain = "raw,html", toLower = true)]
        public string TemplateContentType { get; set; }

        /// <summary>
        /// Optional.
        /// 
        /// Indicates the data's time zone.
        /// 
        /// It is used as the `to-time-zone` setting for `now()` and `timezone()` transformations
        /// if the `to-time-zone` is not set.
        /// 
        /// NOTE: Normally, you should keep the dates in UTC until presented to the user. 
        /// Then, have the client application convert UTC to the user's time zone.
        /// </summary>
        [Cfg(value = "")]
        public string TimeZone { get; set; }

        /// <summary>
        /// Optional
        /// 
        /// If your output is a relational database that supports views, this is the name of
        /// a view that projects fields from all the entities.  This is different from 
        /// the Star view, as it's joins are exactly as configured in the <relationships/> 
        /// collection.
        /// 
        /// If not set, it is the combination of the process name, and "View." 
        /// </summary>
        [Cfg(value = "")]
        public string View { get; set; }

        [Cfg(value = false)]
        public bool ViewEnabled { get; set; }

        /// <summary>
        /// A collection of [Actions](/action)
        /// </summary>
        [Cfg()]
        public List<TflAction> Actions { get; set; }

        /// <summary>
        /// A collection of [Calculated Fields](/calculated-field)
        /// </summary>
        [Cfg()]
        public List<TflField> CalculatedFields { get; set; }

        /// <summary>
        /// A collection of [Connections](/connection)
        /// </summary>
        [Cfg(required = false)]
        public List<TflConnection> Connections { get; set; }

        /// <summary>
        /// A collection of [Entities](/entity)
        /// </summary>
        [Cfg(required = true)]
        public List<TflEntity> Entities { get; set; }

        /// <summary>
        /// Settings to control [file inspection](/file-inspection).
        /// </summary>
        [Cfg()]
        public List<TflFileInspection> FileInspection { get; set; }

        /// <summary>
        /// A collection of [Logs](/log)
        /// </summary>
        [Cfg()]
        public List<TflLog> Log { get; set; }

        /// <summary>
        /// A collection of [Maps](/map)
        /// </summary>
        [Cfg()]
        public List<TflMap> Maps { get; set; }

        /// <summary>
        /// A collection of [Relationships](/relationship)
        /// </summary>
        [Cfg()]
        public List<TflRelationship> Relationships { get; set; }

        /// <summary>
        /// A collection of [Scripts](/script)
        /// </summary>
        [Cfg()]
        public List<TflScript> Scripts { get; set; }

        /// <summary>
        /// A collection of [Search Types](/search-type)
        /// </summary>
        [Cfg()]
        public List<TflSearchType> SearchTypes { get; set; }

        /// <summary>
        /// A collection of [Templates](/template)
        /// </summary>
        [Cfg()]
        public List<TflTemplate> Templates { get; set; }

        /// <summary>
        /// A collection of [Data Sets](/data-sets)
        /// </summary>
        [Cfg()]
        public List<TflDataSet> DataSets { get; set; }

        protected override void PreValidate() {

            if (String.IsNullOrEmpty(Star)) {
                Star = Name + "Star";
            }

            if (string.IsNullOrEmpty(View)) {
                View = Name + "View";
            }

            // calculated fields are not input
            foreach (var calculatedField in CalculatedFields) {
                calculatedField.Input = false;
            }

            ModifyDefaultConnection("input");
            ModifyDefaultConnection("output");
            ModifyDefaultEntityConnections();
            ModifyDefaultOutput();
            ModifyDefaultSearchTypes();

            try {
                AdaptFieldsCreatedFromTransforms(new[] { "fromxml", "fromregex", "fromjson", "fromsplit" });
            } catch (Exception ex) {
                Error("Trouble adapting fields created from transforms. {0}", ex.Message);
            }

            try {
                var factory = new ShortHandFactory(this);
                var problems = factory.ExpandShortHandTransforms();
                if (problems.Any()) {
                    foreach (var problem in problems) {
                        Error(problem);
                    }
                }
            } catch (Exception ex) {
                Error("Trouble expanding short hand transforms. {0}", ex.Message);
            }

            ModifyMergeParameters();
            ModifyMapParameters();

        }

        private void ModifyDefaultEntityConnections() {
            foreach (var entity in Entities.Where(entity => !entity.HasConnection())) {
                entity.Connection = Connections.Any(c => c.Name == "input") ? "input" : Connections.First().Name;
            }
        }

        private void ModifyDefaultConnection(string name) {
            if (Connections.All(c => c.Name != name)) {
                Connections.Add(new TflConnection { Name = name }.WithDefaults());
            }
        }

        private void ModifyMergeParameters() {
            foreach (var entity in Entities) {
                entity.MergeParameters();
            }
            var index = 0;
            foreach (var field in CalculatedFields) {
                foreach (var transform in field.Transforms.Where(t => t.Parameter != string.Empty)) {
                    if (transform.Parameter == ALL) {
                        foreach (var entity in Entities) {
                            foreach (var entityField in entity.GetAllFields().Where(f => f.Output)) {
                                transform.Parameters.Add(GetParameter(entity.Alias, entityField.Alias, entityField.Type));
                            }
                        }
                        var thisField = field;
                        foreach (var cf in CalculatedFields.Take(index).Where(cf => cf.Name != thisField.Name)) {
                            transform.Parameters.Add(GetParameter(string.Empty, cf.Alias, cf.Type));
                        }
                    } else {
                        if (transform.Parameter.IndexOf('.') > 0) {
                            var split = transform.Parameter.Split(new[] { '.' });
                            transform.Parameters.Add(GetParameter(split[0], split[1]));
                        } else {
                            transform.Parameters.Add(GetParameter(transform.Parameter));
                        }
                    }
                    transform.Parameter = string.Empty;
                }
                index++;
            }
        }

        private TflParameter GetParameter(string field) {
            return new TflParameter {
                Field = field
            }.WithDefaults();
        }

        private TflParameter GetParameter(string entity, string field) {
            return new TflParameter {
                Entity = entity,
                Field = field
            }.WithDefaults();
        }

        private TflParameter GetParameter(string entity, string field, string type) {
            return new TflParameter {
                Entity = entity,
                Field = field,
                Type = type
            }.WithDefaults();
        }

        /// <summary>
        /// Map transforms require the map's parameters.
        /// </summary>
        private void ModifyMapParameters() {

            if (Maps.Count == 0)
                return;

            foreach (var transform in GetAllTransforms().Where(t => t.Method == "map" && Maps.Any(m => m.Name == t.Map))) {

                var parameters = Maps
                    .First(m => m.Name == transform.Map)
                    .Items
                    .Where(i => i.Parameter != string.Empty)
                    .Select(p => p.Parameter)
                    .Distinct();

                foreach (var parameter in parameters) {
                    if (parameter.IndexOf('.') > 0) {
                        var split = parameter.Split(new[] { '.' });
                        transform.Parameters.Add(GetParameter(split[0], split[1]));
                    } else {
                        transform.Parameters.Add(GetParameter(parameter));
                    }
                }
            }
        }

        private void ModifyDefaultSearchTypes() {

            if (SearchTypes.All(st => st.Name != "none"))
                SearchTypes.Add(new TflSearchType {
                    Name = "none",
                    MultiValued = false,
                    Store = false,
                    Index = false
                }.WithDefaults());

            if (SearchTypes.All(st => st.Name != "default"))
                SearchTypes.Add(new TflSearchType {
                    Name = "default",
                    MultiValued = false,
                    Store = true,
                    Index = true
                }.WithDefaults());
        }

        private void ModifyDefaultOutput() {
            if (Connections.All(c => c.Name != "output"))
                Connections.Add(new TflConnection {
                    Name = "output",
                    Provider = "internal"
                }.WithDefaults());
        }

        private void AdaptFieldsCreatedFromTransforms(IEnumerable<string> transformToFields) {
            foreach (var field in transformToFields) {
                while (new TransformFieldsToParametersAdapter(this).Adapt(field) > 0) {
                    new TransformFieldsMoveAdapter(this).Adapt(field);
                }
            }
        }

        protected override void Validate() {
            ValidateDuplicateEntities();
            ValidateDuplicateFields();
            ValidateLogConnections();
            ValidateRelationships();
            ValidateEntityConnections();
            ValidateActionConnections();
            ValidateTemplateActionConnections();
            ValidateTransformConnections();
            ValidateMapConnections();
        }

        private void ValidateMapConnections() {
            foreach (var map in Maps.Where(m => m.Query != string.Empty).Where(map => Connections.All(c => c.Name != map.Connection))) {
                Error("The {0} map references an invalid connection: {1}.", map.Name, map.Connection);
            }
        }

        private IEnumerable<TflTransform> GetAllTransforms() {
            var transforms = Entities.SelectMany(entity => entity.GetAllTransforms()).ToList();
            transforms.AddRange(CalculatedFields.SelectMany(field => field.Transforms));
            return transforms;
        }


        private void ValidateTransformConnections() {

            var methodsWithConnections = new[] { "mail", "run" };

            foreach (var transform in GetAllTransforms().Where(t => methodsWithConnections.Any(nc => nc == t.Method))) {
                var connection = Connections.FirstOrDefault(c => c.Name == transform.Connection);
                if (connection == null) {
                    Error("The {0} transform references an invalid connection: {2}.", transform.Method, transform.Connection);
                    continue;
                }

                switch (transform.Method) {
                    case "mail":
                        if (connection.Provider != "mail") {
                            Error("The {0} transform references the wrong type of connection: {1}.", transform.Method, connection.Provider);
                        }
                        break;
                }
            }
        }

        private void ValidateEntityConnections() {
            foreach (var entity in Entities.Where(entity => Connections.All(c => c.Name != entity.Connection))) {
                Error("The {0} entity references an invalid connection: {1}.", entity.Name, entity.Connection);
            }
        }

        private void ValidateTemplateActionConnections() {
            foreach (var action in Templates.SelectMany(template => template.Actions.Where(a => a.Connection != string.Empty).Where(action => Connections.All(c => c.Name != action.Connection)))) {
                Error("The {0} template action references an invalid connection: {1}.", action.Action, action.Connection);
            }
        }

        private void ValidateActionConnections() {
            foreach (var action in Actions.Where(action => action.Connection != string.Empty).Where(action => Connections.All(c => c.Name != action.Connection))) {
                Error("The {0} action references an invalid connection: {1}.", action.Action, action.Connection);
            }
        }

        private void ValidateRelationships() {
            // count check
            if (Entities.Count > 1 && Relationships.Count + 1 < Entities.Count) {
                Error("You have {0} entities so you need {1} relationships. You have {2} relationships.", Entities.Count, Entities.Count - 1, Relationships.Count);
            }

            //entity alias, name check, and if that passes, do field alias, name check
            //TODO: refactor left and right side to single method
            var keys = GetEntityNames();
            foreach (var relationship in Relationships) {

                // validate left side
                if (keys.Contains(relationship.LeftEntity)) {
                    var fieldKeys = GetEntityFieldKeys(relationship.LeftEntity);
                    if (relationship.Join.Count == 0) {
                        if (!fieldKeys.Contains(relationship.LeftField)) {
                            Error("A relationship references a left-field that doesn't exist: {0}", relationship.LeftField);
                        }
                    } else {
                        foreach (var j in relationship.Join.Where(j => !fieldKeys.Contains(j.LeftField))) {
                            Error("A relationship join references a left-field that doesn't exist: {0}", j.LeftField);
                        }
                    }
                } else {
                    Error("A relationship references a left-entity that doesn't exist: {0}", relationship.LeftEntity);
                }

                //validate right side
                if (keys.Contains(relationship.RightEntity)) {
                    var fieldKeys = GetEntityFieldKeys(relationship.RightEntity);
                    if (relationship.Join.Count == 0) {
                        if (!fieldKeys.Contains(relationship.RightField)) {
                            Error("A relationship references a right-field that doesn't exist: {0}",
                                relationship.RightField);
                        }
                    } else {
                        foreach (var j in relationship.Join.Where(j => !fieldKeys.Contains(j.RightField))) {
                            Error("A relationship join references a right-field that doesn't exist: {0}", j.RightField);
                        }
                    }
                } else {
                    Error("A relationship references a right-entity that doesn't exist: {0}", relationship.LeftEntity);
                }
            }

        }

        private HashSet<string> GetEntityFieldKeys(string nameOrAlias) {
            var keys = new HashSet<string>();
            var entity = Entities.FirstOrDefault(e => e.Alias == nameOrAlias || e.Name == nameOrAlias);
            if (entity == null)
                return keys;

            foreach (var field in entity.GetAllFields()) {
                keys.Add(field.Alias);
                if (field.Alias != field.Name) {
                    keys.Add(field.Name);
                }
            }
            return keys;
        }

        private HashSet<string> GetEntityNames() {
            var keys = new HashSet<string>();
            foreach (var entity in Entities) {
                keys.Add(entity.Alias);
                if (entity.Alias != entity.Name) {
                    keys.Add(entity.Name);
                }
            }
            return keys;
        }

        private void ValidateLogConnections() {
            if (Log.Count <= 0)
                return;

            foreach (var log in Log.Where(log => log.Connection != Common.DefaultValue).Where(log => Connections.All(c => c.Name != log.Connection))) {
                Error(string.Format("Log {0}'s connection {1} doesn't exist.", log.Name, log.Connection));
            }
        }

        private void ValidateDuplicateFields() {
            var fieldDuplicates = Entities
                .SelectMany(e => e.GetAllFields())
                .Where(f => !f.PrimaryKey)
                .Union(CalculatedFields)
                .GroupBy(f => f.Alias)
                .Where(group => @group.Count() > 1)
                .Select(group => @group.Key)
                .ToArray();
            foreach (var duplicate in fieldDuplicates) {
                Error(
                    string.Format(
                        "The entity field '{0}' occurs more than once. Remove, alias, or prefix one.",
                        duplicate));
            }
        }

        private void ValidateDuplicateEntities() {
            var entityDuplicates = Entities
                .GroupBy(e => e.Alias)
                .Where(group => @group.Count() > 1)
                .Select(group => @group.Key)
                .ToArray();
            foreach (var duplicate in entityDuplicates) {
                Error(string.Format("The '{0}' entity occurs more than once. Remove or alias one.", duplicate));
            }
        }

        // Register Dependencies
        public IKernel Register(ILogger logger) {
            RegisterVelocity(logger);
            RegisterRazor(logger);

            return new StandardKernel(new NinjectBindings(this, logger));
        }

        private void RegisterRazor(ILogger logger) {
            if (TemplateContentType == "html")
                return;

            if (!Templates.Any() && !GetAllTransforms().Any(t => t.Method == "template" || t.Method == "razor"))
                return;

            var config = new FluentTemplateServiceConfiguration(c => c.WithEncoding(Encoding.Raw));
            var templateService = new TemplateService(config);
            logger.Debug("Switching RazorEngine content type to raw.");
            Razor.SetTemplateService(templateService);
        }

        private void RegisterVelocity(ILogger logger) {
            if (GetAllTransforms().All(t => t.Method != "velocity") && !Templates.Any(t => t.Engine.Equals("velocity")))
                return;

            logger.Info("Initializing nVelocity");
            Velocity.SetProperty(RuntimeConstants.RUNTIME_LOG_LOGSYSTEM, typeof(VelocityLogSystem).FullName);
            Velocity.Init();
        }

        /// <summary>
        /// Resolve Dependencies
        /// </summary>
        /// <param name="kernal"></param>
        public void Resolve(IKernel kernal) {
            foreach (var connection in Connections) {
                var parameters = new IParameter[] {
                    new ConstructorArgument("element", connection)
                };
                connection.Connection = kernal.Get<AbstractConnection>(connection.Provider, parameters);
                connection.Connection.TypeAndAssemblyName = _providerTypes[connection.Provider];

                if (connection.Provider != "solr")
                    continue;

                var solr = (SolrConnection)connection.Connection;
                foreach (var coreUrl in Entities.Select(entity => solr.GetCoreUrl(Common.EntityOutputName(entity, Name)))) {
                    solr.ReadOnlyOperationCores[coreUrl] = kernal.Get<ISolrReadOnlyOperations<Dictionary<string, object>>>(coreUrl);
                    solr.OperationCores[coreUrl] = kernal.Get<ISolrOperations<Dictionary<string, object>>>(coreUrl);
                }
            }

            foreach (var log in Log) {
                if (log.Connection == Common.DefaultValue)
                    continue;

                var connection = Connections.First(c => c.Name == log.Connection);
                var parameters = new IParameter[] {
                    new ConstructorArgument("element", connection)
                };
                log.ConnectionInstance = kernal.Get<AbstractConnection>(connection.Provider, parameters);
                log.ConnectionInstance.TypeAndAssemblyName = _providerTypes[connection.Provider];
            }
        }

    }
}