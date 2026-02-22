using Autofac;
using System;
using System.IO;
using System.Linq;
using Transformalize.Configuration;
using Transformalize.Context;
using Transformalize.Contracts;
using Transformalize.Nulls;
using System.IO;

namespace Transformalize.Providers.Json.Autofac {
   public class JsonProviderBuilder {

      private readonly ContainerBuilder _builder;
      private readonly Process _process;
      private readonly StreamWriter _streamWriter;

      public bool UseAsyncMethods { get; set; } = false;

      public JsonProviderBuilder(Process process, ContainerBuilder builder, StreamWriter streamWriter = null) {
         _process = process ?? throw new ArgumentException("Json Provider Builder's constructor must be provided with a non-null process.", nameof(process));
         _builder = builder ?? throw new ArgumentException("Json Provider Builder's constructor must be provided with a non-null builder.", nameof(builder));
         _streamWriter = streamWriter;
      }

      public void Build() {
         // Json schema reading not supported yet
         foreach (var connection in _process.Connections.Where(c => c.Provider == "json")) {
            _builder.Register<ISchemaReader>(ctx => new NullSchemaReader()).Named<ISchemaReader>(connection.Key);
         }

         // Json input
         foreach (var entity in _process.Entities.Where(e => _process.Connections.First(c => c.Name == e.Input).Provider == "json")) {

            // input version detector
            _builder.RegisterType<NullInputProvider>().Named<IInputProvider>(entity.Key);

            // input read
            _builder.Register<IRead>(ctx => {
               var input = ctx.ResolveNamed<InputContext>(entity.Key);
               var rowFactory = ctx.ResolveNamed<IRowFactory>(entity.Key, new NamedParameter("capacity", input.RowCapacity));
               if (Path.GetExtension(input.Connection.File).ToLower() == ".jsonl") {
                  return new JsonLinesFileReader(input, rowFactory);
               } else {
                  return new JsonFileReader(input, rowFactory);
               }
            }).Named<IRead>(entity.Key);

         }

         if (_process.GetOutputConnection().Provider == "json") {

            foreach (var entity in _process.Entities) {

               // ENTITY WRITER
               _builder.Register<IWrite>(ctx => {
                  bool jsonLines = Path.GetExtension(_process.GetOutputConnection().File).ToLower() == ".jsonl";
                  var output = ctx.ResolveNamed<OutputContext>(entity.Key);
                  if (output.Connection.Stream && _streamWriter != null) {
                     if (UseAsyncMethods) {  // orchard core (asp.net core) requires all output stream operations to be async
                        return new JsonStreamWriter(output, _streamWriter);
                     } else {
                        if(jsonLines) {
                           return new JsonLinesStreamWriterSync(output, _streamWriter); // to avoid: The stream is currently in use by a previous operation on the stream
                        } else {
                           return new JsonStreamWriterSync(output, _streamWriter); // to avoid: The stream is currently in use by a previous operation on the stream
                        }
                     }
                  } else {
                     if (jsonLines) {
                        return new JsonLinesFileWriter(output);
                     } else {
                        return new JsonFileWriter(output);
                     }
                  }

               }).Named<IWrite>(entity.Key);
            }
         }

      }
   }
}
