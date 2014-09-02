using System.Collections.Generic;
using System.Text.RegularExpressions;
using Transformalize.Extensions;
using Transformalize.Libs.Rhino.Etl;
using Transformalize.Operations.Transform;

namespace Transformalize.Main {

    public class SlugOperation : ShouldRunOperation {
        private readonly int _length;
        private static readonly Regex InvalidChars = new Regex(@"[^a-z0-9\s-]", RegexOptions.Compiled);
        private static readonly Regex MultipleSpaces = new Regex(@"\s+", RegexOptions.Compiled);
        private static readonly Regex Spaces = new Regex(@"\s", RegexOptions.Compiled);


        public SlugOperation(string inKey, string outKey, int length)
            : base(inKey, outKey) {
            _length = length;
            Name = "Slug (" + OutKey + ")";
        }

        public override IEnumerable<Row> Execute(IEnumerable<Row> rows) {
            foreach (var row in rows) {
                if (ShouldRun(row)) {
                    row[OutKey] = GenerateSlug(row[InKey].ToString(), _length);
                }
                yield return row;
            }
        }

        //http://stackoverflow.com/questions/2920744/url-slugify-algorithm-in-c
        public static string GenerateSlug(string phrase, int length) {
            var str = RemoveAccent(phrase).ToLower();

            // invalid chars           
            str = InvalidChars.Replace(str, string.Empty);

            // convert multiple spaces into one space   
            str = MultipleSpaces.Replace(str, " ").Trim();

            // cut and trim
            if (length > 0 && str.Length > length) {
                str = str.Left(length);
            }
            str = str.Trim();

            // hyphens
            str = Spaces.Replace(str, "-");
            return str;
        }

        public static string RemoveAccent(string txt) {
            var bytes = System.Text.Encoding.GetEncoding("Cyrillic").GetBytes(txt);
            return System.Text.Encoding.ASCII.GetString(bytes);
        }
    }
}