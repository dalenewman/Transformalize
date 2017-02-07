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
using Cfg.Net;
using Cfg.Net.Ext;
using Transformalize.Impl;

namespace Transformalize.Configuration {
    public class Entity : CfgNode {

        private Pagination _pagination;

        public bool IsMaster { get; set; }

        [Cfg(required = false, unique = true, value = null)]
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

            var fields = new List<Field> {
                TflKey(),
                TflBatchId(),
                TflHashCode(),
                TflDeleted()
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
            return Page > 0 && PageSize >= 0;
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

            var fields = GetAllFields().ToArray();
            var names = new HashSet<string>(fields.Select(f => f.Name).Distinct());
            var aliases = new HashSet<string>(fields.Select(f => f.Alias));

            ValidateVersion(names, aliases);
            ValidateFilter(names, aliases);
            ValidateOrder(names, aliases);

            if (!IsReverse) {
                foreach (var field in GetAllFields().Where(f => !f.System)) {
                    if (Constants.InvalidFieldNames.Contains(field.Alias)) {
                        Error($"{field.Alias} is a reserved word in {Alias}.  Please alias it (<a name='{field.Alias}' alias='{Alias}{field.Alias.Remove(0, 3)}' />).");
                    }
                }
            }

            foreach (var field in GetAllOutputFields().Where(f => f.Sortable == "true" && !string.IsNullOrEmpty(f.SortField))) {
                if (GetField(field.SortField) == null) {
                    Error($"Can't find sort field {field.SortField} defined in field {field.Alias}.");
                }
            }

            // Paging Madness
            if (Page > 0) {
                if (PageSizes.Any()) {
                    if (PageSizes.All(ps => ps.Size != PageSize)) {
                        var first = PageSizes.First().Size;
                        Warn($"The entity {Name} has an invalid PageSize of {PageSize}. Set to {first}.");
                        PageSize = first;
                    }
                } else {
                    if (PageSize > 0) {
                        PageSizes.Add(new PageSize { Size = PageSize }.WithDefaults());
                    } else {
                        PageSizes.Add(new PageSize { Size = 25 }.WithDefaults());
                        PageSizes.Add(new PageSize { Size = 50 }.WithDefaults());
                        PageSizes.Add(new PageSize { Size = 100 }.WithDefaults());
                        if (PageSize != 25 && PageSize != 50 && PageSize != 100) {
                            PageSize = 25;
                        }
                    }
                }
            } else {
                if (PageSize > 0) {
                    PageSize = 0;
                }
            }

        }

        [Cfg(value=false)]
        public bool IsReverse { get; set; }

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


        public IEnumerable<Transform> GetAllTransforms() {
            var transforms = Fields.SelectMany(field => field.Transforms).ToList();
            transforms.AddRange(CalculatedFields.SelectMany(field => field.Transforms));
            return transforms;
        }

        public void MergeParameters() {

            foreach (var field in Fields) {
                foreach (var transform in field.Transforms.Where(t => t.Parameter != string.Empty && !Transform.ProducerSet().Contains(t.Method))) {
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
                foreach (var transform in calculatedField.Transforms.Where(t => t.Parameter != string.Empty && !Transform.ProducerSet().Contains(t.Method))) {
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

        public void AdaptFieldsCreatedFromTransforms() {
            foreach (var method in Transform.ProducerSet()) {
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
            return Fields.FirstOrDefault(f => f.Alias == Constants.TflHashCode) ?? new Field {
                Name = Constants.TflHashCode,
                Alias = Constants.TflHashCode,
                System = true,
                Type = "int",
                Input = false,
                Default = "0",
                Transforms = CalculateHashCode ? new List<Transform> {
                        new Transform {
                            Method = "hashcode",
                            Parameters = Fields.Where(f=>f.Input && !f.PrimaryKey).OrderBy(f=>f.Input).Select(f=>new Parameter { Field = f.Alias}.WithDefaults()).ToList()
                        }.WithDefaults()
                    } : new List<Transform>()
            }.WithDefaults();
        }

        public Field TflKey() {
            return Fields.FirstOrDefault(f => f.Alias == Constants.TflKey) ?? new Field { Name = Constants.TflKey, Alias = Constants.TflKey, System = true, Type = "int", Input = false, Default = "0" }.WithDefaults();
        }

        public Field TflDeleted() {
            return Fields.FirstOrDefault(f => f.Alias == Constants.TflDeleted) ?? new Field { Name = Constants.TflDeleted, Alias = Constants.TflDeleted, System = true, Type = "boolean", Input = false, Default = "false" }.WithDefaults();
        }

        public Field TflBatchId() {
            return Fields.FirstOrDefault(f => f.Alias == Constants.TflBatchId) ?? new Field { Name = Constants.TflBatchId, Alias = Constants.TflBatchId, System = true, Type = "int", Input = false, Default = "0" }.WithDefaults();
        }

        public Field GetVersionField() {
            var fields = GetAllFields().ToArray();
            return fields.LastOrDefault(f => !f.System && f.Name.Equals(Version, StringComparison.OrdinalIgnoreCase)) ?? fields.LastOrDefault(f => !f.System && f.Alias.Equals(Version, StringComparison.OrdinalIgnoreCase));
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

        [Cfg(value = 512)]
        public int InsertSize { get; set; }

        [Cfg(value = 256)]
        public int UpdateSize { get; set; }

        [Cfg(value = 256)]
        public int DeleteSize { get; set; }


        [Cfg(value = 0)]
        public int Page { get; set; }

        [Cfg(value = 0)]
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

        [Cfg(value = Constants.DefaultSetting, toLower = true)]
        public string SearchType { get; set; }

        [Cfg]
        public List<PageSize> PageSizes { get; set; }

        [Cfg(value = Constants.DefaultSetting, domain = "true,false," + Constants.DefaultSetting, ignoreCase = true, toLower = true)]
        public string Sortable { get; set; }

        public bool HasInput() {
            return Fields.Any(f => f.Input);
        }

        public Pagination Pagination => _pagination ?? (_pagination = new Pagination(Hits, Page, PageSize));
    }
}