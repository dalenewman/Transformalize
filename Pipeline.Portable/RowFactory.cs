using Pipeline.Contracts;

namespace Pipeline {
    public class RowFactory : IRowFactory {
        private readonly int _capacity;
        private readonly bool _isMaster;
        private readonly bool _keys;

        public RowFactory(int capacity, bool isMaster, bool keys) {
            _capacity = capacity;
            _isMaster = isMaster;
            _keys = keys;
        }

        public IRow Create() {
            if(_keys)
                return new KeyRow(_capacity);

            return _isMaster ? new MasterRow(_capacity) : (IRow)new SlaveRow(_capacity);
        }
    }
}