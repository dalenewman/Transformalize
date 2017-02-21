#region license
// Transformalize
// Configurable Extract, Transform, and Load
// Copyright 2013-2017 Dale Newman
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
using System.Reflection;
using Cfg.Net;
using Transformalize.Context;
using Transformalize.Contracts;
using Transformalize.Extensions;
using Transformalize.Logging;
using Transformalize.Transforms;

namespace Transformalize.Configuration.Ext {
    public static class ProcessValidate {

        public static void Validate(this Process p, Action<string> error, Action<string> warn) {
            ValidateDuplicateEntities(p, error);
            ValidateDuplicateFields(p, error);
            ValidateRelationships(p, error, warn);
            ValidateEntityConnections(p, error);
            ValidateEntityMeetsProviderExpectations(p, error);
            ValidateEntityFilterMaps(p, error);
            ValidateActionConnections(p, error);
            ValidateTemplateActionConnections(p, error);
            ValidateTransformConnections(p, error);
            ValidateMapConnections(p, error);
            ValidateMapTransforms(p, error);
            ValidateSearchTypes(p, error);
            ValidateTransforms(p, error);
            ValidateShouldRuns(p, error, warn);
            ValidateScripts(p, error);
            ValidateCalculatedFields(p, error);
            ValidateParameterMaps(p, error);
            ValidateDirectoryReaderHasAtLeastOneValidField(p, error);
            ValidateFlatten(p, error, warn);
        }

        private static void ValidateFlatten(Process p, Action<string> error, Action<string> warn) {
            if (!p.Flatten)
                return;

            if (p.Entities.Count < 2) {
                warn("To flatten, you must have at least 2 entities.");
            }
            if (p.Output() == null || !Constants.AdoProviderSet().Contains(p.Output().Provider)) {
                error($"To flatten, you must use an ADO based output provider, e.g. ({Constants.AdoProviderDomain})");
            }
        }

        private static void ValidateDirectoryReaderHasAtLeastOneValidField(Process process, Action<string> error) {

            var fieldNames = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase) {
                {"CreationTimeUtc","date"},
                {"DirectoryName","string"},
                {"Extension","string"},
                {"FullName","string"},
                {"LastWriteTimeUtc","datetime"},
                {"Length","long"},
                {"Name","string"}
            };

            if (process.Connections.All(c => c.Provider != "directory"))
                return;

            foreach (var entity in process.Entities.Where(e => process.Connections.First(c => c.Name == e.Connection).Provider == "directory").Where(entity => !entity.Fields.Where(f => f.Input).Any(f => fieldNames.ContainsKey(f.Name)))) {
                error($"The {entity.Alias} entity reads a directory listing. It needs at least one of these valid fields: {(string.Join(", ", fieldNames).Replace(", Name", ", or Name"))}.");
            }

            foreach (
                var field in process.Entities.Where(e => process.Connections.First(c => c.Name == e.Connection).Provider == "directory").SelectMany(e => e.Fields.Where(f => f.Input && fieldNames.ContainsKey(f.Name)))) {
                var type = fieldNames[field.Name];
                if (field.Type.StartsWith(type, StringComparison.OrdinalIgnoreCase)) {
                    continue;
                }
                if (type == "long" && field.Type.Equals("Int64", StringComparison.OrdinalIgnoreCase)) {
                    continue;
                }
                error($"The field named {field.Name} has an incompatible type of {field.Type}. The type should be {type}.");
            }


        }

        private static void ValidateParameterMaps(Process process, Action<string> error) {
            foreach (var parameter in process.Environments.SelectMany(e => e.Parameters.Where(p => p.Map != string.Empty)).Where(parameter => process.Maps.All(m => m.Name != parameter.Map))) {
                error($"A parameter refers to invalid map: {parameter.Map}.");
            }
        }

