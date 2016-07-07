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
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Cfg.Net.Contracts;
using Cfg.Net.Ext;
using Cfg.Net.Loggers;
using Cfg.Net.Parsers;
using Cfg.Net.Serializers;

namespace Cfg.Net {
    public abstract class CfgNode {

        static readonly object Locker = new object();

        internal IParser Parser { get; set; }
        internal IReader Reader { get; set; }
        internal ISerializer Serializer { get; set; }
        internal ILogger Logger { get; set; }

        internal IDictionary<string, IValidator> Validators { get; set; } = new Dictionary<string, IValidator>();
        internal IDictionary<string, INodeValidator> NodeValidators { get; set; } = new Dictionary<string, INodeValidator>();
        internal IList<IGlobalValidator> GlobalValidators { get; set; } = new List<IGlobalValidator>();

        internal IDictionary<string, IModifier> Modifiers { get; set; } = new Dictionary<string, IModifier>();
        internal IDictionary<string, INodeModifier> NodeModifiers { get; set; } = new Dictionary<string, INodeModifier>();
        internal IList<IGlobalModifier> GlobalModifiers { get; set; } = new List<IGlobalModifier>();
        internal IList<IRootModifier> RootModifiers { get; set; } = new List<IRootModifier>();

        internal Type Type { get; set; }
        internal CfgEvents Events { get; set; }
        protected Dictionary<string, string> UniqueProperties { get; } = new Dictionary<string, string>();

        /// <summary>
        /// Constructor for injecting anything marked with IDependency
        /// </summary>
        /// <param name="dependencies"></param>
        protected CfgNode(params IDependency[] dependencies) {

            if (dependencies != null) {
                foreach (var dependency in dependencies.Where(dependency => dependency != null)) {
                    if (dependency is IReader) {
                        Reader = dependency as IReader;
                    } else if (dependency is IParser) {
                        Parser = dependency as IParser;
                    } else if (dependency is ISerializer) {
                        Serializer = dependency as ISerializer;
                    } else if (dependency is ILogger) {
                        Logger = dependency as ILogger;
                    } else if (dependency is IGlobalModifier) {
                        GlobalModifiers.Add(dependency as IGlobalModifier);
                    } else if (dependency is INodeModifier) {
                        var nodeModifier = dependency as INodeModifier;
                        NodeModifiers[nodeModifier.Name] = nodeModifier;
                    } else if (dependency is INodeValidator) {
                        var nodeValidator = dependency as INodeValidator;
                        NodeValidators[nodeValidator.Name] = nodeValidator;
                    } else if (dependency is IGlobalValidator) {
                        GlobalValidators.Add(dependency as IGlobalValidator);
                    } else if (dependency is INamedDependency) {
                        if (dependency is IModifier) {
                            var modifier = dependency as IModifier;
                            Modifiers[modifier.Name] = modifier;
                        } else if (dependency is IValidator) {
                            var validator = dependency as IValidator;
                            Validators[validator.Name] = validator;
                        }
                    } else if (dependency is IRootModifier) {
                        RootModifiers.Add(dependency as IRootModifier);
                    }
                }
            }

            Type = GetType();
        }

        protected internal void Error(string message, params object[] args) {
            Events.Error(message, args);
        }

        protected internal void Warn(string message, params object[] args) {
            Events.Warning(message, args);
        }

