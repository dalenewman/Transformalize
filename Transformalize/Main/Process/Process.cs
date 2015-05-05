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
using Transformalize.Configuration;
using Transformalize.Libs.Rhino.Etl;
using Transformalize.Libs.Rhino.Etl.Operations;
using Transformalize.Logging;
using Transformalize.Main.Providers;
using Transformalize.Main.Providers.File;
using Transformalize.Operations.Validate;
using Transformalize.Runner;
using Encoding = Transformalize.Libs.RazorEngine.Encoding;

namespace Transformalize.Main {

    public class Process {

        private const StringComparison IC = StringComparison.OrdinalIgnoreCase;

        // fields (for now)
        public IEnumerable<Row> Results = Enumerable.Empty<Row>();
        public Fields CalculatedFields = new Fields();
        public Entities Entities = new Entities();
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

        private string _mode;

        // properties
        public TflProcess Configuration { get; set; }
        public string TimeZone { get; set; }
        public bool IsFirstRun { get; set; }
        public long Anything { get; set; }
        public bool StarEnabled { get; set; }
        public bool ViewEnabled { get; set; }
        public IParameters Parameters { get; set; }
        public IEnumerable<TemplateAction> Actions { get; set; }
        public FileInspectionRequest FileInspectionRequest { get; set; }
        public Dictionary<string, List<Row>> DataSets { get; set; }
        public ILogger Logger { get; private set; }
        public Connections Connections { get; set; }
        public PipelineThreading PipelineThreading { get; set; }
        public bool Enabled { get; set; }
        public List<IOperation> TransformOperations { get; set; }
        public long LogRows { get; set; }
        public bool Parallel { get; set; }
        public bool Complete { get; set; }
        public string Star { get; set; }
        public string View { get; set; }
        public IProcessRunner Runner { get; set; }

        public string Mode {
            get { return _mode; }
            set { _mode = value.ToLower(); }
        }

        //constructor
        public Process(string name, ILogger logger) {
            View = Common.DefaultValue;
            Star = Common.DefaultValue;
            Connections = new Connections();
            PipelineThreading = PipelineThreading.MultiThreaded;
            Enabled = true;
            TransformOperations = new List<IOperation>();
            Parallel = true;
            LogRows = 10000;
            Complete = false;
            Logger = logger;
            Name = name;
            DataSets = new Dictionary<string, List<Row>>();
            Parameters = new Parameters.Parameters(new DefaultFactory(logger));
            Runner = new NullRunner();
        }

        //methods
        public bool IsReady() {
            if (!Enabled && !Options.Force) {
                Logger.Warn("Process is disabled.");
                return false;
            }

            var checks = new Dictionary<string, bool>();
            foreach (var connection in Connections) {
                var ready = connection.Connection.IsReady();
                if (!ready) {
                    Logger.Error("Connection {0} is not ready.", connection.Name);
                }
                checks[connection.Name] = ready;
            }

            return checks.All(kv => kv.Value);
        }

        public void ExecuteScaler() {
            Logger.Start(Configuration);
            Runner.Run(this);
            Logger.Stop();
        }

        public IEnumerable<Row> Execute() {
            Logger.Start(Configuration);
            var rows = Runner.Run(this);
            Logger.Stop();
            return rows;
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

            foreach (var calculatedFields in Entities.Where(e => e.Alias == entity || entity == string.Empty).Select(e => e.CalculatedFields).Where(calculatedFields => calculatedFields.Find(alias).Any())) {
                return calculatedFields.Find(alias).First();
            }

            if (CalculatedFields.Find(alias).Any()) {
                return CalculatedFields.Find(alias).First();
            }

            if (!IsValidationResultField(alias, entity) && issueWarning) {
                Logger.Warn("Can't find field with alias {0}", alias);
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
            if (!Actions.Any(filter))
                return;

            foreach (var action in Actions.Where(filter)) {
                action.Handle(FindFile(action));
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

        public bool IsInitMode() {
            return Mode.Equals("init");
        }

    }
}