#region license
// Transformalize
// A Configurable ETL Solution Specializing in Incremental Denormalization.
// Copyright 2013 Dale Newman
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
using System.Linq;
using System.Text;
using Pipeline.Context;
using Pipeline.Contracts;

namespace Pipeline {
    public class CsvSerializer : ISerialize {

        private const string Quote = "\"";
        private const string EscapedQuote = "\"\"";
        private static readonly char[] CharactersThatMustBeQuoted = { ',', '"', '\n' };

        private readonly OutputContext _context;
        private readonly int _length;

        public CsvSerializer(OutputContext context) {
            _context = context;
            _length = context.OutputFields.Length;
        }
        public string Serialize(IRow row) {
            var builder = new StringBuilder();
            for (var index = 0; index < _length; index++) {
                var field = _context.OutputFields[index];
                builder.Append(Escape(row[field].ToString()));
                if (index < _length - 1) {
                    builder.Append(",");
                }
            }
            return builder.ToString();
        }

        string ISerialize.Header
        {
            get
            {
                return string.Join(",", _context.OutputFields.Select(f => f.Alias));
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