        /// <summary>
        /// Load the configuration into the root (top-most) node.
        /// </summary>
        /// <param name="cfg">by default, cfg should be XML or JSON, but can be other things depending on what IParser is injected.</param>
        /// <param name="parameters">key, value pairs that replace @(PlaceHolders) with values.</param>
        public void Load(string cfg, IDictionary<string, string> parameters = null) {

            Events = new CfgEvents(new DefaultLogger(new MemoryLogger(), Logger));
            this.Clear(Events);

            if (string.IsNullOrEmpty(cfg)) {
                Events.Error("The configuration passed in is null.");
                this.SetDefaults();
                return;
            }

            cfg = cfg.Trim();

            if (parameters == null) {
                parameters = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            }

            INode node;
            try {
                if (Reader != null) {
                    cfg = Reader.Read(cfg, parameters, Events.Logger);
                    if (Events.Errors().Any()) {
                        this.SetDefaults();
                        return;
                    }
                }

                if (Parser == null) {
                    switch (cfg[0]) {
                        case '{':
                            node = new FastJsonParser().Parse(cfg);
                            break;
                        case '<':
                            node = new NanoXmlParser().Parse(cfg);
                            break;
                        default:
                            Events.Error("Without a custom parser, the configuration should be XML or JSON. Your configuration starts with the character {0}.", cfg[0]);
                            this.SetDefaults();
                            return;
                    }
                } else {
                    node = Parser.Parse(cfg);
                }

                if (Serializer == null) {
                    switch (cfg[0]) {
                        case '{':
                            Serializer = new JsonSerializer();
                            break;
                        default:
                            Serializer = new XmlSerializer();
                            break;
                    }
                }

                foreach (var modifier in RootModifiers) {
                    modifier.Modify(node, parameters);
                }

            } catch (Exception ex) {
                Events.ParseException(ex.Message);
                return;
            }

            LoadProperties(node, null, parameters);
            LoadCollections(node, null, parameters);
            PreValidate();
            ValidateBasedOnAttributes(node, parameters);
            ValidateListsBasedOnAttributes(node.Name);
            Validate();
            PostValidate();
        }

        CfgNode Load(
            INode node,
            string parent,
            ISerializer serializer,
            CfgEvents events,
            IDictionary<string, IValidator> validators,
            IDictionary<string, INodeValidator> nodeValidators,
            IList<IGlobalValidator> globalValidators,
            IDictionary<string, IModifier> modifiers,
            IDictionary<string, INodeModifier> nodeModifiers,
            IList<IGlobalModifier> globalModifiers,
            IDictionary<string, string> parameters
        ) {
            this.Clear(events);

            // parser, reader, and mergeParameters do not need to be passed in

            Modifiers = modifiers;
            NodeModifiers = nodeModifiers;
            GlobalModifiers = globalModifiers;

            Validators = validators;
            NodeValidators = nodeValidators;
            GlobalValidators = globalValidators;

            Serializer = serializer;

            LoadProperties(node, parent, parameters);
            LoadCollections(node, parent, parameters);
            PreValidate();
            ValidateBasedOnAttributes(node, parameters);
            ValidateListsBasedOnAttributes(node.Name);
            Validate();
            PostValidate();
            return this;
        }

        /// <summary>
        ///     Override to add custom validation.  Use `Error()` or `Warn()` to record issues.
        /// </summary>
        protected virtual void Validate() { }

        /// <summary>
        ///     Allows for modification of configuration before validation.
        ///     Note: You are not protected from `null` here.
        /// </summary>
        protected virtual void PreValidate() { }

        /// <summary>
        ///     Allows for modification of configuration after validation.
        ///     Note: You can check for Errors() here and modify accordingly.
        /// </summary>
        protected virtual void PostValidate() { }

        public string Serialize() {
            return (Serializer ?? new XmlSerializer()).Serialize(this);
        }

