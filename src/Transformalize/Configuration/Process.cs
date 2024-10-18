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
using Cfg.Net;
using Cfg.Net.Contracts;
using Cfg.Net.Ext;
using Cfg.Net.Serializers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Transformalize.Configuration.Ext;
using Transformalize.Context;
using Transformalize.Impl;
using Transformalize.Logging;

namespace Transformalize.Configuration {

   [Cfg(name = "cfg")]
   public class Process : CfgNode, IDisposable {

      private static readonly Dictionary<string, IEnumerable<Field>> _fieldMatchCache = new Dictionary<string, IEnumerable<Field>>();

      /// <summary>
      /// The default shorthand configuration for children processes
      /// </summary>
      [Cfg(value = "Shorthand.xml")]
      public string Shorthand { get; set; }


      private string _name;

      [Cfg(value = "")]
      public string Request { get; set; }

      [Cfg(value = (short)0)]
      public short Status { get; set; }

      [Cfg(value = "OK")]
      public string Message { get; set; }

      [Cfg(value = (long)0)]
      public long Time { get; set; }

      [Cfg]
      public List<LogEntry> Log { get; set; }

      /// <summary>
      /// Constucts and Loads a Process with external parameters and dependendencies
      /// </summary>
      /// <param name="cfg">xml, json, file name, or web address of an arrangement</param>
      /// <param name="parameters">parameters may be used in place-holders throughout the process</param>
      /// <param name="dependencies">Cfg-Net dependencies</param>
      public Process(
          string cfg,
          IDictionary<string, string> parameters,
          params IDependency[] dependencies
          )
      : base(dependencies) {
         Load(cfg, parameters);
      }

      /// <summary>
      /// Constucts and Loads a Process with dependendencies
      /// </summary>
      /// <param name="cfg">xml, json, file name, or web address of an arrangement</param>
      /// <param name="dependencies">Cfg-Net dependencies</param>
      public Process(string cfg, params IDependency[] dependencies) : this(cfg, null, dependencies) { }

      /// <summary>
      /// Constucts and Process with dependendencies
      /// </summary>
      /// <param name="dependencies">Cfg-Net dependencies</param>
      public Process(params IDependency[] dependencies) : base(dependencies) { }

      /// <summary>
      /// Constucts and Process without external dependencies
      /// </summary>
      public Process() : base(new XmlSerializer()) { }

      /// <summary>
      /// A name (of your choosing) to identify the process.
      /// </summary>
      [Cfg(value = "", required = true, unique = true)]
      public string Name {
         get => _name;
         set {
            _name = value;
            if (value == null)
               return;
            if (LogLimit == 0) {
               LogLimit = value.Length;
            }
         }
      }

      [Cfg(value = "")]
      public string Version { get; set; }

      /// <summary>
      /// Optional.
      ///
      /// `True` by default.
      /// 
      /// Indicates the process is enabled.  The included executable (e.g. `tfl.exe`) 
      /// respects this setting and does not run the process if disabled (or `False`).
      /// </summary>
      [Cfg(value = true)]
      public bool Enabled { get; set; }

      /// <summary>
      /// Optional. 
      /// 
      /// A mode reflects the intent of running the process.
      ///  
      /// * `init` wipes everything out
      /// * `validate` just loads, validates the configuration
      /// * <strong>`default`</strong> moves data through the pipeline, from input to output.
      /// 
      /// Aside from these, you may use any mode (of your choosing).  Then, you can control
      /// whether or not templates and/or actions run by setting their modes.
      /// </summary>
      [Cfg(value = "", toLower = true, trim = true)]
      public string Mode { get; set; }

      /// <summary>
      /// Optional.
      /// 
      /// A choice between `defer`, `linq`, and `parallel.linq`.
      /// 
      /// The default `defer` defers this decision to the entity's Pipeline setting.
      /// </summary>
      [Cfg(value = "defer", domain = "defer,linq,parallel.linq", toLower = true)]
      public string Pipeline { get; set; }

      [Cfg(value = "Star")]
      public string StarSuffix { get; set; }

      /// <summary>
      /// Optional (false)
      /// 
      /// If set to true, a <see cref="FlatSuffix"/> table is created with the same structure as the <see cref="Star"/> view, and all the data is copied into it.
      /// </summary>
      [Cfg(value = false)]
      public bool Flatten { get; set; }

      [Cfg(value = "Flat")]
      public string FlatSuffix { get; set; }

