using System.Text.RegularExpressions;
using Transformalize.Extensions;

namespace Transformalize.Main.Providers {
    public class QueryWriter {
        protected static string SqlIdentifier(string name) {
            return Regex.Replace(name, @"[\s\[\]\`]+", "_").Trim("_".ToCharArray()).Left(128);
        }

        protected static string KeyName(string[] pk) {
            return SqlIdentifier(
                string.Join("_", pk)
            );
        }
    }
}