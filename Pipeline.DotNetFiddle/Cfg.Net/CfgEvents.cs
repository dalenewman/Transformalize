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
using Cfg.Net.Loggers;
namespace Cfg.Net {

    internal sealed class CfgEvents {

        public DefaultLogger Logger { get; set; }

        public CfgEvents(DefaultLogger logger) {
            Logger = logger;
        }

        public void DuplicateSet(string uniqueAttribute, object value, string nodeName) {
            Logger.Error("Duplicate '{0}' value '{1}' in '{2}'.", uniqueAttribute, value, nodeName);
        }

        public void InvalidAttribute(string parentName, string nodeName, string attributeName, string validateAttributes) {
            Logger.Error("A{3} '{0}' '{1}' element contains an invalid '{2}' attribute.  Valid attributes are: {4}.", parentName, nodeName, attributeName,
                Suffix(parentName), validateAttributes);
        }

        public void InvalidElement(string nodeName, string subNodeName) {
            Logger.Error("A{2} '{0}' element has an invalid '{1}' element.  If you need a{2} '{1}' element, decorate it with the Cfg[()] attribute in your Cfg-NET model.", nodeName, subNodeName, Suffix(nodeName));
        }

        public void InvalidNestedElement(string parentName, string nodeName, string subNodeName) {
            Logger.Error("A{3} '{0}' '{1}' element has an invalid '{2}' element.", parentName, nodeName, subNodeName, Suffix(parentName));
        }

        public void MissingAttribute(string parentName, string nodeName, string attributeName) {
            Logger.Error("A{3} '{0}' '{1}' element is missing a '{2}' attribute.", parentName, nodeName, attributeName, Suffix(parentName));
        }

        public void MissingElement(string nodeName, string elementName) {
            Logger.Error("The {0} element is missing a{2} '{1}' element.", nodeName == string.Empty ? "root" : "'" + nodeName + "'",
                elementName, Suffix(elementName));
        }

        public void MissingAddElement(string elementName) {
            Logger.Error("A{1} '{0}' element is missing a child element.", elementName, Suffix(elementName));
        }

        public void MissingValidator(string nodeName, string validatorName) {
            Logger.Warn("A '{0}' attribute can not find the {1} validator.  Please make it's passed in to your Cfg-Net root constructor.", nodeName, validatorName);
        }

        public void ValidatorException(string validatorName, Exception ex, object value) {
            Logger.Error("The '{0}' validator threw an exception when validating the value '{2}'. {1}", validatorName, ex.Message, value);
        }

        public void MissingNestedElement(string parentName, string nodeName, string elementName) {
            Logger.Error("A{3} '{0}' '{1}' element is missing a{4} '{2}' element.", parentName, nodeName, elementName,
                Suffix(parentName), Suffix(elementName));
        }

        public void MissingRequiredAdd(string parent, string key) {
            Logger.Error($"A{Suffix(key)} '{key}' element with at least one item is required{(string.IsNullOrEmpty(parent) ? "." : " in " + parent + ".")}");
        }

        public void SettingValue(string propertyName, object value, string parentName, string nodeName, string message) {
            Logger.Error("Could not set '{0}' to '{1}' inside '{2}' '{3}'. {4}", propertyName, value, parentName, nodeName, message);
        }

        public void UnexpectedElement(string elementName, string subNodeName) {
            Logger.Error("Invalid element {0} in {1}.  Only 'add' elements are allowed here.", subNodeName, elementName);
        }

        public void ShorthandNotLoaded(string parentName, string nodeName, string attributeName) {
            Logger.Error("A{3} '{0}' '{1}' element's '{2}' attribute needs a shorthand configuration, but none was loaded.", parentName, nodeName, attributeName, Suffix(parentName));
        }

        public void ValueNotInDomain(string name, object value, string validValues) {
            Logger.Error("An invalid value of '{0}' is in the '{1}' attribute.  The valid domain is: {2}.", value, name, validValues);
        }

        public void ParseException(string message) {
            Logger.Error("Could not parse the configuration. {0}", message);
        }

        private static string Suffix(string thing) {
            return thing == null || IsVowel(thing[0]) ? "n" : string.Empty;
        }

        public void Error(string problem, params object[] args) {
            Logger.Error(problem, args);
        }

        public void Warning(string problem, params object[] args) {
            Logger.Warn(problem, args);
        }

        private static bool IsVowel(char c) {
            return c == 'a' || c == 'e' || c == 'i' || c == 'o' || c == 'u' || c == 'A' || c == 'E' || c == 'I' ||
                   c == 'O' || c == 'U';
        }

        public void OnlyOneAttributeAllowed(string parentName, string name, int count) {
            Logger.Error("The '{0}' '{1}' collection does not inherit from CfgNode or implement IProperties, so you can only have one value (attribute).  You have {2} defined.", parentName, name, count);
        }

        public void ConstructorNotFound(string parentName, string name) {
            Logger.Error("The '{0}' '{1}' collection implementing IProperties has an incompatible constructor.  Cfg-Net needs a constructorless or single parameter constructor.  The single parameter may be a string[] of names, or an integer representing capacity.", parentName, name);
        }

        public void TypeMismatch(string key, object value, Type propertyType) {
            Logger.Error("The '{0}' attribute default value '{1}' does not have the same type as the property type of '{2}'.", key, value, propertyType);
        }

        public void ValueTooShort(string name, string value, int minLength) {
            Logger.Error("The '{0}' attribute value '{1}' is too short. It is {3} characters. It must be at least {2} characters.", name, value, minLength, value.Length);
        }

        public void ValueTooLong(string name, string value, int maxLength) {
            Logger.Error("The '{0}' attribute value '{1}' is too long. It is {3} characters. It must not exceed {2} characters.", name, value, maxLength, value.Length);
        }

        public void ValueIsNotComparable(string name, object value) {
            Logger.Error("The '{0}' attribute value '{1}' is not comparable.  Having a minValue or maxValue set on an incomparable property type causes this.", name, value);
        }

        public void ValueTooSmall(string name, object value, object minValue) {
            Logger.Error("The '{0}' attribute value '{1}' is too small. The minimum value allowed is '{2}'.", name, value, minValue);
        }

        public void ValueTooBig(string name, object value, object maxValue) {
            Logger.Error("The '{0}' attribute value '{1}' is too big. The maximum value allowed is '{2}'.", name, value, maxValue);
        }

        public string[] Errors() {
            return Logger.Errors();
        }

        public string[] Warnings() {
            return Logger.Warnings();
        }

    }
}