      /// <summary>
      /// Optional.
      /// 
      /// Choices are `html` and <strong>`raw`</strong>.
      /// 
      /// This refers to the razor templating engine's content type.  If you're rendering HTML 
      /// markup, use `html`, if not, using `raw` may inprove performance.
      /// </summary>
      [Cfg(value = "raw", domain = "raw,html", toLower = true)]
      public string TemplateContentType { get; set; }

      /// <summary>
      /// Optional.
      /// 
      /// Indicates the data's time zone.
      /// 
      /// It is used as the `to-time-zone` setting for `now()` and `timezone()` transformations
      /// if the `to-time-zone` is not set.
      /// 
      /// NOTE: Normally, you should keep the dates in UTC until presented to the user. 
      /// Then, have the client application convert UTC to the user's time zone.
      /// </summary>
      [Cfg(value = "")]
      public string TimeZone { get; set; }

      /// <summary>
      /// A collection of [Actions](/action)
      /// </summary>
      [Cfg]
      public List<Action> Actions { get; set; }

      /// <summary>
      /// A collection of [Calculated Fields](/calculated-field)
      /// </summary>
      [Cfg]
      public List<Field> CalculatedFields { get; set; }

      /// <summary>
      /// A collection of [Connections](/connection)
      /// </summary>
      [Cfg(required = false)]
      public List<Connection> Connections { get; set; }

      /// <summary>
      /// A collection of [Entities](/entity)
      /// </summary>
      [Cfg(required = false)]
      public List<Entity> Entities { get; set; }

      /// <summary>
      /// A collection of [Maps](/map)
      /// </summary>
      [Cfg]
      public List<Map> Maps { get; set; }

      /// <summary>
      /// A collection of [Relationships](/relationship)
      /// </summary>
      [Cfg]
      public List<Relationship> Relationships { get; set; }

      /// <summary>
      /// A collection of [Scripts](/script)
      /// </summary>
      [Cfg]
      public List<Script> Scripts { get; set; }

      /// <summary>
      /// A collection of [Search Types](/search-type)
      /// </summary>
      [Cfg]
      public List<SearchType> SearchTypes { get; set; }

      /// <summary>
      /// A collection of [Templates](/template)
      /// </summary>
      [Cfg]
      public List<Template> Templates { get; set; }

      [Cfg]
      public List<Parameter> Parameters { get; set; }

      protected override void PreValidate() {
         this.PreValidate(e => Error(e), w => Warn(w));
      }

      public void ModifyLogLimits() {
         var entitiesAny = Entities.Any();
         var fieldsAny = GetAllFields().Any(f => f.Transforms.Any());
         var transformsAny = GetAllTransforms().Any();

         LogLimit = Name.Length;
         EntityLogLimit = entitiesAny ? Entities.Select(e => e.Alias.Length).Max() : 10;
         FieldLogLimit = fieldsAny ? GetAllFields().Where(f => f.Transforms.Any()).Select(f => f.Alias.Length).Max() : 10;
      }

      public void ModifyKeys() {

         // entities
         foreach (var entity in Entities) {
            if (Name != null && entity.Alias != null) {
               entity.Key = Name + entity.Alias;
            } else {
               entity.Key = entity.GetHashCode().ToString();
            }

            foreach (var field in entity.GetAllFields()) {
               foreach (var transform in field.Transforms) {
                  transform.Key = Name + entity.Alias + field.Alias + transform.Method;
               }
            }

         }

         // templates
         foreach (var template in Templates) {
            if (Name != null && template.Name != null) {
               template.Key = Name + template.Name;
            } else {
               template.Key = template.GetHashCode().ToString();
            }
         }

         // actions do not have unique names, so they get a counter too
         var actionIndex = 0;
         foreach (var action in Actions) {
            if (action.Key != null && action.Type != null) {
               action.Key = action.Key + action.Type + ++actionIndex;
            } else {
               action.Key = action.GetHashCode().ToString();
            }
         }

         // connections
         foreach (var connection in Connections) {
            if (Name != null && connection.Name != null) {
               connection.Key = Name + connection.Name;
            } else {
               connection.Key = connection.GetHashCode().ToString();
            }
         }
      }

      /// <summary>
      /// Log limits, set by ModifyLogLimits
      /// </summary>
      public int LogLimit { get; set; }
      public int EntityLogLimit { get; set; }
      public int FieldLogLimit { get; set; }

