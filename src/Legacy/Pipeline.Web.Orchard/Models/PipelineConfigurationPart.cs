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

      public string MapConfiguration {
         get {
            var cfg = this.Retrieve(x => x.MapConfiguration, versioned: true);
            if (string.IsNullOrEmpty(cfg)) {
               return @"<cfg>
   <styles>
      <add name=""Streets"" url=""mapbox://styles/mapbox/streets-v10"" />
      <add name=""Outdoors"" url=""mapbox://styles/mapbox/outdoors-v10"" />
      <add name=""Light"" url=""mapbox://styles/mapbox/light-v9"" />
      <add name=""Dark"" url=""mapbox://styles/mapbox/dark-v9"" />
      <add name=""Satellite"" url=""mapbox://styles/mapbox/satellite-v9"" />
      <add name=""Satellite Streets"" url=""mapbox://styles/mapbox/satellite-streets-v10"" />
      <add name=""Navigation Preview Day"" url=""mapbox://styles/mapbox/navigation-preview-day-v4"" />
      <add name=""Navigation Preview Night"" url=""mapbox://styles/mapbox/navigation-preview-night-v4"" />
      <add name=""Navigation Guidance Day"" url=""mapbox://styles/mapbox/navigation-guidance-day-v4"" />
      <add name=""Navigation Guidance Night"" url=""mapbox://styles/mapbox/navigation-guidance-night-v4"" />
   </styles>
</cfg>";
            }
            return cfg;
         }
         set { this.Store(x => x.MapConfiguration, value, true); }
      }

      public int MapCircleRadius {
         get { return this.Retrieve(x => x.MapCircleRadius, versioned: true, defaultValue: () => 8); }
         set { this.Store(x => x.MapCircleRadius, value, true); }
      }

      public int MapZoom {
         get { return this.Retrieve(x => x.MapZoom, versioned: true, defaultValue: () => 6); }
         set { this.Store(x => x.MapZoom, value, true); }
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
         get { return this.Retrieve(x => x.PageSizes, versioned: false, defaultValue: () => "100"); }
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

      public int ClipTextAt {
         get { return this.Retrieve(x => x.ClipTextAt, versioned: true, defaultValue: 0); }
         set { this.Store(x => x.ClipTextAt, value, true); }
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

      public bool CalendarEnabled {
         get { return this.Retrieve(x => x.CalendarEnabled, versioned: true, defaultValue: false); }
         set { this.Store(x => x.CalendarEnabled, value, true); }
      }

      public bool CalendarPaging {
         get { return this.Retrieve(x => x.CalendarPaging, versioned: true, defaultValue: true); }
         set { this.Store(x => x.CalendarPaging, value, true); }
      }

      public string CalendarIdField {
         get { return this.Retrieve(x => x.CalendarIdField, versioned: true, defaultValue: "id"); }
         set { this.Store(x => x.CalendarIdField, value, true); }
      }

      public string CalendarTitleField {
         get { return this.Retrieve(x => x.CalendarTitleField, versioned: true, defaultValue: "title"); }
         set { this.Store(x => x.CalendarTitleField, value, true); }
      }

      public string CalendarUrlField {
         get { return this.Retrieve(x => x.CalendarUrlField, versioned: true, defaultValue: "url"); }
         set { this.Store(x => x.CalendarUrlField, value, true); }
      }

      public string CalendarClassField {
         get { return this.Retrieve(x => x.CalendarClassField, versioned: true, defaultValue: "class"); }
         set { this.Store(x => x.CalendarClassField, value, true); }
      }

      public string CalendarStartField {
         get { return this.Retrieve(x => x.CalendarStartField, versioned: true, defaultValue: "start"); }
         set { this.Store(x => x.CalendarStartField, value, true); }
      }

      public string CalendarEndField {
         get { return this.Retrieve(x => x.CalendarEndField, versioned: true, defaultValue: "end"); }
         set { this.Store(x => x.CalendarEndField, value, true); }
      }

      public bool MapEnabled {
         get { return this.Retrieve(x => x.MapEnabled, versioned: true, defaultValue: true); }
         set { this.Store(x => x.MapEnabled, value, true); }
      }

      public bool MapPaging {
         get { return this.Retrieve(x => x.MapPaging, versioned: true, defaultValue: true); }
         set { this.Store(x => x.MapPaging, value, true); }
      }

      public bool MapBulkActions {
         get { return this.Retrieve(x => x.MapBulkActions, versioned: true, defaultValue: true); }
         set { this.Store(x => x.MapBulkActions, value, true); }
      }

      public bool MapRefresh {
         get { return this.Retrieve(x => x.MapRefresh, versioned: true, defaultValue: false); }
         set { this.Store(x => x.MapRefresh, value, true); }
      }

      public string MapColorField {
         get { return this.Retrieve(x => x.MapColorField, versioned: true, defaultValue: "geojson-color"); }
         set { this.Store(x => x.MapColorField, value, true); }
      }

      public string MapPopUpField {
         get { return this.Retrieve(x => x.MapPopUpField, versioned: true, defaultValue: "geojson-description"); }
         set { this.Store(x => x.MapPopUpField, value, true); }
      }

      public string MapLatitudeField {
         get { return this.Retrieve(x => x.MapLatitudeField, versioned: true, defaultValue: "Latitude"); }
         set { this.Store(x => x.MapLatitudeField, value, true); }
      }

      public string MapLongitudeField {
         get { return this.Retrieve(x => x.MapLongitudeField, versioned: true, defaultValue: "Longitude"); }
         set { this.Store(x => x.MapLongitudeField, value, true); }
      }

      public string ReportRowClassField {
         get { return this.Retrieve(x => x.ReportRowClassField, versioned: true, defaultValue: ""); }
         set { this.Store(x => x.ReportRowClassField, value, true); }
      }

      public string ReportRowStyleField {
         get { return this.Retrieve(x => x.ReportRowStyleField, versioned: true, defaultValue: ""); }
         set { this.Store(x => x.ReportRowStyleField, value, true); }
      }

   }
}