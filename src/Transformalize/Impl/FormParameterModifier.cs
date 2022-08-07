#region license
// Transformalize
// Configurable Extract, Transform, and Load
// Copyright 2013-2019 Dale Newman
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
using Cfg.Net.Contracts;
using Cfg.Net.Parsers;

namespace Transformalize.Impl {
   public class FormParameterNode : INode {

      private Dictionary<string, IAttribute> _attributes;

      public FormParameterNode(string name) {
         Name = name;
         Attributes = new List<IAttribute>();
         SubNodes = new List<INode>();
      }

      public string Name { get; }
      public List<IAttribute> Attributes { get; }
      public List<INode> SubNodes { get; }

      public bool TryAttribute(string name, out IAttribute attr) {
         if (_attributes == null) {
            _attributes = new Dictionary<string, IAttribute>();
            for (var i = 0; i < Attributes.Count; i++) {
               _attributes[Attributes[i].Name] = Attributes[i];
            }
         }
         if (_attributes.ContainsKey(name)) {
            attr = _attributes[name];
            return true;
         }
         attr = null;
         return false;
      }
   }

   /// <summary>
   /// If in form mode, this creates parameters for every input field.
   /// </summary>
   public class FormParameterModifier : ICustomizer {
      private readonly ICustomizer _after;

      public FormParameterModifier(ICustomizer after) {
         _after = after;
      }

      public void Customize(string collection, INode node, IDictionary<string, string> parameters, ILogger logger) {
         _after.Customize(collection, node, parameters, logger);
      }

      public void Customize(INode root, IDictionary<string, string> parameters, ILogger logger) {

         if (root.TryAttribute("mode", out var mode)) {
            if (mode.Value.Equals("form")) {

               var parameterCollection = root.SubNodes.FirstOrDefault(n => n.Name.Equals("parameters", StringComparison.OrdinalIgnoreCase));

               if (parameterCollection != null) {

                  var parameterNames = new HashSet<string>();

                  foreach (var node in parameterCollection.SubNodes) {
                     if (node.TryAttribute("name", out var name)) {
                        parameterNames.Add(name.Value.ToString());
                     }
                  }

                  var entityCollection = root.SubNodes.FirstOrDefault(n => n.Name.Equals("entities", StringComparison.OrdinalIgnoreCase));
                  if (entityCollection != null) {
                     foreach (var entity in entityCollection.SubNodes) {
                        var fieldCollection = entity.SubNodes.FirstOrDefault(n => n.Name.Equals("fields", StringComparison.OrdinalIgnoreCase));
                        if (fieldCollection != null) {
                           foreach (var field in fieldCollection.SubNodes) {
                              if (field.TryAttribute("name", out var name)) {

                                 var add = false;
                                 if (field.TryAttribute("input", out var input)) {
                                    if (input.Value.ToString().ToLower() == "true") {
                                       add = true;
                                    }
                                 } else {
                                    add = true;
                                 }

                                 if (!parameterNames.Contains(name.Value) && add) {

                                    var node = new FormParameterNode("add");
                                    node.Attributes.Add(new NodeAttribute("name", name.Value));
                                    if (field.TryAttribute("default", out var def)) {
                                       NodeAttribute n = new NodeAttribute("value", null);
                                       if (def.Value.ToString() == Constants.DefaultSetting) {
                                          n.Value = string.Empty;
                                       } else {
                                          n.Value = def.Value;
                                       }
                                       node.Attributes.Add(n);
                                    } else {
                                       node.Attributes.Add(new NodeAttribute("value", string.Empty));
                                    }

                                    node.Attributes.Add(new NodeAttribute("label", "n/a"));
                                    node.Attributes.Add(new NodeAttribute("invalid-characters", string.Empty));

                                    if (field.TryAttribute("type", out var type)) {
                                       if (type != null && type.Value != null && type.Value.ToString().ToLower().StartsWith("bool")) {
                                          node.Attributes.Add(new NodeAttribute("type", "bool"));
                                       }
                                    }

                                    parameterCollection.SubNodes.Add(node);

                                 }
                              }


                           }
                        }
                     }
                  }

               }

            }
         }

         _after.Customize(root, parameters, logger);
      }
   }
}