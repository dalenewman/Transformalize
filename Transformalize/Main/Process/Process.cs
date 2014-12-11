#region License

// /*
// Transformalize - Replicate, Transform, and Denormalize Your Data...
// Copyright (C) 2013 Dale Newman
// 
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with this program.  If not, see <http://www.gnu.org/licenses/>.
// */

#endregion

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Tracing;
using System.IO;
using System.Linq;
using System.Text;
using Transformalize.Extensions;
using Transformalize.Libs.Dapper;
using Transformalize.Libs.Lucene.Net.Store;
using Transformalize.Libs.Ninject;
using Transformalize.Libs.Rhino.Etl;
using Transformalize.Libs.Rhino.Etl.Operations;
using Transformalize.Libs.SemanticLogging;
using Transformalize.Libs.SemanticLogging.TextFile;
using Transformalize.Libs.SemanticLogging.TextFile.Sinks;
using Transformalize.Logging;
using Transformalize.Main.Providers;
using Transformalize.Main.Providers.File;
using Transformalize.Operations.Validate;
using Transformalize.Runner;
using Encoding = Transformalize.Libs.RazorEngine.Encoding;

namespace Transformalize.Main {

    public class Process {

        private const StringComparison IC = StringComparison.OrdinalIgnoreCase;
        private List<IOperation> _transformOperations = new List<IOperation>();
        private IParameters _parameters = new Parameters.Parameters();
        private bool _enabled = true;
        private Dictionary<string, AbstractConnection> _connections = new Dictionary<string, AbstractConnection>();

        // fields (for now)
        public bool Complete = false;
        public IEnumerable<Row> Results = Enumerable.Empty<Row>();
        public Fields CalculatedFields = new Fields();
        public Entities Entities = new Entities();
        public IKernel Kernal;
        public Dictionary<string, Map> MapEndsWith = new Dictionary<string, Map>();
        public Dictionary<string, Map> MapEquals = new Dictionary<string, Map>();
        public Dictionary<string, Map> MapStartsWith = new Dictionary<string, Map>();
        public Entity MasterEntity;
        public string Name;
        public Options Options = new Options();
        public Dictionary<string, string> Providers = new Dictionary<string, string>();
        public List<Relationship> Relationships = new List<Relationship>();
        public string RelationshipsIndexMode = "";
        public Dictionary<string, Script> Scripts = new Dictionary<string, Script>();
        public Dictionary<string, SearchType> SearchTypes = new Dictionary<string, SearchType>();
        public Encoding TemplateContentType = Encoding.Raw;
        public Dictionary<string, Template> Templates = new Dictionary<string, Template>();
        public AbstractConnection OutputConnection;
        private PipelineThreading _pipelineThreading = PipelineThreading.MultiThreaded;
        private string _star = Common.DefaultValue;
        private string _view = Common.DefaultValue;
        private string _mode;
        private long _logRows = 10000;
        private List<Log> _logList = new List<Log>();
        private bool _shouldLog = true;
        private bool _parallel = true;
        private List<ObservableEventListener> _eventListeners = new List<ObservableEventListener>();
        private List<SinkSubscription> _sinkSubscriptions = new List<SinkSubscription>();

        // properties
        public string TimeZone { get; set; }
        public bool IsFirstRun { get; set; }
        public long Anything { get; set; }
        public bool StarEnabled { get; set; }

        public string Star {
            get { return _star.Equals(Common.DefaultValue) ? Name + "Star" : _star; }
            set { _star = value; }
        }

        public string View {
            get { return _view.Equals(Common.DefaultValue) ? Name + "View" : _view; }
            set { _view = value; }
        }

        public string Mode {
            get { return _mode; }
            set { _mode = value.ToLower(); }
        }

        public Dictionary<string, AbstractConnection> Connections {
            get { return _connections; }
            set { _connections = value; }
        }

        public PipelineThreading PipelineThreading {
            get { return _pipelineThreading; }
            set { _pipelineThreading = value; }
        }

        public bool Enabled {
            get { return _enabled; }
            set { _enabled = value; }
        }

        public List<IOperation> TransformOperations {
            get { return _transformOperations; }
            set { _transformOperations = value; }
        }