        static void ValidateCalculatedFields(Process p, Action<string> error) {
            foreach (var entity in p.Entities) {
                foreach (var field in entity.CalculatedFields.Where(f => !f.Produced)) {
                    var transform = field.Transforms.FirstOrDefault();
                    if (transform != null && Transform.TransformSet().Contains(transform.Method) && !transform.Parameters.Any()) {
                        if (Transform.TransformProducerSet().Contains(transform.Method)) {
                            continue;
                        }

                        // temporary, may have another class of transforms that don't require copy() parameters up front
                        if (transform.Method.In("iif", "geohashencode", "format", "eval"))
                            continue;

                        if (Transform.TransformSet().Contains(transform.Method) && !transform.Parameters.Any()) {
                            error($"The transform {transform.Method} in {entity.Alias}.{field.Alias} requires input.  If using short-hand, use copy().  Otherwise, set the parameter attribute, or define a parameters collection.");
                        }
                    }
                }
            }
        }

        static void ValidateScripts(Process process, Action<string> error) {
            var scriptsRegistered = process.Scripts.Select(s => s.Name);
            var scriptReferences = process.GetAllTransforms().Where(t => t.Scripts.Any()).SelectMany(t => t.Scripts).Select(s => s.Name).Distinct();
            var problems = scriptReferences.Except(scriptsRegistered).ToArray();
            if (problems.Length <= 0)
                return;
            foreach (var problem in problems) {
                error($"The {problem} script is not registered in the scripts collection.");
            }
        }

        static void ValidateMapTransforms(Process p, Action<string> error) {
            foreach (var transform in p.GetAllTransforms().Where(t => t.Method == "map")) {
                if (p.Maps.All(m => m.Name != transform.Map)) {
                    error($"A map transform references an invalid map: {transform.Map}.");
                }
                if (p.Maps.All(m => m.Name != transform.Map)) {
                    error($"The map {transform.Map} is invalid.");
                } else {
                    var map = p.Maps.First(m => m.Name == transform.Map);
                    foreach (var item in map.Items.Where(i => i.Parameter != string.Empty)) {
                        Field field;
                        if (!p.TryGetField(item.Parameter, out field)) {
                            error($"A map transform references an invalid field: {item.Parameter}.");
                        }
                    }
                }
            }
        }


        static void ValidateMapConnections(Process p, Action<string> error) {
            foreach (var map in p.Maps.Where(m => m.Query != string.Empty).Where(map => p.Connections.All(c => c.Name != map.Connection))) {
                error($"The {map.Name} map references an invalid connection: {map.Connection}.");
            }
        }
        static void ValidateTransformConnections(Process p, Action<string> error) {

            var methodsWithConnections = new[] { "mail", "run" };

            foreach (var transform in p.GetAllTransforms().Where(t => methodsWithConnections.Any(nc => nc == t.Method))) {
                var connection = p.Connections.FirstOrDefault(c => c.Name == transform.Connection);
                if (connection == null) {
                    error($"The {transform.Method} transform references an invalid connection: {transform.Connection}.");
                    continue;
                }

                switch (transform.Method) {
                    case "mail":
                        if (connection.Provider != "mail") {
                            error($"The {transform.Method} transform references the wrong type of connection: {connection.Provider}.");
                        }
                        break;
                }
            }
        }

        static void ValidateTemplateActionConnections(Process p, Action<string> error) {
            foreach (var action in p.Templates.SelectMany(template => template.Actions.Where(a => a.Connection != string.Empty).Where(action => p.Connections.All(c => c.Name != action.Connection)))) {
                error($"The {action.Type} template action references an invalid connection: {action.Connection}.");
            }
        }

        static void ValidateActionConnections(Process p, Action<string> error) {
            foreach (var action in p.Actions.Where(action => action.Connection != string.Empty).Where(action => p.Connections.All(c => c.Name != action.Connection))) {
                error($"The {action.Type} action references an invalid connection: {action.Connection}.");
            }
        }

        static void ValidateEntityConnections(Process p, Action<string> error) {
            foreach (var entity in p.Entities.Where(entity => p.Connections.All(c => c.Name != entity.Connection))) {
                error($"The {entity.Name} entity references an invalid connection: {entity.Connection}.");
            }
        }

