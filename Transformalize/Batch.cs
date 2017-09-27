using System.Collections.Generic;
using Transformalize.Contracts;

namespace Transformalize {

    public class Batch {

        private readonly Dictionary<int, IRow> _storage = new Dictionary<int, IRow>();

        public bool Contains(int i) {
            return _storage.ContainsKey(i);
        }

        public IRow this[int i] {
            get => _storage[i];
            set => _storage[i] = value;
        }

    }
}
