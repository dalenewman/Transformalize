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
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Transformalize.Configuration;
using Transformalize.Main.Providers.ElasticSearch;
using Transformalize.Operations;

namespace Transformalize.Main {

    public abstract class OrderedFields : IEnumerable<Field> {

        protected const StringComparison Ic = StringComparison.OrdinalIgnoreCase;
        protected const string TflBatchId = "TflBatchId";
        protected const string TflDeleted = "TflDeleted";
        protected const string TflKey = "TflKey";

        private List<Field> _fields = new List<Field>();

        protected void AddSorted(IEnumerable<Field> fields) {
            _fields.AddRange(fields);
        }

        public void Add(params Fields[] fieldSets) {
            var temp = new List<Field>();
            temp.AddRange(_fields);
            foreach (var fields in fieldSets) {
                temp.AddRange(fields.ToArray());
            }
            _fields = new List<Field>(temp.OrderBy(f => f.EntityIndex).ThenBy(f => f.Index));
        }

        public void Add(Field field) {
            var temp = new List<Field>();
            temp.AddRange(_fields);
            temp.Add(field);
            _fields = new List<Field>(temp.OrderBy(f => f.EntityIndex).ThenBy(f => f.Index));
        }

        protected IList<Field> Storage {
            get {
                return _fields;
            }
        }

        public int Count { get { return _fields.Count; } }
        public bool IsReadOnly { get { return false; } }

        public IEnumerator<Field> GetEnumerator() {
            return _fields.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return GetEnumerator();
        }
    }

    public class Fields : OrderedFields {

        public Fields() {
        }

        public Fields(Field field) {
            Add(field);
        }

        public Fields(params Field[] fields) {
            foreach (var field in fields) {
                Add(field);
            }
        }

        public Fields(params Fields[] fields) {
            Add(fields);
        }

        private Fields(IEnumerable<Field> fields) {
            AddSorted(fields);
        }

        public Fields(Process process, IParameters parameters, string entity) {
            foreach (var parameter in parameters) {
                if (parameter.Value.HasValue()) {
                    var field = new Field(parameter.Value.SimpleType, "64", FieldType.NonKey, false, parameter.Value.Value.ToString()) {
                        Alias = parameter.Value.Name,
                    };
                    Add(field);
                } else {
                    Add(process.GetField(entity, parameter.Value.Name));
                }
            }
        }

        public Fields WithAlias() {
            return new Fields(Storage.Where(f => f.Alias != f.Name));
        }

        public Fields WithFileOutput() {
            return new Fields(Storage.Where(f => f.FileOutput));
        }

        public IEnumerable<string> Aliases() {
            return Storage.Select(f => f.Alias);
        }

        public Field[] ToArray() {
            return Storage.ToArray();
        }

        public Fields WithInput() {
            return new Fields(Storage.Where(f => f.Input));
        }

        public Fields WithOutput() {
            return new Fields(Storage.Where(f => f.Output));
        }

        public IEnumerable<Sort> Sorts() {
            return Storage.Where(f => !string.IsNullOrEmpty(f.Sort)).Select(f => new Sort(f.Alias, f.Sort));
        }

        public Fields WithString() {
            return new Fields(Storage.Where(f => f.SimpleType.Equals("string")));
        }

        public Fields WithIdentifiers() {
            return new Fields(Storage.Where(f => !f.Identifier.Equals(f.Alias)));
        }

        public Fields WithGuid() {
            return new Fields(Storage.Where(f => f.SimpleType.Equals("guid")));
        }

        public Fields WithDate() {
            return new Fields(Storage.Where(f => f.SimpleType.StartsWith("date")));
        }

        public IEnumerable<AliasType> AliasTypes() {
            return Storage.Select(f => new AliasType() { Alias = f.Alias, AliasLower = f.Alias.ToLower(), SimpleType = f.SimpleType });
        }

        public IEnumerable<NameAlias> NameAliases() {
            return Storage.Select(f => new NameAlias() { Name = f.Name, Alias = f.Alias });
        }

        public Fields WithoutPrimaryKey() {
            return new Fields(Storage.Where(f => !f.FieldType.HasFlag(FieldType.PrimaryKey)));
        }

        public Fields WithGroup() {
            return new Fields(Storage.Where(f => f.Aggregate.Equals("group", Ic)));
        }

        public Fields WithAccumulate() {
            return new Fields(Storage.Where(f => !f.Aggregate.Equals("group", Ic)));
        }

        public Fields WithForeignKey() {
            return new Fields(Storage.Where(f => f.FieldType.HasFlag(FieldType.ForeignKey)));
        }

        public Fields WithoutForeignKey() {
            return new Fields(Storage.Where(f => !f.FieldType.HasFlag(FieldType.ForeignKey)));
        }

        public bool Any() {
            return Storage.Any();
        }

        public Fields WithoutRowVersion() {
            return new Fields(Storage.Where(f => !f.SimpleType.Equals("rowversion")));
        }

        public Fields WithBytes() {
            return new Fields(Storage.Where(f => f.SimpleType.Equals("rowversion", Ic) || f.SimpleType.Equals("byte[]", Ic)));
        }

        public Fields WithoutBytes() {
            return new Fields(Storage.Where(f => !f.SimpleType.Equals("rowversion", Ic) && !f.SimpleType.Equals("byte[]", Ic)));
        }

