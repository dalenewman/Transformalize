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
using System;
using System.Linq;
using Cfg.Net;

namespace Pipeline.Configuration {

    public class Parameter : CfgNode {

        Field _loadedField;
        string _type;
        private string _field;
        private string _entity;

        [Cfg(value = "")]
        public string Entity
        {
            get { return _entity; }
            set
            {
                _entity = value;
                _loadedField = null;  //invalidate cache
            }
        }

        [Cfg(value = "")]
        public string Field
        {
            get { return _field; }
            set
            {
                _field = value;
                _loadedField = null; //invalidate cache
            }
        }

        [Cfg(value = "")]
        public string Name { get; set; }

        [Cfg(value = null, validators = "ipc")] // illegal parameter characters
        public string Value { get; set; }

        [Cfg(value = true)]
        public bool Input { get; set; }

        [Cfg(value = false)]
        public bool Prompt { get; set; }

        [Cfg(value = "", toLower = true)]
        public string Map { get; set; }

        protected override void Validate() {
            switch (Type) {
                case "string":
                    break;
                default:
                    if (!string.IsNullOrEmpty(Value) && !Constants.CanConvert()[Type](Value)) {
                        Error($"The parameter {Name} is supposed to be a {Type}, but it can not be parsed as such.");
                    }
                    break;
            }
            if (string.IsNullOrEmpty(Label)) {
                Label = Name;
            }

        }

        [Cfg(value = "string", domain = Constants.TypeDomain, ignoreCase = true)]
        public string Type
        {
            get { return _type; }
            set { _type = value != null && value.StartsWith("sy", StringComparison.OrdinalIgnoreCase) ? value.ToLower().Replace("system.", string.Empty) : value; }
        }

        public bool HasValue() {
            return Value != null;
        }

        public bool IsField(Process process) {

            if (_loadedField != null)
                return true;

            if (string.IsNullOrEmpty(Entity)) {
                _loadedField = process.GetAllFields().FirstOrDefault(f => f.Alias.Equals(Field, StringComparison.OrdinalIgnoreCase)) ?? process.GetAllFields().FirstOrDefault(f => f.Name.Equals(Field, StringComparison.OrdinalIgnoreCase));
                return _loadedField != null;
            }

            Entity entity;
            if (process.TryGetEntity(Entity, out entity)) {
                if (entity.TryGetField(Field, out _loadedField)) {
                    return true;
                }
            }
            return false;
        }

        public Field AsField(Process process) {
            if (_loadedField != null)
                return _loadedField;

            if (string.IsNullOrEmpty(Entity)) {
                _loadedField = process.GetAllFields().FirstOrDefault(f => f.Alias.Equals(Field, StringComparison.OrdinalIgnoreCase)) ?? process.GetAllFields().FirstOrDefault(f => f.Name.Equals(Field, StringComparison.OrdinalIgnoreCase));
                return _loadedField;
            }

            Entity entity;
            if (process.TryGetEntity(Entity, out entity)) {
                if (entity.TryGetField(Field, out _loadedField)) {
                    return _loadedField;
                }
            }
            return null;
        }

        [Cfg(value = "")]
        public string Label { get; set; }


    }

}