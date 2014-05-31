using System.Globalization;

namespace Transformalize.Main.Providers.File {
    public class FileField {
        private string _name;

        public string Name {
            get { return _name; }
            set { _name = Common.CleanIdentifier(value); }
        }

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