        static void ValidateEntityMeetsProviderExpectations(Process p, Action<string> error) {
            foreach (var entity in p.Entities) {
                var connection = p.Output();
                if (connection != null) {
                    switch (connection.Provider) {
                        case "kml":
                        case "geojson":
                            var fields = entity.GetAllFields().ToArray();
                            var lat = fields.FirstOrDefault(f => f.Alias.ToLower() == "latitude") ?? fields.FirstOrDefault(f => f.Alias.ToLower().StartsWith("lat"));
                            if (lat == null) {
                                error($"The {entity.Alias} entity needs a latitude (or lat) output field in order to output {connection.Provider}.");
                            } else {
                                var lon = fields.FirstOrDefault(f => f.Alias.ToLower() == "longitude") ?? fields.FirstOrDefault(f => f.Alias.ToLower().StartsWith("lon"));
                                if (lon == null) {
                                    error($"The {entity.Alias} entity needs a longitude (or lon) output field in order to output {connection.Provider}.");
                                }
                            }
                            break;
                        default:
                            break;
                    }
                }
            }
        }

        static void ValidateEntityFilterMaps(Process p, Action<string> error) {
            foreach (var entity in p.Entities) {
                foreach (var filter in entity.Filter.Where(filter => !string.IsNullOrEmpty(filter.Map)).Where(filter => p.Maps.All(m => m.Name != filter.Map))) {
                    error($"The filter on field {filter.Field} in entity {entity.Alias} refers to invalid map: {filter.Map}");
                }
            }
        }

        static void ValidateRelationships(Process p, Action<string> error, Action<string> warn) {
            // count check
            if (p.Entities.Count > 1 && p.Relationships.Count + 1 < p.Entities.Count) {
                var message = $"You have {p.Entities.Count} entities so you need {p.Entities.Count - 1} relationships. You have {p.Relationships.Count} relationships.";
                if (p.Mode == "check") {
                    warn(message);
                } else {
                    error(message);
                }
            }

            //entity alias, name check, and if that passes, do field alias, name check
            foreach (var relationship in p.Relationships) {
                var problem = false;

                // validate (and modify) left side
                Entity leftEntity;
                if (p.TryGetEntity(relationship.LeftEntity, out leftEntity)) {
                    relationship.Summary.LeftEntity = leftEntity;
                    foreach (var leftField in relationship.GetLeftJoinFields()) {
                        Field field;
                        if (leftEntity.TryGetField(leftField, out field)) {
                            relationship.Summary.LeftFields.Add(field);
                        } else {
                            var message = $"A relationship references a left-field that doesn't exist: {leftField}";
                            if (p.Mode == "check") {
                                warn(message);
                            } else {
                                error(message);
                            }
                            problem = true;
                        }
                    }
                } else {
                    var message = $"A relationship references a left-entity that doesn't exist: {relationship.LeftEntity}";
                    if (p.Mode == "check") {
                        error(message);
                    } else {
                        warn(message);
                    }
                    problem = true;
                }

                //validate (and modify) right side
                Entity rightEntity;
                if (p.TryGetEntity(relationship.RightEntity, out rightEntity)) {
                    relationship.Summary.RightEntity = rightEntity;
                    foreach (var rightField in relationship.GetRightJoinFields()) {
                        Field field;
                        if (rightEntity.TryGetField(rightField, out field)) {
                            relationship.Summary.RightFields.Add(field);
                        } else {
                            var message = $"A relationship references a right-field that doesn't exist: {rightField}";
                            if (p.Mode == "check") {
                                warn(message);
                            } else {
                                error(message);
                            }
                            error(message);
                            problem = true;
                        }
                    }
                } else {
                    if (p.Mode == "check") {
                        warn($"A relationship references a right-entity that doesn't exist: {relationship.RightEntity}");
                    } else {
                        error($"A relationship references a right-entity that doesn't exist: {relationship.RightEntity}");
                    }
                    problem = true;
                }

                //if everything is cool, set the foreign key flags
                if (!problem && relationship.Summary.IsAligned()) {
                    for (var i = 0; i < relationship.Summary.LeftFields.Count; i++) {
                        var leftField = relationship.Summary.LeftFields[i];
                        var rightField = relationship.Summary.RightFields[i];

                        leftField.KeyType |= KeyType.Foreign;
                        if (!leftField.Output) {
                            warn($"Foreign key {leftField.Alias} on left side must be output. Overriding output to true.");
                            leftField.Output = true;
                        }

                        if (leftField.Type != rightField.Type) {
                            warn($"The {leftField.Alias} and {rightField.Alias} relationship fields do not have the same type.");
                        }
                    }
                }

            }

        }