      protected override void Validate() {
         this.Validate(e => Error(e), w => Warn(w));
      }

      public List<Field> ParametersToFields(IEnumerable<Parameter> parameters, Field defaultField) {

         if (defaultField == null) {
            throw new ArgumentNullException(nameof(defaultField));
         }

         var fields = parameters
             .Where(p => p.IsField(this))
             .Select(p => p.AsField(this))
             .Distinct()
             .ToList();

         if (!fields.Any()) {
            fields.Add(defaultField);
         }
         return fields;
      }

      protected override void PostValidate() {

         ModifyKeys();

         if (Errors().Length != 0)
            return;

         if (Entities.Any()) {
            Entities.First().IsMaster = true;
         }

         if (Mode.StartsWith("@")) {
            Warn($"Mode is {Mode}.");
         }

         ModifyLogLimits();
         ModifyRelationshipToMaster();
         ModifyIndexes();

         // create entity field's matcher
         foreach (var entity in Entities) {
            var pattern = @"\b" + string.Join(@"\b|\b", entity.GetAllFields().Where(f => !f.System).OrderByDescending(f => f.Alias.Length).Select(f => f.Alias)) + @"\b";
#if NETS10
            entity.FieldMatcher = new Regex(pattern);
#else
            entity.FieldMatcher = new Regex(pattern, RegexOptions.Compiled);
#endif

            // more efficient post-back strategy for form validation
            if (Mode == "form") {

               var fields = entity.Fields.Where(f => f.Input).ToArray();

               if (fields.Any()) {

                  foreach (var field in fields.Where(f => f.PostBack == "auto").ToArray()) {

                     field.PostBack = field.Validators.Any() ? "true" : "false";

                     if (field.Map == string.Empty)
                        continue;

                     var map = Maps.FirstOrDefault(m => m.Name == field.Map);
                     if (map == null)
                        continue;

                     if (!map.Query.Contains("@"))
                        continue;

                     // it is possible for this map to affect other field's post-back setting
                     foreach (var p in new ParameterFinder().Find(map.Query).Distinct()) {
                        var parameterField = fields.FirstOrDefault(f => f.Name == p || f.Alias == p);
                        if (parameterField != null) {
                           parameterField.PostBack = "true";
                        }
                     }
                  }

                  foreach (var field in fields.Where(f => f.PostBack == "auto").ToArray()) {
                     field.PostBack = "false";
                  }
               }

            }
         }

      }

      public void ModifyIndexes() {
         for (short i = 0; i < Entities.Count; i++) {

            var context = new PipelineContext(new NullLogger(), this, Entities[i]);
            context.Entity.Index = i;

            foreach (var field in context.Entity.GetAllFields()) {
               field.EntityIndex = i;
            }

            if (!context.Entity.IsMaster)
               continue;

            // set the master indexes
            short masterIndex = -1;
            var fields = context.GetAllEntityFields().ToArray();
            foreach (var field in fields) {
               field.MasterIndex = ++masterIndex;
            }

            // set the process calculated fields starting where master entity fields left off
            if (!CalculatedFields.Any())
               continue;

            var index = fields.Where(f => f.Index < short.MaxValue).Select(f => f.Index).Max();
            foreach (var field in CalculatedFields) {
               field.Index = ++index;
            }
         }

         foreach (var field in GetAllFields()) {
            var tCount = 0;
            foreach (var transform in field.Transforms) {
               transform.Index = tCount++;
            }
         }

      }

      void ModifyRelationshipToMaster() {
         foreach (var entity in Entities) {
            entity.RelationshipToMaster = ReadRelationshipToMaster(entity);
         }
      }

      IEnumerable<Relationship> ReadRelationshipToMaster(Entity entity) {

         if (entity.IsMaster)
            return new List<Relationship>();

         var relationships = Relationships.Where(r => r.Summary.RightEntity.Equals(entity)).ToList();

         if (relationships.Any() && !relationships.Any(r => r.Summary.LeftEntity.IsMaster)) {
            var leftEntity = relationships.Last().Summary.LeftEntity;
            relationships.AddRange(ReadRelationshipToMaster(leftEntity));
         }
         return relationships;
      }

      public IEnumerable<Operation> GetAllTransforms() {
         var transforms = Entities.SelectMany(entity => entity.GetAllTransforms()).ToList();
         transforms.AddRange(CalculatedFields.SelectMany(field => field.Transforms));
         return transforms;
      }

