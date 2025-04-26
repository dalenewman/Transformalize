﻿#region license
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
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;  // needed for ! NETS10
using System.Text.RegularExpressions;

namespace Transformalize {
   public static class Constants {

      private static HashSet<string> _types;
      private static HashSet<string> _numericTypes;
      private static HashSet<string> _decimalTypes;
      private static HashSet<string> _providers;
      private static HashSet<string> _adoProviders;
      private static Dictionary<string, object> _typeDefaults;
      private static Dictionary<string, string> _stringDefaults;
      private static Dictionary<string, Type> _typeSystem;
      private static Dictionary<string, Func<string, bool>> _canConvert;

#if NETS10
     private static readonly Regex _timeOffset = new Regex(@"[+|-][0-1][0-9]:[0-5][0-9]|Z$");
#else
      private static readonly Regex _timeOffset = new Regex(@"[+|-][0-1][0-9]:[0-5][0-9]|Z$", RegexOptions.Compiled);
#endif

      public const string ApplicationName = "Transformalize";
      public const string DefaultSetting = "[default]";

      public const string ProviderDomain = "sqlserver,internal,file,folder,elasticsearch,solr,mysql,postgresql,console,trace,sqlce,sqlite,lucene,excel,web,log,filesystem,geojson,json,kml,text,ssas,rethinkdb,word,razor,velocity,clevest,access,bogus,mail,activedirectory,accord.net,aws," + DefaultSetting;
      public const string AdoProviderDomain = "postgresql,sqlite,sqlce,mysql,sqlserver,access";

      public const string TypeDomain = @"bool,boolean,byte,byte[],char,date,datetime,decimal,double,float,guid,int,int16,int32,int64,long,object,real,short,single,string,uint16,uint32,uint64,uint,ushort,ulong";
      public const string NumericTypeDomain = @"byte,decimal,double,float,int,int16,int32,int64,long,object,real,short,single,uint16,uint32,uint64,uint,ushort,ulong";
      public const string DecimalTypeDomain = @"decimal,double,float,object,real,single";

      public const string ComparisonDomain = "equal,equals,eq,notequal,notequals,neq,lessthan,lt,greaterthan,lte,lessthanequal,gt,greaterthanequal,gte,=,==,!=,<,<=,>,>=,in,notin,like,notlike";
      public const string ModelDomain = "decisiontree,knn,rbfkernelperceptron,polykernelperceptron,linearregression," + DefaultSetting;

      public const string TflHashCode = "TflHashCode";
      public const string TflKey = "TflKey";
      public const string TflDeleted = "TflDeleted";
      public const string TflBatchId = "TflBatchId";
      public static string ApplicationFolder = "Transformalize";

      public static HashSet<string> TimeSpanComponents = new HashSet<string>() { "hour", "hours", "minute", "minutes", "second", "seconds", "millisecond", "milliseconds", "day", "days", "tick", "ticks" };

      public static HashSet<string> TypeSet() {
         return _types ?? (_types = new HashSet<string>(TypeDomain.Split(',')));
      }

      public static HashSet<string> NumericTypeSet() {
         return _numericTypes ?? (_numericTypes = new HashSet<string>(NumericTypeDomain.Split(',')));
      }

      public static HashSet<string> DecimalTypeSet() {
         return _decimalTypes ?? (_decimalTypes = new HashSet<string>(DecimalTypeDomain.Split(',')));
      }

      public static bool IsNumericType(string type) {
         return NumericTypeSet().Contains(type);
      }

      public static bool IsDecimalType(string type) {
         return DecimalTypeSet().Contains(type);
      }

      public static HashSet<string> ProviderSet() {
         return _providers ?? (_providers = new HashSet<string>(ProviderDomain.Split(',')));
      }

      public static HashSet<string> AdoProviderSet() {
         return _adoProviders ?? (_adoProviders = new HashSet<string>(AdoProviderDomain.Split(',')));
      }

      public static Dictionary<string, object> TypeDefaults() {
         var maxDate = new DateTime(9999, 12, 30, 23, 59, 59, 999, DateTimeKind.Utc);
         return _typeDefaults ?? (
             _typeDefaults = new Dictionary<string, object> {
                    {"bool",false},
                    {"boolean",false},
                    {"byte",default(byte)},
                    {"byte[]",new byte[0]},
                    {"char",default(char)},
                    {"date",maxDate},
                    {"datetime",maxDate},
                    {"decimal",default(decimal)},
                    {"double",default(double)},
                    {"float",default(float)},
                    {"guid",Guid.Parse("00000000-0000-0000-0000-000000000000")},
                    {"int",default(int)},
                    {"int16",default(short)},
                    {"int32",default(int)},
                    {"int64",default(long)},
                    {"long",default(long)},
                    {"object",null},
                    {"real",default(float)},
                    {"short",default(short)},
                    {"single",default(float)},
                    {"string",string.Empty},
                    {"uint16",default(ushort)},
                    {"uint32",default(uint)},
                    {"uint64",default(ulong)},
                    {"ushort",default(ushort)},
                    {"uint",default(uint)},
                    {"ulong",default(ulong)},
             });
      }

