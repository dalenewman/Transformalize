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
using System.Linq;
using System.Text;
using Transformalize.Extensions;
using Transformalize.Libs.Dapper;
using Transformalize.Libs.Newtonsoft.Json.Utilities;
using Transformalize.Libs.NLog;
using Transformalize.Libs.Ninject;
using Transformalize.Libs.NLog.Config;
using Transformalize.Libs.NLog.Targets;
using Transformalize.Libs.NLog.Targets.Wrappers;
using Transformalize.Libs.Rhino.Etl;
using Transformalize.Libs.Rhino.Etl.Operations;
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
        public IEnumerable<Row> Results = Enumerable.Empty<Row>();
        public Fields CalculatedFields = new Fields();
        public List<Entity> Entities = new List<Entity>();
        public IKernel Kernal;
        public Dictionary<string, Map> MapEndsWith = new Dictionary<string, Map>();
        public Dictionary<string, Map> MapEquals = new Dictionary<string, Map>();
        public Dictionary<string, Map> MapStartsWith = new Dictionary<string, Map>();
        public Entity MasterEntity;
        public string Name;
        public Options Options = new Options();
        public Dictionary<string, string> Providers = new Dictionary<string, string>();
        public List<Relationship> Relationships = new List<Relationship>();
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

        //constructor
        public Process(string name = "") {
            Name = name;
        }

        //methods
        public bool IsReady() {
            if (Enabled || Options.Force) {
                if (Connections.All(cn => cn.Value.IsReady())) {
                    Setup();
                    return true;
                }
                TflLogger.Warn(Name, string.Empty, "Process is not ready.");
                return false;
            }
            TflLogger.Error(Name, string.Empty, "Process is disabled. Data is not being updated.");
            return false;
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
            SetLog();
            using (var runner = GetRunner()) {
                var p = this;
                runner.Run(ref p);
            }
        }

        public IEnumerable<Row> Execute() {
            SetLog();
            using (var runner = GetRunner()) {
                var p = this;
                return runner.Run(ref p);
            }
        }

        public void SetLog() {

            try {

                if (Log == null || Log.Count <= 0)
                    return;

                //preserve memory target (if exists)
                MemoryTarget memoryTarget = null;
                var logs = new string[0];
                if (LogManager.Configuration.AllTargets.Any(t => t is MemoryTarget || t.Name == "memory")) {
                    TflLogger.Info(Name, string.Empty, "Found memory logging target");
                    memoryTarget = (MemoryTarget)LogManager.Configuration.AllTargets.First(t => t is MemoryTarget || t.Name == "memory");
                    if (memoryTarget.Logs != null) {
                        var keep = memoryTarget.Logs.ToArray();
                        if (keep.Length > 0) {
                            logs = new string[keep.Length];
                            Array.Copy(keep, logs, keep.Length); ;
                        }
                    }
                }

                //force a console target if interactive (i.e. in console app)
                if (Environment.UserInteractive && Log.All(l => l.Provider != ProviderType.Console)) {
                    Log.Insert(0, new Log() {
                        Name = "console",
                        Provider = ProviderType.Console
                    });
                    TflLogger.Info(Name, string.Empty, "Added console logging target");
                }

                var config = new LoggingConfiguration();

                foreach (var log in Log) {
                    switch (log.Provider) {
                        case ProviderType.Console:
                            //console
                            var consoleTarget = new ColoredConsoleTarget {
                                Name = log.Name,
                                Layout = log.Layout.Equals(Common.DefaultValue) ? @"${date:format=HH\:mm\:ss} | ${message}" : log.Layout
                            };
                            var consoleRule = new LoggingRule("tfl", log.Level, consoleTarget);
                            config.AddTarget(log.Name, consoleTarget);
                            config.LoggingRules.Add(consoleRule);
                            break;
                        case ProviderType.File:
                            var folder = (log.Folder.Equals(Common.DefaultValue) ? "${basedir}/logs" : log.Folder).TrimEnd('/') + "/";
                            var fileName = (log.File.Equals(Common.DefaultValue) ? "tfl-" + Name + "-${date:format=yyyy-MM-dd}.log" : log.File).TrimStart('/');
                            var fileTarget = new AsyncTargetWrapper(new FileTarget {
                                Name = log.Name,
                                FileName = folder + fileName,
                                Layout = log.Layout.Equals(Common.DefaultValue) ? @"${date:format=HH\:mm\:ss} | ${message}" : log.Layout
                            });
                            var fileRule = new LoggingRule("tfl", log.Level, fileTarget);
                            config.AddTarget(log.Name, fileTarget);
                            config.LoggingRules.Add(fileRule);
                            break;
                        case ProviderType.Mail:
                            if (log.Connection == null) {
                                throw new TransformalizeException("The mail logger needs to reference a mail connection in <connections/> collection.");
                            }
                            var mailTarget = new MailTarget {
                                Name = log.Name,
                                SmtpPort = log.Connection.Port.Equals(0) ? 25 : log.Connection.Port,
                                SmtpUserName = log.Connection.User,
                                SmtpPassword = log.Connection.Password,
                                SmtpServer = log.Connection.Server,
                                EnableSsl = log.Connection.EnableSsl,
                                Subject = log.Subject.Equals(Common.DefaultValue) ? "Tfl Error (" + Name + ")" : log.Subject,
                                From = log.From,
                                To = log.To,
                                Layout = log.Layout.Equals(Common.DefaultValue) ? @"${date:format=HH\:mm\:ss} | ${message}" : log.Layout
                            };
                            var mailRule = new LoggingRule("tfl", LogLevel.Error, mailTarget);
                            config.AddTarget(log.Name, mailTarget);
                            config.LoggingRules.Add(mailRule);
                            break;
                        default:
                            throw new TransformalizeException("Log does not support {0} provider.", log.Provider);
                    }

                }

                LogManager.Configuration = config;
                if (memoryTarget == null)
                    return;

                SimpleConfigurator.ConfigureForTargetLogging(memoryTarget);
                if (logs.Length == 0 || memoryTarget.Logs != null && memoryTarget.Logs.Count > 0)
                    return;

                TflLogger.Info(Name, string.Empty, "Preserving {0} memory log entr{1}.", logs.Length, logs.Length.Pluralize());
                memoryTarget.Logs.AddRange(logs);

            } catch (Exception ex) {
                TflLogger.Warn(Name, string.Empty, "Troubling handling logging configuration. {0} {1}", ex.Message, ex.StackTrace);
            }
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
                TflLogger.Warn(Name, entity, "Can't find field with alias: {0}.", alias);
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
            var schema = master.SchemaPrefix(l, r);

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
            builder.Append(schema);
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
                    builder.Append(rel.RightEntity.SchemaPrefix(l, r));
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
            var view = (MasterEntity.PrependProcessNameToOutputName ? Common.EntityOutputName(MasterEntity, Name) : MasterEntity.Alias) + View;
            var fullName = MasterEntity.SchemaPrefix(OutputConnection.L, OutputConnection.R) + OutputConnection.Enclose(view);

            var connection = OutputConnection.GetConnection();
            connection.Open();

            if (MasterEntity.NeedsSchema()) {
                if (connection.Query("SELECT TOP(1) TABLE_NAME FROM INFORMATION_SCHEMA.VIEWS WHERE TABLE_SCHEMA = @Schema AND TABLE_NAME = @View;", new { MasterEntity.Schema, View = view }).Any()) {
                    connection.Execute(string.Format("DROP VIEW {0};", fullName));
                }
            } else {
                if (connection.Query("SELECT TOP(1) TABLE_NAME FROM INFORMATION_SCHEMA.VIEWS WHERE TABLE_NAME = @View;", new { View = view }).Any()) {
                    connection.Execute(string.Format("DROP VIEW {0};", fullName));
                }
            }

            TflLogger.Debug(Name, string.Empty, ViewSql());
            connection.Execute(string.Format("CREATE VIEW {0} AS {1}", fullName, ViewSql()));
            connection.Close();
        }

        public bool IsInitMode() {
            return Mode.Equals("init", StringComparison.OrdinalIgnoreCase);
        }

    }
}