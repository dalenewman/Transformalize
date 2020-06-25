using Autofac;

namespace Transformalize.Containers.Autofac {
   public interface IPropertyRepository {
      object GetProperty(ContainerBuilder builder, string name);
      void SetProperty(ContainerBuilder builder, string name, object value);
   }
}
