using Autofac;
using System;
using System.IO;
using System.Linq;
using Transformalize.Configuration;
using Transformalize.Context;
using Transformalize.Contracts;
using Transformalize.Impl;
using Transformalize.Nulls;
using Transformalize.Providers.File;

namespace Transformalize.Providers.CsvHelper.Autofac {
   public class CsvHelperProviderModule : Module {

      private readonly StreamWriter _streamWriter;
      private readonly Process _process;

      public CsvHelperProviderModule() {
      }

      public CsvHelperProviderModule(StreamWriter streamWriter) {
         _streamWriter = streamWriter;
      }

      public CsvHelperProviderModule(Process process, StreamWriter streamWriter) {
         _process = process;
         _streamWriter = streamWriter;
      }

      protected override void Load(ContainerBuilder builder) {

         if (_process == null && !builder.Properties.ContainsKey("Process")) {
            return;
         }

         var p = _process ?? (Process)builder.Properties["Process"];

         // connections
         foreach (var connection in p.Connections.Where(c => c.Provider == "file")) {

            // Schema Reader
            builder.Register<ISchemaReader>(ctx => {

               var context = ctx.ResolveNamed<IConnectionContext>(connection.Key);
               var cfg = new FileInspection(context, FileUtility.Find(connection.File), connection.Sample).Create();
               var process = new Process(cfg);

               foreach (var warning in process.Warnings()) {
                  context.Warn(warning);
               }

               if (process.Errors().Any()) {
                  foreach (var error in process.Errors()) {
                     context.Error(error);
                  }
                  return new NullSchemaReader();
               }

               var input = new InputContext(new PipelineContext(ctx.Resolve<IPipelineLogger>(), process, process.Entities.First()));
               var rowFactory = new RowFactory(input.RowCapacity, input.Entity.IsMaster, false);
               var reader = new CsvHelperReader(input, rowFactory);

               return new FileSchemaReader(process, input, reader);


            }).Named<ISchemaReader>(connection.Key);
         }

         // entity input
         foreach (var entity in p.Entities.Where(e => p.Connections.First(c => c.Name == e.Input).Provider == "file")) {

            // input version detector
            builder.RegisterType<NullInputProvider>().Named<IInputProvider>(entity.Key);

            // input read
            builder.Register<IRead>(ctx => {
               var input = ctx.ResolveNamed<InputContext>(entity.Key);
               var rowFactory = ctx.ResolveNamed<IRowFactory>(entity.Key, new NamedParameter("capacity", input.RowCapacity));

               if (input.Connection.Delimiter == string.Empty && input.Entity.Fields.Count(f => f.Input) == 1) {
                  return new FileReader(input, rowFactory);
               }
               return new CsvHelperReader(input, rowFactory);

            }).Named<IRead>(entity.Key);

         }

         // Entity Output
         if (p.GetOutputConnection().Provider == "file") {

            // PROCESS OUTPUT CONTROLLER
            builder.Register<IOutputController>(ctx => new NullOutputController()).As<IOutputController>();

            foreach (var entity in p.Entities) {

               // ENTITY OUTPUT CONTROLLER
               builder.Register<IOutputController>(ctx => {
                  var output = ctx.ResolveNamed<OutputContext>(entity.Key);
                  var fileInfo = new FileInfo(Path.Combine(output.Connection.Folder, output.Connection.File ?? output.Entity.OutputTableName(output.Process.Name)));
                  var folder = Path.GetDirectoryName(fileInfo.FullName);
                  var init = p.Mode == "init" || (folder != null && !Directory.Exists(folder));
                  var initializer = init ? (IInitializer)new FileInitializer(output) : new NullInitializer();
                  return new FileOutputController(output, initializer, new NullInputProvider(), new NullOutputProvider());
               }).Named<IOutputController>(entity.Key);

               // ENTITY WRITER
               builder.Register<IWrite>(ctx => {
                  var output = ctx.ResolveNamed<OutputContext>(entity.Key);

                  switch (output.Connection.Provider) {
                     case "file":
                        if (output.Connection.Delimiter == string.Empty) {
                           output.Error("A delimiter is required for a file output.");
                           return new NullWriter(output, true);
                        } else {
                           if (output.Connection.Stream && _streamWriter != null) {
                              if (output.Connection.Synchronous) {
                                 return new CsvHelperStreamWriterSync(output, _streamWriter);
                              } else {
                                 return new CsvHelperStreamWriter(output, _streamWriter);
                              }
                           } else {
                              var fileInfo = new FileInfo(Path.Combine(output.Connection.Folder, output.Connection.File ?? output.Entity.OutputTableName(output.Process.Name)));
                              if (fileInfo.Exists) {
                                 try { 
                                    fileInfo.Delete(); 
                                 } catch(Exception ex) {
                                    output.Error(ex.Message);
                                 }
                              }
                              var streamWriter = new StreamWriter(System.IO.File.OpenWrite(fileInfo.FullName));
                              return output.Connection.Synchronous ? (IWrite)new CsvHelperStreamWriterSync(output, streamWriter) : new CsvHelperStreamWriter(output, streamWriter);
                           }

                        }
                     default:
                        return new NullWriter(output, true);
                  }
               }).Named<IWrite>(entity.Key);

            }
         }
      }
   }
}
