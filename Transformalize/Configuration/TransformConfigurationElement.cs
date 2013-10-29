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
using Transformalize.Libs.EnterpriseLibrary.Validation.Validators;

namespace Transformalize.Configuration
{
    public class TransformConfigurationElement : ConfigurationElement
    {
        private const string METHOD = "method";
        private const string VALUE = "value";
        private const string PATTERN = "pattern";
        private const string REPLACEMENT = "replacement";
        private const string OLD_VALUE = "old-value";
        private const string NEW_VALUE = "new-value";
        private const string TRIM_CHARS = "trim-chars";
        private const string INDEX = "index";
        private const string COUNT = "count";
        private const string START_INDEX = "start-index";
        private const string LENGTH = "length";
        private const string TOTAL_WIDTH = "total-width";
        private const string PADDING_CHAR = "padding-char";
        private const string MAP = "map";
        private const string SCRIPT = "script";
        private const string TEMPLATE = "template";
        private const string PARAMETERS = "parameters";
        private const string SCRIPTS = "scripts";
        private const string FORMAT = "format";
        private const string SEPARATOR = "separator";
        private const string MODEL = "model";
        private const string NAME = "name";
        private const string TEMPLATES = "templates";
        private const string PARAMETER = "parameter";
        private const string RESULT = "result";
        private const string EXPRESSION = "expression";
        private const string TYPE = "type";
        private const string ROOT = "root";
        private const string FIELDS = "fields";
        private const string TO = "to";
        private const string CLEAN = "clean";
        private const string UNITS = "units";
        private const string DOMAIN = "domain";
        private const string TRY_PARSE = "try-parse";
        private const string LEFT = "left";
        private const string OPERATOR = "operator";
        private const string FIELD = "field";
        private const string RIGHT = "right";
        private const string THEN = "then";
        private const string ELSE = "else";
        private const string FROM_TIME_ZONE = "from-time-zone";
        private const string TO_TIME_ZONE = "to-time-zone";
        private const string FROM_LAT = "from-lat";
        private const string FROM_LONG = "from-long";
        private const string TO_LAT = "to-lat";
        private const string TO_LONG = "to-long";

        //validation
        private const string APPEND_TO = "append-to";
        private const string CHARACTERS = "characters";
        private const string CONTAINS_CHARACTERS = "contains-characters";
        private const string MESSAGE = "message";
        private const string NEGATED = "negated";
        private const string LOWER_BOUND = "lower-bound";
        private const string LOWER_BOUND_TYPE = "lower-bound-type";
        private const string UPPER_BOUND = "upper-bound";
        private const string UPPER_BOUND_TYPE = "upper-bound-type";



        [ConfigurationProperty(METHOD, IsRequired = true)]
        public string Method
        {
            get { return this[METHOD] as string; }
            set { this[METHOD] = value; }
        }

        [ConfigurationProperty(NAME, IsRequired = false, DefaultValue = "")]
        public string Name
        {
            get { return this[NAME] as string; }
            set { this[NAME] = value; }
        }

        [ConfigurationProperty(CLEAN, IsRequired = false, DefaultValue = false)]
        public bool Clean
        {
            get { return (bool)this[CLEAN]; }
            set { this[CLEAN] = value; }
        }

        [ConfigurationProperty(TRY_PARSE, IsRequired = false, DefaultValue = false)]
        public bool TryParse
        {
            get { return (bool)this[TRY_PARSE]; }
            set { this[TRY_PARSE] = value; }
        }

        [ConfigurationProperty(UNITS, IsRequired = false, DefaultValue = "meters")]
        public string Units
        {
            get { return this[UNITS] as string; }
            set { this[UNITS] = value; }
        }

        [ConfigurationProperty(TO, IsRequired = false, DefaultValue = "")]
        public string To
        {
            get { return this[TO] as string; }
            set { this[TO] = value; }
        }

