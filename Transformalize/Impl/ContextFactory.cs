using System;
using System.Collections.Generic;
using System.Linq;
using Transformalize.Configuration;
using Transformalize.Context;
using Transformalize.Contracts;

namespace Transformalize.Impl {
   public class ContextFactory {

      private readonly Process _process;
      private readonly IPipelineLogger _logger;

      public ContextFactory(Process process, IPipelineLogger logger) {
         _process = process;
         _logger = logger;
      }

      public IContext GetProcessContext() {
         return new PipelineContext(_logger, _process);
      }

      public OutputContext GetProcessOutputContext() {
         return new OutputContext(GetProcessContext());
      }

      public IEnumerable<IConnectionContext> GetConnectionContext() {
         return _process.Connections.Select(c => new ConnectionContext(GetProcessContext(), c));
      }

      public IEnumerable<OutputContext> GetOutputContext() {
         return _process.Connections.Select(c => new OutputContext(GetProcessContext()));
      }

      public IEnumerable<IContext> GetEntityContext() {
         return _process.Entities.Select(e => new PipelineContext(_logger, _process, e));
      }

      public IEnumerable<InputContext> GetEntityInputContext() {
         return GetEntityContext().Select(c => new InputContext(c));
      }

      public IRowFactory GetEntityInputRowFactory(IConnectionContext context, Func<IConnectionContext, int> capacity) {
         return new RowFactory(capacity(context), context.Entity.IsMaster, false);
      }

      public IEnumerable<OutputContext> GetEntityOutputContext() {
         return GetEntityContext().Select(c => new OutputContext(c));
      }

      public IEnumerable<ConnectionContext> GetEntityConnectionContext() {
         return GetEntityContext().Select(c => new ConnectionContext(c, _process.Connections.First(cn => cn.Name == c.Entity.Connection)));
      }

   }
}
