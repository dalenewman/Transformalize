#region license
// Transformalize
// Copyright 2013 Dale Newman
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
using System.Collections.Generic;
using System.Linq;
using Pipeline.Configuration;
using Pipeline.Contracts;

namespace Pipeline {
    public class EntityStatus {

        public EntityStatus(IContext ctx) {
            if (ctx.Entity.IsMaster)
                return;

            var master = ctx.Process.Entities.First(e => e.IsMaster);

            FirstRun = master.IsFirstRun;
            MasterUpserted = master.Updates > 0;
            Modified = ctx.Entity.Updates + ctx.Entity.Inserts + ctx.Entity.Deletes > 0;
            ForeignKeys.AddRange(ctx.Entity.Fields.Where(f => f.KeyType.HasFlag(KeyType.Foreign)));
            HasForeignKeys = ForeignKeys.Count > 0;
        }
        public bool FirstRun { get; set; }
        public bool MasterUpserted { get; set; }
        public bool Modified { get; set; }
        public bool HasForeignKeys { get; set; }
        public List<Field> ForeignKeys { get; set; } = new List<Field>();

        public bool NeedsUpdate() {
            // if there are foreign keys and any kind of update, then yes
            if (HasForeignKeys) {
                return (MasterUpserted || Modified);
            }
            return Modified && !FirstRun; //note: a FirstRun would not need to update the master's TflBatchId.
        }
    }
}