        static void ValidateDuplicateFields(Process p, Action<string> error) {
            var fieldDuplicates = p.Entities
                .SelectMany(e => e.GetAllFields())
                .Where(f => f.Output && !f.PrimaryKey && !f.System)
                .Concat(p.CalculatedFields)
                .GroupBy(f => f.Alias.ToLower())
                .Where(group => @group.Count() > 1)
                .Select(group => @group.Key)
                .ToArray();
            foreach (var duplicate in fieldDuplicates) {
                error($"The entity field '{duplicate}' occurs more than once. Remove, alias, or prefix one.");
            }
        }


        static void ValidateDuplicateEntities(Process p, Action<string> error) {
            var entityDuplicates = p.Entities
                .GroupBy(e => e.Alias)
                .Where(group => @group.Count() > 1)
                .Select(group => @group.Key)
                .ToArray();
            foreach (var duplicate in entityDuplicates) {
                error($"The '{duplicate}' entity occurs more than once. Remove or alias one.");
            }
        }


        static void ValidateShouldRuns(Process p, Action<string> error, Action<string> warn) {
            foreach (var entity in p.Entities) {
                foreach (var field in entity.GetAllFields().Where(f => f.Transforms.Any(t => t.RunField != string.Empty))) {
                    foreach (var t in field.Transforms.Where(t => t.RunField != string.Empty)) {
                        Field runField;
                        if (entity.TryGetField(t.RunField, out runField)) {
                            var runValue = runField.Type == "bool" && t.RunValue == Constants.DefaultSetting ? "true" : t.RunValue;
                            try {
                                var value = Constants.ConversionMap[runField.Type](runValue);
                                t.ShouldRun = row => Utility.Evaluate(row[runField], t.RunOperator, value);
                            } catch (Exception ex) {
                                error($"Trouble converting {runValue} to {runField.Type}. {ex.Message}");
                            }
                        } else {
                            warn($"Run Field {t.RunField} does not exist in {entity.Alias}, so it will not be evaluated.");
                        }
                    }
                }
            }
        }

        static void ValidateSearchTypes(Process p, Action<string> error) {
            foreach (var name in p.GetAllFields().Where(f => !f.System).Select(f => f.SearchType).Distinct()) {
                if (name != "none" && p.SearchTypes.All(st => st.Name != name)) {
                    error($"Search type {name} is invalid. Add it to search types, or remove it from the field using it.");
                }
            }
        }

