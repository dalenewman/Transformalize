using System;
using System.Collections;
using System.Collections.Generic;
using Transformalize.Libs.NanoXml;
using Transformalize.Main;

namespace Transformalize.Test {

    public abstract class TflAttributes {

        private readonly NanoXmlNode _node;
        private readonly Hashtable _attributes = new Hashtable(StringComparer.Ordinal);
        private readonly Dictionary<string, List<TflAttributes>> _elements = new Dictionary<string, List<TflAttributes>>();

        public TflAttributes this[string element, int i] {
            get { return _elements[element][i]; }
        }

        public object this[string name] {
            get { return Attributes[name]; }
        }

        protected Dictionary<string, Func<NanoXmlNode, TflAttributes>> ElementLoaders = new Dictionary<string, Func<NanoXmlNode, TflAttributes>>();

        protected TflAttributes(NanoXmlNode node) {
            _node = node;
        }

        protected Hashtable Attributes {
            get { return _attributes; }
        }

        protected void Element<T>(string element) {
            ElementLoaders[element] = n => ((TflAttributes)Activator.CreateInstance(typeof(T), n)).Load();
        }

        protected void Attribute<T>(string attribute, T value) {
            Attributes[attribute] = new object[] { value, Common.ToSimpleType(typeof(T).ToString()) };
        }

        public TflAttributes Load() {
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
                        Console.WriteLine("Invalid attribute: {0}.", attribute.Name);
                    }
                }
            }

            for (var i = 0; i < _node.SubNodes.Count; i++) {
                var node = _node.SubNodes[i];
                if (ElementLoaders.ContainsKey(node.Name)) {
                    _elements[node.Name] = new List<TflAttributes>();
                    for (var j = 0; j < node.SubNodes.Count; j++) {
                        var subNode = node.SubNodes[j];
                        _elements[node.Name].Add(ElementLoaders[node.Name](subNode));
                    }
                } else {
                    //throw new TransformalizeException(string.Empty, string.Empty, "Invalid element: {0}.", node.Name);
                    Console.WriteLine("Invalid element: {0}.", node.Name);
                }
            }

            return this;
        }

        public static object[] String(string value) {
            return new object[] { value, "string" };
        }

        public static object[] Default() {
            return new object[] { Common.DefaultValue, "string" };
        }

        public static object[] True() {
            return new object[] { true, "boolean" };
        }

        public static object[] False() {
            return new object[] { false, "boolean" };
        }

        public static object[] Empty() {
            return new object[] { string.Empty, "string" };
        }

        public static object Number(int i) {
            return new object[] { i, "int" };
        }

        public static object Number(double d) {
            return new object[] { d, "double" };
        }

    }
}