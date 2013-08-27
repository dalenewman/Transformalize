/*
Transformalize - Replicate, Transform, and Denormalize Your Data...
Copyright (C) 2013 Dale Newman

This program is free software: you can redistribute it and/or modify
it under the terms of the GNU General Public License as published by
the Free Software Foundation, either version 3 of the License, or
(at your option) any later version.

This program is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU General Public License for more details.

You should have received a copy of the GNU General Public License
along with this program.  If not, see <http://www.gnu.org/licenses/>.
*/

using System.Collections.Generic;
using System.Linq;
using Transformalize.Core.Fields_;
using Transformalize.Libs.NLog;

namespace Transformalize.Core.Field_ {


    public class FieldSqlWriter
    {

        private readonly Logger _log = LogManager.GetCurrentClassLogger();
        private const string BATCH_ID = "TflBatchId";
        private const string SURROGATE_KEY = "TflKey";
        private SortedDictionary<string, string> _output;
        private Dictionary<string, Field> _original;

        public FieldSqlWriter() {
            StartEmpty();
        }

        public FieldSqlWriter(Field field) {
            StartWithField(field);
        }

        public FieldSqlWriter Reload(Field field) {
            StartWithField(field);
            return this;
        }

        public FieldSqlWriter(IEnumerable<Field> fields) {
            StartWithFields(fields);
        }

        public FieldSqlWriter Reload(IEnumerable<Field> fields) {
            StartWithFields(fields);
            return this;
        }

        public FieldSqlWriter(IDictionary<string, Field> fields) {
            StartWithDictionary(fields);
        }

        public FieldSqlWriter Reload(IDictionary<string, Field> fields) {
            StartWithDictionary(fields);
            return this;
        }

        public FieldSqlWriter(params IDictionary<string, Field>[] fields) {
            StartWithDictionaries(fields);
        }

        public FieldSqlWriter(params IEnumerable<Field>[] fields)
        {
            StartWithEnumerable(fields);
        }

        public FieldSqlWriter(params IFields[] fields)
        {
            StartWithIFields(fields);
        }

        public FieldSqlWriter Reload(params IDictionary<string, Field>[] fields) {
            if (fields.Length == 0)
                StartWithDictionary(_original);
            else
                StartWithDictionaries(fields);
            return this;
        }

        private void StartWithField(Field field) {
            _original = new Dictionary<string, Field> { { field.Alias, field } };
            _output = new SortedDictionary<string, string> { { field.Alias, string.Empty } };
        }

        private void StartEmpty() {
            _original = new Dictionary<string, Field>();
            _output = new SortedDictionary<string, string>();
        }

        private void StartWithFields(IEnumerable<Field> fields) {
            var expanded = fields.ToArray();
            _original = expanded.ToDictionary(f => f.Alias, f => f);
            _output = new SortedDictionary<string, string>(expanded.ToDictionary(f => f.Alias, f => string.Empty));
        }

        private void StartWithDictionary(IDictionary<string, Field> fields) {
            _original = fields.ToDictionary(f => f.Key, f => f.Value);
            _output = new SortedDictionary<string, string>(fields.ToDictionary(f => f.Key, f => string.Empty));
        }

        private void StartWithDictionaries(params IDictionary<string, Field>[] fields) {
            _original = new Dictionary<string, Field>();
            foreach (var dict in fields) {
                foreach (var pair in dict) {
                    _original[pair.Key] = pair.Value;
                }
            }
            _output = new SortedDictionary<string, string>(_original.ToDictionary(f => f.Key, f => string.Empty));
        }

        private void StartWithIFields(params IFields[] fields)
        {
            _original = new Dictionary<string, Field>();
            foreach (var dict in fields)
            {
                if(dict != null)
                    foreach (var pair in dict)
                        _original[pair.Key] = pair.Value;
            }
            _output = new SortedDictionary<string, string>(_original.ToDictionary(f => f.Key, f => string.Empty));
        }

        private void StartWithEnumerable(params IEnumerable<Field>[] fields)
        {
            _original = new Dictionary<string, Field>();
            foreach (var field in fields)
            {
                foreach (var f in field)
                {
                    _original[f.Alias] = f;
                }
            }
            _output = new SortedDictionary<string, string>(_original.ToDictionary(f => f.Key, f => string.Empty));
        }

