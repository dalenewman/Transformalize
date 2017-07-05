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
using Orchard.Tags.Models;

namespace Pipeline.Web.Orchard.Models {
    public class PipelineConfigurationPart : ContentPart<PipelineConfigurationPartRecord> {

        public static List<SelectListItem> EditorModes = new List<SelectListItem> {
                new SelectListItem {Selected = false, Text = "JSON", Value = "json"},
                new SelectListItem {Selected = false, Text = "XML", Value = "xml"}
        };

        public static List<SelectListItem> PlaceHolderStyles = new List<SelectListItem> {
            new SelectListItem {Selected = false, Text = "@(parameter)", Value = "@()"},
            new SelectListItem {Selected = false, Text = "@{parameter}", Value = "@{}"}
        };

        public string Configuration
        {
            get
            {
                var cfg = this.Retrieve(x => x.Configuration, versioned: true);
                if (string.IsNullOrEmpty(cfg)) {
                    return @"<cfg name=""name"">
    <connections>
    </connections>
    <entities>
    </entities>
</cfg>";
                }
                return cfg;
            }
            set { this.Store(x => x.Configuration, value, true); }
        }

        public string Title() {
            return this.As<TitlePart>().Title;
        }

        public IEnumerable<string> Tags() {
            return this.As<TagsPart>().CurrentTags;
        }

        public string StartAddress
        {
            get { return this.Retrieve(x => x.StartAddress, versioned: true) ?? string.Empty; }
            set { this.Store(x => x.StartAddress, value, true); }
        }

        public string EndAddress
        {
            get { return this.Retrieve(x => x.EndAddress, versioned: true) ?? string.Empty; }
            set { this.Store(x => x.EndAddress, value, true); }
        }

        public bool Runnable
        {
            get { return this.Retrieve(x => x.Runnable, versioned: true); }
            set { this.Store(x => x.Runnable, value, true); }
        }

        public bool Reportable
        {
            get { return this.Retrieve(x => x.Reportable, versioned: true); }
            set { this.Store(x => x.Reportable, value, true); }
        }

        public bool NeedsInputFile
        {
            get { return this.Retrieve(x => x.NeedsInputFile, versioned: true); }
            set { this.Store(x => x.NeedsInputFile, value, true); }
        }

        public string EditorMode
        {
            get { return this.Retrieve(x => x.EditorMode, versioned: true) ?? "xml"; }
            set { this.Store(x => x.EditorMode, value, true); }
        }



        public bool Migrated
        {
            get { return this.Retrieve(x => x.Migrated, versioned: false, defaultValue: () => false); }
            set { this.Store(x => x.Migrated, value, versioned: false); }
        }

        public string Modes
        {
            get { return this.Retrieve(x => x.Modes, versioned: false, defaultValue: () => "init,default*"); }
            set { this.Store(x => x.Modes, value, versioned: false); }
        }

        public string PlaceHolderStyle
        {
            get { return this.Retrieve(x => x.PlaceHolderStyle, versioned: false, defaultValue: () => "@()"); }
            set { this.Store(x => x.PlaceHolderStyle, value, versioned: false); }
        }

        public bool IsValid() {
            return PlaceHolderStyle.Length == 3;
        }

    }
}