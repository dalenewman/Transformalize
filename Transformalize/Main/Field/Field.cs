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
using Transformalize.Main.Providers.SqlServer;

namespace Transformalize.Main {

    public class Field {

        private readonly string[] _stringTypes = new[] { "string", "char", "datetime", "guid", "xml" };

        private FieldType _fieldType = FieldType.NonKey;
        private string _identifier = "identifier";
        private string _name = "field";
        private string _sqlDataType;
        private string _type = "System.String";
        private string _simpleType = "string";
        private bool _input = true;
        private bool _unicode = true;
        private bool _variableLength = true;
        private string _nodeType = "element";
        private bool _readInnerXml = true;
        private List<SearchType> _searchTypes = new List<SearchType>();
        private Type _systemType = typeof(string);
        private object _default;
        private string _alias;
        private string _label = string.Empty;
        private string _schema = string.Empty;
        private string _entity = string.Empty;
        private string _entityOutputName = string.Empty;
        private IParameters _parameters = new Parameters.Parameters();
        private string _sort = string.Empty;
        private string _aggregate = string.Empty;
        private string _process = string.Empty;
        private string _length = "64";
        private string _aliasLower = string.Empty;

        public string Alias {
            get { return _alias; }
            set {
                if (!value.Equals(_alias)) {
                    _alias = value;
                    _identifier = Common.CleanIdentifier(value);
                    _aliasLower = value.ToLower();
                }
            }
        }

        public string AliasLower {
            get { return _aliasLower; }
        }

        public string Identifier {
            get { return _identifier; }
        }

        public string Schema {
            get { return _schema; }
            set { _schema = value; }
        }

        public string Entity {
            get { return _entity; }
            set { _entity = value; }
        }

        public string EntityOutputName {
            get { return _entityOutputName; }
            set { _entityOutputName = value; }
        }

        public string Process {
            get { return _process; }
            set { _process = value; }
        }

        public string Length {
            get { return _length; }
            set { _length = value; }
        }

        public int Precision { get; set; }
        public int Scale { get; set; }
        public bool NotNull { get; set; }
        public bool Identity { get; set; }
        public KeyValuePair<string, string> References { get; set; }
        public bool Output { get; set; }
        public bool FileOutput { get; set; }
        public short Index { get; set; }

        public string Aggregate {
            get { return _aggregate; }
            set { _aggregate = value; }
        }

        public string Sort {
            get { return _sort; }
            set { _sort = value; }
        }

        public string Label {
            get { return _label.Equals(string.Empty) ? Alias : _label; }
            set { _label = value; }
        }

        public IParameters Parameters {
            get { return _parameters; }
            set { _parameters = value; }
        }

        public bool HasParameters { get; set; }
        public bool DefaultBlank { get; set; }
        public bool DefaultWhiteSpace { get; set; }
        public char QuotedWith { get; set; }
        public bool Optional { get; set; }
        public List<string> Transforms { get; set; }
        public string Delimiter { get; set; }
        public bool Distinct { get; set; }
        public int EntityIndex { get; set; }
        public bool IsCalculated { get; set; }

        public object Default {
            get { return _default; }
            set { _default = new DefaultFactory().Convert(value, SimpleType); }
        }

        public Type SystemType {
            get { return _systemType; }
        }

        public List<SearchType> SearchTypes {
            get { return _searchTypes; }
            set { _searchTypes = value; }
        }

        public Field(FieldType fieldType)
            : this("System.String", "64", fieldType, true, null) {
        }

        public Field(string typeName, string length, FieldType fieldType, bool output, string @default) {
            Initialize(typeName, length, fieldType, output, @default);
        }

        public string Type {
            get { return _type; }
            set {
                _type = value;
                _simpleType = Common.ToSimpleType(value);
                _systemType = Common.ToSystemType(_simpleType);
                Default = _default; //reset default
                if (_simpleType.Equals("rowversion", StringComparison.OrdinalIgnoreCase)) {
                    Length = "8";
                }
            }
        }

        public string SimpleType {
            get { return _simpleType; }
        }

        public bool Input {
            get { return _input; }
            set { _input = value; }
        }

        public bool Unicode {
            get { return _unicode; }
            set { _unicode = value; }
        }

        public bool VariableLength {
            get { return _variableLength; }
            set { _variableLength = value; }
        }

        public string NodeType {
            get { return _nodeType; }
            set { _nodeType = value; }
        }

        public bool ReadInnerXml {
            get { return _readInnerXml; }
            set { _readInnerXml = value; }
        }

        public string SqlDataType {
            get { return _sqlDataType ?? (_sqlDataType = new SqlServerDataTypeService().GetDataType(this)); }
        }

        /// <summary>
        ///     FieldType can affect Output
        /// </summary>
        public FieldType FieldType {
            get { return _fieldType; }
            set {
                _fieldType = value;
                if (MustBeOutput()) {
                    Output = true;
                }
            }
        }

        /// <summary>
        ///     Alias follows name if no alias provided
        /// </summary>
        public string Name {
            get { return _name; }
            set {
                _name = value;
                if (string.IsNullOrEmpty(Alias)) {
                    Alias = Name;
                }
            }
        }

        public bool MustBeOutput() {
            return FieldType.HasFlag(FieldType.MasterKey) || FieldType.HasFlag(FieldType.ForeignKey) ||
                   FieldType.HasFlag(FieldType.PrimaryKey);
        }

        private void Initialize(string typeName, string length, FieldType fieldType, bool output, string @default) {
            Type = typeName;
            Length = length;
            FieldType = fieldType;
            Output = output || MustBeOutput();
            FileOutput = output;
            Default = @default;
            Transforms = new List<string>();
        }

        public string Quote() {
            return _stringTypes.Any(t => t.Equals(SimpleType)) ? "'" : string.Empty;
        }

        public override string ToString() {
            return string.Format("({0}) {1}", Type, Alias);
        }

        public Parameter ToParameter(bool useDefaultForValue = false) {
            return new Parameter() {
                Name = Alias,
                Value = useDefaultForValue ? Default : null,
                SimpleType = SimpleType
            };
        }

        public bool IsQuoted() {
            return QuotedWith != default(char);
        }
    }
}