        public string Write(string separator = ", ", bool flush = true) {
            var result = string.Join(separator, _output.Select(kv => kv.Value));
            if (flush)
                Flush();
            return result;
        }

        public IEnumerable<string> Values(bool flush = true) {
            return _output.Select(kv => kv.Value);
        }

        public IEnumerable<string> Keys(bool flush = true)
        {
            return _output.Select(kv => kv.Key);
        }

        private void Flush() {
            _output = new SortedDictionary<string, string>(_original.ToDictionary(f => f.Key, f => string.Empty));
        }

        private IEnumerable<string> CopyOutputKeys() {
            var keys = new string[_output.Keys.Count];
            _output.Keys.CopyTo(keys, 0);
            return keys;
        }

        private static string SafeColumn(string name) {
            return string.Concat("[", name, "]");
        }

        public FieldSqlWriter Name() {
            foreach (var key in CopyOutputKeys()) {
                _output[key] = SafeColumn(_original[key].Name);
            }
            return this;
        }

        public FieldSqlWriter Alias() {
            foreach (var key in CopyOutputKeys()) {
                _output[key] = SafeColumn(_original[key].Alias);
            }
            return this;
        }

        public FieldSqlWriter IsNull() {
            foreach (var key in CopyOutputKeys()) {
                var field = _original[key];
                var d = field.Default ?? new ConversionFactory().Convert(string.Empty, field.SimpleType);
                _output[key] = string.Concat("ISNULL(", _output[key], ", ", field.Quote, d, field.Quote, ")");
            }
            return this;
        }

        public FieldSqlWriter ToAlias() {
            foreach (var key in CopyOutputKeys()) {
                var field = _original[key];
                if (field.Alias != field.Name || field.FieldType.HasFlag(Field_.FieldType.Xml)) {
                    _output[key] = string.Concat("[", field.Alias, "] = ", _output[key]);
                }
            }
            return this;
        }

        public FieldSqlWriter AsAlias() {
            foreach (var key in CopyOutputKeys()) {
                var field = _original[key];
                if (field.Alias != field.Name || field.FieldType.HasFlag(Field_.FieldType.Xml)) {
                    _output[key] = string.Concat(_output[key], " AS [", _original[key].Alias, "]");
                }
            }
            return this;
        }

        public FieldSqlWriter DataType() {
            foreach (var key in CopyOutputKeys()) {
                _output[key] = string.Concat(_output[key], " ", _original[key].SqlDataType);
            }
            return this;
        }

        public FieldSqlWriter Prepend(string prefix) {
            foreach (var key in CopyOutputKeys()) {
                _output[key] = string.Concat(prefix, _output[key]);
            }
            return this;
        }

        public FieldSqlWriter Append(string suffix) {
            foreach (var key in CopyOutputKeys()) {
                _output[key] = string.Concat(_output[key], suffix);
            }
            return this;
        }

        public FieldSqlWriter Asc() {
            Append(" ASC");
            return this;
        }

        public FieldSqlWriter Desc() {
            Append(" DESC");
            return this;
        }

        public FieldSqlWriter AppendIf(string suffix, params FieldType[] fieldTypes) {
            foreach (var key in CopyOutputKeys()) {
                var field = _original[key];
                if (fieldTypes.Any(ft=>ft.HasFlag(field.FieldType)))
                    _output[key] = string.Concat(_output[key], suffix);
            }
            return this;
        }

        public FieldSqlWriter AppendIfNot(string suffix, FieldType fieldType) {
            foreach (var key in CopyOutputKeys()) {
                var field = _original[key];
                if (!field.FieldType.HasFlag(fieldType))
                    _output[key] = string.Concat(_output[key], suffix);
            }
            return this;
        }

        private static string XmlValue(Field field) {
            return string.Format("[{0}].value('({1})[{2}]', '{3}')", field.Parent, field.XPath, field.Index, field.SqlDataType);
        }

        public FieldSqlWriter XmlValue() {
            foreach (var key in CopyOutputKeys()) {
                _output[key] = XmlValue(_original[key]);
            }
            return this;
        }

        /// <summary>
        /// Presents the field for a select. 
        /// </summary>
        /// <returns>field's name for a regular field, and the XPath necessary for XML based fields.</returns>
        public FieldSqlWriter Select() {
            foreach (var key in CopyOutputKeys()) {
                var field = _original[key];
                if (field.FieldType.HasFlag(Field_.FieldType.Xml))
                    _output[key] = XmlValue(field);
                else {
                    _output[key] = SafeColumn(field.Name);
                }
            }
            return this;
        }

