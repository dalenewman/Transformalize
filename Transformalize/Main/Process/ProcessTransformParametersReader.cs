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
using Transformalize.Logging;
using Transformalize.Main.Parameters;

namespace Transformalize.Main {

    public class ProcessTransformParametersReader : ITransformParametersReader {

        private readonly Process _process;
        private readonly DefaultFactory _defaultFactory;

        public ProcessTransformParametersReader(Process process, DefaultFactory defaultFactory)
        {
            _process = process;
            _defaultFactory = defaultFactory;
        }

        public IParameters Read(TflTransform transform) {
            var parameters = new Parameters.Parameters(_defaultFactory);
            var fields = new Fields(_process.OutputFields(), _process.CalculatedFields.WithoutOutput());

            foreach (var p in transform.Parameters) {

                if (!string.IsNullOrEmpty(p.Field)) {
                    if (fields.FindByParamater(p).Any()) {
                        var field = fields.FindByParamater(p).Last();
                        var name = string.IsNullOrEmpty(p.Name) ? field.Alias : p.Name;
                        parameters.Add(field.Alias, name, null, field.Type);
                    } else {
                        _process.Logger.Warn("A {0} transform references {1}, but I can't find the definition for {1}.\r\nYou may need to define the entity attribute in the parameter element.\r\nOr, set the output attribute to true in the field element. Process transforms rely on fields being output.\r\nOne other possibility is that the participates in a relationship with another field with the same name and Transformalize doesn't know which one you want.  If that's the case, you have to alias one of them.", transform.Method, p.Field);
                        var name = p.Name.Equals(string.Empty) ? p.Field : p.Name;
                        parameters.Add(p.Field, name, p.HasValue() ? p.Value : null, p.Type);
                    }
                } else {
                    var parameter = new Parameter(p.Name, p.Value) {
                        SimpleType = Common.ToSimpleType(p.Type),
                        ValueReferencesField = p.HasValue() && fields.Find(p.Value).Any()
                    };
                    parameters.Add(p.Name, parameter);
                }
            }

            return parameters;
        }

    }
}