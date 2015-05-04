using System;
using System.Collections.Generic;
using Transformalize.Libs.Cfg.Net;
using Transformalize.Main;

namespace Transformalize.Configuration {
    public class TflField : CfgNode {
        private string _type;
        private string _length;

        /// <summary>
        /// Optional.  Default is `false`
        /// 
        /// Usually a field is set to a default if it is NULL.  If set to true, the default will overwrite blank values as well.
        /// 
        ///     <add name="Name" default="None" default-blank="true" />
        /// </summary>
        [Cfg(value = false)]
        public bool DefaultBlank { get; set; }

        /// <summary>
        /// Optional. Default to `false`
        /// 
        /// Usually a field is set to a default if it is NULL.  If this is set to true, the default will overwrite empty values as well (same as DefaultBlank).
        /// 
        ///     <add name="Name" default="None" default-empty="true" />
        /// </summary>
        [Cfg(value = false)]
        public bool DefaultEmpty { get; set; }

        /// <summary>
        /// Optional. Default to `false`
        /// 
        /// Usually a field is set to a default if it is NULL.  If this is set to true, the default will overwrite white-space values as well.
        /// 
        ///     <add name="Name" default="None" default-empty="true" default-white-space="true" />
        /// </summary>
        [Cfg(value = false)]
        public bool DefaultWhiteSpace { get; set; }

        /// <summary>
        /// Optional. Default is `false`
        /// 
        /// Used in conjunction with count, join, and concat aggregate functions.
        /// </summary>
        [Cfg(value = false)]
        public bool Distinct { get; set; }

        /// <summary>
        /// Optional. Default is `true`
        /// 
        /// Indicates a value is expected from the source (or *input*).
        /// </summary>
        [Cfg(value = true)]
        public bool Input { get; set; }

        /// <summary>
        /// Optional. Default is `false`
        /// 
        /// Used when importing delimited files.  Fields at the end of a line may be marked as optional.
        /// </summary>
        [Cfg(value = false)]
        public bool Optional { get; set; }

        /// <summary>
        /// Optional. Default is `true`
        /// 
        /// Indicates this field is *output* to the defined connection (or destination).
        /// </summary>
        [Cfg(value = true)]
        public bool Output { get; set; }

        /// <summary>
        /// Optional. Default is `false`
        /// 
        /// Indicates this field is (or is part of) the entity's primary key (or unique identifier).
        /// </summary>
        [Cfg(value = false)]
        public bool PrimaryKey { get; set; }

        /// <summary>
        /// Optional. Default is `false`
        /// 
        /// Used to tell rendering tools that the contents of this field should not be encoded.  The contents should be rendered raw (un-touched).
        /// For example: This is a useful indicator if you've created HTML content, and you don't want something encoding it later.
        /// </summary>
        [Cfg(value = false)]
        public bool Raw { get; set; }

        /// <summary>
        /// Optional. Default is `true`
        /// 
        /// Used when fields are defined inside a `fromXml` transform.  In this type of transform, the field's name corresponds to an element's name.
        /// If this setting is true, `<Name>Contents</Name>` yields `Contents` (what's inside the element)
        /// If false, `<Name>Contents</Name>` yields `<Name>Contents</Name>` (the element and what's inside the element)
        /// </summary>
        [Cfg(value = true)]
        public bool ReadInnerXml { get; set; }

        /// <summary>
        /// Optional.
        /// 
        /// Used when importing delimited files.  Some formats, like .csv for example, use a character to quote string (or text) values.
        /// 
        ///     <add name="Name" type="string" quoted-with="&quot;" />
        /// 
        /// </summary>
        [Cfg(value = default(char))]
        public char QuotedWith { get; set; }

        /// <summary>
        /// Optional
        /// 
        /// Used to over-ride the default ordering of the fields.
        /// 
        ///     <add name="Name" index="0" />
        /// </summary>
        [Cfg(value = short.MaxValue)]
        public short Index { get; set; }

        /// <summary>
        /// Optional. Default is `18`
        /// </summary>
        [Cfg(value = 18)]
        public int Precision { get; set; }

        /// <summary>
        /// Optional. Default is `9`
        /// </summary>
        [Cfg(value = 9)]
        public int Scale { get; set; }

        /// <summary>
        /// Optional.
        /// 
        /// An aggregate function is only applicable when entity is set to group (e.g. `group="true"`). Note: When the entity is set to group, all output fields must have an aggregate function defined.
        /// 
        /// * array
        /// * concat
        /// * count
        /// * first
        /// * group
        /// * join
        /// * last
        /// * max
        /// * maxlength
        /// * min
        /// * minlength
        /// * sum
        /// 
        /// </summary>
        [Cfg(value = "last", domain = "array,concat,count,first,group,join,last,max,maxlength,min,minlength,sum", toLower = true)]
        public string Aggregate { get; set; }

        /// <summary>
        /// Optional
        /// 
        /// The name should always correspond with the input field's name.  Alias is used to rename it to something 
        /// else.  An alias must be unique across the entire process.  The only exception to this rule is when the 
        /// field is a primary key that is related to another entity's foreign key (of the same name).
        /// </summary>
        [Cfg(value = "", required = false, unique = true)]
        public string Alias { get; set; }

