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
using System.Linq;
using Transformalize.Libs.EnterpriseLibrary.Validation;
using Transformalize.Libs.Ninject.Syntax;
using Transformalize.Libs.NLog;
using Transformalize.Libs.Ninject;
using Transformalize.Libs.RazorEngine;
using Transformalize.Libs.Rhino.Etl;
using Transformalize.Libs.Rhino.Etl.Operations;
using Transformalize.Main.Providers;
using Transformalize.Operations.Validate;

namespace Transformalize.Main {

    public class Process {

        private ValidationResults _validationResults = new ValidationResults();
        private static readonly Stopwatch Timer = new Stopwatch();
        private readonly Logger _log = LogManager.GetLogger("tfl");
        private const StringComparison IC = StringComparison.OrdinalIgnoreCase;
        private List<IOperation> _transformOperations = new List<IOperation>();
        private IParameters _parameters = new Parameters.Parameters();
        private bool _enabled = true;
        private Dictionary<string, AbstractConnection> _connections = new Dictionary<string, AbstractConnection>();

        // fields (for now)
        public Fields CalculatedFields = new Fields();
        public List<Entity> Entities = new List<Entity>();
        public IKernel Kernal = new StandardKernel();
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
        public long Anything { get; set; }

        // properties
        public string Star { get; set; }
        public string Bcp { get; set; }
        public string TimeZone { get; set; }
        public bool IsFirstRun { get; set; }

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

        public ValidationResults ValidationResults {
            get { return _validationResults; }
            set { _validationResults = value; }
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

        //constructor
        public Process(string name = "") {
            Name = name;
            Kernal.Load<NinjectBindings>();
        }

        //methods
        public bool IsReady() {
            if (Enabled || Options.Force) {
                if (Connections.All(cn => cn.Value.IsReady())) {
                    return true;
                }
                _log.Warn("Process is not ready.");
                return false;
            }
            _log.Error("Process is disabled. Data is not being updated.");
            return false;
        }

        public IDictionary<string, IEnumerable<Row>> Run() {
            Timer.Start();
            var results = Options.ProcessRunner.Run(this);
            Options.ProcessRunner.Dispose();
            Timer.Stop();
            _log.Info("Process affected {0} records in {1}.", Anything, Timer.Elapsed);
            return results;
        }

        public Fields OutputFields() {
            var fields = new Fields();
            foreach (var entity in Entities) {
                fields.AddRange(entity.Fields.Output());
                fields.AddRange(entity.CalculatedFields.Output());
                fields.AddRange(CalculatedFields.Output());
            }
            return fields;
        }

        public IEnumerable<Field> SearchFields() {
            return OutputFields().OrderedFields().Where(f => !f.SearchTypes.Any(st => st.Name.Equals("none")));
        }

        public Entity this[string entity] {
            get {
                return Entities.Find(e => e.Alias.Equals(entity, IC) || e.Name.Equals(entity, IC));
            }
        }

        public int GetNextBatchId() {
            return OutputConnection.NextBatchId(Name);
        }

        public Field GetField(string alias, string entity, bool issueWarning = true) {

            foreach (var fields in Entities.Where(e => e.Alias == entity || entity == string.Empty).Select(e => e.Fields.OrderedFields()).Where(fields => fields.Any(Common.FieldFinder(alias)))) {
                return fields.First(Common.FieldFinder(alias));
            }

            foreach (var fields in Entities.Where(e => e.Alias == entity || entity == string.Empty).Select(e => e.CalculatedFields.OrderedFields()).Where(fields => fields.Any(Common.FieldFinder(alias)))) {
                return fields.First(Common.FieldFinder(alias));
            }

            var calculatedfields = CalculatedFields.OrderedFields().ToArray();
            if (calculatedfields.Any(Common.FieldFinder(alias))) {
                return calculatedfields.First(Common.FieldFinder(alias));
            }

            if (!IsValidationResultField(alias, entity) && issueWarning) {
                _log.Warn("Can't find field with alias: {0}.", alias);
            }

            return null;
        }

        private bool IsValidationResultField(string alias, string entity) {
            return Entities
                    .Where(e => e.Alias == entity || entity == string.Empty)
                    .Any(e => e.OperationsAfterAggregation.OfType<ValidationOperation>().Any(operation => operation.ResultKey.Equals(alias)));
        }

        public bool TryGetField(string alias, string entity, out Field field, bool issueWarning = true) {
            field = GetField(alias, entity, issueWarning);
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
    }
}