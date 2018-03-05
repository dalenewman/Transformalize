#region license
// Transformalize
// Configurable Extract, Transform, and Load
// Copyright 2013-2017 Dale Newman
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
using System.Collections.Generic;

namespace Transformalize {

    public static class TransformDomain {

        public static List<string> List = new List<string>{
            "abs",
            "add",
            "all",
            "any",
            "append",
            "bytes",
            "bytesize",
            "camelize",
            "ceiling",
            "coalesce",
            "commonprefix",
            "commonprefixes",
            "compress",
            "concat",
            "connection",
            "convert",
            "copy",
            "cs",
            "csharp",
            "dasherize",
            "dateadd",
            "datediff",
            "datemath",
            "datepart",
            "decompress",
            "dehumanize",
            "distance",
            "distinct",
            "endswith",
            "eval",
            "exclude",
            "fileext",
            "filename",
            "filepath",
            "first",
            "floor",
            "format",
            "formatphone",
            "formatxml",
            "frommetric",
            "fromroman",
            "geohashencode",
            "geohashneighbor",
            "hashcode",
            "htmldecode",
            "htmlencode",
            "humanize",
            "hyphenate",
            "iif",
            "in",
            "include",
            "insert",
            "invert",
            "isdefault",
            "isempty",
            "ismatch",
            "javascript",
            "jint",
            "join",
            "js",
            "last",
            "lastday",
            "left",
            "lower",
            "map",
            "match",
            "matching",
            "matchcount",
            "multiply",
            "next",
            "now",
            "opposite",
            "ordinalize",
            "padleft",
            "padright",
            "pascalize",
            "pluralize",
            "prepend",
            "razor",
            "regexreplace",
            "remove",
            "replace",
            "right",
            "round",
            "rounddownto",
            "roundto",
            "roundupto",
            "singularize",
            "slice",
            "slugify",
            "split",
            "splitlength",
            "startswith",
            "substring",
            "sum",
            "tag",
            "timeago",
            "timeahead",
            "timezone",
            "timezoneoffset",
            "titleize",
            "tolower",
            "tometric",
            "toordinalwords",
            "toroman",
            "tostring",
            "totime",
            "toupper",
            "towords",
            "toyesno",
            "trim",
            "trimend",
            "trimstart",
            "underscore",
            "upper",
            "urlencode",
            "velocity",
            "vingetmodelyear",
            "vingetworldmanufacturer",
            "web",
            "xmldecode",
            "xpath"
        };

        private static HashSet<string> _hashSet;
        public static HashSet<string> HashSet => _hashSet ?? (_hashSet = new HashSet<string>(List));

        private static string _commaDelimited;
        public static string CommaDelimited => _commaDelimited ?? (_commaDelimited = string.Join(",", List));
    }
}
