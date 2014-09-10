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

using System;
using System.Configuration;
using Transformalize.Libs.EnterpriseLibrary.Validation.Validators;
using Transformalize.Libs.Rhino.Etl.Operations;
using Transformalize.Main;

namespace Transformalize.Configuration {

    public class EntityConfigurationElement : ConfigurationElement {

        private const string SCHEMA = "schema";
        private const string NAME = "name";
        private const string ALIAS = "alias";
        private const string CONNECTION = "connection";
        private const string FIELDS = "fields";
        private const string CALCULATED_FIELDS = "calculated-fields";
        private const string VERSION = "version";
        private const string PREFIX = "prefix";
        private const string GROUP = "group";
        private const string DELETE = "delete";
        private const string PREPEND_PROCESS_NAME_TO_OUTPUT_NAME = "prepend-process-name-to-output-name";
        private const string PIPELINE_THREADING = "pipeline-threading";
        private const string SAMPLE = "sample";
        private const string SQL_OVERRIDE = "sql-override";
        private const string SQL_KEYS_OVERRIDE = "sql-keys-override";
        private const string OUTPUT = "output";
        private const string INPUT = "input";
        private const string DETECT_CHANGES = "detect-changes";
        private const string TRIM_ALL = "trim-all";
        private const string UNICODE = "unicode";
        private const string VARIABLE_LENGTH = "variable-length";
        private const string NO_LOCK = "no-lock";

        public IOperation InputOperation { get; set; }

        [ConfigurationProperty(DETECT_CHANGES, IsRequired = false, DefaultValue = true)]
        public bool DetectChanges {
            get { return (bool)this[DETECT_CHANGES]; }
            set { this[DETECT_CHANGES] = value; }
        }

        [ConfigurationProperty(TRIM_ALL, IsRequired = false, DefaultValue = false)]
        public bool TrimAll {
            get { return (bool)this[TRIM_ALL]; }
            set { this[TRIM_ALL] = value; }
        }

        [ConfigurationProperty(NO_LOCK, IsRequired = false, DefaultValue = false)]
        public bool NoLock {
            get { return (bool)this[NO_LOCK]; }
            set { this[NO_LOCK] = value; }
        }

        [ConfigurationProperty(SCHEMA, IsRequired = false, DefaultValue = "")]
        public string Schema {
            get { return this[SCHEMA] as string; }
            set { this[SCHEMA] = value; }
        }

        [EnumConversionValidator(typeof(PipelineThreading), MessageTemplate = "{1} must be SingleThreaded, MultiThreaded, or Default.")]
        [ConfigurationProperty(PIPELINE_THREADING, IsRequired = false, DefaultValue = "Default")]
        public string PipelineThreading {
            get { return this[PIPELINE_THREADING] as string; }
            set { this[PIPELINE_THREADING] = value; }
        }

        [ConfigurationProperty(NAME, IsRequired = true)]
        public string Name {
            get { return this[NAME] as string; }
            set { this[NAME] = value; }
        }

        [ConfigurationProperty(ALIAS, IsRequired = false, DefaultValue = "")]
        public string Alias {
            get {
                var alias = this[ALIAS] as string;
                return alias == string.Empty ? Name : alias;
            }
            set { this[ALIAS] = value; }
        }

        [ConfigurationProperty(CONNECTION, IsRequired = false, DefaultValue = "input")]
        public string Connection {
            get { return this[CONNECTION] as string; }
            set { this[CONNECTION] = value; }
        }

        [ConfigurationProperty(FIELDS)]
        public FieldElementCollection Fields {
            get { return this[FIELDS] as FieldElementCollection; }
        }

        [ConfigurationProperty(CALCULATED_FIELDS)]
        public FieldElementCollection CalculatedFields {
            get { return this[CALCULATED_FIELDS] as FieldElementCollection; }
        }

        [ConfigurationProperty(VERSION, IsRequired = false, DefaultValue = "")]
        public string Version {
            get { return this[VERSION] as string; }
            set { this[VERSION] = value; }
        }

        [ConfigurationProperty(DELETE, IsRequired = false, DefaultValue = false)]
        public bool Delete {
            get { return (bool)this[DELETE]; }
            set { this[DELETE] = value; }
        }

        [ConfigurationProperty(PREPEND_PROCESS_NAME_TO_OUTPUT_NAME, IsRequired = false, DefaultValue = true)]
        public bool PrependProcessNameToOutputName {
            get { return (bool)this[PREPEND_PROCESS_NAME_TO_OUTPUT_NAME]; }
            set { this[PREPEND_PROCESS_NAME_TO_OUTPUT_NAME] = value; }
        }

        [ConfigurationProperty(PREFIX, IsRequired = false, DefaultValue = "")]
        public string Prefix {
            get { return this[PREFIX] as string; }
            set { this[PREFIX] = value; }
        }

        [ConfigurationProperty(GROUP, IsRequired = false, DefaultValue = false)]
        public bool Group {
            get { return (bool)this[GROUP]; }
            set { this[GROUP] = value; }
        }

        [ConfigurationProperty(SAMPLE, IsRequired = false, DefaultValue = "100")]
        public decimal Sample {
            get { return Convert.ToDecimal(this[SAMPLE]); }
            set { this[SAMPLE] = value; }
        }

        [ConfigurationProperty(SQL_OVERRIDE)]
        public SqlOverride SqlOverride {
            get { return this[SQL_OVERRIDE] as SqlOverride; }
        }

        [ConfigurationProperty(SQL_KEYS_OVERRIDE)]
        public SqlOverride SqlKeysOverride {
            get { return this[SQL_KEYS_OVERRIDE] as SqlOverride; }
        }

        [ConfigurationProperty(OUTPUT)]
        public IoElementCollection Output {
            get { return this[OUTPUT] as IoElementCollection; }
        }

        [ConfigurationProperty(INPUT)]
        public IoElementCollection Input {
            get { return this[INPUT] as IoElementCollection; }
        }

        [ConfigurationProperty(UNICODE, IsRequired = false, DefaultValue = true)]
        public bool Unicode {
            get { return (bool)this[UNICODE]; }
            set { this[UNICODE] = value; }
        }

        [ConfigurationProperty(VARIABLE_LENGTH, IsRequired = false, DefaultValue = true)]
        public bool VariableLength {
            get { return (bool)this[VARIABLE_LENGTH]; }
            set { this[VARIABLE_LENGTH] = value; }
        }

        [ConfigurationProperty("top", IsRequired = false, DefaultValue = 0)]
        public int Top {
            get { return (int)this["top"]; }
            set { this["top"] = value; }
        }

        public override bool IsReadOnly() {
            return false;
        }

    }
}