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
using System.Collections.Generic;
using Cfg.Net.Contracts;

namespace Cfg.Net.Shorthand {
    internal class Node : INode {
        public Node(string name) {
            Name = name;
            Attributes = new List<IAttribute>();
            SubNodes = new List<INode>();
        }

        public string Name { get; }
        public List<IAttribute> Attributes { get; }
        public List<INode> SubNodes { get; }

        public bool TryAttribute(string name, out IAttribute attr) {
            throw new NotImplementedException();
        }
    }
}