      public Entity GetEntity(string nameOrAlias) {
         var entity = Entities.FirstOrDefault(e => e.Alias == nameOrAlias);
         return entity ?? Entities.FirstOrDefault(e => e.Name != e.Alias && e.Name == nameOrAlias);
      }

      public bool TryGetEntity(string nameOrAlias, out Entity entity) {
         entity = GetEntity(nameOrAlias);
         return entity != null;
      }

      public IEnumerable<Field> GetAllFields() {
         var fields = new List<Field>();
         foreach (var e in Entities) {
            fields.AddRange(e.GetAllFields());
         }
         fields.AddRange(CalculatedFields);
         return fields;
      }

      public bool HasMultivaluedSearchType() {
         return GetAllFields().Select(f => f.SearchType).Distinct().Any(st => SearchTypes.First(s => s.Name == st).MultiValued);
      }

      public IEnumerable<Field> GetSearchFields() {
         var fields = new List<Field>();
         var starFields = GetStarFields().ToArray();

         fields.AddRange(starFields[0].Where(f => f.SearchType != "none"));
         fields.AddRange(starFields[1].Where(f => f.SearchType != "none"));
         return fields;
      }

      public IEnumerable<List<Field>> GetStarFields() {
         const int master = 0;
         const int other = 1;

         var starFields = new List<Field>[2];

         starFields[master] = new List<Field>();
         starFields[other] = new List<Field>();

         foreach (var entity in Entities) {
            if (entity.IsMaster) {
               starFields[master].AddRange(new PipelineContext(new NullLogger(), this, entity).GetAllEntityOutputFields());
            } else {
               starFields[other].AddRange(entity.GetAllOutputFields().Where(f => f.KeyType == KeyType.None && !f.System && f.Type != "byte[]"));
            }
         }
         return starFields;
      }

      public Connection GetOutputConnection() {
         return Connections.FirstOrDefault(c => c.Name == Output);
      }

      /// <summary>
      /// References the name of the output connection for this process.  All entities are output to this connection.
      /// </summary>
      [Cfg(value = "output", toLower = true)]
      public string Output { get; set; }

      /// <summary>
      /// clone process, remove entities, and create entity needed for calculated fields
      /// </summary>
      /// <returns>A made-up process that represents the denormalized output's fields that contribute to calculated fields</returns>
      public Process ToCalculatedFieldsProcess() {

         // clone process, remove entities, and create entity needed for calculated fields
         var calc = this.Clone();
         calc.LogLimit = LogLimit;
         calc.EntityLogLimit = EntityLogLimit;
         calc.FieldLogLimit = FieldLogLimit;

         calc.Entities.Clear();
         calc.CalculatedFields.Clear();
         calc.Relationships.Clear();

         var entity = new Entity { Name = "Calculated" };
         entity.Alias = entity.Name;
         entity.Key = calc.Name + entity.Alias;
         entity.Input = calc.Output;
         entity.Fields.Add(new Field {
            Name = Constants.TflKey,
            Alias = Constants.TflKey,
            PrimaryKey = true,
            System = true,
            Input = true,
            Type = "int"
         });

         // Add fields that calculated fields depend on
         entity.Fields.AddRange(CalculatedFields
             .SelectMany(f => f.Transforms)
             .SelectMany(t => t.Parameters)
             .Where(p => !p.HasValue() && p.IsField(this))
             .Select(p => p.AsField(this).Clone())
             .Where(f => f.Output)
             .Distinct()
             .Except(CalculatedFields)
         );

         var mapFields = CalculatedFields
             .SelectMany(cf => cf.Transforms)
             .Where(t => t.Method == "map")
             .Select(t => Maps.First(m => m.Name == t.Map))
             .SelectMany(m => m.Items)
             .Where(i => i.Parameter != string.Empty)
             .Select(i => i.AsParameter().AsField(this))
             .Distinct()
             .Except(entity.Fields)
             .Select(f => f.Clone());

         entity.Fields.AddRange(mapFields);

         entity.CalculatedFields.AddRange(CalculatedFields.Select(cf => cf.Clone()));
         foreach (var parameter in entity.GetAllFields().SelectMany(f => f.Transforms).SelectMany(t => t.Parameters)) {
            parameter.Entity = string.Empty;
         }

         if (CalculatedFields.Any()) {

            Regex matcher;
            var regex = @"\b" + string.Join(@"\b|\b", GetAllFields().Where(f => !f.System).OrderByDescending(f => f.Alias.Length).Select(f => f.Alias)) + @"\b";
#if NETS10
            matcher = new Regex(regex);
#else
            matcher = new Regex(regex, RegexOptions.Compiled);
#endif
            var key = Name;

            if (!_fieldMatchCache.ContainsKey(key)) {

               // get the fields necessary for the calculated fields process
               // in format's format attr
               // in eval's expression attr
               // in cs, js, csscript, and jint's script attr
               // in copy's value attr

               var found = new List<Field>();

               foreach (var cf in entity.CalculatedFields) {

                  foreach (Match match in matcher.Matches(cf.T)) {
                     if (TryGetField(match.Value, out var field)) {
                        if (field.Output) {
                           found.Add(field.Clone());
                        }
                     }
                  }
                  foreach (var t in cf.Transforms) {
                     foreach (Match match in matcher.Matches(string.Join(" ", t.Value, t.OldValue, t.NewValue, t.Format, t.Script, t.Expression, t.Template))) {
                        if (TryGetField(match.Value, out var field)) {
                           if (field.Output) {
                              found.Add(field.Clone());
                           }
                        }
                     }
                  }
               }
               _fieldMatchCache[key] = found.Distinct().Except(entity.Fields).ToList();
            }

            if (_fieldMatchCache[key].Any()) {
               entity.Fields.AddRange(_fieldMatchCache[key]);
            }
         }

         // create entity field's matcher
         var pattern = @"\b" + string.Join(@"\b|\b", entity.GetAllFields().Where(f => !f.System).OrderByDescending(f => f.Alias.Length).Select(f => f.Alias)) + @"\b";
#if NETS10
         entity.FieldMatcher = new Regex(pattern);
#else
         entity.FieldMatcher = new Regex(pattern, RegexOptions.Compiled);
#endif

         foreach (var field in entity.Fields) {
            field.Source = Utility.GetExcelName(field.EntityIndex) + "." + field.FieldName();
         }

         foreach (var field in entity.CalculatedFields) {
            field.Source = Utility.GetExcelName(field.EntityIndex) + "." + field.FieldName();
         }

         entity.ModifyIndexes();
         calc.Entities.Add(entity);
         calc.ModifyKeys();
         calc.ModifyIndexes();

         return calc;
      }

