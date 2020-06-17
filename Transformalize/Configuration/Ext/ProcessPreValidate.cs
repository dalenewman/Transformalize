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
using System;
using System.Collections.Generic;
using System.Linq;
using Transformalize.Extensions;

namespace Transformalize.Configuration.Ext {
   public static class ProcessPreValidate {
      private const string All = "*";

      public static void PreValidate(this Process p, Action<string> error, Action<string> warn) {

         // process-level calculated fields are not input
         foreach (var calculatedField in p.CalculatedFields) {
            calculatedField.Input = false;
            calculatedField.IsCalculated = true;
         }

         AddDefaultDelimiters(p);

         // add internal input if nothing specified
         if (!p.Connections.Any()) {
            p.Connections.Add(new Connection { Name = "input", Provider = "internal" });
         }

         // add an internal output
         if (p.Connections.All(c => c.Name != p.Output)) {
            p.Connections.Add(new Connection { Name = p.Output, Provider = "internal" });
         }

         DefaultEntityConnections(p);
         DefaultSearchTypes(p);
         DefaultFileInspection(p);

         foreach (var entity in p.Entities) {
            try {
               entity.AdaptFieldsCreatedFromTransforms();
            } catch (Exception ex) {
               error($"Trouble adapting fields created from transforms. {ex.Message}");
            }

            if (!p.ReadOnly) {
               entity.AddSystemFields();
               entity.ModifyMissingPrimaryKey();
            }

            entity.ModifyIndexes();
         }


         // possible candidates for PostValidate
         AutomaticMaps(p);
         SetPrimaryKeys(p);

         var output = p.GetOutputConnection();

         // force primary key to output if not internal
         if (output.Provider != "internal") {
            foreach (var field in p.Entities.SelectMany(entity => p.GetAllFields().Where(field => field.PrimaryKey && !field.Output))) {
               warn($"Primary Keys must be output. Overriding output to true for {field.Alias}.");
               field.Output = true;
            }
         }

         // verify entities have level and message field for log output
         if (output.Provider == "log") {
            foreach (var fields in p.Entities.Select(entity => entity.GetAllFields().ToArray())) {
               if (!fields.Any(f => f.Alias.Equals("message", StringComparison.OrdinalIgnoreCase))) {
                  error("Log output requires a message field");
               }
               if (!fields.Any(f => f.Alias.Equals("level", StringComparison.OrdinalIgnoreCase))) {
                  error("Log output requires a level field");
               }
            }
         }

         // for referencing a script file directly in an entity.
         // note: the purpose of an entity script is to be used to create the query for the entity
         // note: not to be confused with transform scripts (i.e. jint, cs).
         if (!p.Scripts.Any()) {
            foreach (var entity in p.Entities) {
               var script = entity.Script;
               if (script.Contains(".")) {
                  p.Scripts.Add(new Script { Name = script, File = script });
               }
            }
         }

      }

      /// <summary>
      /// If in meta mode with file connections, setup delimiters
      /// If no entity is available, create one for the file connection
      /// </summary>
      /// <param name="p"></param>
      private static void DefaultFileInspection(Process p) {
         if (p.Connections.All(c => c.Provider != "file"))
            return;

         if (p.Entities.Any())
            return;

         var connection = (p.Connections.FirstOrDefault(cn => cn.Provider == "file" && cn.Name == "input") ?? p.Connections.First(cn => cn.Provider == "file"));
         p.Entities.Add(
             new Entity {
                Name = connection.Name,
                Alias = connection.Name,
                Input = connection.Name,
             }
         );
      }

      private static void AddDefaultDelimiters(Process p) {
         foreach (var connection in p.Connections.Where(c => c.Provider == "file" && c.Delimiter == string.Empty && !c.Delimiters.Any())) {
            connection.Delimiters.Add(new Delimiter { Name = "comma", Character = ',' });
            connection.Delimiters.Add(new Delimiter { Name = "tab", Character = '\t' });
            connection.Delimiters.Add(new Delimiter { Name = "pipe", Character = '|' });
            connection.Delimiters.Add(new Delimiter { Name = "semicolon", Character = ';' });
            //Delimiters.Add(new Delimiter { Name = "unit", Character = Convert.ToChar(31) });
         }
      }

