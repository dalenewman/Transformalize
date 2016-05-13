#region license
// Transformalize
// Copyright 2013 Dale Newman
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//  
//      http://www.apache.org/licenses/LICENSE-2.0
//  
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
#endregion
using System;
using System.Collections.Generic;
using System.Linq;
using Cfg.Net;
using Cfg.Net.Ext;

namespace Pipeline.Configuration {
    public class Entity : CfgNode {
        public bool IsMaster { get; set; }

        [Cfg(required = false, unique = true)]
        public string Alias { get; set; }

        [Cfg(value = "input", toLower = true)]
        public string Connection { get; set; }

        [Cfg(value = false)]
        public bool Delete { get; set; }

        public Field[] GetPrimaryKey() {
            return GetAllFields().Where(f => f.PrimaryKey).ToArray();
        }

        [Cfg(value = false)]
        public bool Group { get; set; }
        [Cfg(value = "", required = true)]
        public string Name { get; set; }

        [Cfg]
        public List<Dictionary<string, string>> Rows { get; set; }

        /// <summary>
        /// Optional.  Defaults to `linq`.
        /// 
        /// A choice between `linq`, `parallel.linq`, `streams`, `parallel.streams`.
        /// 
        /// **Note**: You can set each entity if you want, or control all entities from the Process' pipeline attribute.
        /// 
        /// In general, you should develop using `linq`, and once everything is stable, switch over to `parallel.linq`.
        /// </summary>
        [Cfg(value = "linq", domain = "linq,streams,parallel.linq,parallel.streams,linq.optimizer,parallel.linq.optimizer", toLower = true)]
        public string Pipeline { get; set; }

        [Cfg(value = "")]
        public string Prefix { get; set; }
        [Cfg(value = true)]
        public bool PrependProcessNameToOutputName { get; set; }
        [Cfg(value = "")]
        public string Query { get; set; }
        [Cfg(value = "")]
        public string QueryKeys { get; set; }
        [Cfg(value = 100)]
        public int Sample { get; set; }
        [Cfg(value = "")]
        public string Schema { get; set; }
        [Cfg(value = "")]
        public string Script { get; set; }
        [Cfg(value = "")]
        public string ScriptKeys { get; set; }
        [Cfg(value = false)]
        public bool TrimAll { get; set; }
        [Cfg(value = true)]
        public bool Unicode { get; set; }
        [Cfg(value = true)]
        public bool VariableLength { get; set; }
        [Cfg(value = "")]
        public string Version { get; set; }
        [Cfg(required = false)]
        public List<Filter> Filter { get; set; }
        [Cfg(required = false)]
        public List<Field> Fields { get; set; }
        [Cfg(required = false)]
        public List<Field> CalculatedFields { get; set; }

        [Cfg(required = false)]
        public List<Order> Order { get; set; }

        [Cfg(value = (long)10000)]
        public long LogInterval { get; set; }


        /// <summary>
        /// Currently only supported for ADO based input.
        /// The default is true, which pulls any record with version >= TFL's max version.
        /// This means you may re-loading records from the source that the destination
        /// already has.
        /// 
        /// If you're certain you've retrieved all the records for TFL's max version, you can 
        /// set this to false, which pulls any record with version > TFL's max version.
        /// 
        /// This saves you from re-loading the same records from the source.  However, if your 
        /// source added to those records with that same version, you wouldn't get them.
        /// </summary>
        [Cfg(value = true)]
        public bool Overlap { get; set; }

        /// <summary>
        /// Set by Process.ModifyKeys for keyed dependency injection
        /// </summary>
        public string Key { get; set; }

        public IEnumerable<Relationship> RelationshipToMaster { get; internal set; }

        public int BatchId { get; set; }

        public object MinVersion { get; set; }
        public object MaxVersion { get; set; }

