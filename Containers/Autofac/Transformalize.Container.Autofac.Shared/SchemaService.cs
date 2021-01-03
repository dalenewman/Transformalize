using Autofac;
using System.Collections.Generic;
using System.Linq;
using Transformalize.Configuration;
using Transformalize.Contracts;
using Transformalize.Extensions;

namespace Transformalize.Containers.Autofac {

   /// <summary>
   /// This is a temporary combination of schema related methods found in legacy and orchard core
   /// </summary>
   public class SchemaService {

      private readonly ILifetimeScope _scope;
      private readonly IContext _context;

      public SchemaService(
         ILifetimeScope scope
      ) {
         _scope = scope;
         _context = scope.Resolve<IContext>();
      }

      public Process GetProcess(Process process) {

         var connectionReaders = new Dictionary<string, ISchemaReader>();
         var entityReaders = new Dictionary<Entity, ISchemaReader>();

         foreach (var connection in process.Connections) {
            if (process.Entities.Any(e => e.Input == connection.Name) && !connectionReaders.ContainsKey(connection.Name)) {
               connectionReaders.Add(connection.Name, _scope.ResolveNamed<ISchemaReader>(connection.Key));
            }
         }
         foreach (var entity in process.Entities.Where(e => connectionReaders.ContainsKey(e.Input))) {
            entityReaders.Add(entity, connectionReaders[entity.Input]);
         }

         var schemas = new List<Schema>();

         foreach (var reader in entityReaders) {
            schemas.Add(reader.Value.Read(reader.Key));
         }
         foreach (var schema in schemas) {
            foreach (var schemaEntity in schema.Entities) {
               var entity = process.Entities.FirstOrDefault(e => e.Name == schemaEntity.Name);
               if (entity != null) {
                  foreach (var schemaField in schemaEntity.Fields) {
                     if (!entity.Fields.Any(f => f.Name == schemaField.Name)) {
                        entity.Fields.Add(schemaField);
                     }
                  }
               }
            }
         }

         // remove some of the cfg-net transformations that are not meant to be seen
         foreach (var connection in process.Connections) {
            connection.ConnectionString = string.Empty;
         }

         foreach (var entity in process.Entities) {

            entity.Fields.RemoveAll(f => f.System);

            if (entity.Alias == entity.Name) {
               entity.Alias = null;
            }

            foreach (var field in entity.Fields) {
               if (field.Alias == field.Name) {
                  field.Alias = null;
               }
               if (field.Label == field.Name) {
                  field.Label = null;
               }
               if (field.SortField == field.Name) {
                  field.SortField = null;
               }
               field.Sortable = null;
            }
         }

         return process;
      }

      public Schema GetSchema(Process process) {

         if (process == null) {
            return new Schema();
         }

         var reader = _scope.ResolveNamed<ISchemaReader>(process.Connections.First().Key);
         return process.Entities.Count == 1 ? reader.Read(process.Entities.First()) : reader.Read();
      }

      public Schema GetSchema(Process process, Entity entity) {
         if (process == null) {
            return new Schema();
         }

         var reader = _scope.ResolveNamed<ISchemaReader>(process.Connections.First(c => c.Provider != Constants.DefaultSetting).Key);
         return reader.Read(entity);
      }

      /// <summary>
      /// If any entity schema's are helped, this should return true so the consumer knows he/she ought to check process.Errors().
      /// </summary>
      /// <returns>true if any schema has been helped, false if no help provided</returns>
      public bool Help(Process process) {

         var helped = false;

         foreach (var entity in process.Entities) {

            if (entity.Fields.Any(f => f.Input)) {
               continue;
            }

            var schema = GetSchema(process, entity);
            if (!schema.Entities.Any()) {
               _context.Warn($"Could not detect {entity.Alias} fields.");
               continue;
            }

            var e = schema.Entities.First();
            var fields = e.Fields;
            foreach (var field in fields.Where(field => Constants.InvalidFieldNames.Contains(field.Name) && Constants.InvalidFieldNames.Contains(field.Alias))) {
               field.Alias = e.Alias + field.Name;
            }
            var add = e.Fields.Where(f => !f.System).ToList();

            var processKeys = new HashSet<string>(process.GetAllFields().Select(f => f.Alias), System.StringComparer.OrdinalIgnoreCase);

            if (add.Any()) {
               _context.Debug(() => $"Detected {add.Count} field{add.Count.Plural()} in {entity.Alias}.");

               var entityKeys = new HashSet<string>(entity.GetAllFields().Select(f => f.Alias), System.StringComparer.OrdinalIgnoreCase);

               foreach (var field in add) {
                  if (!entityKeys.Contains(field.Alias)) {
                     if (!field.PrimaryKey && processKeys.Contains(field.Alias)) {
                        field.Alias = entity.Alias + field.Alias;
                     }
                     entity.Fields.Add(field);
                  }
               }
               process.Connections.First(c => c.Name == e.Input).Delimiter = schema.Connection.Delimiter;
               helped = true;
            } else {
               _context.Warn($"Could not detect {entity.Alias} fields.");
            }
         }

         return helped;

      }
   }
}
