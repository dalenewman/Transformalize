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

using Orchard.ContentManagement;
using Orchard.Core.Title.Models;
using Orchard.Tags.Models;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace Pipeline.Web.Orchard.Models {
    public class PipelineConfigurationPart : ContentPart<PipelineConfigurationPartRecord> {

        private string _defaultMode = null;

        public static List<SelectListItem> EditorModes = new List<SelectListItem> {
                new SelectListItem {Selected = false, Text = "JSON", Value = "json"},
                new SelectListItem {Selected = false, Text = "XML", Value = "xml"}
        };

        public static List<SelectListItem> PlaceHolderStyles = new List<SelectListItem> {
            new SelectListItem {Selected = false, Text = "@(parameter)", Value = "@()"},
            new SelectListItem {Selected = false, Text = "@[parameter]", Value = "@[]"}
        };

        public static List<SelectListItem> MapStyles = new List<SelectListItem> {
            new SelectListItem {Selected = false, Text = "Streets", Value = "streets-v10"},
            new SelectListItem {Selected = false, Text = "Outdoors", Value = "outdoors-v10"},
            new SelectListItem {Selected = false, Text = "Light", Value = "light-v9"},
            new SelectListItem {Selected = false, Text = "Dark", Value = "dark-v9"},
            new SelectListItem {Selected = false, Text = "Satellite", Value = "satellite-v9"},
            new SelectListItem {Selected = false, Text = "Satellite Streets", Value = "satellite-streets-v10"},
            new SelectListItem {Selected = false, Text = "Navigation Preview Day", Value = "navigation-preview-day-v4"},
            new SelectListItem {Selected = false, Text = "Navigation Preview Night", Value = "navigation-preview-night-v4"},
            new SelectListItem {Selected = false, Text = "Navigation Guidance Day", Value = "navigation-guidance-day-v4"},
            new SelectListItem {Selected = false, Text = "Navigation Guidance Night", Value = "navigation-guidance-night-v4"}

        };



        public string Configuration {
            get {
                var cfg = this.Retrieve(x => x.Configuration, versioned: true);
                if (string.IsNullOrEmpty(cfg)) {
                    return @"<cfg name=""name"">
    <parameters>
    </parameters>
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

        public string StartAddress {
            get { return this.Retrieve(x => x.StartAddress, versioned: true) ?? string.Empty; }
            set { this.Store(x => x.StartAddress, value, true); }
        }

        public string EndAddress {
            get { return this.Retrieve(x => x.EndAddress, versioned: true) ?? string.Empty; }
            set { this.Store(x => x.EndAddress, value, true); }
        }

        public bool Runnable {
            get { return this.Retrieve(x => x.Runnable, versioned: true); }
            set { this.Store(x => x.Runnable, value, true); }
        }

        public bool NeedsInputFile {
            get { return this.Retrieve(x => x.NeedsInputFile, versioned: true); }
            set { this.Store(x => x.NeedsInputFile, value, true); }
        }

        public bool EnableInlineParameters {
            get { return this.Retrieve(x => x.EnableInlineParameters, versioned: true); }
            set { this.Store(x => x.EnableInlineParameters, value, true); }
        }

        public string EditorMode {
            get { return this.Retrieve(x => x.EditorMode, versioned: true) ?? "xml"; }
            set { this.Store(x => x.EditorMode, value, true); }
        }

        public string MapStyle {
            get { return this.Retrieve(x => x.MapStyle, versioned: true) ?? "streets-v10"; }
            set { this.Store(x => x.MapStyle, value, true); }
        }

        public int MapCircleRadius {
            get { return this.Retrieve(x => x.MapCircleRadius, versioned: true, defaultValue: () => 8); }
            set { this.Store(x => x.MapCircleRadius, value, true); }
        }

        public double MapCircleOpacity {
            get { return this.Retrieve(x => x.MapCircleOpacity, versioned: true, defaultValue: () => 1.0); }
            set { this.Store(x => x.MapCircleOpacity, value, true); }
        }

        public bool Migrated {
            get { return this.Retrieve(x => x.Migrated, versioned: false, defaultValue: () => false); }
            set { this.Store(x => x.Migrated, value, versioned: false); }
        }

        public string Modes {
            get { return this.Retrieve(x => x.Modes, versioned: false, defaultValue: () => "default"); }
            set { this.Store(x => x.Modes, value, versioned: false); }
        }

        public string PageSizes {
            get { return this.Retrieve(x => x.PageSizes, versioned: false, defaultValue: () => "10,15,25,50,100"); }
            set { this.Store(x => x.PageSizes, value, versioned: false); }
        }

        public string MapSizes {
            get { return this.Retrieve(x => x.MapSizes, versioned: false, defaultValue: () => "1000,5000,10000"); }
            set { this.Store(x => x.MapSizes, value, versioned: false); }
        }

        public string GetDefaultMode() {
            if (_defaultMode != null)
                return _defaultMode;

            var modes = new List<string>();
            foreach (var mode in Modes.ToLower().Split(',')) {
                if (mode.EndsWith("*") || mode.StartsWith("*")) {
                    _defaultMode = mode.Trim('*');
                }
                modes.Add(mode.Trim('*'));
            }

            return _defaultMode ?? (_defaultMode = modes.Any() ? modes.First() : "default");
        }

        public string PlaceHolderStyle {
            get { return this.Retrieve(x => x.PlaceHolderStyle, versioned: false, defaultValue: () => "@()"); }
            set { this.Store(x => x.PlaceHolderStyle, value, versioned: false); }
        }

        public bool ClientSideSorting {
            get { return this.Retrieve(x => x.ClientSideSorting, versioned: true); }
            set { this.Store(x => x.ClientSideSorting, value, true); }
        }

        public bool IsValid() {
            return PlaceHolderStyle.Length == 3 &&
                   MapCircleRadius > 0 &&
                   MapCircleOpacity > 0.0 &&
                   MapCircleOpacity <= 1.0;
        }

        public IEnumerable<int> Sizes(string sizes) {
            if (string.IsNullOrEmpty(sizes)) {
                yield return 0;
            } else {
                foreach (var item in sizes.Split(',')) {
                    int size;
                    if (int.TryParse(item, out size)) {
                        yield return size;
                    }
                }
            }
        }
    }
}