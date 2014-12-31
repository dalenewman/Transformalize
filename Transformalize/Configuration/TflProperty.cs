namespace Transformalize.Configuration {

    public sealed class TflProperty {

        public string Name { get; set; }
        public object Value { get; set; }
        public bool Required { get; set; }
        public bool Unique { get; set; }
        public bool Set { get; set; }

        public TflProperty(string name, object value, bool required = false, bool unique = false) {
            Name = name;
            Value = value;
            Required = required;
            Unique = unique;
        }

    }
}