        public IParameters Parameters {
            get { return _parameters; }
            set { _parameters = value; }
        }

        public IEnumerable<TemplateAction> Actions { get; set; }

        public FileInspectionRequest FileInspectionRequest { get; set; }

        public long LogRows {
            get { return _logRows; }
            set { _logRows = value; }
        }

        public List<Log> Log {
            get { return _logList; }
            set { _logList = value; }
        }

        public bool VelocityInitialized { get; set; }

        public bool ShouldLog {
            get { return _shouldLog; }
            set { _shouldLog = value; }
        }

        public bool Parallel {
            get { return _parallel; }
            set { _parallel = value; }
        }

        //constructor
        public Process(string name = "") {
            Name = name;
        }

        //methods
        public void CheckIfReady() {
            if (!Enabled && !Options.Force)
                throw new TransformalizeException(Name, string.Empty, "Process is disabled.");

            foreach (var connection in Connections.Where(cn => !cn.Value.IsReady())) {
                throw new TransformalizeException(Name, string.Empty, "Connection {0} failed.", connection.Key);
            }
        }

        private IProcessRunner GetRunner() {
            switch (Mode) {
                case "init":
                    return new InitializeRunner();
                case "metadata":
                    return new MetadataRunner();
                default:
                    return new ProcessRunner();
            }
        }

        public void ExecuteScaler() {
            StartLogging();
            var p = this;
            using (var runner = GetRunner()) {
                runner.Run(ref p);
            }
            StopLogging();
        }

        public IEnumerable<Row> Execute() {
            StartLogging();
            var p = this;
            using (var runner = GetRunner()) {
                var rows = runner.Run(ref p);
                StopLogging();
                return rows;
            }
        }

        public List<ObservableEventListener> EventListeners {
            get { return _eventListeners; }
            set { _eventListeners = value; }
        }

        public void StopLogging() {
            foreach (var listener in EventListeners) {
                listener.DisableEvents(TflEventSource.Log);
                listener.Dispose();
            }
            EventListeners.Clear();
            foreach (var sink in SinkSubscriptions) {
                sink.Dispose();
            }
            SinkSubscriptions.Clear();
        }

        public void StartLogging() {

            try {
                if (!ShouldLog || Log == null || Log.Count <= 0)
                    return;

                foreach (var log in Log) {
                    switch (log.Provider) {
                        case ProviderType.File:
                            log.Folder = log.Folder.Replace('/', '\\');
                            log.File = log.File.Replace('/', '\\');
                            log.Folder = (log.Folder.Equals(Common.DefaultValue) ? "logs" : log.Folder).TrimEnd('\\') + "\\";
                            log.File = (log.File.Equals(Common.DefaultValue) ? "tfl-" + Name + ".log" : log.File).TrimStart('\\');

                            var fileListener = new ObservableEventListener();
                            fileListener.EnableEvents(TflEventSource.Log, log.Level);
                            SinkSubscriptions.Add(fileListener.LogToRollingFlatFile(log.Folder + log.File, 5000, "yyyy-MM-dd", RollFileExistsBehavior.Increment, RollInterval.Day, new LegacyLogFormatter(), 0, log.Async));
                            EventListeners.Add(fileListener);
                            break;
                        case ProviderType.Mail:
                            if (log.Connection == null) {
                                throw new TransformalizeException(Name, string.Empty, "The mail logger needs to reference a mail connection in <connections/> collection.");
                            }
                            if (log.Subject.Equals(Common.DefaultValue)) {
                                log.Subject = Name + " " + log.Level;
                            }
                            var mailListener = new ObservableEventListener();
                            mailListener.EnableEvents(TflEventSource.Log, EventLevel.Error);
                            SinkSubscriptions.Add(mailListener.LogToEmail(log));
                            EventListeners.Add(mailListener);
                            break;
                        case ProviderType.ElasticSearch:
                            //note: does not work
                            if (log.Connection == null) {
                                throw new TransformalizeException(Name, string.Empty, "The elasticsearch logger needs to reference an elasticsearch connection in the <connections/> collection.");
                            }
                            var elasticListener = new ObservableEventListener();
                            elasticListener.EnableEvents(TflEventSource.Log, log.Level);
                            SinkSubscriptions.Add(
                                elasticListener.LogToElasticSearch(
                                    Name,
                                    log,
                                    TimeSpan.FromSeconds(5),
                                    TimeSpan.FromSeconds(10),
                                    500,
                                    2000
                                )
                            );
                            EventListeners.Add(elasticListener);
                            break;
                    }

                }
            } catch (Exception ex) {
                if (!ShouldLog || Log == null || Log.Count <= 0)
                    return;
                foreach (var exception in ex.FlattenHierarchy()) {
                    if (exception is IOException) {
                        TflLogger.Warn(Name, string.Empty, exception.Message);

                    } else {
                        TflLogger.Warn(Name, string.Empty, "Troubling handling logging configuration. {0} {1} {2}", exception.Message, exception.StackTrace, exception.GetType());
                    }
                }
            }

            foreach (var log in Log) {
                switch (log.Provider) {
                    case ProviderType.File:
                        TflLogger.Info(Name, "Log", "Writing errors to {0}", log.Folder + log.File);
                        break;
                    case ProviderType.Mail:
                        TflLogger.Info(Name, "Log", "Mailing errors to {0}", log.To);
                        break;
                    case ProviderType.ElasticSearch:
                        TflLogger.Info(Name, "Log", "Writing errors to {0}Transformalize/LogEntry", log.Connection.Uri());
                        break;
                    default:
                        TflLogger.Warn(Name, "Log", "Log does not support {0} provider. You may only add mail and file providers from the configuration.", log.Provider);
                        break;
                }
            }


        }

