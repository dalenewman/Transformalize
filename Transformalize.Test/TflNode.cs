using System;
using System.Collections;
using System.Collections.Generic;
using Transformalize.Libs.NanoXml;
using Transformalize.Main;

namespace Transformalize.Test {

    public abstract class TflNode {

        private readonly NanoXmlNode _node;
        private readonly Hashtable _attributes = new Hashtable(StringComparer.Ordinal);
        private readonly Dictionary<string, List<TflNode>> _elements = new Dictionary<string, List<TflNode>>();

        public TflNode this[string element, int i] {
            get { return _elements[element][i]; }
        }

        public object this[string name] {
            get { return Attributes[name]; }
        }

        protected Dictionary<string, Func<NanoXmlNode, TflNode>> ElementLoaders = new Dictionary<string, Func<NanoXmlNode, TflNode>>();

        protected TflNode(NanoXmlNode node) {
            _node = node;
        }

        protected Hashtable Attributes {
            get { return _attributes; }
        }

        protected void Elements<T>(string element) {
            ElementLoaders[element] = n => ((TflNode)Activator.CreateInstance(typeof(T), n)).Load();
        }

        protected void Attribute<T>(T value, params string[] args) {
            for (var i = 0; i < args.Length; i++) {
                var attribute = args[i];
                Attributes[attribute] = new object[] { value, Common.ToSimpleType(typeof(T).ToString()) };
            }
        }

        public TflNode Load() {
            if (_node.Attributes.Count > 0) {
                var converter = Common.GetObjectConversionMap();
                for (var i = 0; i < _node.Attributes.Count; i++) {
                    var attribute = _node.Attributes[i];
                    if (Attributes.ContainsKey(attribute.Name)) {
                        var temp = (object[])Attributes[attribute.Name];
                        var defaultValue = temp[0];
                        var type = temp[1].ToString();
                        Attributes[attribute.Name] = type.Equals("string") ?
                            new object[] { attribute.Value ?? defaultValue, type } :
                            new object[] { converter[type](attribute.Value), type };
                    } else {
                        //throw new TransformalizeException(string.Empty, string.Empty, "Invalid attribute: {0}.", attribute.Name);
                        Console.WriteLine("Invalid attribute {0} in {1}.", attribute.Name, _node.Name);
                    }
                }
            }

            for (var i = 0; i < _node.SubNodes.Count; i++) {
                var node = _node.SubNodes[i];
                if (ElementLoaders.ContainsKey(node.Name)) {
                    _elements[node.Name] = new List<TflNode>();
                    for (var j = 0; j < node.SubNodes.Count; j++) {
                        var add = node.SubNodes[j];
                        if (add.Name == "add") {
                            _elements[node.Name].Add(ElementLoaders[node.Name](add));
                        } else {
                            Console.WriteLine("Invalid element {0} in {1}.", add.Name, node.Name);
                        }
                    }
                } else {
                    //throw new TransformalizeException(string.Empty, string.Empty, "Invalid element: {0}.", node.Name);
                    Console.WriteLine("Invalid element {0} in {1}.", node.Name, _node.Name);
                }
            }

            return this;
        }

    }
}