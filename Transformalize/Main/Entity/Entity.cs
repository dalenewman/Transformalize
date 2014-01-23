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
using System.Data;
using System.Globalization;
using System.Linq;
using Antlr.Runtime;
using Transformalize.Libs.Rhino.Etl;
using Transformalize.Libs.Rhino.Etl.Operations;
using Transformalize.Main.Providers;

namespace Transformalize.Main {
    public class Entity {
        private readonly int _tflBatchId;
        private List<AbstractOperation> _operations = new List<AbstractOperation>();
        private List<AbstractOperation> _validatorOperations = new List<AbstractOperation>();
        private IEnumerable<Row> _rows = new List<Row>();
        private const StringComparison IC = StringComparison.OrdinalIgnoreCase;

        public Entity(int batchId) {
            _tflBatchId = batchId;
            Name = string.Empty;
            Alias = string.Empty;
            Schema = string.Empty;
            PrimaryKey = new Fields();
            Fields = new Fields();
            Joins = new Dictionary<string, Relationship>();
            InputKeys = new List<Row>();
            Prefix = string.Empty;
            CalculatedFields = new Fields();
        }

        public string Schema { get; set; }
        public PipelineThreading PipelineThreading { get; set; }
        public string ProcessName { get; set; }
        public string Alias { get; set; }
        public AbstractConnection InputConnection { get; set; }
        public Field Version { get; set; }
        public Fields PrimaryKey { get; set; }
        public Fields Fields { get; set; }
        public Dictionary<string, Relationship> Joins { get; set; }
        public object Begin { get; set; }
        public object End { get; set; }
        public int InputCount { get; set; }
        public int OutputCount { get; set; }
        public IEnumerable<Relationship> RelationshipToMaster { get; set; }
        public List<Row> InputKeys { get; set; }
        public IDbCommand InputKeysCommand { get; set; }
        public string Prefix { get; set; }
        public bool Group { get; set; }
        public string Name { get; set; }
        public Fields CalculatedFields { get; set; }
        public bool HasRows { get; set; }
        public bool HasRange { get; set; }
        public long Updates { get; set; }
        public long Inserts { get; set; }
        public long Deletes { get; set; }
        public int TflBatchId { get { return _tflBatchId; } }
        public bool IsFirstRun { get; set; }
        public bool UseBcp { get; set; }
        public bool IndexOptimizations { get; set; }
        public bool Delete { get; set; }

        public IEnumerable<Row> Rows {
            get { return _rows; }
            set { _rows = value; }
        }

        public List<AbstractOperation> Operations {
            get { return _operations; }
            set { _operations = value; }
        }

        public string FirstKey() {
            return PrimaryKey.First().Key;
        }

        public bool IsMaster() {
            return PrimaryKey.Any(kv => kv.Value.FieldType.HasFlag(FieldType.MasterKey));
        }

        public string OutputName() {
            return Common.EntityOutputName(Alias, ProcessName);
        }

        public bool HasForeignKeys() {
            return Fields.Any(f => f.Value.FieldType.HasFlag(FieldType.ForeignKey));
        }

        public bool NeedsUpdate() {
            if (!HasRows)
                return false;

            return (!HasRange || !BeginAndEndAreEqual());
        }

        public List<string> SelectKeys(AbstractProvider p) {
            var selectKeys = new List<string>();
            foreach (var field in PrimaryKey.ToEnumerable().Where(f => f.Input)) {
                selectKeys.Add(field.Alias.Equals(field.Name)
                    ? string.Concat(p.L, field.Name, p.R)
                    : string.Format("{0} = {1}", field.Alias, p.Enclose(field.Name)));
            }
            return selectKeys;
        }

        public string KeysAllQuery() {
            return InputConnection.EntityKeysAllQueryWriter.Write(this);
        }

        public string KeysQuery() {
            return CanDetectChanges()
                ? InputConnection.EntityKeysQueryWriter.Write(this)
                : InputConnection.EntityKeysAllQueryWriter.Write(this);
        }

        public string KeysRangeQuery() {
            return InputConnection.EntityKeysRangeQueryWriter.Write(this);
        }

        public Fields InputFields() {
            return new FieldSqlWriter(Fields, CalculatedFields).Input().Context();
        }

        public override string ToString() {
            return Alias;
        }

        public bool BeginAndEndAreEqual() {
            if (HasRange) {
                var bytes = new[] { "byte[]", "rowversion" };
                if (bytes.Any(t => t == Version.SimpleType)) {
                    var beginBytes = (byte[])Begin;
                    var endBytes = (byte[])End;
                    return Common.AreEqual(beginBytes, endBytes);
                }
                return Begin.Equals(End);
            }
            return false;
        }

        public void CheckForChanges(Process process) {
            if (!CanDetectChanges())
                return;
            process.OutputConnection.LoadBeginVersion(this);
            InputConnection.LoadEndVersion(this);
        }

        public string GetVersionField() {
            switch (Version.SimpleType) {
                case "rowversion":
                    return "BinaryVersion";
                case "byte[]":
                    return "BinaryVersion";
                default:
                    return Version.SimpleType[0].ToString(CultureInfo.InvariantCulture).ToUpper() +
                           Version.SimpleType.Substring(1) + "Version";
            }
        }

        public bool CanDetectChanges() {
            return Version != null && Version.Input && InputConnection.Provider.IsDatabase;
        }

        public bool NeedsSchema() {
            return !(string.IsNullOrEmpty(Schema) || Schema.Equals("dbo", IC));
        }
    }
}