#region license
// Transformalize
// A Configurable ETL Solution Specializing in Incremental Denormalization.
// Copyright 2013 Dale Newman
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
using System.Collections.Generic;
using Cfg.Net.Contracts;

namespace Cfg.Net.Environment {
    public sealed class PlaceHolderValidator : IGlobalValidator {

        private readonly char _placeHolderMarker;
        private readonly char _placeHolderOpen;
        private readonly char _placeHolderClose;

        public PlaceHolderValidator() : this('@', '(', ')'){}

        public PlaceHolderValidator(char placeHolderMarker, char placeHolderOpen, char placeHolderClose) {
            _placeHolderMarker = placeHolderMarker;
            _placeHolderOpen = placeHolderOpen;
            _placeHolderClose = placeHolderClose;
        }
        public void Validate(string name, string value, IDictionary<string, string> parameters, ILogger logger) {
            if (value.IndexOf(_placeHolderMarker) < 0)
                return;

            List<string> badKeys = null;
            for (var j = 0; j < value.Length; j++) {
                if (value[j] == _placeHolderMarker && value.Length > j + 1 && value[j + 1] == _placeHolderOpen) {
                    var length = 2;
                    while (value.Length > j + length && value[j + length] != _placeHolderClose) {
                        length++;
                    }
                    if (length > 2) {
                        var key = value.Substring(j + 2, length - 2);
                        if (!parameters.ContainsKey(key)) {
                            if (badKeys == null) {
                                badKeys = new List<string> { key };
                            } else {
                                badKeys.Add(key);
                            }
                        }
                    }
                    j = j + length;
                }
            }

            if (badKeys == null)
                return;

            var formatted = $"{_placeHolderMarker}{_placeHolderOpen}{string.Join($"{_placeHolderClose}, {_placeHolderMarker}{_placeHolderOpen}", badKeys)}{_placeHolderClose}";
            logger.Error("Missing {0} for {1}.", badKeys.Count == 1 ? "a parameter value" : "parameter values", formatted);
        }
    }
}