        public bool NeedsUpdate() {
            if (MinVersion == null)
                return true;
            if (MaxVersion == null)
                return true;

            var field = GetVersionField();
            var minVersionType = MinVersion.GetType();
            var maxVersionType = MaxVersion.GetType();

            if (field.Type == "byte[]" && minVersionType == typeof(byte[]) && maxVersionType == typeof(byte[])) {
                var beginBytes = (byte[])MinVersion;
                var endBytes = (byte[])MaxVersion;
                return !beginBytes.SequenceEqual(endBytes);
            }

            if (minVersionType != maxVersionType) {
                MinVersion = field.Convert(MinVersion.ToString());
                MaxVersion = field.Convert(MaxVersion.ToString());
            }

            return !MinVersion.Equals(MaxVersion);
        }

        public short Index { get; internal set; }

        public IEnumerable<Field> GetAllFields() {
            var fields = new List<Field>();
            foreach (var f in Fields) {
                fields.Add(f);
                fields.AddRange(f.Transforms.SelectMany(transform => transform.Fields));
            }
            fields.AddRange(CalculatedFields);
            return fields;
        }

        public IEnumerable<Field> GetAllOutputFields() {
            return GetAllFields().Where(f => f.Output);
        }

        protected override void PreValidate() {
            if (string.IsNullOrEmpty(Alias)) {
                Alias = Name;
            }
            foreach (var cf in CalculatedFields) {
                cf.Input = false;
                cf.IsCalculated = true;
            }
            if (!string.IsNullOrEmpty(Prefix)) {
                foreach (var field in Fields.Where(f => f.Alias == f.Name && !f.Alias.StartsWith(Prefix))) {
                    field.Alias = Prefix + field.Name;
                }
            }

            try {
                AdaptFieldsCreatedFromTransforms();
            } catch (Exception ex) {
                Error("Trouble adapting fields created from transforms. {0}", ex.Message);
            }

            AddSystemFields();
            ModifyMissingPrimaryKey();
            ModifyIndexes();
        }

        public void ModifyIndexes() {
            short fieldIndex = -1;
            foreach (var field in GetAllFields()) {
                field.Index = ++fieldIndex;
            }
            short keyIndex = -1;
            foreach (var field in GetPrimaryKey()) {
                field.KeyIndex = ++keyIndex;
            }
        }

        void AddSystemFields() {
            var fields = new List<Field> {
                new Field {
                    Name = Constants.TflKey,
                    Alias = Constants.TflKey,
                    System = true,
                    Type = "int",
                    Input = false
                }.WithDefaults(),
                new Field {
                    Name = Constants.TflBatchId,
                    Alias = Constants.TflBatchId,
                    System = true,
                    Type = "int",
                    Input = false
                }.WithDefaults(),
                new Field {
                    Name = Constants.TflHashCode,
                    Alias = Constants.TflHashCode,
                    Type = "int",
                    System = true,
                    Input = false,
                    Transforms = CalculateHashCode ? new List<Transform> {
                        new Transform {
                            Method = "hashcode",
                            Parameters = Fields.Where(f=>f.Input && !f.PrimaryKey).OrderBy(f=>f.Input).Select(f=>new Parameter { Field = f.Alias}.WithDefaults()).ToList()
                        }.WithDefaults()
                    } : new List<Transform>()
                }.WithDefaults(),
                new Field {
                    Name = Constants.TflDeleted,
                    Alias = Constants.TflDeleted,
                    System = true,
                    Type = "bool",
                    Input = false
                }.WithDefaults()
            };

            foreach (var field in fields) {
                field.Input = false;
            }

            Fields.InsertRange(0, fields);
        }

        public bool IsPageRequest() {
            return Page > 0 && PageSize > 0;
        }

        /// <summary>
        /// Adds a primary key if there isn't one.
        /// </summary>
        void ModifyMissingPrimaryKey() {

            if (!Fields.Any())
                return;

            if (Fields.Any(f => f.PrimaryKey))
                return;

            if (CalculatedFields.Any(cf => cf.PrimaryKey))
                return;

            TflKey().PrimaryKey = true;
        }

