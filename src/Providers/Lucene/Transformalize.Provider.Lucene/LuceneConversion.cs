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
using System;
using System.Collections.Generic;
using Lucene.Net.Documents;
using Lucene.Net.Index;
using Lucene.Net.Search;
using Field = Transformalize.Configuration.Field;

namespace Transformalize.Providers.Lucene {

    public static class LuceneConversion {

        public static string DefaultDecimalFormat { get; } = "0000000000.000000000;-000000000.000000000";

        public static string CalculateDecimalFormat(int precision, int scale) {
            var count = precision - scale;
            if (count <= 0)
                return DefaultDecimalFormat;

            return "0" + new string('0', count) + "." + new string('0', scale) + ";-" + new string('0', count) + "." + new string('0', scale);
        }

        private static readonly Dictionary<string, SortFieldType> _typeSort = new Dictionary<string, SortFieldType> {
            {"bool", SortFieldType.STRING},
            {"boolean",SortFieldType.STRING},
            {"byte",SortFieldType.INT16 },
            {"byte[]",SortFieldType.STRING},
            {"char",SortFieldType.STRING},
            {"date",SortFieldType.STRING},
            {"datetime",SortFieldType.STRING},
            {"decimal",SortFieldType.DOUBLE},
            {"double",SortFieldType.DOUBLE},
            {"float",SortFieldType.SINGLE},
            {"guid", SortFieldType.STRING},
            {"int",SortFieldType.INT32},
            {"int16",SortFieldType.INT16},
            {"int32",SortFieldType.INT32},
            {"int64",SortFieldType.INT64},
            {"long",SortFieldType.INT64},
            {"object",SortFieldType.STRING},
            {"real",SortFieldType.DOUBLE},
            {"short",SortFieldType.INT16},
            {"single",SortFieldType.SINGLE},
            {"string",SortFieldType.STRING},
            {"uint16",SortFieldType.INT32},
            {"uint32",SortFieldType.INT64},
            {"uint64",SortFieldType.INT64}
        };

        private static readonly Dictionary<string, Func<Field, string, object, Query>> _typeSearch = new Dictionary<string, Func<Configuration.Field, string, object, Query>> {
            {"bool", (f,n,v)=>new TermQuery(new Term(n,(bool)v?"1":"0"))},
            {"boolean",(f,n,v)=>new TermQuery(new Term(n,(bool)v?"1":"0"))},
            {"byte",(f,n,v)=>NumericRangeQuery.NewInt32Range(n,Convert.ToInt32(v),Convert.ToInt32(v),true,true)},
            {"byte[]",(f,n,v)=>new TermQuery(new Term(n,Utility.BytesToHexString((byte[])v)))},
            {"char",(f,n,v)=>new TermQuery(new Term(n,v.ToString()))},
            {"date",(f,n,v)=>new TermQuery(new Term(n,DateTools.DateToString((DateTime)v,DateResolution.MILLISECOND)))},
            {"datetime",(f,n,v)=>new TermQuery(new Term(n,DateTools.DateToString((DateTime)v,DateResolution.MILLISECOND)))},
            {"decimal",(f,n,v)=>new TermQuery(new Term(n,((decimal)v).ToString(CalculateDecimalFormat(f.Precision, f.Scale))))},
            {"double",(f,n,v)=>NumericRangeQuery.NewDoubleRange(n,Convert.ToDouble(v),Convert.ToDouble(v),true,true)},
            {"float",(f,n,v)=>NumericRangeQuery.NewDoubleRange(n,Convert.ToSingle(v),Convert.ToSingle(v),true,true)},
            {"guid", (f,n,v)=>new TermQuery(new Term(n,v.ToString()))},
            {"int",(f,n,v)=>NumericRangeQuery.NewInt32Range(n,(int)v,(int)v,true,true)},
            {"int16",(f,n,v)=>NumericRangeQuery.NewInt32Range(n,Convert.ToInt32(v),Convert.ToInt32(v),true,true)},
            {"int32",(f,n,v)=>NumericRangeQuery.NewInt32Range(n,(int)v,(int)v,true,true)},
            {"int64",(f,n,v)=>NumericRangeQuery.NewInt64Range(n,(long)v,(long)v,true,true)},
            {"long",(f,n,v)=>NumericRangeQuery.NewInt64Range(n,(long)v,(long)v,true,true)},
            {"object",(f,n,v)=>new TermQuery(new Term(n,v.ToString()))},
            {"real",(f,n,v)=>NumericRangeQuery.NewSingleRange(n,Convert.ToSingle(v),Convert.ToSingle(v),true,true)},
            {"short",(f,n,v)=>NumericRangeQuery.NewInt32Range(n,Convert.ToInt32(v),Convert.ToInt32(v),true,true)},
            {"single",(f,n,v)=>NumericRangeQuery.NewSingleRange(n,(float)v,(float)v,true,true)},
            {"string",(f,n,v)=>new TermQuery(new Term(n,v.ToString()))},
            {"uint16",(f,n,v)=>NumericRangeQuery.NewInt32Range(n,Convert.ToInt32(v),Convert.ToInt32(v),true,true)},
            {"uint32",(f,n,v)=>NumericRangeQuery.NewInt64Range(n,Convert.ToInt64(v),Convert.ToInt64(v),true,true)},
            {"uint64",(f,n,v)=>NumericRangeQuery.NewInt64Range(n,Convert.ToInt64(v),Convert.ToInt64(v),true,true)}
        };

        public static SortFieldType GetSortFieldType(string type) {
            return _typeSort[type];
        }

        public static Query TypeSearch(Field field, string nameOrAlias, object v) {
            return _typeSearch[field.Type](field, nameOrAlias, v);
        }
    }
}