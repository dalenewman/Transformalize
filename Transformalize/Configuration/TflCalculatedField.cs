namespace Transformalize.Configuration {
    public class TflCalculatedField : TflField {
        public TflCalculatedField() {
            Property(name: "input", value: false);
            Property(name: "alias", value: string.Empty);
        }
    }
}