        public FieldSqlWriter Output(bool answer = true) {
            foreach (var key in CopyOutputKeys()) {
                var field = _original[key];
                if (field.Output != answer)
                    _output.Remove(key);
            }
            return this;
        }

        public FieldSqlWriter Input(bool answer = true) {
            foreach (var key in CopyOutputKeys()) {
                var field = _original[key];
                if (field.Input != answer)
                    _output.Remove(key);
            }
            return this;
        }

        public FieldSqlWriter Aggregate() {
            foreach (var key in CopyOutputKeys()) {
                var field = _original[key];
                if (field.Aggregate == string.Empty || field.FieldType.HasFlag(Field_.FieldType.PrimaryKey) || field.FieldType.HasFlag(Field_.FieldType.MasterKey))
                    _output.Remove(key);
            }
            return this;
        }

        public FieldSqlWriter HasTransform() {
            foreach (var key in CopyOutputKeys()) {
                var field = _original[key];
                if (!field.HasTransforms)
                    _output.Remove(key);
            }
            return this;
        }

        public FieldSqlWriter HasDefault()
        {
            foreach (var key in CopyOutputKeys())
            {
                var field = _original[key];
                if (field.Default == null)
                    _output.Remove(key);
            }
            return this;
        }

        public FieldSqlWriter Xml(bool answer) {
            foreach (var key in CopyOutputKeys()) {
                var field = _original[key];
                if (field.InnerXml.Any() != answer)
                    _output.Remove(key);
            }
            return this;
        }

        public FieldSqlWriter FieldType(params FieldType[] answers) {
            foreach (var key in CopyOutputKeys()) {
                var field = _original[key];
                if (!answers.Any(a => field.FieldType.HasFlag(a)))
                    _output.Remove(key);
            }
            return this;
        }

        public FieldSqlWriter Set(string left, string right) {
            foreach (var key in CopyOutputKeys()) {
                var name = _output[key];
                _output[key] = string.Concat(left, ".", name, " = ", right, ".", name);
            }
            return this;
        }

        public FieldSqlWriter SetParam() {
            foreach (var key in CopyOutputKeys()) {
                var name = _output[key];
                _output[key] = string.Concat(name, " = @", name.Trim("[]".ToCharArray()));
            }
            return this;
        }

        public FieldSqlWriter ExpandXml() {
            foreach (var key in CopyOutputKeys()) {

                var field = _original[key];
                if (!field.InnerXml.Any()) continue;

                foreach (var xml in field.InnerXml) {
                    _original[xml.Key] = xml.Value;
                    _output[xml.Key] = string.Empty;
                }

                _output.Remove(key);
            }
            return this;
        }

        public FieldSqlWriter AddBatchId(bool forCreate = true) {

            _original[BATCH_ID] = new Field("System.Int32", "8", Field_.FieldType.Field, true, "0") {
                Alias = BATCH_ID,
                NotNull = forCreate
            };

            _output[BATCH_ID] = string.Empty;
            return this;
        }

        public FieldSqlWriter AddSurrogateKey(bool forCreate = true) {
            if (forCreate)
                _original[SURROGATE_KEY] = new Field("System.Int32", "8", Field_.FieldType.Field, true, "0") { Alias = SURROGATE_KEY, NotNull = true, Clustered = true, Identity = true };
            else
                _original[SURROGATE_KEY] = new Field("System.Int32", "8", Field_.FieldType.Field, true, "0") { Alias = SURROGATE_KEY };

            _output[SURROGATE_KEY] = string.Empty;
            return this;
        }

        public override string ToString() {
            return Write();
        }

        public IFields Context() {
            var results = new Fields();
            foreach (var pair in _output) {
                results[pair.Key] = _original[pair.Key];
            }
            return results;
        }

        public Field[] ToArray()
        {
            var results = new Fields();
            foreach (var pair in _output)
            {
                results[pair.Key] = _original[pair.Key];
            }
            return results.ToEnumerable().ToArray();
        }

        public FieldSqlWriter Remove(string @alias)
        {
            foreach (var key in CopyOutputKeys())
            {
                var field = _original[key];
                if (field.Alias == @alias)
                    _output.Remove(key);
            }
            return this;
        }

    }
}