using System;
using System.Collections.Generic;
using System.Linq;
using Transformalize.Actions;
using Transformalize.Configuration;
using Transformalize.Context;
using Transformalize.Contracts;
using Action = Transformalize.Configuration.Action;

namespace Transformalize.Providers.Ado {
    public class AdoEntityFormCommands : IAction {
        private readonly IContext _context;
        private readonly Action _action;
        private readonly IConnectionFactory _factory;

        public AdoEntityFormCommands(IContext context, Action action, IConnectionFactory factory) {
            _context = context;
            _action = action;
            _factory = factory;
        }

        public ActionResponse Execute() {

            foreach (var entity in _context.Process.Entities) {
                var context = new PipelineContext(_context.Logger, _context.Process, entity);
                entity.CreateCommand = SqlCreateFormTable(context, _factory);
                entity.InsertCommand = SqlInsertFormTable(context, _factory);
                entity.UpdateCommand = SqlUpdateFormTable(context, _factory);
                entity.DeleteCommand = SqlDeleteFormTable(context, _factory);
            }



            return new ActionResponse { Action = _action };
        }

        private static string SqlCreateFormTable(IContext c, IConnectionFactory cf) {
            var definitions = new List<string>();
            var added = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            foreach (var field in c.Entity.GetAllFields().Where(f => f.Input)) {
                if (added.Add(field.Name)) {
                    if (field.PrimaryKey) {
                        definitions.Add(cf.Enclose(field.Name) + "INT NOT NULL PRIMARY KEY IDENTITY(1,1)"); // for now

                    } else {
                        definitions.Add(cf.Enclose(field.Name) + " " + cf.SqlDataType(field) + " NOT NULL");
                    }
                }
            }

            foreach (var parameter in c.Process.GetActiveParameters().Where(p => !string.IsNullOrEmpty(p.Name) && p.Input)) {
                var name = parameter.Name.Replace(".", "_");

                if (added.Add(name)) {
                    var field = new Field { Name = name, Type = parameter.Type };
                    definitions.Add(cf.Enclose(name) + " " + cf.SqlDataType(field));
                }
            }

            return $"CREATE TABLE {cf.Enclose(c.Entity.Name)} ({string.Join(",", definitions)}){cf.Terminator}";
        }

        private static string SqlInsertFormTable(IContext c, IConnectionFactory cf) {
            var fields = c.Entity.GetAllFields().Where(f => f.Input && !f.PrimaryKey).ToList();
            var added = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            var keys = new HashSet<string>(c.Entity.GetPrimaryKey().Select(f => f.Name));

            foreach (var field in fields) {
                added.Add(field.Name);
            }

            foreach (var parameter in c.Process.GetActiveParameters().Where(p => !string.IsNullOrEmpty(p.Name) && p.Input && p.Scope != "update")) {
                var name = parameter.Name.Replace(".", "_");
                if (!keys.Contains(name) && added.Add(name)) {
                    fields.Add(new Field { Name = name, Type = parameter.Type });
                }
            }

            var fieldNames = string.Join(",", fields.Select(f => cf.Enclose(f.Name)));
            var parameters = cf.AdoProvider == AdoProvider.Access ? string.Join(",", fields.Select(f => "?")) : string.Join(",", fields.Select(f => "@" + f.Name));
            return $"INSERT INTO {cf.Enclose(c.Entity.Name)}({fieldNames}) VALUES({parameters}){cf.Terminator}";
        }

        private static string SqlUpdateFormTable(IContext c, IConnectionFactory cf) {
            var fields = c.Entity.GetAllFields().Where(f => f.Input).ToList();
            var added = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            var keys = new HashSet<string>(c.Entity.GetPrimaryKey().Select(f => f.Name));

            foreach (var field in fields) {
                added.Add(field.Name);
            }

            foreach (var parameter in c.Process.GetActiveParameters().Where(p => !string.IsNullOrEmpty(p.Name) && p.Input && p.Scope != "insert")) {
                var name = parameter.Name.Replace(".", "_");
                if (!keys.Contains(name) && added.Add(name)) {
                    fields.Add(new Field { Name = name, Type = parameter.Type });
                }
            }

            var sets = string.Join(",", fields.Where(f => !f.PrimaryKey).Select(f => cf.Enclose(f.Name) + (cf.AdoProvider == AdoProvider.Access ? " = ?" : " = @" + f.Name)));
            var criteria = string.Join(" AND ", fields.Where(f => f.PrimaryKey).OrderBy(f => f.Index).Select(f => f.Name).Select(n => cf.Enclose(n) + (cf.AdoProvider == AdoProvider.Access ? " = ?" : " = @" + n)));
            return $"UPDATE {cf.Enclose(c.Entity.Name)} SET {sets} WHERE {criteria}{cf.Terminator}";
        }

        private static string SqlDeleteFormTable(IContext c, IConnectionFactory cf) {
            var criteria = string.Join(" AND ", c.Entity.GetAllFields().Where(f => f.PrimaryKey).OrderBy(f => f.Index).Select(f => f.Name).Select(n => cf.Enclose(n) + (cf.AdoProvider == AdoProvider.Access ? " = ?" : " = @" + n)));
            return $"DELETE FROM {cf.Enclose(c.Entity.Name)} WHERE {criteria}{cf.Terminator}";
        }

    }

}