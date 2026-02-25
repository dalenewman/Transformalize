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

      public ConsoleProviderModule() { }

      protected override void Load(ContainerBuilder builder) {

         if (_process == null && !builder.Properties.ContainsKey("Process")) {
            return;
         }

         var process = _process ?? (Process)builder.Properties["Process"];

         foreach (var action in process.Templates.Where(t => t.Enabled).SelectMany(t => t.Actions).Where(a => a.GetModes().Any(m => m == process.Mode || m == "*"))) {
            if (_consoleActions.Contains(action.Type)) {
               builder.Register(ctx => {
                  return SwitchAction(ctx, process, action);
               }).Named<IAction>(action.Key);
            }
         }
         foreach (var action in process.Actions.Where(a => a.GetModes().Any(m => m == process.Mode || m == "*"))) {
            if (_consoleActions.Contains(action.Type)) {
               builder.Register(ctx => {
                  return SwitchAction(ctx, process, action);
               }).Named<IAction>(action.Key);
            }
         }

         // Connections
         foreach (var connection in process.Connections.Where(c => c.Provider == "console")) {
            builder.RegisterType<NullSchemaReader>().Named<ISchemaReader>(connection.Key);
         }

         // Entity input
         foreach (var entity in process.Entities.Where(e => process.Connections.First(c => c.Name == e.Input).Provider == "console")) {

            builder.Register(ctx => {
               var input = ctx.ResolveNamed<InputContext>(entity.Key);
               var rowFactory = ctx.ResolveNamed<IRowFactory>(entity.Key, new NamedParameter("capacity", input.RowCapacity));
               return input.Connection.Command == string.Empty ? (IRead)new ConsoleInputReader(input, rowFactory) : new ConsoleCommandReader(input, rowFactory);
            }).Named<IRead>(entity.Key);

            builder.Register<IInputProvider>(ctx => new ConsoleInputProvider(ctx.ResolveNamed<IRead>(entity.Key))).Named<IInputProvider>(entity.Key);
         }

         // Entity Output
         var output = process.GetOutputConnection();
         if (output.Provider == "console") {

            // PROCESS OUTPUT CONTROLLER
            builder.Register<IOutputController>(ctx => new NullOutputController()).As<IOutputController>();

            foreach (var entity in process.Entities) {
               builder.Register<IOutputController>(ctx => new NullOutputController()).Named<IOutputController>(entity.Key);

               builder.Register<IWrite>(ctx => {
                  var context = ctx.ResolveNamed<OutputContext>(entity.Key);
                  if (output.Format == "text") {
                     return new ConsoleWriter(context, new TextSerializer(context));
                  } else {
                     return new ConsoleWriter(context, new CsvSerializer(context));
                  }
               }).Named<IWrite>(entity.Key);

               builder.Register<IOutputProvider>(ctx => {
                  var context = ctx.ResolveNamed<OutputContext>(entity.Key);
                  return new ConsoleOutputProvider(context, ctx.ResolveNamed<IWrite>(entity.Key));
               })
               .Named<IOutputProvider>(entity.Key);
            }
         }
      }

      private IAction SwitchAction(IComponentContext ctx, Process process, Action action) {
         var context = new PipelineContext(ctx.Resolve<IPipelineLogger>(), process);
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