      private static Map CreateMap(string inlineMap) {
         var map = new Map { Name = inlineMap };
         var split = inlineMap.Split(',');
         foreach (var item in split) {
            if (item.Contains(":")) {
               var innerSplit = item.Split(new[] { ':' }, StringSplitOptions.RemoveEmptyEntries);
               var from = innerSplit.First();
               var to = innerSplit.Last();
               map.Items.Add(new MapItem { From = from, To = to });
            } else {
               map.Items.Add(new MapItem { From = item, To = item });
            }
         }

         return map;
      }

      /// <summary>
      /// When a filter and parameter are related via a parameter name, create the map between them, the provider fills the map
      /// </summary>
      /// <param name="p"></param>
      private static void AutomaticMaps(Process p) {

         // create maps for inline field transform maps
         foreach (var field in p.GetAllFields().Where(f => f.Map.Contains(","))) {
            if (p.Maps.All(m => m.Name != field.Map)) {
               p.Maps.Add(CreateMap(field.Map));
            }
         }

         // create maps for inline parameter transform maps
         foreach (var transform in p.Parameters.SelectMany(pr => pr.Transforms.Where(t => t.Map.Contains(",")))) {
            if (p.Maps.All(m => m.Name != transform.Map)) {
               p.Maps.Add(CreateMap(transform.Map));
            }
         }

         var autoMapCounter = 0;
         foreach (var entity in p.Entities.Where(e => e.Filter.Any(QualifiesForAutomaticMap()))) {
            var connection = p.Connections.FirstOrDefault(c => c.Name.Equals(entity.Input));
            if (connection != null) {
               foreach (var filter in entity.Filter.Where(QualifiesForAutomaticMap())) {
                  var parameter = p.Parameters.FirstOrDefault(pr => string.IsNullOrEmpty(pr.Map) && pr.Name == filter.Field && pr.Value == filter.Value);
                  if (parameter != null) {

                     // create map
                     autoMapCounter++;
                     var mapName = $"map-{filter.Field}-{autoMapCounter}";
                     parameter.Map = mapName;
                     filter.Map = mapName;
                     if (p.Maps == null) {
                        p.Maps = new List<Map>();
                     }
                     p.Maps.Add(new Map { Name = mapName });

                  }
               }
            }
         }
      }

      private static Func<Filter, bool> QualifiesForAutomaticMap() {
         return f => f.Type == "facet" && !string.IsNullOrEmpty(f.Field) && string.IsNullOrEmpty(f.Map);
      }

      private static void DefaultEntityConnections(Process p) {
         // take a connection named `input` if defined, otherwise take the first connection
         foreach (var entity in p.Entities.Where(entity => !entity.HasConnection())) {
            entity.Input = p.Connections.Any(c => c.Name == "input") ? "input" : p.Connections.First().Name;
         }
      }

      private static void DefaultSearchTypes(Process p) {

         var searchFields = p.GetSearchFields().ToArray();
         var output = p.GetOutputConnection();

         if (searchFields.Any()) {
            if (p.SearchTypes.All(st => st.Name != "none")) {
               p.SearchTypes.Add(new SearchType {
                  Name = "none",
                  MultiValued = false,
                  Store = false,
                  Index = false
               });
            }

            if (p.SearchTypes.All(st => st.Name != "default")) {
               p.SearchTypes.Add(new SearchType {
                  Name = "default",
                  MultiValued = false,
                  Store = true,
                  Index = true,
                  Analyzer = output != null && output.Provider == "elasticsearch" ? "keyword" : string.Empty
               });
            }

         }

      }

      private static void SetPrimaryKeys(Process p) {
         foreach (var field in p.Entities.SelectMany(entity => entity.GetAllFields().Where(field => field.PrimaryKey))) {
            field.KeyType = KeyType.Primary;
         }
      }

      private static Parameter GetParameter(string field) {
         return new Parameter { Field = field };
      }

      private static Parameter GetParameter(string entity, string field) {
         return new Parameter { Entity = entity, Field = field };
      }

      private static Parameter GetParameter(string entity, string field, string type) {
         return new Parameter { Entity = entity, Field = field, Type = type };
      }
   }
}
