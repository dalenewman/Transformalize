#region License

// /*
// Transformalize - Replicate, Transform, and Denormalize Your Data...
// Copyright (C) 2013 Dale Newman
// 
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with this program.  If not, see <http://www.gnu.org/licenses/>.
// */

#endregion

using System.Configuration;
using Transformalize.Libs.EnterpriseLibrary.Validation.Validators;
using Transformalize.Main;

namespace Transformalize.Configuration {
    public class ProcessConfigurationElement : ConfigurationElement {

        private const string TEMPLATE_CONTENT_TYPE = "template-content-type";
        private const string NAME = "name";
        private const string STAR = "star";
        private const string ENABLED = "enabled";
        private const string INHERIT = "inherit";
        private const string TIMEZONE = "time-zone";
        private const string PIPELINE_THREADING = "pipeline-threading";
        private const string ACTIONS = "actions";
        private const string TEMPLATES = "templates";

        [ConfigurationProperty(NAME, IsRequired = true)]
        public string Name {
            get { return this[NAME] as string; }
            set { this[NAME] = value; }
        }

        [ConfigurationProperty(INHERIT, IsRequired = false, DefaultValue = "")]
        public string Inherit {
            get { return this[INHERIT] as string; }
            set { this[INHERIT] = value; }
        }

        [ConfigurationProperty(TIMEZONE, IsRequired = false, DefaultValue = "")]
        public string TimeZone {
            get { return this[TIMEZONE] as string; }
            set { this[TIMEZONE] = value; }
        }


        [ConfigurationProperty(ENABLED, IsRequired = false, DefaultValue = true)]
        public bool Enabled {
            get { return (bool)this[ENABLED]; }
            set { this[ENABLED] = value; }
        }

        [ConfigurationProperty(STAR, IsRequired = false, DefaultValue = "")]
        public string Star {
            get { return this[STAR] as string; }
            set { this[STAR] = value; }
        }

        [ConfigurationProperty(TEMPLATE_CONTENT_TYPE, IsRequired = false, DefaultValue = "raw")]
        public string TemplateContentType {
            get { return this[TEMPLATE_CONTENT_TYPE] as string; }
            set { this[TEMPLATE_CONTENT_TYPE] = value; }
        }


        [ConfigurationProperty("connections")]
        public ConnectionElementCollection Connections {
            get { return this["connections"] as ConnectionElementCollection; }
        }

        [ConfigurationProperty("providers")]
        public ProviderElementCollection Providers {
            get { return this["providers"] as ProviderElementCollection; }
        }

        [ConfigurationProperty("search-types")]
        public SearchTypeElementCollection SearchTypes {
            get { return this["search-types"] as SearchTypeElementCollection; }
        }

        [ConfigurationProperty("maps")]
        public MapElementCollection Maps {
            get { return this["maps"] as MapElementCollection; }
        }

        [ConfigurationProperty("scripts")]
        public ScriptElementCollection Scripts {
            get { return this["scripts"] as ScriptElementCollection; }
        }

        [ConfigurationProperty("entities")]
        public EntityElementCollection Entities {
            get { return this["entities"] as EntityElementCollection; }
        }

        [ConfigurationProperty("relationships")]
        public RelationshipElementCollection Relationships {
            get { return this["relationships"] as RelationshipElementCollection; }
        }

        [ConfigurationProperty("calculated-fields")]
        public FieldElementCollection CalculatedFields {
            get { return this["calculated-fields"] as FieldElementCollection; }
        }

        [ConfigurationProperty(TEMPLATES)]
        public TemplateElementCollection Templates {
            get { return this[TEMPLATES] as TemplateElementCollection; }
        }

        [EnumConversionValidator(typeof(PipelineThreading), MessageTemplate = "{1} must be SingleThreaded, or MultiThreaded.")]
        [ConfigurationProperty(PIPELINE_THREADING, IsRequired = false, DefaultValue = "MultiThreaded")]
        public string PipelineThreading {
            get { return this[PIPELINE_THREADING] as string; }
            set { this[PIPELINE_THREADING] = value; }
        }

        [ConfigurationProperty(ACTIONS)]
        public ActionElementCollection Actions {
            get { return this[ACTIONS] as ActionElementCollection; }
        }

        public override bool IsReadOnly() {
            return false;
        }

        public string Serialize() {
            var config = new TransformalizeConfiguration();
            config.Processes.Add(this);
            return config.Serialize(null, "transformalize", ConfigurationSaveMode.Minimal);
        }

        public void Merge(ProcessConfigurationElement child) {
            //properties
            Name = child.Name;
            Star = child.Star;
            TimeZone = child.TimeZone;
            Enabled = child.Enabled;

            //collections
            CalculatedFields.Merge(child.CalculatedFields);
            Connections.Merge(child.Connections);
            Entities.Merge(child.Entities);
            Maps.Merge(child.Maps);
            Providers.Merge(child.Providers);
            Relationships.Merge(child.Relationships);
            Scripts.Merge(child.Scripts);
            SearchTypes.Merge(child.SearchTypes);
            Templates.Merge(child.Templates);
            Actions.Merge(child.Actions);
        }
    }
}