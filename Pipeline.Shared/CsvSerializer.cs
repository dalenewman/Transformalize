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