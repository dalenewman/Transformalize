#region License

// /*
// Transformalize - Replicate, Transform, and Denormalize Your Data...
// Copyright (C) 2013 Dale Newman
// 
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with this program.  If not, see <http://www.gnu.org/licenses/>.
// */

#endregion

using System;
using System.Linq;
using Transformalize.Configuration;
using Transformalize.Libs.NLog;
using Transformalize.Main.Providers.SqlServer;

namespace Transformalize.Main {

    public class EntityConfigurationLoader {
        private const StringComparison IC = StringComparison.OrdinalIgnoreCase;
        private readonly Logger _log = LogManager.GetCurrentClassLogger();
        private readonly Process _process;

        public EntityConfigurationLoader(Process process) {
            _process = process;
        }

        public Entity Read(EntityConfigurationElement element, bool isMaster) {
            var entity = new Entity {
                ProcessName = _process.Name,
                Schema = element.Schema,
                Name = element.Name,
                InputConnection = _process.Connections[element.Connection],
                OutputConnection = _process.Connections["output"],
                Prefix = element.Prefix,
                Group = element.Group,
                Auto = element.Auto,
                Alias = string.IsNullOrEmpty(element.Alias) ? element.Name : element.Alias,
            };

            GuardAgainstInvalidGrouping(element, entity);

            if (entity.Auto) {
                var autoReader = new SqlServerEntityAutoFieldReader();
                entity.All = autoReader.Read(entity, isMaster);
                entity.Fields = new FieldSqlWriter(entity.All).FieldType(FieldType.Field).Context();
                entity.PrimaryKey = new FieldSqlWriter(entity.All).FieldType(FieldType.PrimaryKey, FieldType.MasterKey).Context();
            }

            var pkIndex = 0;
            foreach (FieldConfigurationElement pk in element.PrimaryKey) {
                var fieldType = isMaster ? FieldType.MasterKey : FieldType.PrimaryKey;

                var keyField = new FieldReader(_process, entity, new FieldTransformParametersReader(), new EmptyParametersReader()).Read(pk, fieldType);
                keyField.Index = pkIndex;

                entity.PrimaryKey[keyField.Alias] = keyField;
                entity.All[keyField.Alias] = keyField;

                pkIndex++;
            }

            var fieldIndex = 0;
            foreach (FieldConfigurationElement f in element.Fields) {
                var field = new FieldReader(_process, entity, new FieldTransformParametersReader(), new EmptyParametersReader()).Read(f);
                field.Index = fieldIndex;

                if (entity.Auto && entity.Fields.ContainsKey(field.Name)) {
                    entity.Fields.Remove(field.Name);
                    entity.All.Remove(field.Name);
                }

                entity.Fields[field.Alias] = field;
                entity.All[field.Alias] = field;

                fieldIndex++;
            }

            if (!String.IsNullOrEmpty(element.Version)) {
                if (entity.All.ContainsKey(element.Version)) {
                    entity.Version = entity.All[element.Version];
                } else {
                    if (entity.All.Any(kv => kv.Value.Name.Equals(element.Version, IC))) {
                        entity.Version = entity.All.ToEnumerable().First(v => v.Name.Equals(element.Version, IC));
                    } else {
                        _log.Error("version field reference '{0}' is undefined in {1}.", element.Version, element.Name);
                        Environment.Exit(0);
                    }
                }
                entity.Version.Input = true;
                entity.Version.Output = true;
            }

            foreach (FieldConfigurationElement field in element.CalculatedFields)
            {
                var transformParametersReader = new EntityTransformParametersReader(entity);
                var parametersReader = new EntityParametersReader(entity);
                var fieldReader = new FieldReader(_process, entity, transformParametersReader, parametersReader, usePrefix:false);
                entity.CalculatedFields.Add(field.Alias, fieldReader.Read(field));
            }

            return entity;
        }

        private void GuardAgainstInvalidGrouping(EntityConfigurationElement element, Entity entity)
        {
            if (entity.Group)
            {
                if (element.Fields.Cast<FieldConfigurationElement>().Any(f => string.IsNullOrEmpty(f.Aggregate)))
                {
                    _log.Error("Entity {0} is set to group, but not all your fields have aggregate defined.", entity.Alias);
                    Environment.Exit(1);
                }
            }
        }
    }
}