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

using System.Configuration;

namespace Transformalize.Configuration
{
    public class TransformConfigurationElement : ConfigurationElement
    {
        private const string METHOD = "method";
        private const string VALUE = "value";
        private const string PATTERN = "pattern";
        private const string REPLACEMENT = "replacement";
        private const string OLD_VALUE = "oldValue";
        private const string NEW_VALUE = "newValue";
        private const string TRIM_CHARS = "trimChars";
        private const string INDEX = "index";
        private const string COUNT = "count";
        private const string START_INDEX = "startIndex";
        private const string LENGTH = "length";
        private const string TOTAL_WIDTH = "totalWidth";
        private const string PADDING_CHAR = "paddingChar";
        private const string MAP = "map";
        private const string SCRIPT = "script";
        private const string TEMPLATE = "template";
        private const string PARAMETERS = "parameters";
        private const string RESULTS = "results";
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

        [ConfigurationProperty(METHOD, IsRequired = true)]
        public string Method
        {
            get
            {
                return this[METHOD] as string;
            }
            set { this[METHOD] = value; }
        }

        [ConfigurationProperty(NAME, IsRequired = false, DefaultValue = "")]
        public string Name
        {
            get
            {
                return this[NAME] as string;
            }
            set { this[NAME] = value; }
        }

        [ConfigurationProperty(VALUE, IsRequired = false, DefaultValue = "")]
        public string Value
        {
            get
            {
                return this[VALUE] as string;
            }
            set { this[VALUE] = value; }
        }

        [ConfigurationProperty(PATTERN, IsRequired = false, DefaultValue = "")]
        public string Pattern
        {
            get
            {
                return this[PATTERN] as string;
            }
            set { this[PATTERN] = value; }
        }

        [ConfigurationProperty(REPLACEMENT, IsRequired = false, DefaultValue = "")]
        public string Replacement
        {
            get
            {
                return this[REPLACEMENT] as string;
            }
            set { this[REPLACEMENT] = value; }
        }

        [ConfigurationProperty(OLD_VALUE, IsRequired = false, DefaultValue = "")]
        public string OldValue
        {
            get
            {
                return this[OLD_VALUE] as string;
            }
            set { this[OLD_VALUE] = value; }
        }


        [ConfigurationProperty(NEW_VALUE, IsRequired = false, DefaultValue = "")]
        public string NewValue
        {
            get
            {
                return this[NEW_VALUE] as string;
            }
            set { this[NEW_VALUE] = value; }
        }

        [ConfigurationProperty(TRIM_CHARS, IsRequired = false, DefaultValue = "")]
        public string TrimChars
        {
            get
            {
                return this[TRIM_CHARS] as string;
            }
            set { this[TRIM_CHARS] = value; }
        }

        [ConfigurationProperty(INDEX, IsRequired = false, DefaultValue = 0)]
        public int Index
        {
            get
            {
                return (int)this[INDEX];
            }
            set { this[INDEX] = value; }
        }

        [ConfigurationProperty(COUNT, IsRequired = false, DefaultValue = 0)]
        public int Count
        {
            get
            {
                return (int)this[COUNT];
            }
            set { this[COUNT] = value; }
        }

        [ConfigurationProperty(START_INDEX, IsRequired = false, DefaultValue = 0)]
        public int StartIndex
        {
            get
            {
                return (int)this[START_INDEX];
            }
            set { this[START_INDEX] = value; }
        }

        [ConfigurationProperty(LENGTH, IsRequired = false, DefaultValue = 0)]
        public int Length
        {
            get
            {
                return (int)this[LENGTH];
            }
            set { this[LENGTH] = value; }
        }

        [ConfigurationProperty(TOTAL_WIDTH, IsRequired = false, DefaultValue = 0)]
        public int TotalWidth
        {
            get
            {
                return (int)this[TOTAL_WIDTH];
            }
            set { this[TOTAL_WIDTH] = value; }
        }

        [StringValidator(MaxLength = 1)]
        [ConfigurationProperty(PADDING_CHAR, IsRequired = false, DefaultValue = "0")]
        public string PaddingChar
        {
            get
            {
                return (string)this[PADDING_CHAR];
            }
            set { this[PADDING_CHAR] = value; }
        }

        [ConfigurationProperty(MAP, IsRequired = false, DefaultValue = "")]
        public string Map
        {
            get
            {
                return this[MAP] as string;
            }
            set { this[MAP] = value; }
        }
        
        [ConfigurationProperty(SCRIPT, IsRequired = false, DefaultValue = "")]
        public string Script
        {
            get
            {
                return this[SCRIPT] as string;
            }
            set { this[SCRIPT] = value; }
        }

        [ConfigurationProperty(TEMPLATE, IsRequired = false, DefaultValue = "")]
        public string Template
        {
            get
            {
                return this[TEMPLATE] as string;
            }
            set { this[TEMPLATE] = value; }
        }

        [ConfigurationProperty(FORMAT, IsRequired = false, DefaultValue = "")]
        public string Format
        {
            get
            {
                return this[FORMAT] as string;
            }
            set { this[FORMAT] = value; }
        }

        [ConfigurationProperty(PARAMETER, IsRequired = false, DefaultValue = "")]
        public string Parameter
        {
            get
            {
                return this[PARAMETER] as string;
            }
            set { this[PARAMETER] = value; }
        }

        [ConfigurationProperty(RESULT, IsRequired = false, DefaultValue = "")]
        public string Result
        {
            get
            {
                return this[RESULT] as string;
            }
            set { this[RESULT] = value; }
        }

        [ConfigurationProperty(SEPARATOR, IsRequired = false, DefaultValue = ",")]
        public string Separator
        {
            get
            {
                return this[SEPARATOR] as string;
            }
            set { this[SEPARATOR] = value; }
        }

        [ConfigurationProperty(MODEL, IsRequired = false, DefaultValue = "dynamic")]
        public string Model
        {
            get
            {
                return this[MODEL] as string;
            }
            set { this[MODEL] = value; }
        }

        [ConfigurationProperty(EXPRESSION, IsRequired = false, DefaultValue = "")]
        public string Expression
        {
            get
            {
                return this[EXPRESSION] as string;
            }
            set { this[EXPRESSION] = value; }
        }


        [ConfigurationProperty(PARAMETERS)]
        public ParameterElementCollection Parameters
        {
            get
            {
                return this[PARAMETERS] as ParameterElementCollection;
            }
        }

        [ConfigurationProperty(SCRIPTS)]
        public TransformScriptElementCollection Scripts
        {
            get
            {
                return this[SCRIPTS] as TransformScriptElementCollection;
            }
        }

        [ConfigurationProperty(TEMPLATES)]
        public TransformTemplateElementCollection Templates
        {
            get { return this[TEMPLATES] as TransformTemplateElementCollection; }
        }

        [ConfigurationProperty(TYPE, IsRequired = false, DefaultValue = "")]
        public string Type
        {
            get
            {
                return this[TYPE] as string;
            }
            set { this[TYPE] = value; }
        }

    }
}