using System;
using System.Collections.Concurrent;

namespace Transformalize.Operations.Transform {

    public class ObjectPool<T> {
        private readonly ConcurrentBag<T> _objects;
        private readonly Func<T> _objectGenerator;

        public ObjectPool(Func<T> objectGenerator) {
            _objects = new ConcurrentBag<T>();
            _objectGenerator = objectGenerator;
        }

        public T GetObject() {
            T item;
            return _objects.TryTake(out item) ? item : _objectGenerator();
        }

        public void PutObject(T item) {
            _objects.Add(item);
        }
    }
}