        void LoadCollections(INode node, string parentName, IDictionary<string, string> parameters = null) {
            var metadata = CfgMetadataCache.GetMetadata(Type, Events);
            var elementNames = CfgMetadataCache.ElementNames(Type).ToList();
            var elements = new Dictionary<string, IList>();
            var elementHits = new HashSet<string>();
            var addHits = new HashSet<string>();

            //initialize all the lists
            for (var i = 0; i < elementNames.Count; i++) {
                var key = elementNames[i];
                elements.Add(key, (IList)metadata[key].Getter(this));
            }

            for (var i = 0; i < node.SubNodes.Count; i++) {
                var subNode = node.SubNodes[i];
                var subNodeKey = CfgMetadataCache.NormalizeName(Type, subNode.Name);
                if (metadata.ContainsKey(subNodeKey)) {
                    elementHits.Add(subNodeKey);
                    var item = metadata[subNodeKey];

                    for (var j = 0; j < subNode.SubNodes.Count; j++) {
                        var add = subNode.SubNodes[j];
                        if (add.Name.Equals("add", StringComparison.Ordinal)) {
                            var addKey = CfgMetadataCache.NormalizeName(Type, subNode.Name);
                            addHits.Add(addKey);
                            if (item.Loader == null) {
                                if (typeof(IProperties).IsAssignableFrom(item.ListType)) {
                                    object obj = null;
                                    foreach (var cp in item.ListType.GetConstructors().Select(c => c.GetParameters())) {
                                        if (!cp.Any()) {
                                            obj = Activator.CreateInstance(item.ListType);
                                            break;
                                        }

                                        if (cp.Count() == 1) {
                                            if (cp.First().ParameterType == typeof(int)) {
                                                obj = Activator.CreateInstance(item.ListType, add.Attributes.Count);
                                                break;
                                            }

                                            if (cp.First().ParameterType == typeof(string[])) {
                                                var names = add.Attributes.Select(a => a.Name).ToArray();
                                                obj = Activator.CreateInstance(item.ListType, new object[] { names });
                                                break;
                                            }
                                        }
                                    }
                                    if (obj == null) {
                                        Events.ConstructorNotFound(parentName, subNode.Name);
                                    } else {
                                        var properties = obj as IProperties;
                                        for (var k = 0; k < add.Attributes.Count; k++) {
                                            var attribute = add.Attributes[k];
                                            properties[attribute.Name] = attribute.Value;
                                        }
                                        elements[addKey].Add(obj);
                                    }
                                } else {
                                    if (add.Attributes.Count == 1) {
                                        var attrValue = add.Attributes[0].Value;
                                        if (item.ListType == typeof(string) || item.ListType == typeof(object)) {
                                            elements[addKey].Add(attrValue);
                                        } else {
                                            try {
                                                elements[addKey].Add(CfgConstants.Converter[item.ListType](attrValue));
                                            } catch (Exception ex) {
                                                Events.SettingValue(subNode.Name, attrValue, parentName, subNode.Name, ex.Message);
                                            }
                                        }
                                    } else {
                                        Events.OnlyOneAttributeAllowed(parentName, subNode.Name, add.Attributes.Count);
                                    }
                                }
                            } else {
                                var loaded = item.Loader().Load(add, subNode.Name, Serializer, Events, Validators, NodeValidators, GlobalValidators, Modifiers, NodeModifiers, GlobalModifiers, parameters);
                                elements[addKey].Add(loaded);
                            }
                        } else {
                            Events.UnexpectedElement(add.Name, subNode.Name);
                        }
                    }
                } else {
                    if (parentName == null) {
                        Events.InvalidElement(node.Name, subNode.Name);
                    } else {
                        Events.InvalidNestedElement(parentName, node.Name, subNode.Name);
                    }
                }
            }
        }

        protected internal void ValidateListsBasedOnAttributes(string parent) {
            var metadata = CfgMetadataCache.GetMetadata(Type, Events);
            var elementNames = CfgMetadataCache.ElementNames(Type).ToList();
            foreach (var listName in elementNames) {
                var listMetadata = metadata[listName];
                var list = (IList)metadata[listName].Getter(this);
                ValidateUniqueAndRequiredProperties(parent, listName, listMetadata, list);
            }
        }