        public List<SinkSubscription> SinkSubscriptions {
            get { return _sinkSubscriptions; }
            set { _sinkSubscriptions = value; }
        }

        public Fields OutputFields() {
            return Fields().WithOutput();
        }

        public Fields Fields() {
            var fields = new Fields();
            foreach (var entity in Entities) {
                fields.Add(entity.Fields);
                fields.Add(entity.CalculatedFields);
            }
            fields.Add(CalculatedFields);
            return fields;
        }

        public Fields SearchFields() {
            var fields = new Fields();
            foreach (var pair in new StarFields(this).TypedFields()) {
                fields.Add(pair.Value.WithSearchType());
            }
            return fields;
        }

        public Entity this[string entity] {
            get {
                return Entities.Find(e => e.Alias.Equals(entity, IC) || e.Name.Equals(entity, IC));
            }
        }

        public int GetNextBatchId() {
            if ((new[] { "init", "metadata" }).Any(m => m.Equals(Mode)) || !OutputConnection.IsDatabase)
                return 1;
            return OutputConnection.NextBatchId(Name);
        }

        public Field GetField(string entity, string alias, bool issueWarning = true) {

            foreach (var fields in Entities.Where(e => e.Alias == entity || entity == string.Empty).Select(e => e.Fields).Where(fields => fields.Find(alias).Any())) {
                return fields.Find(alias).First();
            }

            foreach (var fields in Entities.Where(e => e.Alias == entity || entity == string.Empty).Select(e => e.CalculatedFields).Where(fields => fields.Find(alias).Any())) {
                return fields.Find(alias).First();
            }

            if (CalculatedFields.Find(alias).Any()) {
                return CalculatedFields.Find(alias).First();
            }

            if (!IsValidationResultField(alias, entity) && issueWarning) {
                TflLogger.Warn(Name, entity, "Can't find field with alias {0}", alias);
            }

            return null;
        }

        private bool IsValidationResultField(string alias, string entity) {
            return Entities
                    .Where(e => e.Alias == entity || entity == string.Empty)
                    .Any(e => e.OperationsAfterAggregation.OfType<ValidationOperation>().Any(operation => operation.ResultKey.Equals(alias)));
        }

        public bool TryGetField(string entity, string alias, out Field field, bool issueWarning = true) {
            field = GetField(entity, alias, issueWarning);
            return field != null;
        }

        public void CreateOutput(Entity entity) {
            OutputConnection.Create(this, entity);
        }

        public bool UpdatedAnything() {
            return Anything > 0;
        }

