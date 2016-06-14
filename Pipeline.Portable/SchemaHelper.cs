using System.Collections.Generic;
using System.Linq;
using Pipeline.Configuration;
using Pipeline.Contracts;
using Pipeline.Extensions;

namespace Pipeline {

    /// <summary>
    /// This class tries to help discover input fields when none are specified
    /// </summary>
    public class SchemaHelper : ISchemaHelper {
        private readonly IContext _context;
        private readonly IRunTimeSchemaReader _schemaReader;

        public SchemaHelper(IContext context, IRunTimeSchemaReader schemaReader) {
            _context = context;
            _schemaReader = schemaReader;
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

                _schemaReader.Process = process;
                var schema = _schemaReader.Read(entity);
                if (!schema.Entities.Any()) {
                    _context.Warn($"Could not detect {entity.Alias} fields.");
                    continue;
                }

                var e = schema.Entities.First();
                var fields = e.Fields;
                foreach (var field in fields.Where(field => Constants.InvalidFieldNames.Contains(field.Name) && Constants.InvalidFieldNames.Contains(field.Alias))) {
                    field.Alias = e.Alias + field.Name;
                }
                var add = schema.Entities.First().Fields.Where(f => !f.System).ToList();
                if (add.Any()) {
                    _context.Info($"Detected {add.Count} field{add.Count.Plural()} in {entity.Alias}.");
                    var keys = new HashSet<string>(entity.Fields.Select(f=>f.Alias));
                    foreach (var field in add) {
                        if (!keys.Contains(field.Alias)) {
                            entity.Fields.Add(field);
                        }
                    }
                    helped = true;
                } else {
                    _context.Warn($"Could not detect {entity.Alias} fields.");
                }
            }

            if (helped) {
                var cfg = process.Serialize();
                process.Load(cfg);
            }

            return helped;

        }
    }
}