        void ValidateUniqueAndRequiredProperties(
            string parent,
            string listName,
            CfgMetadata listMetadata,
            ICollection list
        ) {

            //if more than one then uniqueness comes into question
            if (list.Count > 1) {
                lock (Locker) {
                    if (listMetadata.UniquePropertiesInList == null) {
                        listMetadata.UniquePropertiesInList = CfgMetadataCache.GetMetadata(listMetadata.ListType, Events)
                            .Where(p => p.Value.Attribute.unique)
                            .Select(p => p.Key)
                            .ToArray();
                    }
                }

                if (listMetadata.UniquePropertiesInList.Length <= 0)
                    return;

                for (var j = 0; j < listMetadata.UniquePropertiesInList.Length; j++) {
                    var unique = listMetadata.UniquePropertiesInList[j];
                    var duplicates = list
                        .Cast<CfgNode>()
                        .Where(n => n.UniqueProperties.ContainsKey(unique))
                        .Select(n => n.UniqueProperties[unique])
                        .GroupBy(n => n)
                        .Where(group => @group.Count() > 1)
                        .Select(group => @group.Key)
                        .ToArray();

                    for (var l = 0; l < duplicates.Length; l++) {
                        Events.DuplicateSet(unique, duplicates[l], listName);
                    }
                }
            } else if (list.Count == 0 && listMetadata.Attribute.required) {
                Events.MissingRequiredAdd(parent, listName);
            }
        }

        void LoadProperties(INode node, string parentName, IDictionary<string, string> parameters = null) {
            var metadata = CfgMetadataCache.GetMetadata(Type, Events);
            var keys = CfgMetadataCache.PropertyNames(Type).ToArray();

            if (!keys.Any())
                return;

            var keyHits = new HashSet<string>();
            var nullWarnings = new HashSet<string>();

            for (var i = 0; i < node.Attributes.Count; i++) {
                var attribute = node.Attributes[i];
                var attributeKey = CfgMetadataCache.NormalizeName(Type, attribute.Name);
                if (metadata.ContainsKey(attributeKey)) {
                    var item = metadata[attributeKey];

                    if (attribute.Value == null) {
                        // if attribute is null, use the getter
                        var maybe = item.Getter(this);
                        if (maybe == null) {
                            if (nullWarnings.Add(attribute.Name)) {
                                Events.Warning($"'{attribute.Name}' in '{parentName}' is susceptible to nulls.");
                            }
                            continue;
                        }
                        attribute.Value = maybe;
                    }

                    // run injected property specific modifiers
                    if (item.Attribute.ModifiersSet) {
                        foreach (var modifier in item.Attribute.modifiers.Split(item.Attribute.delimiter)) {
                            var isModifier = Modifiers.ContainsKey(modifier);
                            var isNodeModifier = !isModifier && NodeModifiers.ContainsKey(modifier);
                            if (isModifier || isNodeModifier) {
                                if (isModifier) {
                                    attribute.Value = Modifiers[modifier].Modify(attribute.Name, attribute.Value, parameters);
                                } else {
                                    NodeModifiers[modifier].Modify(node, attribute.Value, parameters);
                                }
                            } else {
                                Events.Warning($"'{attribute.Name}' has a '{modifier}' modifier, but no '{modifier}' was injected.");
                            }
                        }
                    }

                    // run global modifiers
                    foreach (var modifier in GlobalModifiers) {
                        attribute.Value = modifier.Modify(attribute.Name, attribute.Value, parameters);
                    }

                    if (item.PropertyInfo.PropertyType == typeof(string)) {

                        if (item.Attribute.toLower) {
                            attribute.Value = attribute.Value.ToString().ToLower();
                        } else if (item.Attribute.toUpper) {
                            attribute.Value = attribute.Value.ToString().ToUpper();
                        }
                        item.Setter(this, attribute.Value);
                        keyHits.Add(attributeKey);
                    } else {
                        try {
                            item.Setter(this, CfgConstants.Converter[item.PropertyInfo.PropertyType](attribute.Value));
                            keyHits.Add(attributeKey);
                        } catch (Exception ex) {
                            Events.SettingValue(attribute.Name, attribute.Value, parentName, node.Name, ex.Message);
                        }
                    }
                } else {
                    Events.InvalidAttribute(parentName, node.Name, attribute.Name, string.Join(", ", keys));
                }
            }

            // missing any required attributes?
            foreach (var key in keys.Except(keyHits)) {
                var item = metadata[key];
                if (item.Attribute.required) {
                    Events.MissingAttribute(parentName, node.Name, key);
                }
            }

        }