        static void ValidateTransform(IContext context, Transform lastTransform, Action<string> error) {

            var t = context.Transform;
            var fields = context.Process.ParametersToFields(context.Transform.Parameters, context.Field);
            var allFields = context.GetAllEntityFields().ToArray();
            var input = fields.First();

            // check input types
            switch (t.Method) {
                case "include":
                case "exclude":
                    if (input.Type == "byte[]") {
                        error($"The {t.Method} method doesn't work with byte arrays.");
                    }
                    if (input.Type != "string" && !Constants.CanConvert()[input.Type](t.Value)) {
                        error($"The {t.Method} method's value of {t.Value} can't be converted to a {input.Type} for comparison.");
                    }
                    break;
                case "add":
                case "sum":
                case "round":
                case "abs":
                case "ceiling":
                case "floor":
                case "multiply":
                    foreach (var f in fields) {
                        if (!f.IsNumeric()) {
                            error($" The {t.Method} method expects numeric input, but {input.Alias} is {input.Type}.");
                        }
                    }
                    break;
                case "timezone":
                case "datepart":
                case "next":
                case "timeago":
                case "datemath":
                case "timeahead":
                case "isdaylightsavings":
                case "last":
                    if (input.Type != "datetime") {
                        error($"The {t.Method} method expects a datetime input, but {input.Alias} is {input.Type}.");
                    }
                    break;
                case "datediff":
                    if (fields.All(f => !f.Type.StartsWith("date"))) {
                        error($"The {t.Method} method expects from one to two datetime parameters.");
                    }
                    break;
                case "invert":
                case "toyesno":
                    if (input.Type != "bool") {
                        error($"The {t.Method} expects a bool input, but {input.Alias} is {input.Type}.");
                    }
                    break;
                case "totime":
                    if (!input.IsNumeric()) {
                        error($"The {t.Method} expects a numeric input.");
                    }
                    if (!t.TimeComponent.In("hour", "minute", "second", "millisecond", "day", "tick")) {
                        error($"The {t.Method} expects a time component of day, hour, minute, second, millisecond, or tick.");
                    }
                    break;
                case "contains":
                case "decompress":
                case "htmldecode":
                case "htmlencode":
                case "insert":
                case "left":
                case "padleft":
                case "padright":
                case "regexreplace":
                case "remove":
                case "replace":
                case "right":
                case "splitlength":
                case "substring":
                case "tolower":
                case "lower":
                case "toupper":
                case "upper":
                case "trim":
                case "trimend":
                case "trimstart":
                case "xmldecode":
                case "fileext":
                case "filepath":
                case "match":
                case "filename":
                case "startswith":
                case "endswith":
                case "fromsplit":
                case "fromlengths":
                case "fromxml":
                case "formatxml":
                case "isempty":
                case "xpath":
                case "camelize":
                case "dehumanize":
                case "dasherize":
                case "hyphenate":
                case "pascalize":
                case "pluralize":
                case "singularize":
                case "titleize":
                case "frommetric":
                case "fromroman":
                case "geohashneighbor":
                case "fromaddress":
                case "underscore":
                case "vinisvalid":
                case "vingetworldmanufacturer":
                case "vingetmodelyear":
                    if (input.Type != "string") {
                        error($"The {t.Method} expects a string, but {input.Alias} is {input.Type}.");
                    }
                    break;

                case "tometric":
                case "toordinalwords":
                case "ordinalize":
                case "toroman":
                case "towords":
                    if (!input.IsNumeric()) {
                        error($"The {t.Method} expects a numeric input, but {input.Alias} is {input.Type}.");
                    }
                    break;
                case "humanize":
                    if (input.Type != "string" && !input.Type.StartsWith("date", StringComparison.OrdinalIgnoreCase)) {
                        error($"The {t.Method} expects a string or date, but {input.Alias} is {input.Type}.");
                    }
                    break;
                case "addticks":
                case "addmilliseconds":
                case "addseconds":
                case "addminutes":
                case "addhours":
                case "adddays":
                    if (!input.Type.StartsWith("date")) {
                        error($"The {t.Method} expects a date, but {input.Alias} is {input.Type}.");
                    }
                    break;
                case "tostring":
                    if (input.Type == "string") {
                        error($"The {t.Method} method is already a string.");
                    }
                    break;
            }

            // check parameters
            switch (t.Method) {
                case "shorthand":
                    if (string.IsNullOrEmpty(context.Transform.T)) {
                        error("The shorthand transform requires t attribute.");
                    }
                    break;
                case "tag":
                    if (string.IsNullOrEmpty(context.Transform.Tag)) {
                        error($"The {t.Method} transform requires a tag (e.g. a, span, div, etc).");
                    }
                    break;
                case "format":

                    if (t.Format == string.Empty) {
                        error("The format transform requires a format parameter.");
                    } else {
                        if (t.Format.IndexOf('{') == -1) {
                            error("The format transform's format must contain a curly braced place-holder.");
                        } else if (t.Format.IndexOf('}') == -1) {
                            error("The format transform's format must contain a curly braced place-holder.");
                        }
                    }
                    break;
                case "left":
                case "right":
                    if (t.Length == 0) {
                        error($"The {t.Method} transform requires a length parameter.");
                    }
                    break;
                case "copy":
                    if (t.Parameter == string.Empty && !t.Parameters.Any()) {
                        error("The copy transform requires at least one parameter.");
                    }
                    break;
                case "fromlengths":
                case "fromsplit":
                case "fromjson":
                case "fromxml":
                    if (!t.Parameters.Any()) {
                        error($"The {t.Method} transform requires a collection of output fields.");
                    }
                    if (t.Method == "fromsplit" && t.Separator == Constants.DefaultSetting) {
                        error("The fromsplit method requires a separator.");
                    }
                    if (t.Method == "fromlengths" && fields.Any(f => f.Length == "max")) {
                        error("The can not max length fields in a fromlengths transform. Set it to a numeric length.");
                    }
                    break;
                case "fromaddress":
                    if (t.Parameters.Any()) {
                        var lat = t.Parameters.FirstOrDefault(p => p.Name.ToLower().In("lat", "latitude"));
                        if (lat == null) {
                            error("The fromaddress (geocode) transform requires an output field named lat, or latitude.");
                        } else {
                            if (lat.Type != "double") {
                                error($"The goecode {lat.Name} field must be of type double.");
                            }
                        }
                        var lon = t.Parameters.FirstOrDefault(p => p.Name.ToLower().In("lon", "long", "longitude"));
                        if (lon == null) {
                            error("The fromaddress (geocode) transform requires an output field named lon, long, or longitude.");
                        } else {
                            if (lon.Type != "double") {
                                error($"The goecode {lon.Name} field must be of type double.");
                            }
                        }
                    } else {
                        error($"The {t.Method} (geocode) transform requires a collection of output fields; namely: latitude, longitude, and formattedaddress (optional).");
                    }
                    break;
                case "padleft":
                    if (t.TotalWidth == 0) {
                        error("The padleft transform requires total width.");
                    }
                    if (t.PaddingChar == default(char)) {
                        error("The padleft transform requires a padding character.");
                    }
                    break;
                case "padright":
                    if (t.TotalWidth == 0) {
                        error("The padright transform requires total width.");
                    }
                    if (t.PaddingChar == default(char)) {
                        error("The padright transform requires a padding character.");
                    }
                    break;
                case "map":
                    if (t.Map == string.Empty) {
                        error("The map method requires a map method");
                    }
                    break;
                case "splitlength":
                    if (t.Separator == Constants.DefaultSetting) {
                        error("The splitlength transform requires a separator.");
                    }
                    break;
                case "insert":
                case "remove":
                    if (input.Type != "string") {
                        error($"The {t.Method} only works on strings.  {input.Alias} is {input.Type}.");
                    }
                    if (t.StartIndex == 0) {
                        error($"The {t.Method} transform requires a start-index greater than 0.");
                    }
                    if (t.Method == "insert" && t.Value == string.Empty) {
                        error($"The {t.Method} transform requires a value.");
                    }
                    break;
                case "startswith":
                case "endswith":
                case "contains":
                    if (t.Value == Constants.DefaultSetting || t.Value == string.Empty) {
                        error($"The {t.Method} validator requires a value.");
                    }
                    break;
                case "is":
                    if (t.Type == Constants.DefaultSetting) {
                        error("The is validator requires a type.");
                    }
                    break;
                case "trimstart":
                case "trimend":
                case "trim":
                    if (t.TrimChars == string.Empty) {
                        error("The {t.Transform} transform requires trim-chars.");
                    }
                    break;
                case "join":
                    if (t.Separator == Constants.DefaultSetting) {
                        error($"The {t.Method} transform requires a separator.");
                    }
                    break;
                case "timezone":
                    if (input.Type != "datetime") {
                        error($"The {t.Method} expects a datetime input, but {input.Alias} is {input.Type}.");
                    }
                    if (t.FromTimeZone == Constants.DefaultSetting) {
                        error($"The {t.Method} transform requires from-time-zone to be set.");
                    }
                    if (t.ToTimeZone == Constants.DefaultSetting) {
                        error($"The {t.Method} transform requires to-time-zone to be set.");
                    }
                    break;
                case "replace":
                    if (t.OldValue == string.Empty) {
                        error($"The {t.Method} transform requires an old-value.");
                    }
                    break;
                case "match":
                case "regexreplace":
                    if (t.Pattern == string.Empty) {
                        error($"The {t.Method} transform requires a pattern.");
                    }
                    break;
                case "next":
                case "last":
                    if (string.IsNullOrEmpty(t.DayOfWeek)) {
                        error($"The {t.Method} transform requires a day-of-week.");
                    }
                    break;
                case "javascript":
                case "js":
                    if (t.Script == string.Empty) {
                        error($"The {t.Method} transform requires a script.");
                    }
                    break;
                case "velocity":
                case "razor":
                    if (t.Template == string.Empty) {
                        error($"The {t.Method} transform requires a template.");
                    }
                    if (t.ContentType == string.Empty) {
                        t.ContentType = "raw"; //other would be html
                    }
                    break;
                case "include":
                case "exclude":
                case "any":
                    if (string.IsNullOrEmpty(t.Operator)) {
                        error($"The {t.Method} transform requires an operator.");

                    }
                    if (string.IsNullOrEmpty(t.Value)) {
                        error($"The {t.Method} transform requires a value.");
                    }
                    break;
                case "connection":
                    if (string.IsNullOrEmpty(t.Name)) {
                        error("The connection transform requires a name.");

                    }
                    if (string.IsNullOrEmpty(t.Property)) {
                        error("The connection transform requires a property.");
                    }
#if NETS10
                    var props = typeof(Connection).GetRuntimeProperties().Where(prop => prop.GetCustomAttribute(typeof(CfgAttribute), true) != null).Select(prop => prop.Name).ToArray();
#else
                    var props = typeof(Connection).GetProperties().Where(prop => prop.GetCustomAttributes(typeof(CfgAttribute), true).FirstOrDefault() != null).Select(prop => prop.Name).ToArray();
#endif
                    if (!t.Property.In(props)) {
                        error($"The connection property {t.Property} is not allowed.  The allowed properties are {(string.Join(", ", props))}.");
                    }
                    break;
                case "xpath":
                    if (string.IsNullOrEmpty(t.XPath)) {
                        error("The xpath transform requires a xpath expression (or a the name of a field that contains an xpath expression).");
                    }
                    if (!string.IsNullOrEmpty(t.NameSpace) && string.IsNullOrEmpty(t.Url)) {
                        error("If you set a namespace, you must also set the url that references the name space.");
                    }
                    break;
                case "in":
                    if (string.IsNullOrEmpty(t.Domain)) {
                        error("The in transform requires a domain (a list of allowed values).");
                    }
                    break;
                case "addticks":
                    long addTicksLong;
                    if (!long.TryParse(t.Value, out addTicksLong)) {
                        error(
                            $"The addticks transform requires a long (int64) numeric parameter.  {t.Value} can not be parsed as a long.");
                    }
                    break;
                case "addmilliseconds":
                case "addseconds":
                case "addminutes":
                case "addhours":
                case "adddays":
                    double addXDouble;
                    if (!double.TryParse(t.Value, out addXDouble)) {
                        error($"The {t.Method} transform requires a double numeric parameter.  {t.Value} can not be parsed as a double.");
                    }
                    break;

                case "iif":
                    if (allFields.All(f => f.Alias != t.TrueField)) {
                        error($"The iif method's true portion: {t.TrueField}, is not a valid field.");
                    }
                    if (allFields.All(f => f.Alias != t.FalseField)) {
                        error($"The iif method's false portion: {t.FalseField}, is not a valid field.");
                    }
                    break;
                case "geohashencode":
                    CheckDouble(allFields, t, t.Latitude, "latitude", error);
                    CheckDouble(allFields, t, t.Longitude, "longitude", error);
                    if (t.Length < 1 || t.Length > 13) {
                        error("The GeohashEncode method's precision must be between 1 and 13.");
                    }
                    break;
                case "distance":
                    CheckDouble(allFields, t, t.FromLat, "from-lat", error);
                    CheckDouble(allFields, t, t.FromLon, "from-lon", error);
                    CheckDouble(allFields, t, t.ToLat, "to-lat", error);
                    CheckDouble(allFields, t, t.ToLon, "to-lon", error);
                    break;
                case "eval":
                case "datemath":
                    if (t.Expression == string.Empty) {
                        if (t.Method == "datemath")
                            error($"The {t.Method} method requires operators and/or rounding expression (e.g. +1d+2h/m).");
                        else
                            error($"The {t.Method} method requires an expression (e.g. field1 + field2).");
                    }
                    break;

            }

            // check output types
            if (context.Transform == lastTransform) {
                switch (lastTransform.Method) {
                    case "sum":
                    case "add":
                    case "round":
                    case "abs":
                    case "ceiling":
                    case "floor":
                    case "multiply":
                        if (!context.Field.IsNumeric()) {
                            error($"The {context.Field.Alias} field is a {context.Field.Type}, but it needs to be numeric to accept the output of the {lastTransform.Method} method.");
                        }
                        break;
                    case "convert":
                        if (lastTransform.Type != Constants.DefaultSetting && lastTransform.Type != context.Field.Type) {
                            error($"The {context.Field.Alias} field is a {context.Field.Type}, but your last transform is converting to a {lastTransform.Type}.");
                        }
                        break;
                    case "next":
                    case "last":
                    case "datemath":
                    case "timezone":
                        if (!context.Field.Type.StartsWith("date")) {
                            error($"The {lastTransform.Method} returns a date, but {context.Field.Alias} is a {context.Field.Type}.");
                        }
                        break;
                    case "datepart":
                        var returnType = DatePartTransform.PartReturns[lastTransform.TimeComponent];
                        if (returnType != context.Field.Type) {
                            error($"The {lastTransform.Method} returns a {returnType}, but {context.Field.Alias} is a {context.Field.Type}.");
                        }
                        break;
                    case "camelize":
                    case "humanize":
                    case "dehumanize":
                    case "dasherize":
                    case "hyphenate":
                    case "ordinalize":
                    case "pascalize":
                    case "pluralize":
                    case "singularize":
                    case "titleize":
                    case "tometric":
                    case "toordinalwords":
                    case "toroman":
                    case "towords":
                    case "commonprefix":
                    case "underscore":
                        if (context.Field.Type != "string") {
                            error($"The {lastTransform.Method} returns a string, but {context.Field.Alias} is a {context.Field.Type}.");
                        }
                        break;
                    case "vingetmodelyear":
                        if (!context.Field.Type.In("int", "int32")) {
                            error($"The {lastTransform.Method} returns an int, but {context.Field.Alias} is a {context.Field.Type}.");
                        }
                        break;
                    case "frommetric":
                    case "fromroman":
                        if (!context.Field.IsNumeric()) {
                            error($"The {lastTransform.Method} returns a numeric output, but {context.Field.Alias} is a {context.Field.Type}.");
                        }
                        break;

                    case "filename":
                    case "filepath":
                    case "fileext":
                    case "tag":
                    case "formatxml":
                    case "slugify":
                    case "geohashencode":
                    case "vingetworldmanufacturer":
                    case "totime":
                        if (context.Field.Type != "string") {
                            error($"The {lastTransform.Method} returns a string, but {context.Field.Alias} is a {context.Field.Type}.");
                        }
                        break;
                    case "equal":
                    case "equals":
                    case "startswith":
                    case "endswith":
                    case "invert":
                    case "is":
                    case "contains":
                    case "isempty":
                    case "isdefault":
                    case "in":
                    case "any":
                    case "vin.isvalid":
                    case "isvin":
                    case "isdaylightsavings":
                        if (!context.Field.Type.StartsWith("bool")) {
                            error($"The {lastTransform.Method} returns a bool, but {context.Field.Alias} is a {context.Field.Type}.");
                        }
                        break;
                    case "addticks":
                    case "addmilliseconds":
                    case "addseconds":
                    case "addminutes":
                    case "addhours":
                    case "adddays":
                        if (!context.Field.Type.StartsWith("date")) {
                            error($"The {lastTransform.Method} returns a date, but {context.Field.Alias} is {context.Field.Type}.");
                        }
                        break;

                }
            }
        }

        private static void CheckDouble(Field[] fields, Transform t, string valueOrField, string name, Action<string> error) {
            double doubleValue;
            if (fields.All(f => f.Alias != valueOrField) && !double.TryParse(valueOrField, out doubleValue)) {
                error($"The {t.Method} method's {name} parameter: {valueOrField}, is not a valid field or numeric value.");
            }
        }

        static void ValidateTransforms(Process p, Action<string> error) {

            foreach (var e in p.Entities) {
                foreach (var f in e.GetAllFields()) {
                    if (f.Transforms.Any()) {
                        var lastTransform = f.Transforms.Last();
                        foreach (var t in f.Transforms) {
                            var context = new PipelineContext(new NullLogger(), p, e, f, t);
                            ValidateTransform(context, lastTransform, error);
                        }
                    }
                }
            }
            foreach (var f in p.CalculatedFields) {
                if (f.Transforms.Any()) {
                    var lastTransform = f.Transforms.Last();
                    foreach (var t in f.Transforms) {
                        //TODO: Once the calculated columns are broken into their own virtual entity, replace null with it.
                        var context = new PipelineContext(new NullLogger(), p, null, f, t);
                        ValidateTransform(context, lastTransform, error);
                    }
                }
            }

        }

    }
}