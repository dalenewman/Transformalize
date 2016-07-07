#region license
// Cfg.Net
// Copyright 2015 Dale Newman
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//  
//      http://www.apache.org/licenses/LICENSE-2.0
//  
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
#endregion
using System;
using System.Linq;
using System.Reflection;
using System.Text;
using Cfg.Net.Contracts;
using Cfg.Net.Loggers;
using Cfg.Net.Parsers;

namespace Cfg.Net.Ext {

    public static class Extensions {

        internal static void SetDefaults(this CfgNode node) {

            var metadata = CfgMetadataCache.GetMetadata(node.GetType(), node.Events);
            foreach (var pair in metadata)
            {
                bool? isGenericType = pair.Value.PropertyInfo.PropertyType.IsGenericType;

                if (isGenericType.Value) {
                    var value = pair.Value.Getter(node);
                    if (value == null) {
                        pair.Value.Setter(node, Activator.CreateInstance(pair.Value.PropertyInfo.PropertyType));
                    }
                } else {
                    if (pair.Value.TypeMismatch || pair.Value.Attribute.value == null)
                        continue;

                    var value = pair.Value.Getter(node);

                    if (value == null) {
                        pair.Value.Setter(node, pair.Value.Attribute.value);
                    } else if (value.Equals(pair.Value.Default)) {
                        if (pair.Value.Default.Equals(pair.Value.Attribute.value)) {
                            if (!pair.Value.Attribute.ValueIsSet) {
                                pair.Value.Setter(node, pair.Value.Attribute.value);
                            }
                        } else {
                            pair.Value.Setter(node, pair.Value.Attribute.value);
                        }
                    }
                }
            }
        }

        internal static void Clear(this CfgNode node, CfgEvents events) {
            var metadata = CfgMetadataCache.GetMetadata(node.GetType(), node.Events);
            foreach (var pair in metadata)
            {
                bool? isGenericType = pair.Value.PropertyInfo.PropertyType.IsGenericType;
                if (isGenericType.Value) {
                    pair.Value.Setter(node, Activator.CreateInstance(pair.Value.PropertyInfo.PropertyType));
                } else {
                    pair.Value.Setter(node, pair.Value.TypeMismatch ? pair.Value.Default : pair.Value.Attribute.value);
                }
            }
            node.Events = events ?? new CfgEvents(new DefaultLogger(new MemoryLogger(), null));
        }

        /// <summary>
        /// When you want to clone yourself 
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        public static T Clone<T>(this T node) where T : CfgNode {
            return CfgMetadataCache.Clone(node);
        }


        public static T WithDefaults<T>(this T node) where T : CfgNode {
            node.SetDefaults();
            return node;
        }

        public static T WithValidation<T>(this T node, string parent = "") where T : CfgNode {
            node.WithDefaults();
            if (node.Events == null) {
                node.Events = new CfgEvents(new DefaultLogger(new MemoryLogger(), null));
            }
            node.ValidateBasedOnAttributes(new XmlNode(), null);
            node.ValidateListsBasedOnAttributes(parent);
            return node;
        }

        public static void TrimEnd(this StringBuilder sb, string trimChars) {
            var length = sb.Length;
            if (length != 0) {
                var chars = trimChars.ToCharArray();
                var i = length - 1;

                if (trimChars.Length == 1) {
                    while (i > -1 && sb[i] == trimChars[0]) {
                        i--;
                    }
                } else {
                    while (i > -1 && chars.Any(c => c.Equals(sb[i]))) {
                        i--;
                    }
                }

                if (i < (length - 1)) {
                    sb.Remove(i + 1, (length - i) - 1);
                }
            }
        }


    }
}