      public static Dictionary<string, string> StringDefaults() {
         return _stringDefaults ?? (
             _stringDefaults = new Dictionary<string, string> {
                    {"bool","false"},
                    {"boolean","false"},
                    {"byte", default(byte).ToString()},
                    {"byte[]", "0x0000000000000000"},
                    {"char",default(char).ToString()},
                    {"date","9999-12-31T00:00:00Z"},
                    {"datetime","9999-12-31T00:00:00Z"},
                    {"decimal","0.0"},
                    {"double","0.0"},
                    {"float","0.0"},
                    {"guid","00000000-0000-0000-0000-000000000000"},
                    {"int","0"},
                    {"int16","0"},
                    {"int32","0"},
                    {"int64","0"},
                    {"long","0"},
                    {"object",string.Empty},
                    {"real","0.0"},
                    {"short","0"},
                    {"single","0.0"},
                    {"string",string.Empty},
                    {"uint16","0"},
                    {"uint32","0"},
                    {"uint64","0"},
                    {"ushort","0"},
                    {"uint","0"},
                    {"ulong","0"}
             });
      }

      public static readonly Dictionary<string, Func<string, object>> ConversionMap = new Dictionary<string, Func<string, object>> {
            {"string", (x => x)},
            {"int16", (x => Convert.ToInt16(x))},
            {"short", (x => Convert.ToInt16(x))},
            {"int32", (x => Convert.ToInt32(x))},
            {"int", (x => Convert.ToInt32(x))},
            {"int64", (x => Convert.ToInt64(x))},
            {"long", (x => Convert.ToInt64(x))},
            {"uint16", (x => Convert.ToUInt16(x))},
            {"ushort", (x => Convert.ToUInt16(x))},
            {"uint32", (x => Convert.ToUInt32(x))},
            {"uint", (x => Convert.ToUInt32(x))},
            {"ulong", (x => Convert.ToUInt64(x))},
            {"uint64", (x => Convert.ToUInt64(x))},
            {"double", (x => Convert.ToDouble(x))},
            {"decimal", (x => decimal.Parse(x, NumberStyles.Float | NumberStyles.AllowThousands | NumberStyles.AllowCurrencySymbol, (IFormatProvider)CultureInfo.CurrentCulture.GetFormat(typeof(NumberFormatInfo))))},
            {"char", (x => string.IsNullOrEmpty(x) ? ' ' : Convert.ToChar(x))},
            {"date", x => {
                  return _timeOffset.IsMatch(x) ? DateTimeOffset.Parse(x).UtcDateTime : Convert.ToDateTime(x);
               }
            },
            { "datetime", x => {
                  return _timeOffset.IsMatch(x) ? DateTimeOffset.Parse(x).UtcDateTime : Convert.ToDateTime(x);
               }
            },
            {"boolean", (x => Convert.ToBoolean(NormalizeBool(x)))},
            {"bool", (x => Convert.ToBoolean(NormalizeBool(x))) },
            {"single", (x => Convert.ToSingle(x))},
            {"real", (x => Convert.ToSingle(x))},
            {"float", (x => Convert.ToSingle(x))},
            {"guid", (x => Guid.Parse(x))},
            {"byte", (x => Convert.ToByte(x))},
            {"byte[]", (ObjectToByteArray)}
        };

      public static readonly Dictionary<string, Func<object, object>> ObjectConversionMap = new Dictionary<string, Func<object, object>> {
            {"string", (x => x.ToString())},
            {"int16", (x => Convert.ToInt16(x))},
            {"short", (x => Convert.ToInt16(x))},
            {"int32", (x => Convert.ToInt32(x))},
            {"int", (x => Convert.ToInt32(x))},
            {"int64", (x => Convert.ToInt64(x))},
            {"long", (x => Convert.ToInt64(x))},
            {"uint16", (x => Convert.ToUInt16(x))},
            {"ushort", (x => Convert.ToUInt16(x))},
            {"uint32", (x => Convert.ToUInt32(x))},
            {"uint", (x => Convert.ToUInt32(x))},
            {"ulong", (x => Convert.ToUInt64(x))},
            {"uint64", (x => Convert.ToUInt64(x))},
            {"double", (x => Convert.ToDouble(x))},
            {"decimal", (x => decimal.Parse(x.ToString(), NumberStyles.Float | NumberStyles.AllowThousands | NumberStyles.AllowCurrencySymbol, (IFormatProvider)CultureInfo.CurrentCulture.GetFormat(typeof(NumberFormatInfo))))},
            {"char", (x => x == (object)string.Empty ? ' ' : Convert.ToChar(x))},
            {"date", (x => {
               if(x is string str) {
                  return ConversionMap["date"](str);
               }
               return Convert.ToDateTime(x);
               })},
            {"datetime", (x => {
               if(x is string str) {
                  return ConversionMap["datetime"](str);
               }
               return Convert.ToDateTime(x);
               })},
            {"bool", (x => Convert.ToBoolean(x))},
            {"boolean", (x => Convert.ToBoolean(x))},
            {"single", (x => Convert.ToSingle(x))},
            {"real", (x => Convert.ToSingle(x))},
            {"float", (x => Convert.ToSingle(x))},
            {"guid", (x => Guid.Parse(x.ToString()))},
            {"byte", (x => Convert.ToByte(x))},
            {"byte[]", (ObjectToByteArray)}
        };

