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
using System;
using System.Linq;
using System.Text;
using Transformalize.Configuration;
using Transformalize.Context;
using Transformalize.Contracts;

namespace Transformalize.Impl {
   public class CsvSerializer : ISerialize {

      private const string Quote = "\"";
      private const string EscapedQuote = "\"\"";
      private static readonly char[] CharactersThatMustBeQuoted = { ',', '"', '\n' };

      private readonly int _length;
      private readonly Field[] _fields;

      public CsvSerializer(OutputContext context) {
         _fields = context.OutputFields.Where(f => !f.System).ToArray();
         _length = _fields.Length;
      }
      public string Serialize(IRow row) {
         var builder = new StringBuilder();
         for (var index = 0; index < _length; index++) {
            var field = _fields[index];
            var value = row[field];
            switch (field.Type) {
               case "byte[]":
                  builder.Append(value == null ? string.Empty : value is byte[]? Convert.ToBase64String((byte[])value) : value);
                  break;
               case "date":
               case "datetime":
                  if (field.Format != string.Empty) {
                     var date = (DateTime)value;
                     var formatted = date.ToString(field.Format);
                     builder.Append(Escape(formatted));
                  } else {
                     builder.Append(Escape(value.ToString()));
                  }
                  break;
               default:
                  builder.Append(Escape(value.ToString()));
                  break;
            }
            if (index < _length - 1) {
               builder.Append(",");
            }
         }
         return builder.ToString();
      }

      string ISerialize.Header {
         get {
            return string.Join(",", _fields.Select(f => f.Alias));
         }
      }

      public string Footer => string.Empty;
      public string RowSuffix { get; } = string.Empty;
      public string RowPrefix { get; } = string.Empty;

      public static string Escape(string s) {
         if (s.Contains(Quote))
            s = s.Replace(Quote, EscapedQuote);

         if (s.IndexOfAny(CharactersThatMustBeQuoted) > -1)
            s = Quote + s + Quote;

         return s;
      }

   }
}