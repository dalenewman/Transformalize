using System.Collections.Generic;
using System.Linq;

namespace Pipeline {
    public class ValueComparer : IEqualityComparer<IEnumerable<object>> {

        public bool Equals(IEnumerable<object> x, IEnumerable<object> y) {
            return x.SequenceEqual(y);
        }

        public int GetHashCode(IEnumerable<object> values) {
            unchecked {
                return values.Aggregate((int)2166136261, (current, value) => current * 16777619 ^ value.GetHashCode());
            }
        }
    }
}