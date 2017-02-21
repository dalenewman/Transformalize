using System.Collections;
using System.Collections.Generic;
using Transformalize.Contracts;

namespace Transformalize.Transforms {

    public class Transforms : IEnumerable<ITransform> {

        public bool Valid { get; set; }

        private readonly List<ITransform> _transforms;

        public Transforms() {
            _transforms = new List<ITransform>();
        }

        public Transforms(IEnumerable<ITransform> transforms, bool valid) {
            _transforms = new List<ITransform>(transforms);
            Valid = valid;
        }
        public IEnumerator<ITransform> GetEnumerator() {
            return _transforms.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return GetEnumerator();
        }
    }
}