        [ConfigurationProperty(VALUE, IsRequired = false, DefaultValue = "")]
        public string Value
        {
            get { return this[VALUE] as string; }
            set { this[VALUE] = value; }
        }

        [ConfigurationProperty(PATTERN, IsRequired = false, DefaultValue = "")]
        public string Pattern
        {
            get { return this[PATTERN] as string; }
            set { this[PATTERN] = value; }
        }

        [ConfigurationProperty(REPLACEMENT, IsRequired = false, DefaultValue = "")]
        public string Replacement
        {
            get { return this[REPLACEMENT] as string; }
            set { this[REPLACEMENT] = value; }
        }

        [ConfigurationProperty(OLD_VALUE, IsRequired = false, DefaultValue = "")]
        public string OldValue
        {
            get { return this[OLD_VALUE] as string; }
            set { this[OLD_VALUE] = value; }
        }


        [ConfigurationProperty(NEW_VALUE, IsRequired = false, DefaultValue = "")]
        public string NewValue
        {
            get { return this[NEW_VALUE] as string; }
            set { this[NEW_VALUE] = value; }
        }

        [ConfigurationProperty(TRIM_CHARS, IsRequired = false, DefaultValue = " ")]
        public string TrimChars
        {
            get { return this[TRIM_CHARS] as string; }
            set { this[TRIM_CHARS] = value; }
        }

        [ConfigurationProperty(INDEX, IsRequired = false, DefaultValue = 0)]
        public int Index
        {
            get { return (int)this[INDEX]; }
            set { this[INDEX] = value; }
        }

        [ConfigurationProperty(COUNT, IsRequired = false, DefaultValue = 0)]
        public int Count
        {
            get { return (int)this[COUNT]; }
            set { this[COUNT] = value; }
        }

        [ConfigurationProperty(START_INDEX, IsRequired = false, DefaultValue = 0)]
        public int StartIndex
        {
            get { return (int)this[START_INDEX]; }
            set { this[START_INDEX] = value; }
        }

        [ConfigurationProperty(LENGTH, IsRequired = false, DefaultValue = 0)]
        public int Length
        {
            get { return (int)this[LENGTH]; }
            set { this[LENGTH] = value; }
        }

        [ConfigurationProperty(TOTAL_WIDTH, IsRequired = false, DefaultValue = 0)]
        public int TotalWidth
        {
            get { return (int)this[TOTAL_WIDTH]; }
            set { this[TOTAL_WIDTH] = value; }
        }

        [StringValidator(MaxLength = 1)]
        [ConfigurationProperty(PADDING_CHAR, IsRequired = false, DefaultValue = "0")]
        public string PaddingChar
        {
            get { return (string)this[PADDING_CHAR]; }
            set { this[PADDING_CHAR] = value; }
        }

        [ConfigurationProperty(MAP, IsRequired = false, DefaultValue = "")]
        public string Map
        {
            get { return this[MAP] as string; }
            set { this[MAP] = value; }
        }

        [SettingsDescription("root, for xml transform.  designates a single XML root to use for parsing xml.")]
        [ConfigurationProperty(ROOT, IsRequired = false, DefaultValue = "")]
        public string Root
        {
            get { return this[ROOT] as string; }
            set { this[ROOT] = value; }
        }

        [ConfigurationProperty(SCRIPT, IsRequired = false, DefaultValue = "")]
        public string Script
        {
            get { return this[SCRIPT] as string; }
            set { this[SCRIPT] = value; }
        }

        [ConfigurationProperty(TEMPLATE, IsRequired = false, DefaultValue = "")]
        public string Template
        {
            get { return this[TEMPLATE] as string; }
            set { this[TEMPLATE] = value; }
        }

        [ConfigurationProperty(FORMAT, IsRequired = false, DefaultValue = "")]
        public string Format
        {
            get { return this[FORMAT] as string; }
            set { this[FORMAT] = value; }
        }

