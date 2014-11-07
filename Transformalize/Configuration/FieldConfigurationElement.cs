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
        private const string SHORT_HAND = "t";
        private const string RAW = "raw";

        /// <summary>
        /// **Required**
        /// 
        ///     <add name="Name" />
        /// </summary>
        [ConfigurationProperty(NAME, IsRequired = true)]
        public string Name {
            get { return this[NAME] as string; }
            set { this[NAME] = value; }
        }

        /// <summary>
        /// Optional
        /// 
        /// Used to over-ride the default ordering of the fields.
        /// 
        ///     <add name="Name" index="0" />
        /// </summary>
        [ConfigurationProperty(INDEX, IsRequired = false, DefaultValue = short.MaxValue)]
        public short Index {
            get { return (short)this[INDEX]; }
            set { this[INDEX] = value; }
        }

        /// <summary>
        /// Optional
        /// 
        /// The name should always correspond with the input field's name.  Alias is used to rename it to something 
        /// else.  An alias must be unique across the entire process.  The only exception to this rule is when the 
        /// field is a primary key that is related to another entity's foreign key (of the same name).
        /// </summary>
        [ConfigurationProperty(ALIAS, IsRequired = false, DefaultValue = "")]
        public string Alias {
            get {
                var alias = this[ALIAS] as string;
                return alias == null || alias.Equals(string.Empty) ? Name : alias;
            }
            set { this[ALIAS] = value; }
        }

        /// <summary>
        /// Optional.  Default is `string`
        /// 
        /// This may be one of these types:
        /// 
        /// * string
        /// * xml
        /// * int16
        /// * short
        /// * int32
        /// * int
        /// * int64
        /// * long
        /// * double
        /// * decimal
        /// * char
        /// * datetime
        /// * date
        /// * boolean
        /// * bool
        /// * single
        /// * real
        /// * float
        /// * guid
        /// * byte
        /// * byte[]
        /// * rowversion
        /// * uint64
        /// * object
        /// 
        /// </summary>
        [ConfigurationProperty(TYPE, IsRequired = false, DefaultValue = "System.String")]
        public string Type {
            get { return this[TYPE] as string; }
            set { this[TYPE] = value; }
        }

        /// <summary>
        /// Optional.  Default is `, `
        /// 
        /// Used in join aggregations.  Note: This only takes affect when the entity has the `group="true"` attribute set.
        /// 
        ///     <add name="Name" delimiter="|" aggregate="join" />
        /// </summary>
        [ConfigurationProperty(DELIMITER, IsRequired = false, DefaultValue = ", ")]
        public string Delimiter {
            get { return this[DELIMITER] as string; }
            set { this[DELIMITER] = value; }
        }

        /// <summary>
        /// Optional.
        /// 
        /// Used when importing delimited files.  Some formats, like .csv for example, use a character to quote string (or text) values.
        /// 
        ///     <add name="Name" type="string" quoted-with="&quot;" />
        /// 
        /// </summary>
        [ConfigurationProperty(QUOTED_WITH, IsRequired = false, DefaultValue = default(char))]
        public char QuotedWith {
            get { return (char) this[QUOTED_WITH]; }
            set { this[QUOTED_WITH] = value; }
        }

        /// <summary>
        /// Optional. Default is `false`
        /// 
        /// Used when importing delimited files.  Fields at the end of a line may be marked as optional.
        /// </summary>
        [ConfigurationProperty(OPTIONAL, IsRequired = false, DefaultValue = false)]
        public bool Optional {
            get { return (bool)this[OPTIONAL]; }
            set { this[OPTIONAL] = value; }
        }

        /// <summary>
        /// Optional. Default is `false`
        /// 
        /// Used to tell rendering tools that the contents of this field should not be encoded.  The contents should be rendered raw (un-touched).
        /// For example: This is a useful indicator if you've created HTML content, and you don't want something encoding it later.
        /// </summary>
        [ConfigurationProperty(RAW, IsRequired = false, DefaultValue = false)]
        public bool Raw {
            get { return (bool)this[RAW]; }
            set { this[RAW] = value; }
        }

        /// <summary>
        /// Optional.  Default is `false`
        /// 
        /// Usually a field is set to a default if it is NULL.  If set to true, the default will overwrite blank values as well.
        /// 
        ///     <add name="Name" default="None" default-blank="true" />
        /// </summary>
        [ConfigurationProperty(DEFAULT_BLANK, IsRequired = false, DefaultValue = false)]
        public bool DefaultBlank {
            get { return (bool)this[DEFAULT_BLANK] || (bool)this[DEFAULT_EMPTY]; }
            set { this[DEFAULT_BLANK] = value; }
        }

        /// <summary>
        /// Optional. Default to `false`
        /// 
        /// Usually a field is set to a default if it is NULL.  If this is set to true, the default will overwrite empty values as well (same as DefaultBlank).
        /// 
        ///     <add name="Name" default="None" default-empty="true" />
        /// </summary>
        [ConfigurationProperty(DEFAULT_EMPTY, IsRequired = false, DefaultValue = false)]
        public bool DefaultEmpty {
            get { return (bool)this[DEFAULT_EMPTY] || (bool)this[DEFAULT_BLANK]; }
            set { this[DEFAULT_EMPTY] = value; }
        }

        /// <summary>
        /// Optional. Default to `false`
        /// 
        /// Usually a field is set to a default if it is NULL.  If this is set to true, the default will overwrite white-space values as well.
        /// 
        ///     <add name="Name" default="None" default-empty="true" default-white-space="true" />
        /// </summary>
        [ConfigurationProperty(DEFAULT_WHITE_SPACE, IsRequired = false, DefaultValue = false)]
        public bool DefaultWhiteSpace {
            get { return (bool)this[DEFAULT_WHITE_SPACE]; }
            set { this[DEFAULT_WHITE_SPACE] = value; }
        }

        /// <summary>
        /// Optional.  Default is `default`
        /// 
        /// Used with search engine outputs like Lucene, Elasticsearch, and SOLR.  Corresponds to a defined search type.
        /// 
        ///     <add name="Name" search-type="keyword" />
        /// </summary>
        [ConfigurationProperty(SEARCH_TYPE, IsRequired = false, DefaultValue = "default")]
        public string SearchType {
            get { return this[SEARCH_TYPE] as string; }
            set { this[SEARCH_TYPE] = value; }
        }

        /// <summary>
        /// A collection of search types.
        /// </summary>
        [ConfigurationProperty(SEARCH_TYPES)]
        public FieldSearchTypeElementCollection SearchTypes {
            get { return this[SEARCH_TYPES] as FieldSearchTypeElementCollection; }
        }

        /// <summary>
        /// Optional. Default is `64`
        /// 
        /// This is the maximum length allowed for a field.  Any content exceeding this length will be truncated. 
        /// Note: A warning is issued in the logs when this occurs, so you can increase the length if necessary.
        /// </summary>
        [ConfigurationProperty(LENGTH, IsRequired = false, DefaultValue = "64")]
        public string Length {
            get { return this[LENGTH] as string; }
            set { this[LENGTH] = value; }
        }

        /// <summary>
        /// Optional. Default is `18`
        /// </summary>
        [ConfigurationProperty(PRECISION, IsRequired = false, DefaultValue = 18)]
        public int Precision {
            get { return (int)this[PRECISION]; }
            set { this[PRECISION] = value; }
        }

        /// <summary>
        /// Optional. Default is `9`
        /// </summary>
        [ConfigurationProperty(SCALE, IsRequired = false, DefaultValue = 9)]
        public int Scale {
            get { return (int)this[SCALE]; }
            set { this[SCALE] = value; }
        }

        /// <summary>
        /// Optional. Default is `true`
        /// 
        /// Indicates a value is expected from the source (or *input*).
        /// </summary>
        [ConfigurationProperty(INPUT, IsRequired = false, DefaultValue = true)]
        public bool Input {
            get { return (bool)this[INPUT]; }
            set { this[INPUT] = value; }
        }

        /// <summary>
        /// Optional. Default is `false`
        /// 
        /// Indicates this field is (or is part of) the entity's primary key (or unique identifier).
        /// </summary>
        [ConfigurationProperty(PRIMARY_KEY, IsRequired = false, DefaultValue = false)]
        public bool PrimaryKey {
            get { return (bool)this[PRIMARY_KEY]; }
            set { this[PRIMARY_KEY] = value; }
        }

        /// <summary>
        /// Optional. Default is `true`
        /// 
        /// Indicates this field is *output* to the defined connection (or destination).
        /// </summary>
        [ConfigurationProperty(OUTPUT, IsRequired = false, DefaultValue = true)]
        public bool Output {
            get { return (bool)this[OUTPUT]; }
            set { this[OUTPUT] = value; }
        }

        /// <summary>
        /// Optional. Default is `true`
        /// 
        /// Used when fields are defined inside a `fromXml` transform.  In this type of transform, the field's name corresponds to an element's name.
        /// If this setting is true, `<Name>Contents</Name>` yields `Contents` (what's inside the element)
        /// If false, `<Name>Contents</Name>` yields `<Name>Contents</Name>` (the element and what's inside the element)
        /// </summary>
        [ConfigurationProperty(READ_INNER_XML, IsRequired = false, DefaultValue = true)]
        public bool ReadInnerXml {
            get { return (bool)this[READ_INNER_XML]; }
            set { this[READ_INNER_XML] = value; }
        }

        /// <summary>
        /// Optional. Default defers to entity's unicode attribute, which defaults to `true`
        /// 
        /// Sometimes source data is ASCII, and you want it to stay that way. Take the SQL Server output connection for example: 
        /// If set to true, a string is stored in the NVARCHAR data type (unicode).
        /// If set to false, a string is stored in the VARCHAR data type (not unicode).
        /// </summary>
        [ConfigurationProperty(UNICODE, IsRequired = false, DefaultValue = Common.DefaultValue)]
        public string Unicode {
            get { return (string)this[UNICODE]; }
            set { this[UNICODE] = value; }
        }

        /// <summary>
        /// Optional. Default defers to entity's variable-length attribute, which defaults to `true`
        /// 
        /// Sometimes source data is not a variable length, and you want it to stay that way. Take the SQL Server output connection for example:
        /// If set to true, a string is stored in the NVARCHAR data type (variable length).
        /// If set to false, a string is stored in the NCHAR data type (fixed length).
        /// </summary>
        [ConfigurationProperty(VARIABLE_LENGTH, IsRequired = false, DefaultValue = Common.DefaultValue)]
        public string VariableLength {
            get { return (string)this[VARIABLE_LENGTH]; }
            set { this[VARIABLE_LENGTH] = value; }
        }

        /// <summary>
        /// Optional. The default varies based on type.
        /// 
        /// This value overwrites NULL values so you don't have to worry about NULLs in the pipeline.  It can also be configured to overwrite blanks 
        /// and white-space by other attributes. 
        /// </summary>
        [ConfigurationProperty(DEFAULT, IsRequired = false, DefaultValue = "")]
        public string Default {
            get { return (string)this[DEFAULT]; }
            set { this[DEFAULT] = value; }
        }

        /// <summary>
        /// Optional. Default is `element`
        /// 
        /// Used when fields are defined inside a `fromXml` transform.  In this type of transform, 
        /// the field's name corresponds to an *element*'s name or an *attribute*'s name.
        /// </summary>
        [ConfigurationProperty(NODE_TYPE, IsRequired = false, DefaultValue = "element")]
        public string NodeType {
            get { return (string)this[NODE_TYPE]; }
            set { this[NODE_TYPE] = value; }
        }

        /// <summary>
        /// A collection of transforms.
        /// </summary>
        [ConfigurationProperty(TRANSFORMS)]
        public TransformElementCollection Transforms {
            get { return this[TRANSFORMS] as TransformElementCollection; }
            set { this[TRANSFORMS] = value; }
        }

        /// <summary>
        /// Optional.
        /// 
        /// An aggregate function is only applicable when entity is set to group (e.g. `group="true"`). Note: When the entity is set to group, all output fields must have an aggregate function defined.
        /// 
        /// * min
        /// * minlength
        /// * max
        /// * maxlength
        /// * join
        /// * concat
        /// * array
        /// * sum
        /// * count
        /// * group
        /// * first
        /// * last
        /// 
        /// </summary>
        [ConfigurationProperty(AGGREGATE, IsRequired = false, DefaultValue = "")]
        public string Aggregate {
            get { return (string)this[AGGREGATE]; }
            set { this[AGGREGATE] = value; }
        }

        /// <summary>
        /// Optional
        /// 
        /// * asc
        /// * desc
        /// </summary>
        [ConfigurationProperty(SORT, IsRequired = false, DefaultValue = "")]
        public string Sort {
            get { return (string)this[SORT]; }
            set { this[SORT] = value; }
        }

        /// <summary>
        /// Optional.
        /// 
        /// A field's label.  A label is a more descriptive name that can contain spaces, etc.
        /// </summary>
        [ConfigurationProperty(LABEL, IsRequired = false, DefaultValue = "")]
        public string Label {
            get { return (string)this[LABEL]; }
            set { this[LABEL] = value; }
        }

        /// <summary>
        /// Optional. Default is `false`
        /// 
        /// Used in conjunction with count, join, and concat aggregate functions.
        /// </summary>
        [ConfigurationProperty(DISTINCT, IsRequired = false, DefaultValue = false)]
        public bool Distinct {
            get { return (bool)this[DISTINCT]; }
            set { this[DISTINCT] = value; }
        }

        /// <summary>
        /// Optional.
        /// 
        /// An alternate (shorter) way to define simple transformations.
        /// 
        ///     <add name="Name" t="trim()" />
        /// </summary>
        [ConfigurationProperty(SHORT_HAND, IsRequired = false, DefaultValue = "")]
        public string ShortHand {
            get { return (string)this[SHORT_HAND]; }
            set { this[SHORT_HAND] = value; }
        }

        public override bool IsReadOnly() {
            return false;
        }
    }
}