        protected override void Validate() {
            var fields = GetAllFields().ToArray();
            var names = new HashSet<string>(fields.Select(f => f.Name).Distinct());
            var aliases = new HashSet<string>(fields.Select(f => f.Alias));

            ValidateVersion(names, aliases);
            ValidateFilter(names, aliases);
            ValidateOrder(names, aliases);

            foreach (var field in GetAllFields().Where(f => !f.System)) {
                if (Constants.InvalidFieldNames.Contains(field.Alias)) {
                    Error($"{field.Alias} is a reserved word in {Alias}.  Please alias it (<a name='{field.Alias}' alias='{Alias}{field.Alias.Remove(0, 3)}' />).");
                }
            }

        }

        void ValidateVersion(ICollection<string> names, ICollection<string> aliases) {
            if (Version == string.Empty)
                return;

            if (names.Contains(Version))
                return;

            if (aliases.Contains(Version))
                return;

            Error("Cant't find version field '{0}' in entity '{1}'", Version, Name);
        }

        void ValidateFilter(ICollection<string> names, ICollection<string> aliases) {
            if (Filter.Count == 0)
                return;

            foreach (var f in Filter) {
                if (f.Expression != string.Empty)
                    return;

                if (aliases.Contains(f.Field))
                    continue;

                if (names.Contains(f.Field))
                    continue;

                Error("A filter's left attribute must reference a defined field. '{0}' is not defined.", f.Field);
            }
        }

        void ValidateOrder(ICollection<string> names, ICollection<string> aliases) {
            if (Order.Count == 0)
                return;

            foreach (var o in Order.Where(o => !aliases.Contains(o.Field)).Where(o => !names.Contains(o.Field))) {
                Error("An order's field attribute must reference a defined field. '{0}' is not defined.", o.Field);
            }
        }


        public IEnumerable<Transform> GetAllTransforms() {
            var transforms = Fields.SelectMany(field => field.Transforms).ToList();
            transforms.AddRange(CalculatedFields.SelectMany(field => field.Transforms));
            return transforms;
        }

        public void MergeParameters() {

            foreach (var field in Fields) {
                foreach (var transform in field.Transforms.Where(t => t.Parameter != string.Empty && !Transform.Producers().Contains(t.Method))) {
                    if (transform.Parameter == "*") {
                        foreach (var f in Fields.Where(f => !f.System)) {
                            if (transform.Parameters.All(p => p.Field != f.Alias)) {
                                transform.Parameters.Add(GetParameter(Alias, f.Alias, f.Type));
                            }
                        }
                    } else {
                        transform.Parameters.Add(GetParameter(Alias, transform.Parameter));
                    }
                    transform.Parameter = string.Empty;
                    if (transform.Parameters.Count == 1 && transform.Parameters.First().Field == "*") {
                        foreach (var f in Fields.Where(f => !f.System)) {
                            transform.Parameters.Add(GetParameter(Alias, f.Alias, f.Type));
                        }
                    }
                }
            }

            var index = 0;
            foreach (var calculatedField in CalculatedFields) {
                foreach (var transform in calculatedField.Transforms.Where(t => t.Parameter != string.Empty && !Transform.Producers().Contains(t.Method))) {
                    if (transform.Parameter == "*") {
                        foreach (var field in GetAllFields().Where(f => !f.System)) {
                            if (transform.Parameters.All(p => p.Field != field.Alias)) {
                                transform.Parameters.Add(GetParameter(Alias, field.Alias, field.Type));
                            }
                        }
                        var thisField = calculatedField.Name;
                        foreach (var calcField in CalculatedFields.Take(index).Where(cf => cf.Name != thisField)) {
                            transform.Parameters.Add(GetParameter(Alias, calcField.Alias, calcField.Type));
                        }
                    } else {
                        transform.Parameters.Add(GetParameter(Alias, transform.Parameter));
                    }
                    transform.Parameter = string.Empty;
                    if (transform.Parameters.Count == 1 && transform.Parameters.First().Field == "*") {
                        foreach (var f in GetAllFields().Where(f => !f.System)) {
                            transform.Parameters.Add(GetParameter(Alias, f.Alias, f.Type));
                        }
                    }
                }
                index++;
            }

        }

