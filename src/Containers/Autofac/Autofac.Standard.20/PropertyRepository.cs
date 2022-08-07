using Autofac;

namespace Transformalize.Containers.Autofac {
   public class PropertyRepository : IPropertyRepository {
      public object GetProperty(ContainerBuilder builder, string name) {
         if (builder.Properties.ContainsKey(name)) {
            return builder.Properties[name];
         }
         return null;
      }

      public void SetProperty(ContainerBuilder builder, string name, object value) {
         builder.Properties[name] = value;
      }
   }
}
