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

using System.Configuration;
using Transformalize.Main;

namespace Transformalize.Configuration {

    public class FieldConfigurationElement : ConfigurationElement {

        private const string AGGREGATE = "aggregate";
        private const string NAME = "name";
        private const string ALIAS = "alias";
        private const string TYPE = "type";
        private const string QUOTED_WITH = "quoted-with";
        private const string OPTIONAL = "optional";
        private const string LENGTH = "length";
        private const string PRECISION = "precision";
        private const string SCALE = "scale";
        private const string INPUT = "input";
        private const string OUTPUT = "output";
        private const string UNICODE = "unicode";
        private const string VARIABLE_LENGTH = "variable-length";
        private const string DEFAULT = "default";
        private const string DEFAULT_BLANK = "default-blank";
        private const string DEFAULT_EMPTY = "default-empty";
        private const string DEFAULT_WHITE_SPACE = "default-white-space";
        private const string TRANSFORMS = "transforms";
        private const string SEARCH_TYPE = "search-type";
        private const string SEARCH_TYPES = "search-types";
        private const string PRIMARY_KEY = "primary-key";
        private const string INDEX = "index";
        private const string NODE_TYPE = "node-type";
        private const string READ_INNER_XML = "read-inner-xml";
        private const string SORT = "sort";
        private const string LABEL = "label";
        private const string DELIMITER = "delimiter";
        private const string DISTINCT = "distinct";

        [ConfigurationProperty(NAME, IsRequired = true)]
        public string Name {
            get { return this[NAME] as string; }
            set { this[NAME] = value; }
        }

        [ConfigurationProperty(INDEX, IsRequired = false, DefaultValue = 0)]
        public int Index {
            get { return (int)this[INDEX]; }
            set { this[INDEX] = value; }
        }

        [ConfigurationProperty(ALIAS, IsRequired = false, DefaultValue = "")]
        public string Alias {
            get {
                var alias = this[ALIAS] as string;
                return alias == null || alias.Equals(string.Empty) ? Name : alias;
            }
            set { this[ALIAS] = Common.CleanIdentifier(value); }
        }

        [ConfigurationProperty(TYPE, IsRequired = false, DefaultValue = "System.String")]
        public string Type {
            get { return this[TYPE] as string; }
            set { this[TYPE] = value; }
        }

        [ConfigurationProperty(DELIMITER, IsRequired = false, DefaultValue = ", ")]
        public string Delimiter {
            get { return this[DELIMITER] as string; }
            set { this[DELIMITER] = value; }
        }

        [ConfigurationProperty(QUOTED_WITH, IsRequired = false)]
        public string QuotedWith {
            get { return this[QUOTED_WITH] as string; }
            set { this[QUOTED_WITH] = value; }
        }

        [ConfigurationProperty(OPTIONAL, IsRequired = false, DefaultValue = false)]
        public bool Optional {
            get { return (bool)this[OPTIONAL]; }
            set { this[OPTIONAL] = value; }
        }

        [ConfigurationProperty(DEFAULT_BLANK, IsRequired = false, DefaultValue = false)]
        public bool DefaultBlank {
            get { return (bool)this[DEFAULT_BLANK] || (bool)this[DEFAULT_EMPTY]; }
            set { this[DEFAULT_BLANK] = value; }
        }

        [ConfigurationProperty(DEFAULT_EMPTY, IsRequired = false, DefaultValue = false)]
        public bool DefaultEmpty {
            get { return (bool)this[DEFAULT_EMPTY] || (bool)this[DEFAULT_BLANK]; }
            set { this[DEFAULT_EMPTY] = value; }
        }

        [ConfigurationProperty(DEFAULT_WHITE_SPACE, IsRequired = false, DefaultValue = false)]
        public bool DefaultWhiteSpace {
            get { return (bool)this[DEFAULT_WHITE_SPACE]; }
            set { this[DEFAULT_WHITE_SPACE] = value; }
        }

