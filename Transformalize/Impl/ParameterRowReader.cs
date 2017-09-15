#region license
// Transformalize
// Configurable Extract, Transform, and Load
// Copyright 2013-2017 Dale Newman
//  
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//   
//       http://www.apache.org/licenses/LICENSE-2.0
//   
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
#endregion
using System;
using System.Collections.Generic;
using System.Linq;
using Transformalize.Configuration;
using Transformalize.Contracts;

namespace Transformalize.Impl {

    public class ParameterRowReader : IRead {

        private readonly IContext _context;
        private readonly IRead _parentReader;
        private readonly IDictionary<string, Parameter> _parameters = new Dictionary<string, Parameter>(StringComparer.OrdinalIgnoreCase);

        public ParameterRowReader(IContext context, IRead parentReader) {
            _context = context;
            _parentReader = parentReader;

            foreach (var p in context.Process.GetActiveParameters()) {
                _parameters[p.Name] = p;
            }

            // attempt to disable validation if parameter can't be converted to field's type
            foreach (var field in _context.Entity.GetAllFields()) {
                Parameter p = null;
                if (_parameters.ContainsKey(field.Alias)) {
                    p = _parameters[field.Alias];
                } else if (_parameters.ContainsKey(field.Name)) {
                    p = _parameters[field.Name];
                }

                if (p != null) {
                    if (!Constants.CanConvert()[field.Type](p.Value)) {
                        field.Validators.Clear();
                    }
                }

            }

        }

        public IEnumerable<IRow> Read() {
            foreach (var row in _parentReader.Read()) {
                foreach (var field in _context.Entity.GetAllFields()) {
                    Parameter p = null;
                    if (_parameters.ContainsKey(field.Alias)) {
                        p = _parameters[field.Alias];
                    } else if (_parameters.ContainsKey(field.Name)) {
                        p = _parameters[field.Name];
                    }

                    if (p != null) {
                        if (Constants.CanConvert()[field.Type](p.Value)) {
                            row[field] = field.Convert(p.Value);
                        } else {
                            if (field.ValidField != string.Empty) {
                                var validField = _context.Entity.CalculatedFields.First(f => f.Alias == field.ValidField);
                                row[validField] = false;
                            }
                            if (field.MessageField != string.Empty) {
                                var messageField = _context.Entity.CalculatedFields.First(f => f.Alias == field.MessageField);
                                row[messageField] = $"Can not convert {p.Value} to a {field.Type}.|";
                            }
                        }

                    }

                }
                yield return row;
            }
        }
    }
}