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

using Transformalize.Configuration;
using Transformalize.Libs.NLog;

namespace Transformalize.Main {
    public class EntityTransformParametersReader : ITransformParametersReader {
        private readonly Entity _entity;
        private readonly Logger _log = LogManager.GetCurrentClassLogger();

        public EntityTransformParametersReader(Entity entity) {
            _entity = entity;
        }

        public IParameters Read(TransformConfigurationElement transform) {
            var parameters = new Parameters.Parameters();

            if (transform.Parameter != string.Empty && transform.Parameter != "*") {
                transform.Parameters.Insert(new ParameterConfigurationElement {
                    Entity = _entity.Alias,
                    Field = transform.Parameter
                });
            }

            foreach (ParameterConfigurationElement p in transform.Parameters) {
                if (string.IsNullOrEmpty(p.Field) && (string.IsNullOrEmpty(p.Name) || string.IsNullOrEmpty(p.Value))) {
                    _log.Error("The entity {0} has a {1} transform parameter without a field attribute, or name and value attributes.  Entity parameters require one or the other.", _entity.Alias, transform.Method);
                    System.Environment.Exit(1);
                }

                if (!string.IsNullOrEmpty(p.Field)) {
                    var fields = new FieldSqlWriter(_entity.Fields, _entity.CalculatedFields).Context();
                    if (fields.Any(Common.FieldFinder(p))) {
                        var field = fields.Last(Common.FieldFinder(p));
                        var key = string.IsNullOrEmpty(p.Name) ? field.Key : p.Name;
                        parameters.Add(field.Key, key, null, field.Value.Type);
                    } else {
                        _log.Warn("The entity {0} has a {1} transform parameter that references field {2}.  This field hasn't been defined yet in {0}.", _entity.Alias, transform.Method, p.Field);
                        parameters.Add(p.Field, p.Field, null, "System.String");
                    }
                } else {
                    parameters.Add(p.Name, p.Name, p.Value, p.Type);
                }
            }

            return parameters;
        }
    }
}