        public void PerformActions(Func<TemplateAction, bool> filter) {
            if (Actions.Any(filter)) {
                foreach (var action in Actions.Where(filter)) {
                    if (action.Conditional) {
                        if (UpdatedAnything()) {
                            action.Handle(FindFile(action));
                        }
                    } else {
                        action.Handle(FindFile(action));
                    }
                }
            }
        }

        private static string FindFile(TemplateAction action) {
            var file = action.File;
            if (!string.IsNullOrEmpty(file))
                return file;

            if (action.Connection != null && (action.Connection.Type == ProviderType.File || action.Connection.Type == ProviderType.Html)) {
                file = action.Connection.File;
            }
            return file;
        }

        public void Setup() {
            IsFirstRun = false;
            Anything = 0;
            var batchId = GetNextBatchId();
            foreach (var entity in Entities) {
                entity.Inserts = 0;
                entity.Updates = 0;
                entity.Deletes = 0;
                entity.Sampled = false;
                entity.HasRows = false;
                entity.HasRange = false;
                entity.TflBatchId = batchId;
                batchId++;
            }
        }

        public string ViewSql() {
            const string fieldSpacer = ",\r\n    ";
            var builder = new StringBuilder();
            var master = MasterEntity;
            var l = OutputConnection.L;
            var r = OutputConnection.R;

            builder.AppendLine("SELECT");
            builder.Append("    ");
            builder.Append(new FieldSqlWriter(master.OutputFields()).Alias(l, r).Prepend("m.").Write(fieldSpacer));

            foreach (var rel in Relationships) {
                var joinFields = rel.Fields();
                foreach (var field in rel.RightEntity.OutputFields()) {
                    if (!joinFields.HaveField(field.Alias)) {
                        builder.Append(fieldSpacer);
                        builder.Append(new FieldSqlWriter(new Fields(field)).Alias(l, r).Prepend("r" + rel.RightEntity.Index + ".").Write(fieldSpacer));
                    }
                }
            }

            builder.AppendLine();
            builder.Append("FROM ");
            builder.Append(l);
            builder.Append(master.OutputName());
            builder.Append(r);
            builder.Append(" m");

            foreach (var rel in Relationships) {
                builder.AppendLine();
                builder.Append("LEFT OUTER JOIN ");
                if (rel.RightEntity.IsMaster()) {
                    builder.Append("m");
                } else {
                    builder.Append(l);
                    builder.Append(rel.RightEntity.OutputName());
                    builder.Append(r);
                }
                builder.Append(" ");
                builder.Append("r");
                builder.Append(rel.RightEntity.Index);
                builder.Append(" ON (");
                foreach (var j in rel.Join) {
                    if (rel.LeftEntity.IsMaster()) {
                        builder.Append("m");
                    } else {
                        builder.Append("r");
                        builder.Append(rel.LeftEntity.Index);
                    }
                    builder.Append(".");
                    builder.Append(l);
                    builder.Append(j.LeftField.Alias);
                    builder.Append(r);
                    builder.Append(" = ");
                    builder.Append("r");
                    builder.Append(rel.RightEntity.Index);
                    builder.Append(".");
                    builder.Append(l);
                    builder.Append(j.RightField.Alias);
                    builder.Append(r);
                    builder.Append(" AND ");
                }
                builder.TrimEnd(" AND ");
                builder.Append(")");
            }

            builder.Append(";");
            return builder.ToString();

        }

        public void InitializeView() {

            if (!OutputConnection.IsDatabase || Relationships.Count <= 0)
                return;

            var connection = OutputConnection.GetConnection();
            connection.Open();

            if (connection.Query("SELECT TOP(1) TABLE_NAME FROM INFORMATION_SCHEMA.VIEWS WHERE TABLE_NAME = @View;", new { View }).Any()) {
                connection.Execute(string.Format("DROP VIEW {0};", OutputConnection.Enclose(View)));
            }

            var outputSql = string.Format("CREATE VIEW {0} AS {1}", OutputConnection.Enclose(View), ViewSql());
            TflLogger.Debug(Name, string.Empty, outputSql);
            connection.Execute(outputSql);
            connection.Close();
        }

        public bool IsInitMode() {
            return Mode.Equals("init", StringComparison.OrdinalIgnoreCase);
        }

    }
}