        [ConfigurationProperty(PARAMETER, IsRequired = false, DefaultValue = "")]
        public string Parameter
        {
            get { return this[PARAMETER] as string; }
            set { this[PARAMETER] = value; }
        }

        [ConfigurationProperty(RESULT, IsRequired = false, DefaultValue = "")]
        public string Result
        {
            get { return this[RESULT] as string; }
            set { this[RESULT] = value; }
        }

        [ConfigurationProperty(SEPARATOR, IsRequired = false, DefaultValue = ",")]
        public string Separator
        {
            get { return this[SEPARATOR] as string; }
            set { this[SEPARATOR] = value; }
        }

        [ConfigurationProperty(FROM_TIME_ZONE, IsRequired = false, DefaultValue = "")]
        public string FromTimeZone {
            get { return this[FROM_TIME_ZONE] as string; }
            set { this[FROM_TIME_ZONE] = value; }
        }

        [ConfigurationProperty(TO_TIME_ZONE, IsRequired = false, DefaultValue = "")]
        public string ToTimeZone {
            get { return this[TO_TIME_ZONE] as string; }
            set { this[TO_TIME_ZONE] = value; }
        }

        [ConfigurationProperty(MODEL, IsRequired = false, DefaultValue = "dynamic")]
        public string Model
        {
            get { return this[MODEL] as string; }
            set { this[MODEL] = value; }
        }

        [ConfigurationProperty(EXPRESSION, IsRequired = false, DefaultValue = "")]
        public string Expression
        {
            get { return this[EXPRESSION] as string; }
            set { this[EXPRESSION] = value; }
        }

        [ConfigurationProperty(PARAMETERS)]
        public ParameterElementCollection Parameters
        {
            get { return this[PARAMETERS] as ParameterElementCollection; }
        }

        [ConfigurationProperty(SCRIPTS)]
        public TransformScriptElementCollection Scripts
        {
            get { return this[SCRIPTS] as TransformScriptElementCollection; }
        }

        [ConfigurationProperty(TEMPLATES)]
        public TransformTemplateElementCollection Templates
        {
            get { return this[TEMPLATES] as TransformTemplateElementCollection; }
        }

        [ConfigurationProperty(TYPE, IsRequired = false, DefaultValue = "")]
        public string Type
        {
            get { return this[TYPE] as string; }
            set { this[TYPE] = value; }
        }

        [ConfigurationProperty(FIELDS)]
        public FieldElementCollection Fields
        {
            get { return this[FIELDS] as FieldElementCollection; }
        }

        //validation
        [ConfigurationProperty(APPEND_TO, IsRequired = false, DefaultValue = "")]
        public string AppendTo
        {
            get { return this[APPEND_TO] as string; }
            set { this[APPEND_TO] = value; }
        }

        [ConfigurationProperty(CHARACTERS, IsRequired = false, DefaultValue = "")]
        public string Characters
        {
            get { return this[CHARACTERS] as string; }
            set { this[CHARACTERS] = value; }
        }

        [ConfigurationProperty(MESSAGE, IsRequired = false, DefaultValue = "")]
        public string Message
        {
            get { return this[MESSAGE] as string; }
            set { this[MESSAGE] = value; }
        }

        [EnumConversionValidator(typeof(ContainsCharacters), MessageTemplate = "{1} must be All, or Any.")]
        [ConfigurationProperty(CONTAINS_CHARACTERS, IsRequired = false, DefaultValue = "All")]
        public string ContainsCharacters
        {
            get { return this[CONTAINS_CHARACTERS] as string; }
            set { this[CONTAINS_CHARACTERS] = value; }
        }

        [ConfigurationProperty(NEGATED, IsRequired = false, DefaultValue = false)]
        public bool Negated
        {
            get { return (bool)this[NEGATED]; }
            set { this[NEGATED] = value; }
        }

        [ConfigurationProperty(LOWER_BOUND, IsRequired = false)]
        public string LowerBound
        {
            get { return this[LOWER_BOUND] as string; }
            set { this[LOWER_BOUND] = value; }
        }

