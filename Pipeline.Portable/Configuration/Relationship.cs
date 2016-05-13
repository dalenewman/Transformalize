#region license
// Transformalize
// A Configurable ETL Solution Specializing in Incremental Denormalization.
// Copyright 2013 Dale Newman
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
using Cfg.Net;
using Cfg.Net.Ext;

namespace Pipeline.Configuration {
    public class Relationship : CfgNode {

        public RelationshipSummary Summary { get; set; }

        public Relationship() {
            Summary = new RelationshipSummary();
        }

        [Cfg(value = "", required = true, unique = false)]
        public string LeftEntity { get; set; }

        [Cfg(value = "", required = true, unique = false)]
        public string RightEntity { get; set; }

        [Cfg(value = "")]
        public string LeftField { get; set; }

        [Cfg(value = "")]
        public string RightField { get; set; }

        [Cfg(value = false)]
        public bool Index { get; set; }

        [Cfg]
        public List<Join> Join { get; set; }

        protected override void PreValidate() {
            PreValidateNormalizeJoin();
        }

        private void PreValidateNormalizeJoin() {
            if (LeftField == string.Empty)
                return;

            Join.Insert(0, new Join { LeftField = LeftField, RightField = RightField }.WithDefaults());

            LeftField = string.Empty;
            RightField = string.Empty;
        }

        public IEnumerable<string> GetLeftJoinFields() {
            return Join.Select(j => j.LeftField).ToArray();
        }

        public IEnumerable<string> GetRightJoinFields() {
            return Join.Select(j => j.RightField).ToArray();
        }

    }
}