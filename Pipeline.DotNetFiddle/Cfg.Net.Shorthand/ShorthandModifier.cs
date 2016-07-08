using System;
using System.Collections.Generic;
using System.Linq;
using Cfg.Net.Contracts;

namespace Cfg.Net.Shorthand {
    public class ShorthandModifier : INodeModifier {
        private readonly ShorthandRoot _root;
        internal static char NamedParameterSplitter = ':';

        public ShorthandModifier(ShorthandRoot root, string name) {
            _root = root;
            Name = name;
        }

        public string Name { get; set; }
        public void Modify(INode node, object value, IDictionary<string, string> parameters)
        {
            var str = value as string;

            if (str == null)
                return;

            var expressions = new Expressions(str);
            var shorthandNodes = new Dictionary<string, List<INode>>();

            foreach (var expression in expressions) {
                MethodData methodData;

                if (!_root.MethodDataLookup.TryGetValue(expression.Method, out methodData))
                    continue;

                if (methodData.Target.Collection == string.Empty || methodData.Target.Property == string.Empty)
                    continue;

                var shorthandNode = new Node("add");
                shorthandNode.Attributes.Add(new ShorthandAttribute(methodData.Target.Property, expression.Method));

                var signatureParameters = methodData.Signature.Parameters.Select(p => new Parameter { Name = p.Name, Value = p.Value }).ToList();
                var passedParameters = expression.Parameters.Select(p => new string(p.ToCharArray())).ToArray();

                // single parameters
                if (methodData.Signature.Parameters.Count == 1 && expression.SingleParameter != string.Empty) {
                    var name = methodData.Signature.Parameters[0].Name;
                    var val = expression.SingleParameter.StartsWith(name + ":",
                        StringComparison.OrdinalIgnoreCase)
                        ? expression.SingleParameter.Remove(0, name.Length + 1)
                        : expression.SingleParameter;
                    shorthandNode.Attributes.Add(new ShorthandAttribute(name, val));
                } else {
                    // named parameters
                    foreach (var parameter in passedParameters) {
                        var split = Utility.Split(parameter, NamedParameterSplitter);
                        if (split.Length != 2)
                            continue;

                        var name = Utility.NormalizeName(split[0]);
                        shorthandNode.Attributes.Add(new ShorthandAttribute(name, split[1]));
                        signatureParameters.RemoveAll(p => Utility.NormalizeName(p.Name) == name);
                        var parameter1 = parameter;
                        expression.Parameters.RemoveAll(p => p == parameter1);
                    }

                    // ordered nameless parameters
                    for (var m = 0; m < signatureParameters.Count; m++) {
                        var signatureParameter = signatureParameters[m];
                        var parameterValue = m < expression.Parameters.Count ? expression.Parameters[m] : (signatureParameter.Value ?? string.Empty);

                        if (parameterValue.Contains("\\" + NamedParameterSplitter)) {
                            parameterValue = parameterValue.Replace("\\" + NamedParameterSplitter, NamedParameterSplitter.ToString());
                        }

                        var attribute = new ShorthandAttribute(signatureParameter.Name, parameterValue);
                        shorthandNode.Attributes.Add(attribute);
                    }
                }

                if (shorthandNodes.ContainsKey(methodData.Target.Collection)) {
                    shorthandNodes[methodData.Target.Collection].Add(shorthandNode);
                } else {
                    shorthandNodes[methodData.Target.Collection] = new List<INode> { shorthandNode };
                }
            }

            foreach (var pair in shorthandNodes) {
                var shorthandCollection = node.SubNodes.FirstOrDefault(sn => sn.Name == pair.Key);
                if (shorthandCollection == null) {
                    shorthandCollection = new Node(pair.Key);
                    shorthandCollection.SubNodes.AddRange(pair.Value);
                    node.SubNodes.Add(shorthandCollection);
                } else {
                    shorthandCollection.SubNodes.InsertRange(0, pair.Value);
                }
            }
        }
    }
}