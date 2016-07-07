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
using System.Collections.Generic;
using System.Linq;
using Cfg.Net.Contracts;
using Cfg.Net.Parsers.nanoXML;

namespace Cfg.Net.Parsers
{
    public sealed class XmlNode : INode
    {
        private Dictionary<string, IAttribute> _attributes;

        public XmlNode(){}

        public XmlNode(NanoXmlNode nanoXmlNode)
        {
            Name = nanoXmlNode.Name;
            Attributes =
                new List<IAttribute>(
                    nanoXmlNode.Attributes.Select(a => new NodeAttribute {Name = a.Name, Value = a.Value}));
            SubNodes = new List<INode>(nanoXmlNode.SubNodes.Select(n => new XmlNode(n)));
        }

        public string Name { get; }
        public List<IAttribute> Attributes { get; }
        public List<INode> SubNodes { get; }

        public bool TryAttribute(string name, out IAttribute attr)
        {
            if (_attributes == null)
            {
                _attributes = new Dictionary<string, IAttribute>();
                for (int i = 0; i < Attributes.Count; i++)
                {
                    _attributes[Attributes[i].Name] = Attributes[i];
                }
            }
            if (_attributes.ContainsKey(name))
            {
                attr = _attributes[name];
                return true;
            }
            attr = null;
            return false;
        }
    }
}