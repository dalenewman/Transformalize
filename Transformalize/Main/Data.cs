namespace Transformalize.Main {

    public class Data<T> {

        public Data(object value) {
            _object = _value = (T)value;
        }

        public Data(T value) {
            _object = _value = value;
        }

        private object _object;
        private T _value;

        public T Value {
            get { return _value; }
            set { _object = _value = value; }
        }

        public object Object {
            get { return _object; }
            set { _object = value; }
        }
    }
}