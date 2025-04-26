#region license
// Transformalize
// Configurable Extract, Transform, and Load
// Copyright 2013-2025 Dale Newman
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
using Cfg.Net;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Transformalize.Contracts;

namespace Transformalize.Configuration {
   public class Field : CfgNode, IField {

      public static readonly List<string> InvalidNames = new List<string> { Constants.TflHashCode.ToLower(), Constants.TflBatchId.ToLower(), Constants.TflKey.ToLower(), Constants.TflDeleted.ToLower() };

      private string _type;
      private string _length;
      private Func<string, object> _toDateTime;

      public Field() {
         _toDateTime = GetDateTimeConversion();
      }

      private Func<string, object> GetDateTimeConversion() {
         if (!Type.StartsWith("date")) return null;
         if (Format == string.Empty) {
            return s => Constants.ConversionMap[Type](s);
         }
         return s => DateTime.ParseExact(s, Format, CultureInfo.InvariantCulture);
      }

      [Cfg(required = true)]
      public string Name { get; set; }

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
      [Cfg(value = "string", domain = Constants.TypeDomain, toLower = true)]
      public string Type {
         get => _type;
         set {
            if (value == null)
               return;

            // legacy
            if (value.StartsWith("system.", StringComparison.Ordinal)) {
               value = value.Replace("system.", string.Empty);
            }

            // normalize
            switch (value) {
               case "date":
                  value = "datetime";
                  break;
               case "int32":
                  value = "int";
                  break;
               case "int64":
                  value = "long";
                  break;
               case "int16":
                  value = "short";
                  break;
               case "boolean":
                  value = "bool";
                  break;
               default:
                  break;
            }

            _type = value;
            _toDateTime = GetDateTimeConversion();
         }
      }

      [Cfg(value = "defer", domain = "defer,button,checkbox,color,date,datetime-local,email,file,google-places-autocomplete,hidden,image,location,month,number,password,radio,range,reset,scan,search,submit,tel,text,time,url,week", toLower = true)]
      public string InputType { get; set; }

      [Obsolete("InputAccept is only needed in Parameter.")]
      [Cfg(value = "", toLower = true)]
      public string InputAccept { get; set; }

      [Obsolete("InputCapture is only needed in Parameter.")]
      [Cfg(value = "", toLower = true)]
      public string InputCapture { get; set; }

      /// <summary>
      /// Optional. Default to `false`
      /// 
      /// Usually a field is set to a default if it is NULL.  If this is set to true, the default will overwrite white-space values as well.
      /// 
      ///     <add name="Name" default="None" default-empty="true" default-white-space="true" />
      /// </summary>
      [Cfg(value = false)]
      public bool DefaultWhiteSpace { get; set; }

      [Cfg(value = false)]
      public bool DefaultEmpty { get; set; }

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

      [Cfg(serialize = false)]
      public short Index { get; set; }

      public short KeyIndex { get; set; }

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
      [Cfg(required = false, unique = true, value = null)]
      public string Alias { get; set; }

      /// <summary>
      /// Optional. The default varies based on type.
      /// 
      /// This value overwrites NULL values so you don't have to worry about NULLs in the pipeline.  It can also be configured to overwrite blanks 
      /// and white-space by other attributes. 
      /// </summary>
      [Cfg(value = Constants.DefaultSetting)]
      public string Default { get; set; }

      public object DefaultValue() {
         return Default != Constants.DefaultSetting ? Convert(Default) : Constants.TypeDefaults()[Type];
      }

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
         get => _length;
         set {
            if (value == null)
               return;
            if (int.TryParse(value, out var number)) {
               if (number <= 0) {
                  Error("A field's length must be a number greater than zero, or max.");
               }
            } else {
               if (!value.Equals("max", StringComparison.OrdinalIgnoreCase)) {
                  Error("A field's length must be a number greater than zero, or max.");
               }
            }
            _length = value;
         }
      }


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
      [Cfg(value = "default", toLower = true)]
      public string SearchType { get; set; }

      /// <summary>
      /// Optional.
      /// 
      /// An alternate (shorter) way to define simple transformations.
      /// 
      ///     <add name="Name" t="trim()" />
      /// </summary>
      [Cfg(value = "")]
      public string T { get; set; }

      [Cfg(value = "")]
      public string V { get; set; }

      [Cfg(value = "")]
      public string ValidField { get; set; }

      [Cfg(value = "")]
      public string MessageField { get; set; }

      [Cfg(value = true)]
      public bool Unicode { get; set; }

      [Cfg(value = true)]
      public bool VariableLength { get; set; }

      //lists
      [Cfg]
      public List<Operation> Transforms { get; set; }

      [Cfg]
      public List<Operation> Validators { get; set; }

      [Cfg]
      public List<string> Domain { get; set; }

      /// <summary>
      /// Set by Process.ModifyKeyTypes
      /// </summary>
      public KeyType KeyType { get; set; }

      [Cfg(serialize = false)]
      public short EntityIndex { get; internal set; }

      public short MasterIndex { get; set; }

      [Cfg(value = false)]
      public bool System { get; set; }

      protected override void Validate() {

         // handle missing valid fields
         if (!string.IsNullOrEmpty(V) || Validators.Any()) {
            if (ValidField == string.Empty) {
               ValidField = Alias + "Valid";
            }
            if (MessageField == string.Empty) {
               MessageField = Alias + "Message";
            }
         }

         if (!Map.Contains(",")) {
            Map = Map.ToLower();
         }
      }

