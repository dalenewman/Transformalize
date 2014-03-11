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
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using Transformalize.Libs.EnterpriseLibrary.Validation;
using Transformalize.Libs.Ninject.Syntax;
using Transformalize.Libs.NLog;
using Transformalize.Libs.Ninject;
using Transformalize.Libs.RazorEngine;
using Transformalize.Libs.Rhino.Etl;
using Transformalize.Libs.Rhino.Etl.Operations;
using Transformalize.Main.Providers;
using Transformalize.Main.Providers.AnalysisServices;
using Transformalize.Main.Providers.File;
using Transformalize.Main.Providers.Folder;
using Transformalize.Main.Providers.Internal;
using Transformalize.Main.Providers.MySql;
using Transformalize.Main.Providers.SqlServer;
using Transformalize.Operations.Validate;

namespace Transformalize.Main {

    public class Process {

        private ValidationResults _validationResults = new ValidationResults();
        private static readonly Stopwatch Timer = new Stopwatch();
        private readonly Logger _log = LogManager.GetLogger(string.Empty);
        private const StringComparison IC = StringComparison.OrdinalIgnoreCase;
        private List<IOperation> _transformOperations = new List<IOperation>();
        private IParameters _parameters = new Parameters.Parameters();
        private bool _enabled = true;

        // fields (for now)
        public Fields CalculatedFields = new Fields();
        public Dictionary<string, AbstractConnection> Connections = new Dictionary<string, AbstractConnection>();
        public Entities Entities = new Entities();
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

        // properties
        public string Star { get; set; }
        public string Bcp { get; set; }
        public string TimeZone { get; set; }
        public bool IsFirstRun { get; set; }

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

        //constructor
        public Process(string name = "") {
            Name = name;
            GlobalDiagnosticsContext.Set("process", name);
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

        public IEnumerable<IEnumerable<Row>> Run() {
            Timer.Start();
            var results = Options.ProcessRunner.Run(this);
            Options.ProcessRunner.Dispose();
            Timer.Stop();
            _log.Info("Process completed in {0}.", Timer.Elapsed);
            return results;
        }

        public Fields OutputFields() {
            var fields = new Fields();
            foreach (var entity in Entities) {
                fields.AddRange(new FieldSqlWriter(entity.Fields, entity.CalculatedFields, CalculatedFields).Output().ToArray());
            }
            return fields;
        }

        public IEnumerable<Field> SearchFields() {
            return new StarFields(this).Fields().Where(f => !f.SearchTypes.Any(st => st.Name.Equals("none")));
        }

        public Entity this[string entity] {
            get {
                return Entities.Find(e => e.Alias.Equals(entity, IC) || e.Name.Equals(entity, IC));
            }
        }

        public int GetNextBatchId() {
            return OutputConnection.NextBatchId(Name);
        }

        public Field GetField(string alias, string entity) {

            foreach (var fields in Entities.Where(e => e.Alias == entity || entity == string.Empty).Select(e => e.Fields.ToEnumerable()).Where(fields => fields.Any(Common.FieldFinder(alias)))) {
                return fields.First(Common.FieldFinder(alias));
            }

            foreach (var fields in Entities.Where(e => e.Alias == entity || entity == string.Empty).Select(e => e.CalculatedFields.ToEnumerable()).Where(fields => fields.Any(Common.FieldFinder(alias)))) {
                return fields.First(Common.FieldFinder(alias));
            }

            var calculatedfields = CalculatedFields.ToEnumerable().ToArray();
            if (calculatedfields.Any(Common.FieldFinder(alias))) {
                return calculatedfields.First(Common.FieldFinder(alias));
            }

            if (!IsValidationResultField(alias, entity)) {
                _log.Warn("Can't find field with alias: {0}.", alias);
            }

            return null;
        }

        private bool IsValidationResultField(string alias, string entity) {
            return Entities
                    .Where(e => e.Alias == entity || entity == string.Empty)
                    .Any(e => e.Operations.OfType<ValidationOperation>().Any(operation => operation.ResultKey.Equals(alias)));
        }

        public bool TryGetField(string alias, string entity, out Field field) {
            field = GetField(alias, entity);
            return field != null;
        }

        public void CreateOutput(Entity entity) {
            OutputConnection.Create(this, entity);
        }
    }
}