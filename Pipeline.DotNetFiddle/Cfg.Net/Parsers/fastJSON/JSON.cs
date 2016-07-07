#region license
// Cfg.Net
// Copyright 2015 Dale Newman
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//  
//      http://www.apache.org/licenses/LICENSE-2.0
//  
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
#endregion
using System;
using System.Collections.Generic;

namespace Cfg.Net.Parsers.fastJSON
{
    public sealed class JSONParameters
    {
        /// <summary>
        ///     Serialize DateTime milliseconds i.e. yyyy-MM-dd HH:mm:ss.nnn (default = false)
        /// </summary>
        public bool DateTimeMilliseconds = false;

        /// <summary>
        ///     Anonymous types have read only properties
        /// </summary>
        public bool EnableAnonymousTypes = false;

        /// <summary>
        ///     Ignore attributes to check for (default : XmlIgnoreAttribute)
        /// </summary>
        public List<Type> IgnoreAttributes = new List<Type>();

        /// <summary>
        ///     Ignore case when processing json and deserializing
        /// </summary>
        [Obsolete("Not needed anymore and will always match")] public bool IgnoreCaseOnDeserialize = false;

        /// <summary>
        ///     Inline circular or already seen objects instead of replacement with $i (default = False)
        /// </summary>
        public bool InlineCircularReferences = false;

        /// <summary>
        ///     Output string key dictionaries as "k"/"v" format (default = False)
        /// </summary>
        public bool KVStyleStringDictionary = false;

        /// <summary>
        ///     If you have parametric and no default constructor for you classes (default = False)
        ///     IMPORTANT NOTE : If True then all initial values within the class will be ignored and will be not set
        /// </summary>
        public bool ParametricConstructorOverride = false;

        /// <summary>
        ///     Serialize null values to the output (default = True)
        /// </summary>
        public bool SerializeNullValues = true;

        /// <summary>
        ///     Save property/field names as lowercase (default = false)
        /// </summary>
        public bool SerializeToLowerCaseNames = false;

        /// <summary>
        ///     Maximum depth for circular references in inline mode (default = 20)
        /// </summary>
        public byte SerializerMaxDepth = 20;

        /// <summary>
        ///     Show the readonly properties of types in the output (default = False)
        /// </summary>
        public bool ShowReadOnlyProperties = false;

        /// <summary>
        ///     Use escaped unicode i.e. \uXXXX format for non ASCII characters (default = True)
        /// </summary>
        public bool UseEscapedUnicode = true;

        /// <summary>
        ///     Enable fastJSON extensions $types, $type, $map (default = True)
        /// </summary>
        public bool UseExtensions = true;

        /// <summary>
        ///     Use the fast GUID format (default = True)
        /// </summary>
        public bool UseFastGuid = true;

        /// <summary>
        ///     Use the optimized fast Dataset Schema format (default = True)
        /// </summary>
        public bool UseOptimizedDatasetSchema = true;

        /// <summary>
        ///     Use the UTC date format (default = True)
        /// </summary>
        public bool UseUTCDateTime = true;

        /// <summary>
        ///     Output Enum values instead of names (default = False)
        /// </summary>
        public bool UseValuesOfEnums = false;

        /// <summary>
        ///     Use the $types extension to optimise the output json (default = True)
        /// </summary>
        public bool UsingGlobalTypes = true;

        public void FixValues()
        {
            if (UseExtensions == false) // disable conflicting params
            {
                UsingGlobalTypes = false;
                InlineCircularReferences = true;
            }
            if (EnableAnonymousTypes)
                ShowReadOnlyProperties = true;
        }
    }

    public static class JSON
    {
        /// <summary>
        ///     Globally set-able parameters for controlling the serializer
        /// </summary>
        public static JSONParameters Parameters = new JSONParameters();

        /// <summary>
        ///     Parse a json string and generate a Dictionary&lt;string,object&gt; or List&lt;object&gt; structure
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        public static object Parse(string json)
        {
            return new JsonParser(json).Decode();
        }

        internal static long CreateLong(out long num, string s, int index, int count)
        {
            num = 0;
            bool neg = false;
            for (int x = 0; x < count; x++, index++)
            {
                char cc = s[index];

                if (cc == '-')
                    neg = true;
                else if (cc == '+')
                    neg = false;
                else
                {
                    num *= 10;
                    num += cc - '0';
                }
            }
            if (neg) num = -num;

            return num;
        }
    }
}