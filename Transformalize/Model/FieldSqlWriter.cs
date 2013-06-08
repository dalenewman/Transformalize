using System.Collections.Generic;
using System.Linq;

namespace Transformalize.Model {
    public class FieldSqlWriter {

        private Dictionary<string, string> _output;
        private readonly Dictionary<string, IField> _original;

        public FieldSqlWriter(IField field) {
            _original = new Dictionary<string, IField> { { field.Alias, field } };
            _output = new Dictionary<string, string> { { field.Alias, string.Empty } };
        }

        public FieldSqlWriter(IEnumerable<IField> fields) {
            var expanded = fields.ToArray();
            _original = expanded.ToArray().ToDictionary(f => f.Alias, f => f);
            _output = expanded.ToDictionary(f => f.Alias, f => string.Empty);
        }

        public FieldSqlWriter(IDictionary<string, IField> fields) {
            _original = fields.ToDictionary(f => f.Key, f => f.Value);
            _output = fields.ToDictionary(f => f.Key, f => string.Empty);
        }

        public FieldSqlWriter(params IDictionary<string, IField>[] fields) {
            _original = new Dictionary<string, IField>();
            foreach (var dict in fields) {
                foreach (var key in dict.Keys) {
                    _original[key] = dict[key];
                }
            }
            _output = _original.ToDictionary(f => f.Key, f => string.Empty);
        }

        public string Write(string separator = ", ", bool flush = true) {
            var result = string.Join(separator, _output.Select(kv => kv.Value));
            if (flush)
                Flush();
            return result;
        }

        private void Flush() {
            _output = _original.ToDictionary(f => f.Key, f => string.Empty);
        }

        public FieldSqlWriter Name() {
            foreach (var key in CopyOutputKeys()) {
                _output[key] += string.Concat("[", _original[key].Name, "]");
            }
            return this;
        }

        public FieldSqlWriter Alias() {
            foreach (var key in CopyOutputKeys()) {
                _output[key] += string.Concat("[", _original[key].Alias, "]");
            }
            return this;
        }

        private IEnumerable<string> CopyOutputKeys() {
            var keys = new string[_output.Keys.Count];
            _output.Keys.CopyTo(keys, 0);
            return keys;
        }

        public FieldSqlWriter IsNull() {
            foreach (var key in CopyOutputKeys()) {
                var field = _original[key];
                _output[key] = string.Concat("ISNULL(", _output[key], ", ", field.Quote, field.Default, field.Quote, ")");
            }
            return this;
        }

        public FieldSqlWriter ToAlias() {
            foreach (var key in CopyOutputKeys()) {
                var field = _original[key];
                if (field.Alias != field.Name || field.FieldType == Model.FieldType.Xml) {
                    _output[key] = string.Concat("[", field.Alias, "] = ", _output[key]);
                }
            }
            return this;
        }

        public FieldSqlWriter AsAlias() {
            foreach (var key in CopyOutputKeys()) {
                var field = _original[key];
                if (field.Alias != field.Name || field.FieldType == Model.FieldType.Xml) {
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

        private void Append(string suffix) {
            foreach (var key in CopyOutputKeys()) {
                _output[key] = string.Concat(_output[key], suffix);
            }
        }

        public FieldSqlWriter Null() {
            Append(" NULL");
            return this;
        }

        public FieldSqlWriter NotNull() {
            Append(" NOT NULL");
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

        public FieldSqlWriter AppendIf(string suffix, FieldType fieldType) {
            foreach (var key in CopyOutputKeys()) {
                var field = _original[key];
                if (field.FieldType == fieldType)
                    _output[key] = string.Concat(_output[key], suffix);
            }
            return this;
        }

        public FieldSqlWriter AppendIfNot(string suffix, FieldType fieldType) {
            foreach (var key in CopyOutputKeys()) {
                var field = _original[key];
                if (field.FieldType != fieldType)
                    _output[key] = string.Concat(_output[key], suffix);
            }
            return this;
        }

        public FieldSqlWriter XmlValue() {
            foreach (var key in CopyOutputKeys()) {
                var field = _original[key];
                _output[key] = string.Format("t.[{0}].value('({1})[{2}]', '{3}')", field.Parent, field.XPath, field.Index, field.SqlDataType);
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

        public FieldSqlWriter Xml(bool answer) {
            foreach (var key in CopyOutputKeys()) {
                var field = _original[key];
                if (field.InnerXml.Any() != answer)
                    _output.Remove(key);
            }
            return this;
        }

        public FieldSqlWriter FieldType(FieldType answer) {
            foreach (var key in CopyOutputKeys()) {
                var field = _original[key];
                if (field.FieldType != answer)
                    _output.Remove(key);
            }
            return this;
        }

    }
}