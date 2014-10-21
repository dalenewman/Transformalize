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
    /// <summary>
    /// A specialized ETL process focusing on de-normalization, transformation
    /// and syncing a destination with it's sources.
    /// </summary>
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
        /// A name (of your choosing) to identify the process.
        /// </summary>
        [ConfigurationProperty(NAME, IsRequired = true)]
        public string Name {
            get { return this[NAME] as string; }
            set { this[NAME] = value; }
        }

        /// <summary>
        /// Optional.
        /// 
        /// A choice between `Multithreaded`, `SingleThreaded`, and <strong>`Default`</strong>.
        /// 
        /// `Default` defers this decision to the entity's PipelineThreading setting.
        /// </summary>
        [EnumConversionValidator(typeof(PipelineThreading), MessageTemplate = "{1} must be SingleThreaded, MultiThreaded, or Default.")]
        [ConfigurationProperty(PIPELINE_THREADING, IsRequired = false, DefaultValue = "Default")]
        public string PipelineThreading {
            get { return this[PIPELINE_THREADING] as string; }
            set { this[PIPELINE_THREADING] = value; }
        }

        /// <summary>
        /// Optional. 
        /// 
        /// A mode reflects the intent of running the process.
        ///  
        /// * `init` wipes everything out
        /// * <strong>`default`</strong> moves data through the pipeline, from input to output.
        /// 
        /// Aside from these, you may use any mode (of your choosing).  Then, you can control
        /// whether or not templates and/or actions run by setting their modes.
        /// </summary>
        [ConfigurationProperty(MODE, IsRequired = false, DefaultValue = "default")]
        public string Mode {
            get { return this[MODE] as string; }
            set { this[MODE] = value.ToLower(); }
        }

        /// <summary>
        /// Optional.
        /// 
        /// Refers to another Transformalize process via:
        /// 
        /// * a process name as defined in your *.config
        /// * a file containing a process
        /// * a web address (url) to an http hosted file containing a process
        /// 
        /// Set this if you'd like to inherit settings from another process.
        /// </summary>
        [ConfigurationProperty(INHERIT, IsRequired = false, DefaultValue = "")]
        public string Inherit {
            get { return this[INHERIT] as string; }
            set { this[INHERIT] = value; }
        }

        /// <summary>
        /// Optional.
        /// 
        /// Indicates the data's time zone.
        /// 
        /// It is used as the `to-time-zone` setting for `now()` and `timezone()` transformations
        /// if the `to-time-zone` is not set.
        /// 
        /// NOTE: Normally, you should keep the dates in UTC until presented to the user. 
        /// Then, have the client application convert UTC to the user's time zone.
        /// </summary>
        [ConfigurationProperty(TIMEZONE, IsRequired = false, DefaultValue = "")]
        public string TimeZone {
            get { return this[TIMEZONE] as string; }
            set { this[TIMEZONE] = value; }
        }

        /// <summary>
        /// Optional.
        /// 
        /// `True` by default.
        /// 
        /// Indicates the process is enabed.  The included executable (e.g. `tfl.exe`) 
        /// respects this setting and does not run the process if disabled (or `False`).
        /// </summary>
        [ConfigurationProperty(ENABLED, IsRequired = false, DefaultValue = true)]
        public bool Enabled {
            get { return (bool)this[ENABLED]; }
            set { this[ENABLED] = value; }
        }

        /// <summary>
        /// Optional.
        /// 
        /// `True` by default.
        /// 
        /// Star refers to star-schema Transformalize is creating.  You can turn this off 
        /// if your intention is not to create a star-schema.  A `False` setting here may
        /// speed things up.
        /// </summary>
        [ConfigurationProperty(STAR_ENABLED, IsRequired = false, DefaultValue = true)]
        public bool StarEnabled {
            get { return (bool)this[STAR_ENABLED]; }
            set { this[STAR_ENABLED] = value; }
        }

        /// <summary>
        /// Optional.
        /// 
        /// If your output is a relational database that supports views and `StarEnabled` is `True`,
        /// this is the name of a view that projects fields from all the entities in the
        /// star-schema as a single flat projection.
        /// 
        /// If not set, it is the combination of the process name, and "Star." 
        /// </summary>
        [ConfigurationProperty(STAR, IsRequired = false, DefaultValue = Common.DefaultValue)]
        public string Star {
            get { return this[STAR] as string; }
            set { this[STAR] = value; }
        }

        /// <summary>
        /// Optional
        /// 
        /// If your output is a relational database that supports views, this is the name of
        /// a view that projects fields from all the entities.  This is different from 
        /// the Star view, as it's joins are exactly as configured in the <relationships/> 
        /// collection.
        /// 
        /// If not set, it is the combination of the process name, and "View." 
        /// </summary>
        [ConfigurationProperty(VIEW, IsRequired = false, DefaultValue = Common.DefaultValue)]
        public string View {
            get { return this[VIEW] as string; }
            set { this[VIEW] = value; }
        }

        /// <summary>
        /// Optional.
        /// 
        /// Choices are `html` and <strong>`raw`</strong>.
        /// 
        /// This refers to the razor templating engine's content type.  If you're rendering HTML 
        /// markup, use `html`, if not, using `raw` may inprove performance.
        /// </summary>
        [ConfigurationProperty(TEMPLATE_CONTENT_TYPE, IsRequired = false, DefaultValue = "raw")]
        public string TemplateContentType {
            get { return this[TEMPLATE_CONTENT_TYPE] as string; }
            set { this[TEMPLATE_CONTENT_TYPE] = value; }
        }

        /// <summary>
        /// A collection of [Parameters](/parameter).
        /// </summary>
        [ConfigurationProperty(PARAMETERS)]
        public ParameterElementCollection Parameters {
            get { return this[PARAMETERS] as ParameterElementCollection; }
        }

        /// <summary>
        /// A collection of [Connections](/connection)
        /// </summary>
        [ConfigurationProperty(CONNECTIONS)]
        public ConnectionElementCollection Connections {
            get { return this[CONNECTIONS] as ConnectionElementCollection; }
        }

        /// <summary>
        /// A collection of [Providers](/provider)
        /// </summary>
        [ConfigurationProperty(PROVIDERS)]
        public ProviderElementCollection Providers {
            get { return this[PROVIDERS] as ProviderElementCollection; }
        }

        /// <summary>
        /// A collection of [Logs](/log)
        /// </summary>
        [ConfigurationProperty(LOG)]
        public LogElementCollection Log {
            get { return this[LOG] as LogElementCollection; }
        }

        /// <summary>
        /// A collection of [Search Types](/search-type)
        /// </summary>
        [ConfigurationProperty(SEARCH_TYPES)]
        public SearchTypeElementCollection SearchTypes {
            get { return this[SEARCH_TYPES] as SearchTypeElementCollection; }
        }

        /// <summary>
        /// A collection of [Maps](/map)
        /// </summary>
        [ConfigurationProperty(MAPS)]
        public MapElementCollection Maps {
            get { return this[MAPS] as MapElementCollection; }
        }

        /// <summary>
        /// A collection of [Scripts](/script)
        /// </summary>
        [ConfigurationProperty(SCRIPTS)]
        public ScriptElementCollection Scripts {
            get { return this[SCRIPTS] as ScriptElementCollection; }
        }

        /// <summary>
        /// A collection of [Entities](/entity)
        /// </summary>
        [ConfigurationProperty(ENTITIES)]
        public EntityElementCollection Entities {
            get { return this[ENTITIES] as EntityElementCollection; }
        }

        /// <summary>
        /// A collection of [Relationships](/relationship)
        /// </summary>
        [ConfigurationProperty(RELATIONSHIPS)]
        public RelationshipElementCollection Relationships {
            get { return this[RELATIONSHIPS] as RelationshipElementCollection; }
        }

        /// <summary>
        /// A collection of [Calculated Fields](/calculated-field)
        /// </summary>
        [ConfigurationProperty(CALCULATED_FIELDS)]
        public FieldElementCollection CalculatedFields {
            get { return this[CALCULATED_FIELDS] as FieldElementCollection; }
        }

        /// <summary>
        /// A collection of [Templates](/template)
        /// </summary>
        [ConfigurationProperty(TEMPLATES)]
        public TemplateElementCollection Templates {
            get { return this[TEMPLATES] as TemplateElementCollection; }
        }

        /// <summary>
        /// A collection of [Actions](/action)
        /// </summary>
        [ConfigurationProperty(ACTIONS)]
        public ActionElementCollection Actions {
            get { return this[ACTIONS] as ActionElementCollection; }
        }

        /// <summary>
        /// Settings to control [file inspection](/file-inspection).
        /// </summary>
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

            Scripts.Path = child.Scripts.Path == Common.DefaultValue ? Scripts.Path : child.Scripts.Path;
            Templates.Path = child.Templates.Path == Common.DefaultValue ? Templates.Path : child.Templates.Path;
            Relationships.IndexMode = child.Relationships.IndexMode == Common.DefaultValue ? Relationships.IndexMode : child.Relationships.IndexMode;
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