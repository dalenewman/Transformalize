namespace Transformalize.Operations {

    public class FieldTypeDefault {
        public string Alias;
        public string Type;
        public object Default;

        public FieldTypeDefault(string alias, string type, object @default) {
            Alias = alias;
            Type = type;
            Default = @default;
        }
    }
}