        internal void ValidateBasedOnAttributes(INode node, IDictionary<string, string> parameters) {
            var metadata = CfgMetadataCache.GetMetadata(Type, Events);
            var keys = CfgMetadataCache.PropertyNames(Type).ToArray();

            if (!keys.Any())
                return;

            for (var i = 0; i < keys.Length; i++) {
                var key = keys[i];
                var item = metadata[key];

                var objectValue = item.Getter(this);
                if (objectValue == null)
                    continue;

                var stringValue = item.PropertyInfo.PropertyType == typeof(string) ? (string)objectValue : objectValue.ToString();

                if (item.Attribute.unique) {
                    UniqueProperties[key] = stringValue;
                }

                // run global validators
                foreach (var validator in GlobalValidators) {
                    try {
                        validator.Validate(key, stringValue, parameters, Events.Logger);
                    } catch (Exception ex) {
                        Events.ValidatorException(validator.GetType().Name, ex, stringValue);
                    }
                }

                if (item.Attribute.DomainSet) {
                    if (!item.IsInDomain(stringValue)) {
                        Events.ValueNotInDomain(key, stringValue, item.Attribute.domain.Replace(item.Attribute.delimiter.ToString(), ", "));
                    }
                }

                CheckValueLength(item.Attribute, key, stringValue);

                CheckValueBoundaries(item.Attribute, key, objectValue);

                CheckValidators(node, item, key, stringValue, parameters);
            }
        }

        void CheckValidators(INode node, CfgMetadata item, string name, string value, IDictionary<string, string> parameters) {
            if (!item.Attribute.ValidatorsSet)
                return;

            foreach (var validator in item.Validators()) {
                var isValidator = Validators.ContainsKey(validator);
                var isNodeValidator = !isValidator && NodeValidators.ContainsKey(validator);
                if (isValidator || isNodeValidator) {
                    try {
                        if (isValidator) {
                            Validators[validator].Validate(name, value, parameters, Events.Logger);
                        } else {
                            NodeValidators[validator].Validate(node, value, parameters, Events.Logger);
                        }
                    } catch (Exception ex) {
                        Events.ValidatorException(validator, ex, value);
                    }
                } else {
                    Events.MissingValidator(name, validator);
                }
            }
        }

        void CheckValueLength(CfgAttribute itemAttributes, string name, string value) {
            if (itemAttributes.MinLengthSet) {
                if (value.Length < itemAttributes.minLength) {
                    Events.ValueTooShort(name, value, itemAttributes.minLength);
                }
            }

            if (!itemAttributes.MaxLengthSet)
                return;

            if (value.Length > itemAttributes.maxLength) {
                Events.ValueTooLong(name, value, itemAttributes.maxLength);
            }
        }

        void CheckValueBoundaries(CfgAttribute itemAttributes, string name, object value) {
            if (!itemAttributes.MinValueSet && !itemAttributes.MaxValueSet)
                return;

            var comparable = value as IComparable;
            if (comparable == null) {
                Events.ValueIsNotComparable(name, value);
            } else {
                if (itemAttributes.MinValueSet) {
                    if (comparable.CompareTo(itemAttributes.minValue) < 0) {
                        Events.ValueTooSmall(name, value, itemAttributes.minValue);
                    }
                }

                if (!itemAttributes.MaxValueSet)
                    return;

                if (comparable.CompareTo(itemAttributes.maxValue) > 0) {
                    Events.ValueTooBig(name, value, itemAttributes.maxValue);
                }
            }
        }

        public string[] Errors() {
            return Events == null ? new string[0] : Events.Errors();
        }

        public string[] Warnings() {
            return Events == null ? new string[0] : Events.Warnings();
        }

    }
}