      protected override void PreValidate() {

         if (Name == null) {
            Error("Name in field is null. Name is required.");
            Name = "Error";
         }

         if (string.IsNullOrEmpty(Alias)) { Alias = Name; }

         if (Label == string.Empty) { Label = Alias; }

         if (Type == "rowversion") { Length = "8"; }

         if (Type == "char" && Length == "64") {
            if (Length != "64") {
               Warn($"The field {Alias} is a char, but has a length of {Length}.  A char may only hold 1 character, so it's length is set to 1.");
            }
            Length = "1";
         }

      }

      public object Convert(string value) {
         switch (Type) {
            case "string":
               return value;
            case "date":
            case "datetime":
               return _toDateTime(value);
            default:
               return Constants.ConversionMap[Type](value);
         }
      }

      public object Convert(object value) {
         return Constants.ObjectConversionMap[Type](value);
      }

      internal bool Is(string type) {
         return type == Type;
      }

      public string FieldName() {
         return Utility.GetExcelName(EntityIndex) + (Index + 1);
      }

      public override string ToString() {
         return $"{Alias}:{Type}({FieldName()})";
      }

      public bool IsCalculated { get; set; }
      public bool Produced { get; set; }

      public override bool Equals(object obj) {
         var other = obj as Field;

         if (other == null)
            return false;

         return Alias == other.Alias;

      }

      [Cfg(value = "", trim = true)]
      public string SortField { get; set; }


      [Cfg(value = "defer", trim = true, toLower = true, ignoreCase = true, domain = "true,false,defer")]
      public string Export { get; set; }


      [Cfg(value = Constants.DefaultSetting, domain = "true,false," + Constants.DefaultSetting, ignoreCase = true, toLower = true)]
      public string Sortable { get; set; }

      public override int GetHashCode() {
         return Alias.GetHashCode();
      }

      public int Ordinal { get; set; }

      [Obsolete("Use IsNumericType instead.")]
      public bool IsNumeric() {
         return Constants.IsNumericType(Type);
      }

      public bool IsNumericType() {
         return Constants.IsNumericType(Type);
      }

      public bool IsDecimalType() {
         return Constants.IsDecimalType(Type);
      }

      [Cfg(value = "")]
      public string Class { get; set; }
      [Cfg(value = "")]
      public string Style { get; set; }
      [Cfg(value = "")]
      public string Role { get; set; }
      [Cfg(value = "")]
      public string HRef { get; set; }
      [Cfg(value = "")]
      public string Target { get; set; }

      [Cfg(value = "")]
      public string Body { get; set; }

      [Cfg(value = "")]
      public string Src { get; set; }

      [Cfg(value = "")]
      public string Format { get; set; }

      [Cfg(value = "")]
      public string Connection { get; set; }

      [Cfg(value = false)]
      public bool Facet { get; set; }

      [Cfg(value = false)]
      public bool Learn { get; set; }

      [Cfg(value = "default", domain = "true,false,default", toLower = true, ignoreCase = true)]
      public string Dimension { get; set; }

      [Cfg(value = false)]
      public bool Measure { get; set; }

      [Cfg(value = "sum", domain = @"Sum,Count,Min,Max,DistinctCount,None,ByAccount,AverageOfChildren,FirstChild,LastChild,FirstNonEmpty,LastNonEmpty", ignoreCase = true, toLower = true)]
      public string AggregateFunction { get; set; }

      [Cfg(value = "")]
      public string Expression { get; set; }

      public string Source { get; set; }

      [Cfg(value = 0)]
      public int Width { get; set; }

      [Cfg(value = 0)]
      public int Height { get; set; }

      /// <summary>
      /// Used for validation messages
      /// </summary>
      [Cfg(value = "")]
      public string Help { get; set; }

      /// <summary>
      /// Used for form mode sections
      /// </summary>
      [Cfg(value = "")]
      public string Section { get; set; }

      /// <summary>
      /// Used for form mode sections
      /// </summary>
      [Cfg(value = "")]
      public string Hint { get; set; }

      /// <summary>
      /// Used in form mode to populate drop downs
      /// </summary>
      [Cfg(value = "", trim = true)]
      public string Map { get; set; }

      /// <summary>
      /// Used in form mode to control whether a change causes a post back (server-side validation)
      /// </summary>
      [Cfg(value = "auto", domain = "auto,true,false", ignoreCase = true, toLower = true, trim = true)]
      public string PostBack { get; set; }

      [Cfg(value = null)]
      public object Min { get; set; }

      [Cfg(value = null)]
      public object Max { get; set; }

      /// <summary>
      /// Run c# transforms in remote app domain? (to avoid memory leak).  It's slower, but shouldn't leak.
      /// </summary>
      [Cfg(value = false)]
      public bool Remote { get; set; }

      public Parameter ToFormParameter() {
         return new Parameter {
            Name = Name,
            Type = PrimaryKey ? Type : "string",
            Value = Default == Constants.DefaultSetting ? Constants.StringDefaults()[Type] : Default
         };
      }

      // match up a parameter by something other than this field's alias / name
      [Cfg()]
      public string Parameter { get; set; }

      [Cfg(value = false)]
      public bool Property { get; set; }

   }
}