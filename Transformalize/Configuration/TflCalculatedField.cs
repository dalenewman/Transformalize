namespace Transformalize.Configuration {
    public class TflCalculatedField : TflField {
        public TflCalculatedField() {
            Property(n:"input", v:false);
            Property(n:"alias", v:string.Empty);
        }
    }
}