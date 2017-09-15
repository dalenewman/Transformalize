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
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Cfg.Net.Contracts;
using Transformalize.Contracts;

namespace Transformalize.Impl {
    public class CfgRow : BaseRow, IRow, IProperties {

        public Dictionary<string, short> Map { get; set; }

        /// <summary>
        /// A constructor for Cfg-Net to handle
        /// </summary>
        /// <param name="names">Names for input, Aliases for output</param>
        public CfgRow(string[] names) : base(names.Length) {
            Map = new Dictionary<string, short>(names.Length);
            for (short i = 0; i < Convert.ToInt16(names.Length); i++) {
                var name = names[i];
                Map[name] = i;
            }
        }

        public override object GetValue(IField field) {
            throw new NotImplementedException("This method is not meant to be called for serializable rows.  They are not aware of IField properties.");
        }

        public override void SetValue(IField field, object value) {
            throw new NotImplementedException("This method is not meant to be called for serializable rows.  They are not aware of IField properties.");
        }

        public object this[IField field] {
            get { return GetValue(field); }
            set { SetValue(field, value); }
        }

        public object this[string key] {
            get { return Storage[Map[key]]; }
            set { Storage[Map[key]] = value; }
        }

        public IEnumerator<KeyValuePair<string, object>> GetEnumerator() {
            return Map.ToDictionary(item => item.Key, item => Storage[item.Value]).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return GetEnumerator();
        }
    }
}