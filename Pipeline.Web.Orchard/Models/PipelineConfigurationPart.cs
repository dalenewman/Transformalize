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
using System.Web.Mvc;
using Orchard.ContentManagement;
using Orchard.Core.Title.Models;

namespace Pipeline.Web.Orchard.Models {
    public class PipelineConfigurationPart : ContentPart<PipelineConfigurationPartRecord> {

        public List<SelectListItem> EditorModes { get; set; }

        public PipelineConfigurationPart() {
            EditorModes = new List<SelectListItem> {
                new SelectListItem {Selected = false, Text = "JSON", Value = "json"}, 
                new SelectListItem {Selected = false, Text = "XML", Value = "xml"}, 
                new SelectListItem {Selected = false, Text = "YAML", Value = "yaml"}
            };
        }

        public string Configuration {
            get {
                if (string.IsNullOrEmpty(Record.Configuration)) {
                    return @"<cfg name=""name"">
    <connections>
    </connections>
    <entities>
    </entities>
</cfg>";
                }
                return Record.Configuration;
            }
            set {
                Record.Configuration = value;
            }
        }

        public string Title() {
            return this.As<TitlePart>().Title;
        }

        public string StartAddress {
            get { return Record.StartAddress ?? string.Empty; }
            set { Record.StartAddress = value; }
        }

        public string EndAddress {
            get { return Record.EndAddress ?? string.Empty; }
            set { Record.EndAddress = value; }
        }

        public bool Runnable {
            get { return Record.Runnable; }
            set { Record.Runnable = value; }
        }

        public bool Reportable {
            get { return Record.Reportable; }
            set { Record.Reportable = value; }
        }

        public bool NeedsInputFile {
            get { return Record.NeedsInputFile; }
            set { Record.NeedsInputFile = value; }
        }

        public string EditorMode {
            get { return Record.EditorMode ?? "xml"; }
            set { Record.EditorMode = value; }
        }

        public bool IsValid() {
            return true;
        }

    }
}