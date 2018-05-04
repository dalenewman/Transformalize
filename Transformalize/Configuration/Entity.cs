#region license
// Transformalize
// Configurable Extract, Transform, and Load
// Copyright 2013-2017 Dale Newman
//  
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//   
//       http://www.apache.org/licenses/LICENSE-2.0
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
using System.Text.RegularExpressions;
using Cfg.Net;
using Transformalize.Impl;

namespace Transformalize.Configuration {
    public class Entity : CfgNode {

        private Pagination _pagination;

        public bool IsMaster { get; set; }

        [Cfg(required = false, unique = true, value = null)]
        public string Alias { get; set; }

        [Cfg(value="")]
        public string Label { get; set; }

        [Cfg(value = "input", toLower = true)]
        public string Connection { get; set; }

        // insert, update, and/or delete options (default behavior is insert=true, update=true, delete=false)
        [Cfg(value = true)]
        public bool Insert { get; set; }
        [Cfg(value = true)]
        public bool Update { get; set; }
        [Cfg(value = false)]
        public bool Delete { get; set; }

        // insert, update, and delete sizes
        [Cfg(value = 500)]
        public int InsertSize { get; set; }
        [Cfg(value = 250)]
        public int UpdateSize { get; set; }
        [Cfg(value = 250)]
        public int DeleteSize { get; set; }

        // counts of the actual inserts, updates, and deletes performed
        public uint Inserts { get; set; }
        public uint Updates { get; set; }
        public uint Deletes { get; set; }


        // primarly for form mode
        [Cfg(value = "")]
        public string InsertCommand { get; set; }
        [Cfg(value = "")]
        public string UpdateCommand { get; set; }
        [Cfg(value = "")]
        public string DeleteCommand { get; set; }
        [Cfg(value="")]
        public string CreateCommand { get; set; }

        public Field[] GetPrimaryKey() {
            return GetAllFields().Where(f => f.PrimaryKey).ToArray();
        }

        [Cfg(value = false)]
        public bool Group { get; set; }
        [Cfg(value = "", required = true)]
        public string Name { get; set; }

        [Cfg]
        public List<CfgRow> Rows { get; set; }

        /// <summary>
        /// Optional.  Defaults to `linq`.
        /// 
        ///  **Note**: You can set each entity if you want, or control all entities from the Process' pipeline attribute.
        /// 
        /// In general, you should develop using `linq`, and once everything is stable, switch over to `parallel.linq`.
        /// </summary>
        [Cfg(value = "linq", domain = "linq,parallel.linq", toLower = true)]
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
        [Cfg(value = "", toLower = true)]
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

        [Cfg(value = 10000)]
        public int LogInterval { get; set; }


        /// <summary>
        /// Currently only supported for ADO based input.
        /// The default is true, which pulls any record with version >= TFL's max version.
        /// This means you may re-load records from the source that the destination
        /// already has.
        /// 
        /// If you're certain you've retrieved all the records for TFL's max version, you can 
        /// set this to false, which pulls any record with version > TFL's max version.
        /// 
        /// This saves you from re-loading the same records from the source.  However, if your 
        /// source added to those records using the same version as when you queried them, 
        /// you wouldn't get them the next time around.
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

        protected override void PostValidate() {
            if (!Errors().Any()) {
                PostValidateFilters();
            }
        }

        private void PostValidateFilters() {
            if (!Filter.Any())
                return;

            for (int i = 0; i < Filter.Count; i++) {
                var filter = Filter[i];
                Field field;
                if (TryGetField(filter.Field, out field)) {
                    filter.LeftField = field;
                    filter.IsField = true;
                    filter.Key = field.Name + "_filter_" + i;
                }
                if (TryGetField(filter.Value, out field)) {
                    filter.ValueField = field;
                    filter.ValueIsField = true;
                }
            }
        }

        protected override void PreValidate() {
            if (string.IsNullOrEmpty(Alias)) {
                Alias = Name;
            }

            PreValidateSorting();

            foreach (var cf in CalculatedFields) {
                cf.Input = false;
                cf.IsCalculated = true;
            }

            if (!string.IsNullOrEmpty(Prefix)) {
                foreach (var field in Fields.Where(f => f.Alias == f.Name && !f.Alias.StartsWith(Prefix))) {
                    field.Alias = Prefix + field.Name;
                }
            }

            if (SearchType != Constants.DefaultSetting) {
                foreach (var field in Fields) {
                    field.SearchType = SearchType;
                }
            }

        }

