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

using System.Collections.Generic;
using System.Linq;
using Transformalize.Configuration;
using Transformalize.Contracts;

namespace Transformalize.Providers.Ado {
   public class AdoSqlModel {

      public Field[] Fields { get; }
      public string[] Aliases { get; }
      public string[] FieldNames { get; }
      public string Flat { get; }
      public string Star { get; }
      public int Threshold { get; }
      public string Batch { get; set; }
      public string EnclosedKeyShortName { get; set; }
      public string KeyShortName { get; set; }
      public string KeyLongName { get; set; }
      public string EnclosedKeyLongName { get; set; }
      public string Master { get; set; }
      public Entity MasterEntity { get; set; }

      public AdoProvider AdoProvider { get; set; }

      public AdoSqlModel(IContext output, IConnectionFactory cf) {

         var starFields = output.Process.GetStarFields().ToArray();
         var ordered = new List<Field>();
         foreach (Field field in starFields[0].Where(f => f.System)) {
            ordered.Add(field);
         }
         foreach (Field field in starFields[0].Where(f => !f.System).Union(starFields[1]).OrderBy(f => f.Alias)) {
            ordered.Add(field);
         }

         Fields = ordered.ToArray();
         Aliases = Fields.Select(f => cf.Enclose(f.Alias)).ToArray();
         FieldNames = Fields.Select(f => f.FieldName()).ToArray();
         Flat = cf.Enclose(output.Process.Name + output.Process.FlatSuffix);
         Star = cf.Enclose(output.Process.Name + output.Process.StarSuffix);
         Threshold = output.Process.Entities.Select(e => e.BatchId).ToArray().Min() - 1;
         MasterEntity = output.Process.Entities.First();
         Master = cf.Enclose(MasterEntity.OutputTableName(output.Process.Name));
         KeyLongName = Constants.TflKey;
         KeyShortName = MasterEntity.Fields.First(f => f.Name == Constants.TflKey).FieldName();
         EnclosedKeyShortName = cf.Enclose(KeyShortName);
         EnclosedKeyLongName = cf.Enclose(Constants.TflKey);
         Batch = cf.Enclose(MasterEntity.Fields.First(f => f.Name == Constants.TflBatchId).FieldName());
         AdoProvider = cf.AdoProvider;
      }

   }
}