using Autofac;
using System.Collections.Generic;
using System.Linq;
using Transformalize.Actions;
using Transformalize.Configuration;
using Transformalize.Context;
using Transformalize.Contracts;
using Transformalize.Impl;
using Transformalize.Nulls;

namespace Transformalize.Providers.Console.Autofac {

   public class ConsoleProviderModule : Module {

      private readonly Process _process;
      private readonly HashSet<string> _consoleActions = new HashSet<string> { "print" };

      public ConsoleProviderModule(Process process) {
         _process = process;
      }

      protected override void Load(ContainerBuilder builder) {

         if (_process == null)
            return;

         foreach (var action in _process.Templates.Where(t => t.Enabled).SelectMany(t => t.Actions).Where(a => a.GetModes().Any(m => m == _process.Mode || m == "*"))) {
            if (_consoleActions.Contains(action.Type)) {
               builder.Register(ctx => {
                  return SwitchAction(ctx, action);
               }).Named<IAction>(action.Key);
            }
         }
         foreach (var action in _process.Actions.Where(a => a.GetModes().Any(m => m == _process.Mode || m == "*"))) {
            if (_consoleActions.Contains(action.Type)) {
               builder.Register(ctx => {
                  return SwitchAction(ctx, action);
               }).Named<IAction>(action.Key);
            }
         }

         // Connections
         foreach (var connection in _process.Connections.Where(c => c.Provider == "console")) {
            builder.RegisterType<NullSchemaReader>().Named<ISchemaReader>(connection.Key);
         }

         // Entity input
         foreach (var entity in _process.Entities.Where(e => _process.Connections.First(c => c.Name == e.Connection).Provider == "console")) {

            builder.Register(ctx => {
               var input = ctx.ResolveNamed<InputContext>(entity.Key);
               var rowFactory = ctx.ResolveNamed<IRowFactory>(entity.Key, new NamedParameter("capacity", input.RowCapacity));
               return input.Connection.Command == string.Empty ? (IRead)new ConsoleInputReader(input, rowFactory) : new ConsoleCommandReader(input, rowFactory);
            }).Named<IRead>(entity.Key);

            builder.Register<IInputProvider>(ctx => new ConsoleInputProvider(ctx.ResolveNamed<IRead>(entity.Key))).Named<IInputProvider>(entity.Key);
         }

         // Entity Output
         var output = _process.Output();
         if (output.Provider == "console") {

            // PROCESS OUTPUT CONTROLLER
            builder.Register<IOutputController>(ctx => new NullOutputController()).As<IOutputController>();

            foreach (var entity in _process.Entities) {
               builder.Register<IOutputController>(ctx => new NullOutputController()).Named<IOutputController>(entity.Key);

               builder.Register<IWrite>(ctx => {
                  return new ConsoleWriter(new CsvSerializer(ctx.ResolveNamed<OutputContext>(entity.Key)));
               }).Named<IWrite>(entity.Key);

               builder.Register<IOutputProvider>(ctx => {
                  var context = ctx.ResolveNamed<OutputContext>(entity.Key);
                  return new ConsoleOutputProvider(context, ctx.ResolveNamed<IWrite>(entity.Key));
               })
               .Named<IOutputProvider>(entity.Key);
            }
         }
      }

      private IAction SwitchAction(IComponentContext ctx, Configuration.Action action) {
         var context = new PipelineContext(ctx.Resolve<IPipelineLogger>(), _process);
         switch (action.Type) {
            case "print":
               return new PrintAction(action);
            default:
               context.Error($"Attempting to register unsupported file system action: {action.Type}");
               return new NullAction();
         }
      }
   }
}
