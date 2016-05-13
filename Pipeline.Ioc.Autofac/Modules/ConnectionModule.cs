using System.Collections.Generic;
using Autofac;
using Pipeline.Configuration;

namespace Pipeline.Ioc.Autofac.Modules {
    public abstract class ConnectionModule : Module {
        readonly IEnumerable<Connection> _connections;

        protected ConnectionModule() {}

        protected ConnectionModule(IEnumerable<Connection> connections) {
            _connections = connections;
        }

        protected abstract void RegisterConnection(ContainerBuilder builder, Connection connection);

        protected override void Load(ContainerBuilder builder) {
            if (_connections == null)
                return;
            foreach (var c in _connections) {
                RegisterConnection(builder, c);
            }
        }
    }
}