        [EnumConversionValidator(typeof(RangeBoundaryType), MessageTemplate = "{1} must be a valid RangeBoundaryType. (e.g. Inclusive, Exclusive, or Ignore)")]
        [ConfigurationProperty(LOWER_BOUND_TYPE, IsRequired = false, DefaultValue = "Inclusive")]
        public string LowerBoundType
        {
            get { return this[LOWER_BOUND_TYPE] as string; }
            set { this[LOWER_BOUND_TYPE] = value; }
        }

        [ConfigurationProperty(UPPER_BOUND, IsRequired = false)]
        public string UpperBound
        {
            get { return this[UPPER_BOUND] as string; }
            set { this[UPPER_BOUND] = value; }
        }

        [EnumConversionValidator(typeof(RangeBoundaryType), MessageTemplate = "{1} must be a valid RangeBoundaryType. (e.g. Inclusive, Exclusive, or Ignore)")]
        [ConfigurationProperty(UPPER_BOUND_TYPE, IsRequired = false, DefaultValue = "Inclusive")]
        public string UpperBoundType
        {
            get { return this[UPPER_BOUND_TYPE] as string; }
            set { this[UPPER_BOUND_TYPE] = value; }
        }

        [ConfigurationProperty(DOMAIN, IsRequired = false, DefaultValue = "")]
        public string Domain
        {
            get { return this[DOMAIN] as string; }
            set { this[DOMAIN] = value; }
        }

        [ConfigurationProperty(LEFT, IsRequired = false, DefaultValue = "")]
        public string Left {
            get { return this[LEFT] as string; }
            set { this[LEFT] = value; }
        }

        [EnumConversionValidator(typeof(ComparisonOperator), MessageTemplate = "{1} must be a valid ComparisonOperator. (e.g. Equal, NotEqual, LessThan, LessThanEqual, GreaterThan, GreaterThanEqual)")]
        [ConfigurationProperty(OPERATOR, IsRequired = false, DefaultValue = "Equal")]
        public string Operator {
            get { return this[OPERATOR] as string; }
            set { this[OPERATOR] = value; }
        }

        [ConfigurationProperty(FIELD, IsRequired = false, DefaultValue = "")]
        public string Field {
            get { return this[FIELD] as string; }
            set { this[FIELD] = value; }
        }

        [ConfigurationProperty(RIGHT, IsRequired = false, DefaultValue = "")]
        public string Right {
            get { return this[RIGHT] as string; }
            set { this[RIGHT] = value; }
        }

        [ConfigurationProperty(THEN, IsRequired = false, DefaultValue = "")]
        public string Then {
            get { return this[THEN] as string; }
            set { this[THEN] = value; }
        }

        [ConfigurationProperty(ELSE, IsRequired = false, DefaultValue = "")]
        public string Else {
            get { return this[ELSE] as string; }
            set { this[ELSE] = value; }
        }

        [ConfigurationProperty(FROM_LAT, IsRequired = false, DefaultValue = "0.0")]
        public string FromLat {
            get { return this[FROM_LAT] as string; }
            set { this[FROM_LAT] = value; }
        }

        [ConfigurationProperty(FROM_LONG, IsRequired = false, DefaultValue = "0.0")]
        public string FromLong {
            get { return this[FROM_LONG] as string; }
            set { this[FROM_LONG] = value; }
        }

        [ConfigurationProperty(TO_LAT, IsRequired = false, DefaultValue = "0.0")]
        public string ToLat {
            get { return this[TO_LAT] as string; }
            set { this[TO_LAT] = value; }
        }

        [ConfigurationProperty(TO_LONG, IsRequired = false, DefaultValue = "0.0")]
        public string ToLong {
            get { return this[TO_LONG] as string; }
            set { this[TO_LONG] = value; }
        }

        public override bool IsReadOnly()
        {
            return false;
        }
    }
}