        private void PreValidateSorting() {

            // if set, an entity's sortable setting will over-ride it's individual input field's sortable settings
            if (Sortable != Constants.DefaultSetting) {
                foreach (var field in Fields.Where(f => f.Sortable == Constants.DefaultSetting)) {
                    field.Sortable = Sortable;
                }
                foreach (var field in CalculatedFields.Where(f => f.Sortable == Constants.DefaultSetting)) {
                    field.Sortable = Sortable;
                }
            }

            // if field settings are still default, input fields default to true and other default to false
            foreach (var field in GetAllFields().Where(f => f.Sortable == Constants.DefaultSetting)) {
                field.Sortable = field.Input ? "true" : "false";
            }

            // any fields that are sortable should have sort-field populated
            foreach (var field in GetAllFields().Where(f => f.Sortable == "true" && string.IsNullOrEmpty(f.SortField))) {
                field.SortField = string.IsNullOrEmpty(Query) ? field.Alias : field.Name;
            }

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

        public void AddSystemFields() {

            if (Fields.Any(f => f.Alias == Constants.TflKey))
                return;

            var fields = new List<Field> {
                new Field { Name = Constants.TflKey, Alias = Constants.TflKey, System = true, Type = "int", Input = false, Default = "0" },
                new Field { Name = Constants.TflBatchId, Alias = Constants.TflBatchId, System = true, Type = "int", Input = false, Default = "0" },
                new Field {
                    Name = Constants.TflHashCode,
                    Alias = Constants.TflHashCode,
                    System = true,
                    Type = "int",
                    Input = false,
                    Default = "0",
                    Transforms = new List<Operation> {
                        new Operation {
                            Method = "hashcode",
                            Parameters = Fields.Where(f=>f.Input && !f.PrimaryKey).OrderBy(f=>f.Input).Select(f=>new Parameter { Entity = Alias, Field = f.Alias}).ToList()
                        }
                    }
                },
                new Field { Name = Constants.TflDeleted, Alias = Constants.TflDeleted, System = true, Type = "boolean", Input = false, Default = "false" }
            };

            foreach (var field in fields) {
                field.System = true;
                field.Input = false;
                field.Output = true;
                if (SearchType != Constants.DefaultSetting) {
                    field.SearchType = SearchType;
                }
            }

            Fields.InsertRange(0, fields);
        }

        public bool IsPageRequest() {
            return Page > 0 && Size >= 0;
        }

        /// <summary>
        /// Adds a primary key if there isn't one.
        /// </summary>
        public void ModifyMissingPrimaryKey() {

            if (!Fields.Any())
                return;

            if (Fields.Any(f => f.PrimaryKey))
                return;

            if (CalculatedFields.Any(cf => cf.PrimaryKey))
                return;

            TflKey().PrimaryKey = true;
        }

        protected override void Validate() {

            // if validation has been defined, check to see if corresponding valid and message fields are present and create them if not
            var calculatedKeys = new HashSet<string>(CalculatedFields.Select(f => f.Alias ?? f.Name).Distinct(), StringComparer.OrdinalIgnoreCase);

            if (Fields.Any(f => f.Validators.Any())) {
                foreach (var field in Fields.Where(f => f.Validators.Any())) {

                    if (!calculatedKeys.Contains(field.ValidField)) {
                        CalculatedFields.Add(new Field {
                            Name = field.ValidField,
                            Alias = field.ValidField,
                            Input = false,
                            Type = "bool",
                            Default = "true",
                            IsCalculated = true
                        });
                    }

                    if (!calculatedKeys.Contains(field.MessageField)) {
                        CalculatedFields.Add(new Field { Name = field.MessageField, Alias = field.MessageField, Length = "255", Default = "", IsCalculated = true, Input = false });
                    }
                }
                // create an entity-wide valid field if necessary
                if (ValidField == string.Empty) {
                    var valid = Alias + "Valid";
                    if (!CalculatedFields.Any(f => f.Name.Equals(valid))) {
                        var add = new Field { Name = valid, Alias = valid, Type = "bool", ValidField = valid, Input = false, IsCalculated = true, Default = "true"};
                        add.Validators.Add(new Operation {
                            Method = "all",
                            Operator = "equals",
                            Value = "true",
                            Parameters = GetAllFields().Where(f => f.ValidField != string.Empty).Select(f => f.ValidField).Distinct().Select(n => new Parameter { Field = n }).ToList()
                        });
                        CalculatedFields.Add(add);
                        ValidField = valid;
                    }
                }
            }

            var fields = GetAllFields().ToArray();
            var names = new HashSet<string>(fields.Select(f => f.Name).Distinct());
            var aliases = new HashSet<string>(fields.Select(f => f.Alias));

            ValidateVersion(names, aliases);
            ValidateFilter(names, aliases);
            ValidateOrder(names, aliases);

            foreach (var field in GetAllOutputFields().Where(f => f.Sortable == "true" && !string.IsNullOrEmpty(f.SortField))) {
                if (GetField(field.SortField) == null) {
                    Error($"Can't find sort field {field.SortField} defined in field {field.Alias}.");
                }
            }

        }

        [Cfg(value = "")]
        public string ValidField { get; set; }

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

                Error("A filter's field attribute must reference a defined field. '{0}' is not defined.", f.Field);
            }
        }

        void ValidateOrder(ICollection<string> names, ICollection<string> aliases) {
            if (Order.Count == 0)
                return;

            foreach (var o in Order.Where(o => !aliases.Contains(o.Field)).Where(o => !names.Contains(o.Field))) {
                Error("An order's field attribute must reference a defined field. '{0}' is not defined.", o.Field);
            }
        }


