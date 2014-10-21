using System.Configuration;
using System.Linq;

namespace Transformalize.Configuration {
    public abstract class MyConfigurationElementCollection : ConfigurationElementCollection {

        public void Merge(ConfigurationElementCollection elements) {
            foreach (ConfigurationElement element in elements) {
                var key = GetElementKey(element);
                if (BaseGetAllKeys().Any(k => k.Equals(key))) {
                    var index = this.BaseIndexOf(this.BaseGet(key));
                    BaseRemoveAt(index);
                    BaseAdd(index, element);
                } else {
                    BaseAdd(element);
                }
            }
        }

        public void Insert(ConfigurationElement element) {
            BaseAdd(0, element);
        }

        public void InsertAt(ConfigurationElement element, int at) {
            BaseAdd(at, element);
        }

        public void Add(ConfigurationElement element) {
            BaseAdd(element);
        }

        public void Remove(object key) {
            BaseRemove(key);
        }

        public void RemoveAt(int index) {
            BaseRemoveAt(index);
        }

        public void Clear() {
            BaseClear();
        }

        public int IndexOf(ConfigurationElement element) {
            return BaseIndexOf(element);
        }


    }
}