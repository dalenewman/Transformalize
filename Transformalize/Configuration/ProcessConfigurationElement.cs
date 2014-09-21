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
        private const string VIEW = "view";
        private const string STAR_ENABLED = "star-enabled";
        private const string ENABLED = "enabled";
        private const string INHERIT = "inherit";
        private const string TIMEZONE = "time-zone";
        private const string PIPELINE_THREADING = "pipeline-threading";
        private const string ACTIONS = "actions";
        private const string TEMPLATES = "templates";
        private const string PARAMETERS = "parameters";
        private const string CONNECTIONS = "connections";
        private const string PROVIDERS = "providers";
        private const string SEARCH_TYPES = "search-types";
        private const string MAPS = "maps";
        private const string SCRIPTS = "scripts";
        private const string ENTITIES = "entities";
        private const string RELATIONSHIPS = "relationships";
        private const string CALCULATED_FIELDS = "calculated-fields";
        private const string MODE = "mode";
        private const string FILE_INSPECTION = "file-inspection";
        private const string LOG = "log";

        /// <summary>
        /// A name (of your choosing) used to identify the process.
        /// </summary>
        [ConfigurationProperty(NAME, IsRequired = true)]
        public string Name {
            get { return this[NAME] as string; }
            set { this[NAME] = value; }
        }

        /// <summary>
        /// A choice between `Multithreaded`, `SingleThreaded`, and `Default`.
        /// The default defers to the entity's PipelineThreading.
        /// </summary>
        [EnumConversionValidator(typeof(PipelineThreading), MessageTemplate = "{1} must be SingleThreaded, MultiThreaded, or Default.")]
        [ConfigurationProperty(PIPELINE_THREADING, IsRequired = false, DefaultValue = "Default")]
        public string PipelineThreading {
            get { return this[PIPELINE_THREADING] as string; }
            set { this[PIPELINE_THREADING] = value; }
        }

        /// <summary>
        /// A mode reflects the intent of running the process.
        /// Built in modes are init, and default.
        /// 
        /// * init wipes everything out
        /// * default runs with default behavior, which is to move data from input to output; detecting changes along the way.
        /// 
        /// Aside from these, you may use any mode you want to make up, and set corresponding modes in
        /// templates and/or actions that will run only if the mode matches.
        /// </summary>
        [ConfigurationProperty(MODE, IsRequired = false, DefaultValue = "default")]
        public string Mode {
            get { return this[MODE] as string; }
            set { this[MODE] = value.ToLower(); }
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

        [ConfigurationProperty(STAR_ENABLED, IsRequired = false, DefaultValue = true)]
        public bool StarEnabled {
            get { return (bool)this[STAR_ENABLED]; }
            set { this[STAR_ENABLED] = value; }
        }

        [ConfigurationProperty(STAR, IsRequired = false, DefaultValue = Common.DefaultValue)]
        public string Star {
            get { return this[STAR] as string; }
            set { this[STAR] = value; }
        }

        [ConfigurationProperty(VIEW, IsRequired = false, DefaultValue = Common.DefaultValue)]
        public string View {
            get { return this[VIEW] as string; }
            set { this[VIEW] = value; }
        }

        [ConfigurationProperty(TEMPLATE_CONTENT_TYPE, IsRequired = false, DefaultValue = "raw")]
        public string TemplateContentType {
            get { return this[TEMPLATE_CONTENT_TYPE] as string; }
            set { this[TEMPLATE_CONTENT_TYPE] = value; }
        }


        [ConfigurationProperty(PARAMETERS)]
        public ParameterElementCollection Parameters {
            get { return this[PARAMETERS] as ParameterElementCollection; }
        }

        [ConfigurationProperty(CONNECTIONS)]
        public ConnectionElementCollection Connections {
            get { return this[CONNECTIONS] as ConnectionElementCollection; }
        }

        [ConfigurationProperty(PROVIDERS)]
        public ProviderElementCollection Providers {
            get { return this[PROVIDERS] as ProviderElementCollection; }
        }

        [ConfigurationProperty(LOG)]
        public LogElementCollection Log {
            get { return this[LOG] as LogElementCollection; }
        }

        [ConfigurationProperty(SEARCH_TYPES)]
        public SearchTypeElementCollection SearchTypes {
            get { return this[SEARCH_TYPES] as SearchTypeElementCollection; }
        }

        [ConfigurationProperty(MAPS)]
        public MapElementCollection Maps {
            get { return this[MAPS] as MapElementCollection; }
        }

        [ConfigurationProperty(SCRIPTS)]
        public ScriptElementCollection Scripts {
            get { return this[SCRIPTS] as ScriptElementCollection; }
        }

        [ConfigurationProperty(ENTITIES)]
        public EntityElementCollection Entities {
            get { return this[ENTITIES] as EntityElementCollection; }
        }

        [ConfigurationProperty(RELATIONSHIPS)]
        public RelationshipElementCollection Relationships {
            get { return this[RELATIONSHIPS] as RelationshipElementCollection; }
        }

        [ConfigurationProperty(CALCULATED_FIELDS)]
        public FieldElementCollection CalculatedFields {
            get { return this[CALCULATED_FIELDS] as FieldElementCollection; }
        }

        [ConfigurationProperty(TEMPLATES)]
        public TemplateElementCollection Templates {
            get { return this[TEMPLATES] as TemplateElementCollection; }
        }

        [ConfigurationProperty(ACTIONS)]
        public ActionElementCollection Actions {
            get { return this[ACTIONS] as ActionElementCollection; }
        }

        [ConfigurationProperty(FILE_INSPECTION, IsRequired = false)]
        public FileInspectionElement FileInspection {
            get { return this[FILE_INSPECTION] as FileInspectionElement; }
            set { this[FILE_INSPECTION] = value; }
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
            Mode = child.Mode;
            StarEnabled = child.StarEnabled;
            TimeZone = child.TimeZone;
            Enabled = child.Enabled;
            Scripts.Path = child.Scripts.Path;
            Templates.Path = child.Templates.Path;
            FileInspection = child.FileInspection;

            //collections
            Parameters.Merge(child.Parameters);
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
            Log.Merge(child.Log);
        }
    }
}