        public IEnumerable<Operation> GetAllTransforms() {
            var transforms = Fields.SelectMany(field => field.Transforms).ToList();
            transforms.AddRange(CalculatedFields.SelectMany(field => field.Transforms));
            return transforms;
        }

        public void MergeParameters() {

            foreach (var field in Fields) {
                foreach (var transform in field.Transforms.Where(t => t.Parameter != string.Empty && !Operation.ProducerSet().Contains(t.Method))) {
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
                foreach (var transform in calculatedField.Transforms.Where(t => t.Parameter != string.Empty && !Operation.ProducerSet().Contains(t.Method))) {
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
            return new Parameter { Entity = entity, Field = field, Type = type };
        }

        static Parameter GetParameter(string entity, string field) {
            return new Parameter { Entity = entity, Field = field };
        }

        public bool HasConnection() {
            return Connection != string.Empty;
        }

        public void AdaptFieldsCreatedFromTransforms() {
            foreach (var method in Operation.ProducerSet()) {
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
            try {
                return Fields.First(f => f.Alias == Constants.TflHashCode);
            } catch {
                throw new Exception("TflHashCode isn't present!");
            }
        }

        public Field TflKey() {
            try {
                return Fields.First(f => f.Alias == Constants.TflKey);
            } catch {
                throw new Exception("TflKey isn't present!");
            }
        }

        public Field TflDeleted() {
            try {
                return Fields.First(f => f.Alias == Constants.TflDeleted);
            } catch {
                throw new Exception("TflDeleted isn't present!");
            }
        }

        public Field TflBatchId() {
            try {
                return Fields.First(f => f.Alias == Constants.TflBatchId);
            } catch {
                throw new Exception("TfBatchId isn't present!");
            }
        }

        public Field GetVersionField() {
            var fields = GetAllFields().ToArray();
            return fields.LastOrDefault(f => !f.System && f.Name.Equals(Version, StringComparison.OrdinalIgnoreCase)) ?? fields.LastOrDefault(f => !f.System && f.Alias.Equals(Version, StringComparison.OrdinalIgnoreCase));
        }

        public Field GetField(string aliasOrName) {
            return GetAllFields().FirstOrDefault(f => f.Alias.Equals(aliasOrName, StringComparison.OrdinalIgnoreCase)) ?? GetAllFields().FirstOrDefault(f => f.Name.Equals(aliasOrName, StringComparison.OrdinalIgnoreCase));
        }

        public bool TryGetField(string aliasOrName, out Field field) {
            if (string.IsNullOrEmpty(aliasOrName) || aliasOrName == Constants.DefaultSetting) {
                field = null;
                return false;
            }

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
        /// if Transformalize reads cause blocking for other clients. A non-zero setting  
        /// may reduce locking. However, this technique reads slower and may be much slower if 
        /// your data source is a view with many joins.
        /// </summary>
        [Cfg(value = 0)]
        public int ReadSize { get; set; }

        [Cfg(value = 0)]
        public int Page { get; set; }

        [Cfg(value = 0)]
        public int Size { get; set; }

        [Cfg(value = "all", domain = "all,none,some", toLower = true, ignoreCase = true)]
        public string OrderBy { get; set; }
        public string FileName() {
            return Utility.Identifier(OutputName(string.Empty)) + ".txt";
        }

        [Cfg(value = false)]
        public bool NoLock { get; set; }

        // state, aagghhh!!!
        public int Identity;

        public int RowNumber;

        [Cfg]
        public int Hits { get; set; }

        [Cfg(value = true)]
        public bool DataTypeWarnings { get; set; }

        [Cfg(value = Constants.DefaultSetting, toLower = true)]
        public string SearchType { get; set; }

        [Cfg(value = Constants.DefaultSetting, domain = "true,false," + Constants.DefaultSetting, ignoreCase = true, toLower = true)]
        public string Sortable { get; set; }

        public bool HasInput() {
            return Fields.Any(f => f.Input);
        }

        public Pagination Pagination => _pagination ?? (_pagination = new Pagination(Hits, Page, Size));

        internal Regex FieldMatcher { get; set; }
        public IEnumerable<Field> GetFieldMatches(string content) {
            var matches = FieldMatcher.Matches(content);

            var names = new HashSet<string>();
            foreach (Match match in matches) {
                names.Add(match.Value);
            }

            var fields = new List<Field>();

            foreach (var name in names) {
                if (TryGetField(name, out var newField)) {
                    fields.Add(newField);
                }

            }
            return fields;
        }

        [Cfg(value = false)]
        public bool IgnoreDuplicateKey { get; set; }

        [Cfg(value="en", domain = "az,cz,de,de_AT,de_CH,el,en,en_AU,en_au_ocker,en_BORK,en_CA,en_GB,en_IE,en_IND,en_US,es,es_MX,fa,fr,fr_CA,ge,id_ID,it,ja,ko,lv,nb_NO,nep,nl,nl_BE,pl,nl_BE,pl,pt_BR,pt_PT,ro,ru,sk,sv,tr,uk,vi,zh_CN,zh_TW")]
        public string Locale { get; set; }
    }
}