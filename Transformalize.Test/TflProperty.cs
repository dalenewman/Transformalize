namespace Transformalize.Test {
    public sealed class TflProperty {
        public object Value { get; set; }
        public bool Required { get; set; }
        public bool Unique { get; set; }
        public bool Set { get; set; }

        public TflProperty(object value, bool required = false, bool unique = false) {
            Value = value;
            Required = required;
            Unique = unique;
        }

    }
}