        [ConfigurationProperty(SEARCH_TYPE, IsRequired = false, DefaultValue = "default")]
        public string SearchType {
            get { return this[SEARCH_TYPE] as string; }
            set { this[SEARCH_TYPE] = value; }
        }

        [ConfigurationProperty(SEARCH_TYPES)]
        public FieldSearchTypeElementCollection SearchTypes {
            get { return this[SEARCH_TYPES] as FieldSearchTypeElementCollection; }
        }

        [ConfigurationProperty(LENGTH, IsRequired = false, DefaultValue = "64")]
        public string Length {
            get { return this[LENGTH] as string; }
            set { this[LENGTH] = value; }
        }

        [ConfigurationProperty(PRECISION, IsRequired = false, DefaultValue = 18)]
        public int Precision {
            get { return (int)this[PRECISION]; }
            set { this[PRECISION] = value; }
        }

        [ConfigurationProperty(SCALE, IsRequired = false, DefaultValue = 9)]
        public int Scale {
            get { return (int)this[SCALE]; }
            set { this[SCALE] = value; }
        }

        [ConfigurationProperty(INPUT, IsRequired = false, DefaultValue = true)]
        public bool Input {
            get { return (bool)this[INPUT]; }
            set { this[INPUT] = value; }
        }

        [ConfigurationProperty(PRIMARY_KEY, IsRequired = false, DefaultValue = false)]
        public bool PrimaryKey {
            get { return (bool)this[PRIMARY_KEY]; }
            set { this[PRIMARY_KEY] = value; }
        }

        [ConfigurationProperty(OUTPUT, IsRequired = false, DefaultValue = true)]
        public bool Output {
            get { return (bool)this[OUTPUT]; }
            set { this[OUTPUT] = value; }
        }

        [ConfigurationProperty(READ_INNER_XML, IsRequired = false, DefaultValue = true)]
        public bool ReadInnerXml {
            get { return (bool)this[READ_INNER_XML]; }
            set { this[READ_INNER_XML] = value; }
        }

        [ConfigurationProperty(UNICODE, IsRequired = false, DefaultValue = "[default]")]
        public string Unicode {
            get { return (string)this[UNICODE]; }
            set { this[UNICODE] = value; }
        }

        [ConfigurationProperty(VARIABLE_LENGTH, IsRequired = false, DefaultValue = "[default]")]
        public string VariableLength {
            get { return (string)this[VARIABLE_LENGTH]; }
            set { this[VARIABLE_LENGTH] = value; }
        }

        [ConfigurationProperty(DEFAULT, IsRequired = false, DefaultValue = "")]
        public string Default {
            get { return (string)this[DEFAULT]; }
            set { this[DEFAULT] = value; }
        }

        [ConfigurationProperty(NODE_TYPE, IsRequired = false, DefaultValue = "element")]
        public string NodeType {
            get { return (string)this[NODE_TYPE]; }
            set { this[NODE_TYPE] = value; }
        }

        [ConfigurationProperty(TRANSFORMS)]
        public TransformElementCollection Transforms {
            get { return this[TRANSFORMS] as TransformElementCollection; }
            set { this[TRANSFORMS] = value; }
        }

        [ConfigurationProperty(AGGREGATE, IsRequired = false, DefaultValue = "")]
        public string Aggregate {
            get { return (string)this[AGGREGATE]; }
            set { this[AGGREGATE] = value; }
        }

        [ConfigurationProperty(SORT, IsRequired = false, DefaultValue = "")]
        public string Sort {
            get { return (string)this[SORT]; }
            set { this[SORT] = value; }
        }

        [ConfigurationProperty(LABEL, IsRequired = false, DefaultValue = "")]
        public string Label {
            get { return (string)this[LABEL]; }
            set { this[LABEL] = value; }
        }

        [ConfigurationProperty(DISTINCT, IsRequired = false, DefaultValue = false)]
        public bool Distinct {
            get { return (bool)this[DISTINCT]; }
            set { this[DISTINCT] = value; }
        }

        public override bool IsReadOnly() {
            return false;
        }
    }
}