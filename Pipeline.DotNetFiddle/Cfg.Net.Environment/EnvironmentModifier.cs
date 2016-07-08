using System;
using System.Collections.Generic;
using System.Linq;
using Cfg.Net.Contracts;

namespace Cfg.Net.Environment {
    public class EnvironmentModifier : IRootModifier {
        private readonly IGlobalModifier _placeHolderReplacer;
        private readonly IRootModifier _mergeParameters;
        private readonly string _environmentsElementName;
        private readonly string _defaultEnvironmentAttribute;
        private readonly string _environmentNameAttribute;
        private readonly string _parametersElementName;

        public string Name { get; set; }

        public EnvironmentModifier(
            IGlobalModifier placeHolderReplacer,
            IRootModifier mergeParameters) :
            this(
                placeHolderReplacer,
                mergeParameters,
                "environments",
                "environment",
                "name",
                "parameters"
            ) { }

        public EnvironmentModifier(
            IGlobalModifier placeHolderReplacer,
            IRootModifier mergeParameters,
            string environmentsElementName,
            string defaultEnvironmentAttribute,
            string environmentNameAttribute,
            string parametersElementName
            ) {
            _placeHolderReplacer = placeHolderReplacer;
            _mergeParameters = mergeParameters;
            _environmentsElementName = environmentsElementName;
            _defaultEnvironmentAttribute = defaultEnvironmentAttribute;
            _environmentNameAttribute = environmentNameAttribute;
            _parametersElementName = parametersElementName;
        }

        public void Modify(INode root, IDictionary<string, string> parameters) {

            for (var i = 0; i < root.SubNodes.Count; i++) {

                var environments = root.SubNodes.FirstOrDefault(n => n.Name.Equals(_environmentsElementName, StringComparison.OrdinalIgnoreCase));
                if (environments == null)
                    continue;

                if (environments.SubNodes.Count == 0)
                    break;

                if (environments.SubNodes.Count > 1) {
                    IAttribute defaultEnvironment;
                    if (!root.TryAttribute(_defaultEnvironmentAttribute, out defaultEnvironment))
                        continue;

                    foreach (var node in environments.SubNodes) {
                        IAttribute environmentName;
                        if (!node.TryAttribute(_environmentNameAttribute, out environmentName))
                            continue;

                        // for when the default environment is set with a place-holder (e.g. @(environment))
                        var value = _placeHolderReplacer.Modify(_defaultEnvironmentAttribute, defaultEnvironment.Value, parameters);

                        if (!value.Equals(environmentName.Value) || node.SubNodes.Count == 0)
                            continue;

                        if (node.SubNodes[0].Name == _parametersElementName) {
                            _mergeParameters.Modify(node.SubNodes[0], parameters);
                        }
                    }

                }

                // default to first environment
                var environment = environments.SubNodes[0];
                if (environment.SubNodes.Count == 0)
                    break;

                var parametersNode = environment.SubNodes[0];

                if (parametersNode.Name != _parametersElementName || environment.SubNodes.Count == 0)
                    break;

                _mergeParameters.Modify(parametersNode, parameters);
            }


        }

    }
}