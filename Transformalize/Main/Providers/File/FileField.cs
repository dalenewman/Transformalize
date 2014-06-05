using System.Globalization;

namespace Transformalize.Main.Providers.File {
    public class FileField {
        public string Name { get; set; }
        public char Quote { get; set; }
        public string Type { get; set; }
        public string Length { get; set; }

        public FileField(string name, string type, string length) {
            Quote = default(char);
            Name = name;
            Type = type;
            Length = length;
        }

        public bool IsQuoted() {
            return Quote != default(char);
        }

        public string QuoteString() {
            return Quote == default(char) ? string.Empty : Quote.ToString(CultureInfo.InvariantCulture);
        }

    }
}