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

        private static readonly Dictionary<string, int> _typeSort = new Dictionary<string, int> {
            {"bool", SortField.STRING},
            {"boolean",SortField.STRING},
            {"byte",SortField.SHORT},
            {"byte[]",SortField.STRING},
            {"char",SortField.STRING},
            {"date",SortField.STRING},
            {"datetime",SortField.STRING},
            {"decimal",SortField.STRING},
            {"double",SortField.DOUBLE},
            {"float",SortField.FLOAT},
            {"guid", SortField.STRING},
            {"int",SortField.INT},
            {"int16",SortField.INT},
            {"int32",SortField.INT},
            {"int64",SortField.LONG},
            {"long",SortField.LONG},
            {"object",SortField.STRING},
            {"real",SortField.FLOAT},
            {"short",SortField.INT},
            {"single",SortField.FLOAT},
            {"string",SortField.STRING},
            {"uint16",SortField.INT},
            {"uint32",SortField.INT},
            {"uint64",SortField.INT}
        };

        private static readonly Dictionary<string, Func<Field, string, object, Query>> _typeSearch = new Dictionary<string, Func<Configuration.Field, string, object, Query>> {
            {"bool", (f,n,v)=>new TermQuery(new Term(n,(bool)v?"1":"0"))},
            {"boolean",(f,n,v)=>new TermQuery(new Term(n,(bool)v?"1":"0"))},
            {"byte",(f,n,v)=>NumericRangeQuery.NewIntRange(n,Convert.ToInt32(v),Convert.ToInt32(v),true,true)},
            {"byte[]",(f,n,v)=>new TermQuery(new Term(n,Utility.BytesToHexString((byte[])v)))},
            {"char",(f,n,v)=>new TermQuery(new Term(n,v.ToString()))},
            {"date",(f,n,v)=>new TermQuery(new Term(n,DateTools.DateToString((DateTime)v,DateTools.Resolution.MILLISECOND)))},
            {"datetime",(f,n,v)=>new TermQuery(new Term(n,DateTools.DateToString((DateTime)v,DateTools.Resolution.MILLISECOND)))},
            {"decimal",(f,n,v)=>new TermQuery(new Term(n,((decimal)v).ToString(CalculateDecimalFormat(f.Precision, f.Scale))))},
            {"double",(f,n,v)=>NumericRangeQuery.NewDoubleRange(n,Convert.ToDouble(v),Convert.ToDouble(v),true,true)},
            {"float",(f,n,v)=>NumericRangeQuery.NewFloatRange(n,Convert.ToSingle(v),Convert.ToSingle(v),true,true)},
            {"guid", (f,n,v)=>new TermQuery(new Term(n,v.ToString()))},
            {"int",(f,n,v)=>NumericRangeQuery.NewIntRange(n,(int)v,(int)v,true,true)},
            {"int16",(f,n,v)=>NumericRangeQuery.NewIntRange(n,Convert.ToInt32(v),Convert.ToInt32(v),true,true)},
            {"int32",(f,n,v)=>NumericRangeQuery.NewIntRange(n,(int)v,(int)v,true,true)},
            {"int64",(f,n,v)=>NumericRangeQuery.NewLongRange(n,(long)v,(long)v,true,true)},
            {"long",(f,n,v)=>NumericRangeQuery.NewLongRange(n,(long)v,(long)v,true,true)},
            {"object",(f,n,v)=>new TermQuery(new Term(n,v.ToString()))},
            {"real",(f,n,v)=>NumericRangeQuery.NewFloatRange(n,Convert.ToSingle(v),Convert.ToSingle(v),true,true)},
            {"short",(f,n,v)=>NumericRangeQuery.NewIntRange(n,Convert.ToInt32(v),Convert.ToInt32(v),true,true)},
            {"single",(f,n,v)=>NumericRangeQuery.NewFloatRange(n,(float)v,(float)v,true,true)},
            {"string",(f,n,v)=>new TermQuery(new Term(n,v.ToString()))},
            {"uint16",(f,n,v)=>NumericRangeQuery.NewIntRange(n,Convert.ToInt32(v),Convert.ToInt32(v),true,true)},
            {"uint32",(f,n,v)=>NumericRangeQuery.NewLongRange(n,Convert.ToInt64(v),Convert.ToInt64(v),true,true)},
            {"uint64",(f,n,v)=>NumericRangeQuery.NewLongRange(n,Convert.ToInt64(v),Convert.ToInt64(v),true,true)}
        };

        public static int TypeSort(string type) {
            return _typeSort[type];
        }

        public static Query TypeSearch(Configuration.Field field, string nameOrAlias, object v) {
            return _typeSearch[field.Type](field, nameOrAlias, v);
        }
    }
}