        public Fields WithoutKey() {
            return new Fields(Storage.Where(f => f.FieldType.HasFlag(FieldType.NonKey)));
        }

        public Fields FindByParamater(ParameterConfigurationElement element) {
            if (element.Entity != string.Empty) {
                if (Storage.Any(f => f.Alias.Equals(element.Field, Ic) && f.Entity.Equals(element.Entity, Ic))) {
                    return new Fields(Storage.Where(f => f.Alias.Equals(element.Field, Ic) && f.Entity.Equals(element.Entity, Ic)));
                }
                if (Storage.Any(f => f.Name.Equals(element.Field, Ic) && f.Entity.Equals(element.Entity, Ic))) {
                    return new Fields(Storage.Where(f => f.Name.Equals(element.Field, Ic) && f.Entity.Equals(element.Entity, Ic)));
                }
                if (!element.Field.StartsWith("tfl", Ic)) {
                    throw new TransformalizeException("Can not find parameter with entity '{0}' and field name (or alias) of '{1}'.", element.Entity, element.Field);
                }
            }

            if (Storage.Any(f => f.Alias.Equals(element.Field, Ic) || f.Name.Equals(element.Field, Ic))) {
                return new Fields(Storage.First(f => f.Alias.Equals(element.Field, Ic) || f.Name.Equals(element.Field, Ic)));
            }
            if (!element.Field.StartsWith("tfl", Ic)) {
                throw new TransformalizeException("Can not find parameter with name (or alias) of '{0}'.", element.Field);
            }
            return new Fields(new Field(element.Type, "128", FieldType.NonKey, false, element.Value) { Name = element.Field.Equals(string.Empty) ? element.Name : element.Field });
        }

        public bool HaveField(string nameOrAlias) {
            return Storage.Any(f => f.Alias.Equals(nameOrAlias, Ic) || f.Name.Equals(nameOrAlias, Ic));
        }

        public Fields Find(string nameOrAlias) {
            return new Fields(Storage.Where(f => f.Alias.Equals(nameOrAlias, Ic) || f.Name.Equals(nameOrAlias, Ic)));
        }

        public Field First() {
            return Storage.First();
        }

        public Field Last() {
            return Storage.Last();
        }

        public Fields WithMasterKey() {
            return new Fields(Storage.Where(f => f.FieldType.HasFlag(FieldType.MasterKey)));
        }

        public Fields WithSearchType() {
            return new Fields(Storage.Where(f => !f.SearchTypes.Any(st => st.Name.Equals("none", Ic))));
        }

        public bool HaveSort() {
            return Storage.Any(f => !f.Sort.Equals(string.Empty));
        }

        public Fields AddBatchId(int entityIndex, bool forCreate = true) {
            Add(GetBatchField(entityIndex, forCreate));
            return this;
        }

        public Fields AddDeleted(int entityIndex, bool forCreate = true) {
            Add(GetDeletedField(entityIndex, forCreate));
            return this;
        }

        public Fields AddSurrogateKey(int entityIndex, bool forCreate = true) {
            Add(GetSurrogateKeyField(entityIndex, forCreate));
            return this;
        }

        public static Field GetBatchField(int entityIndex, bool forCreate = true) {
            return new Field("System.Int32", "8", FieldType.NonKey, true, "0") {
                Alias = TflBatchId,
                NotNull = forCreate,
                EntityIndex = entityIndex,
                Index = 1000
            };
        }

        public static Field GetDeletedField(int entityIndex, bool forCreate = true) {
            return new Field("System.Boolean", "8", FieldType.NonKey, true, "true") {
                Alias = TflDeleted,
                NotNull = forCreate,
                EntityIndex = entityIndex,
                Index = 1002
            };
        }
        public static Field GetSurrogateKeyField(int entityIndex, bool forCreate = true) {

            if (forCreate)
                return new Field("System.Int32", "8", FieldType.NonKey, true, "0") {
                    Alias = TflKey,
                    NotNull = true,
                    Identity = true,
                    EntityIndex = entityIndex,
                    Index = 1001
                };

            return new Field("System.Int32", "8", FieldType.NonKey, true, "0") {
                Alias = TflKey,
                EntityIndex = entityIndex,
                Index = 1001
            };
        }


        public Field this[string nameOrAlias] {
            get {
                return Find(nameOrAlias).First();
            }
        }

        public Field this[int index] {
            get { return Storage[index]; }
        }

        public Fields WithCalculated() {
            return new Fields(Storage.Where(f => f.IsCalculated));
        }

        public Fields WithoutCalculated() {
            return new Fields(Storage.Where(f => !f.IsCalculated));
        }

        public AliasDefault[] AsAliasDefaults() {
            return Storage.Select(f => new AliasDefault() { Alias = f.Alias, Default = f.Default, DefaultBlank = f.DefaultBlank, DefaultWhiteSpace = f.DefaultWhiteSpace }).ToArray();
        }


        public Fields WithDefaultBlank() {
            return new Fields(Storage.Where(f => f.DefaultBlank));
        }

        public Fields WithDefaultWhiteSpace() {
            return new Fields(Storage.Where(f => f.DefaultWhiteSpace));
        }

        public Fields WithoutInput() {
            return new Fields(Storage.Where(f => !f.Input));
        }
    }

}