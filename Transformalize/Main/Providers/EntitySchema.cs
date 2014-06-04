namespace Transformalize.Main.Providers {
    public class EntitySchema {
        private Fields _fields = new Fields();

        public EntitySchema() {
        }

        public EntitySchema(Fields fields) {
            Fields.Add(fields);
        }

        public Fields Fields {
            get { return _fields; }
            set { _fields = value; }
        }
    }
}