        /// <summary>
        /// Optional. The default varies based on type.
        /// 
        /// This value overwrites NULL values so you don't have to worry about NULLs in the pipeline.  It can also be configured to overwrite blanks 
        /// and white-space by other attributes. 
        /// </summary>
        [Cfg(value = "")]
        public string Default { get; set; }

        /// <summary>
        /// Optional.  Default is `, `
        /// 
        /// Used in join aggregations.  Note: This only takes affect when the entity has the `group="true"` attribute set.
        /// 
        ///     <add name="Name" delimiter="|" aggregate="join" />
        /// </summary>
        [Cfg(value = ", ")]
        public string Delimiter { get; set; }

        /// <summary>
        /// Optional.
        /// 
        /// A field's label.  A label is a more descriptive name that can contain spaces, etc.  Used by user interface builders.
        /// </summary>
        [Cfg(value = "")]
        public string Label { get; set; }

        /// <summary>
        /// Optional. Default is `64`
        /// 
        /// This is the maximum length allowed for a field.  Any content exceeding this length will be truncated. 
        /// Note: A warning is issued in the logs when this occurs, so you can increase the length if necessary.
        /// </summary>
        [Cfg(value = "64", toLower = true)]
        public string Length {
            get { return _length; }
            set {
                if (value == null)
                    return;
                int number;
                if (int.TryParse(value, out number)) {
                    if (number <= 0) {
                        AddProblem("A field's length must be a number greater than zero, or max.");
                    }
                } else {
                    if (value != "max") {
                        AddProblem("A field's length must be a number greater than zero, or max.");
                    }
                }
                _length = value;
            }
        }

        /// <summary>
        /// **Required**
        /// 
        ///     <add name="Name" />
        /// </summary>
        [Cfg(required = true)]
        public string Name { get; set; }

        /// <summary>
        /// Optional. Default is `element`
        /// 
        /// Used when fields are defined inside a `fromXml` transform.  In this type of transform, 
        /// the field's name corresponds to an *element*'s name or an *attribute*'s name.
        /// </summary>
        [Cfg(value = "element")]
        public string NodeType { get; set; }

        /// <summary>
        /// Optional.  Default is `default`
        /// 
        /// Used with search engine outputs like Lucene, Elasticsearch, and SOLR.  Corresponds to a defined search type.
        /// 
        ///     <add name="Name" search-type="keyword" />
        /// </summary>
        [Cfg(value = "default")]
        public string SearchType { get; set; }

        /// <summary>
        /// Optional
        /// 
        /// * asc
        /// * desc
        /// </summary>
        [Cfg(value = "none", domain = "asc,desc,none", toLower = true)]
        public string Sort { get; set; }

        /// <summary>
        /// Optional.
        /// 
        /// An alternate (shorter) way to define simple transformations.
        /// 
        ///     <add name="Name" t="trim()" />
        /// </summary>
        [Cfg(value = "")]
        public string T { get; set; }

        /// <summary>
        /// Optional.  Default is `string`
        /// 
        /// This may be one of these types:
        /// 
        /// * bool
        /// * boolean
        /// * byte
        /// * byte[]
        /// * char
        /// * date
        /// * datetime
        /// * decimal
        /// * double
        /// * float
        /// * guid
        /// * int
        /// * int16
        /// * int32
        /// * int64
        /// * long
        /// * object
        /// * real
        /// * rowversion
        /// * short
        /// * single
        /// * string
        /// * uint64
        /// * xml
        /// </summary>
        [Cfg(value = "string", domain = Common.ValidTypes, toLower = true)]
        public string Type {
            get { return _type; }
            set {
                if (value == null)
                    return;

                if (value == "date") {
                    _type = "datetime";
                } else {
                    _type = value.StartsWith("system.") ? value.Replace("system.", string.Empty) : value;
                }
            }
        }

        /// <summary>
        /// Optional. Default defers to entity's unicode attribute, which defaults to `true`
        /// 
        /// Sometimes source data is ASCII, and you want it to stay that way. Take the SQL Server output connection for example: 
        /// If set to true, a string is stored in the NVARCHAR data type (unicode).
        /// If set to false, a string is stored in the VARCHAR data type (not unicode).
        /// </summary>
        [Cfg(value = Common.DefaultValue)]
        public string Unicode { get; set; }

        /// <summary>
        /// Optional. Default defers to entity's variable-length attribute, which defaults to `true`
        /// 
        /// Sometimes source data is not a variable length, and you want it to stay that way. Take the SQL Server output connection for example:
        /// If set to true, a string is stored in the NVARCHAR data type (variable length).
        /// If set to false, a string is stored in the NCHAR data type (fixed length).
        /// </summary>
        [Cfg(value = Common.DefaultValue)]
        public string VariableLength { get; set; }


        //lists
        [Cfg()]
        public List<TflNameReference> SearchTypes { get; set; }
        [Cfg()]
        public List<TflTransform> Transforms { get; set; }
        [Cfg()]
        public List<string> Domain { get; set; }

        //custom
        protected override void Modify() {
            if (Alias == string.Empty) { Alias = Name; }
            if (Label == string.Empty) { Label = Alias; }
            if (Type != "string") { DefaultBlank = true; }
            if (Type == "rowversion") { Length = "8"; }
        }

    }
}