      public bool TryGetField(string aliasOrName, out Field field) {
         foreach (var f in Entities.Select(entity => entity.GetField(aliasOrName)).Where(f => f != null)) {
            field = f;
            return true;
         }
         field = null;
         return false;
      }

      /// <summary>
      /// this method gets a field or calculated field by alias or name anywhere in the process
      /// </summary>
      /// <param name="aliasOrName">an alias or name of the field</param>
      /// <returns>a field or null if not found</returns>
      public Field GetField(string aliasOrName) {
         if (TryGetField(aliasOrName, out Field field)) {
            return field;
         }
         return null;
      }

      [Cfg(value = "")]
      public string Description { get; set; }

      [Cfg(value = "")]
      public string MaxMemory { get; set; }

      [Cfg]
      public List<Schedule> Schedule { get; set; }

      public bool Preserve { get; set; }

      [Cfg]
      public List<CfgRow> Rows { get; set; }

      /// <summary>
      /// This makes it so Transformalize doesn't:
      /// - Produce system fields for the output
      /// - Consider updating the output
      /// </summary>
      [Cfg(value = false)]
      public bool ReadOnly { get; set; }

      /// <summary>
      /// An optional Id (used in Orchard CMS module)
      /// </summary>
      [Cfg(value = (long)0)]
      public long Id { get; set; }

      public void Dispose() {
         if (Preserve)
            return;

         Log?.Clear();
         Entities?.Clear();
         Actions?.Clear();
         CalculatedFields?.Clear();
         Connections?.Clear();
         Maps?.Clear();
         Relationships?.Clear();
         Scripts?.Clear();
         SearchTypes?.Clear();
         Templates?.Clear();
      }

      public bool OutputIsRelational() {
         return GetOutputConnection() != null && Constants.AdoProviderSet().Contains(GetOutputConnection().Provider);
      }

      public bool OutputIsConsole() {
         // check if this is a master job with actions, and no real entities
         if (Actions.Count > 0 && Entities.Count == 0) {
            return false;
         }
         return Connections.Any(c => c.Name == Constants.OriginalOutput && c.Provider == "console" || c.Name == Output && c.Provider == "console");
      }


   }
}