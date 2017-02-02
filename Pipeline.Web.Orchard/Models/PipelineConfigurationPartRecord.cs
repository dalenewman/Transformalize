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
using System.ComponentModel.DataAnnotations;
using Orchard.ContentManagement;
using Orchard.ContentManagement.Records;
using Orchard.Data.Conventions;

namespace Pipeline.Web.Orchard.Models {

    public class PipelineConfigurationPartRecord : ContentPartRecord {

        [StringLength(64)]
        public virtual string EditorMode { get; set; }

        [StringLengthMax]
        public virtual string Configuration { get; set; }

        [StringLength(64)]
        public virtual string StartAddress { get; set; }

        [StringLength(64)]
        public virtual string EndAddress { get; set; }

        public virtual bool Runnable { get; set; }

        public virtual bool Reportable { get; set; }

        public virtual bool NeedsInputFile { get; set; }

    }
}