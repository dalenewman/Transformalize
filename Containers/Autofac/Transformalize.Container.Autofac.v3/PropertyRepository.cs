using Autofac;
using System.Collections.Generic;

namespace Transformalize.Containers.Autofac {
   public class PropertyRepository : IPropertyRepository {

      private readonly Dictionary<string, object> _storage = new Dictionary<string, object>();

      public object GetProperty(ContainerBuilder builder, string name) {
         if (_storage.ContainsKey(name)) {
            return _storage[name];
         }
         return null;
      }

      public void SetProperty(ContainerBuilder builder, string name, object value) {
         _storage[name] = value;         
      }
   }
}