        static Parameter GetParameter(string entity, string field, string type) {
            return new Parameter { Entity = entity, Field = field, Type = type }.WithDefaults();
        }

        static Parameter GetParameter(string entity, string field) {
            return new Parameter { Entity = entity, Field = field }.WithDefaults();
        }

        public bool HasConnection() {
            return Connection != string.Empty;
        }

        void AdaptFieldsCreatedFromTransforms() {
            foreach (var method in Transform.Producers()) {
                while (new TransformFieldsToParametersAdapter(this).Adapt(method) > 0) {
                    new TransformFieldsMoveAdapter(this).Adapt(method);
                }
            }
        }

        public override string ToString() {
            return $"{Alias}({GetExcelName()})";
        }

        string OutputName(string processName) {
            return (PrependProcessNameToOutputName ? processName + Alias : Alias);
        }

        public Field TflHashCode() {
            return Fields.First(f => f.Name == Constants.TflHashCode);
        }

        public Field TflKey() {
            return Fields.First(f => f.Name == Constants.TflKey);
        }

        public Field TflDeleted() {
            return Fields.First(f => f.Name == Constants.TflDeleted);
        }

        public Field TflBatchId() {
            return Fields.First(f => f.Name == Constants.TflBatchId);
        }

        public Field GetVersionField() {
            var fields = GetAllFields().ToArray();
            return fields.FirstOrDefault(f => !f.System && f.Alias.Equals(Version, StringComparison.OrdinalIgnoreCase)) ?? fields.FirstOrDefault(f => !f.System && f.Name.Equals(Version, StringComparison.OrdinalIgnoreCase));
        }

        public Field GetField(string aliasOrName) {
            return GetAllFields().FirstOrDefault(f => f.Alias.Equals(aliasOrName, StringComparison.OrdinalIgnoreCase)) ?? GetAllFields().FirstOrDefault(f => f.Name.Equals(aliasOrName, StringComparison.OrdinalIgnoreCase));
        }

        public bool TryGetField(string aliasOrName, out Field field) {
            field = GetField(aliasOrName);
            return field != null;
        }

        public string OutputTableName(string processName) {
            return OutputName(processName) + "Table";
        }

        public string OutputViewName(string processName) {
            return OutputName(processName);
        }

        public string GetExcelName() {
            return Utility.GetExcelName(Index);
        }

        /// <summary>
        /// A value greater than zero will pull all primary keys first 
        /// and then "batch-read" the keys out of the table. Only change this 
        /// if Pipeline.NET reads cause blocking for other clients. A non-zero setting  
        /// may reduce locking. However, this technique reads slower and may be much slower if 
        /// your data source is a view with many joins.
        /// </summary>
        [Cfg(value = 0)]
        public int ReadSize { get; set; }

        [Cfg(value = 250)]
        public int InsertSize { get; set; }

        [Cfg(value = 99)]
        public int UpdateSize { get; set; }

        [Cfg(value = 99)]
        public int DeleteSize { get; set; }


        [Cfg(value = 0)]
        public int Page { get; set; }

        [Cfg(value = 10)]
        public int PageSize { get; set; }

        [Cfg(value = "all", domain = "all,none,some", toLower = true, ignoreCase = true)]
        public string OrderBy { get; set; }
        public string FileName() {
            return Utility.Identifier(OutputName(string.Empty)) + ".txt";
        }

        [Cfg(value = false)]
        public bool NoLock { get; set; }

        // state, aagghhh!!!
        public int Identity;
        public int Inserts { get; set; }
        public int Updates { get; set; }
        public int Deletes { get; set; }

        [Cfg]
        public int Hits { get; set; }

        [Cfg]
        public bool IsFirstRun { get; set; }

        [Cfg(value = true)]
        public bool DataTypeWarnings { get; set; }

        // Determines whether or not a hash code will be calculated and placed in TflHashCode column (in output).
        [Cfg(value = true)]
        public bool CalculateHashCode { get; set; }
    }
}