      public static HashSet<string> InvalidFieldNames { get; internal set; } = new HashSet<string>(new[] { TflKey, TflBatchId, TflDeleted, TflHashCode }, StringComparer.OrdinalIgnoreCase);
      public static string OriginalOutput { get; set; } = "original-output";

      public static string NormalizeBool(string s) {
         if (string.IsNullOrEmpty(s))
            return "false";

         s = s.ToLower();
         switch (s) {
            case "1":
            case "on":
               return "true";
            case "0":
            case "off":
               return "false";
            default:
               return s;
         }

      }


      // Convert an object to a byte array
      private static byte[] ObjectToByteArray(object obj) {
         if (obj is byte[] bytes)
            return bytes;

#if NETS10
                return new byte[0];
#else
         var bf = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
         using (var ms = new MemoryStream()) {
            bf.Serialize(ms, obj);
            return ms.ToArray();
         }
#endif

      }

      public static Dictionary<string, Func<string, bool>> CanConvert() {
         return _canConvert ?? (
             _canConvert = new Dictionary<string, Func<string, bool>> {
                    {"bool",s=> bool.TryParse(NormalizeBool(s), out _)},
                    {"boolean",s=> bool.TryParse(NormalizeBool(s), out _)},
                    {"byte",s=>byte.TryParse(s, out _)},
                    {"byte[]", s => false},
                    {"char",s=>char.TryParse(s, out _)},
                    {"date",s=> s.Length > 5 && DateTime.TryParse(s, out _)},
                    {"datetime",s=> s.Length > 5 && DateTime.TryParse(s, out _)},
                    {"decimal",s=>decimal.TryParse(s, NumberStyles.Float | NumberStyles.AllowThousands | NumberStyles.AllowCurrencySymbol, (IFormatProvider)CultureInfo.CurrentCulture.GetFormat(typeof(NumberFormatInfo)), out _)},
                    {"double",s=>double.TryParse(s, out _)},
                    {"float",s=>float.TryParse(s, out _)},
                    {"guid", s=>Guid.TryParse(s, out _)},
                    {"int",s=>int.TryParse(s, out _)},
                    {"int16", s=>short.TryParse(s, out _)},
                    {"int32",s=>int.TryParse(s, out _)},
                    {"int64",s=>long.TryParse(s, out _)},
                    {"long",s=>long.TryParse(s, out _)},
                    {"object", s=>true},
                    {"real",s=>float.TryParse(s, out _)},
                    {"short",s=>short.TryParse(s, out _)},
                    {"single",s=>float.TryParse(s, out _)},
                    {"string",s=>true},
                    {"uint16",s=>ushort.TryParse(s, out _)},
                    {"uint32",s=>uint.TryParse(s, out _)},
                    {"uint64",s=>ulong.TryParse(s, out _)}
             });
      }

      public static Dictionary<string, Type> TypeSystem() {
         return _typeSystem ?? (
             _typeSystem = new Dictionary<string, Type> {
                    {"bool", typeof(bool)},
                    {"boolean",typeof(bool)},
                    {"byte",typeof(byte)},
                    {"byte[]",typeof(byte[])},
                    {"char",typeof(char)},
                    {"date",typeof(DateTime)},
                    {"datetime",typeof(DateTime)},
                    {"decimal",typeof(decimal)},
                    {"double",typeof(double)},
                    {"float",typeof(float)},
                    {"guid", typeof(Guid)},
                    {"int",typeof(int)},
                    {"int16",typeof(short)},
                    {"int32",typeof(int)},
                    {"int64",typeof(long)},
                    {"long",typeof(long)},
                    {"object",null},
                    {"real",typeof(float)},
                    {"short",typeof(short)},
                    {"single",typeof(float)},
                    {"string",typeof(string)},
                    {"uint16",typeof(ushort)},
                    {"uint32",typeof(uint)},
                    {"uint64",typeof(ulong)},
             });
      }
   }
}
