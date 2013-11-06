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
using System.IO;
using Transformalize.Libs.EnterpriseLibrary.Validation;
using Transformalize.Libs.EnterpriseLibrary.Validation.Validators;
using Transformalize.Libs.NLog.Internal;
using Transformalize.Libs.Rhino.Etl.Operations;
using Transformalize.Main.Providers;

namespace Transformalize.Configuration {
    [HasSelfValidation]
    public class ConnectionConfigurationElement : ConfigurationElement {
        private const string NAME = "name";
        private const string COMPATABILITY_LEVEL = "compatability-level";
        private const string PROVIDER = "provider";
        private const string BATCH_SIZE = "batchSize";
        private const string ENABLED = "enabled";
        private const string DELIMITER = "delimiter";
        private const string DATABASE = "database";
        private const string SERVER = "server";
        private const string PORT = "port";
        private const string FILE = "file";
        private const string USER = "user";
        private const string PASSWORD = "password";
        private const string CONNECTION_STRING = "connection-string";
        private const string LINE_DELIMITER = "line-delimiter";
        private const string START = "start";
        private const string END = "end";
        private const StringComparison IC = StringComparison.OrdinalIgnoreCase;

        public AbstractOperation InputOperation { get; set; }

        [ConfigurationProperty(NAME, IsRequired = true)]
        public string Name {
            get { return this[NAME] as string; }
            set { this[NAME] = value; }
        }

        [ConfigurationProperty(USER, IsRequired = false, DefaultValue = "")]
        public string User {
            get { return this[USER] as string; }
            set { this[USER] = value; }
        }

        [ConfigurationProperty(PORT, IsRequired = false, DefaultValue = 0)]
        public int Port {
            get { return (int)this[PORT]; }
            set { this[PORT] = value; }
        }

        [ConfigurationProperty(START, IsRequired = false, DefaultValue = 0)]
        public int Start {
            get { return (int)this[START]; }
            set { this[START] = value; }
        }

        [ConfigurationProperty(END, IsRequired = false, DefaultValue = 0)]
        public int End {
            get { return (int)this[END]; }
            set { this[END] = value; }
        }

        [ConfigurationProperty(CONNECTION_STRING, IsRequired = false, DefaultValue = "")]
        public string ConnectionString {
            get { return this[CONNECTION_STRING] as string; }
            set { this[CONNECTION_STRING] = value; }
        }

        [ConfigurationProperty(PASSWORD, IsRequired = false, DefaultValue = "")]
        public string Password {
            get { return this[PASSWORD] as string; }
            set { this[PASSWORD] = value; }
        }

        [ConfigurationProperty(FILE, IsRequired = false, DefaultValue = "")]
        public string File {
            get { return this[FILE] as string; }
            set { this[FILE] = value; }
        }

        [ConfigurationProperty(DELIMITER, IsRequired = false, DefaultValue = "")]
        public string Delimiter {
            get { return this[DELIMITER] as string; }
            set { this[DELIMITER] = value; }
        }

        [ConfigurationProperty(LINE_DELIMITER, IsRequired = false, DefaultValue = "")]
        public string LineDelimiter {
            get { return this[LINE_DELIMITER] as string; }
            set { this[LINE_DELIMITER] = value; }
        }

        [ConfigurationProperty(DATABASE, IsRequired = false, DefaultValue = "")]
        public string Database {
            get { return this[DATABASE] as string; }
            set { this[DATABASE] = value; }
        }

        [ConfigurationProperty(SERVER, IsRequired = false, DefaultValue = "localhost")]
        public string Server {
            get { return this[SERVER] as string; }
            set { this[SERVER] = value; }
        }

        [ConfigurationProperty(COMPATABILITY_LEVEL, IsRequired = false, DefaultValue = 0)]
        public int CompatabilityLevel {
            get { return (int)this[COMPATABILITY_LEVEL]; }
            set { this[COMPATABILITY_LEVEL] = value; }
        }

        [RegexStringValidator(@"(?i)SqlServer|AnalysisServices|MySql|File|Excel|Internal")]
        [ConfigurationProperty(PROVIDER, IsRequired = false, DefaultValue = "SqlServer")]
        public string Provider {
            get { return this[PROVIDER] as string; }
            set { this[PROVIDER] = value; }
        }

        [RangeValidator(1, RangeBoundaryType.Inclusive, 1, RangeBoundaryType.Ignore, MessageTemplate = "{1} must be greater than 0.")]
        [ConfigurationProperty(BATCH_SIZE, IsRequired = false, DefaultValue = 500)]
        public int BatchSize {
            get { return (int)this[BATCH_SIZE]; }
            set { this[BATCH_SIZE] = value; }
        }

        [ConfigurationProperty(ENABLED, IsRequired = false, DefaultValue = true)]
        public bool Enabled {
            get { return (bool)this[ENABLED]; }
            set { this[ENABLED] = value; }
        }

        public override bool IsReadOnly() {
            return false;
        }

        [SelfValidation]
        public void Validate(ValidationResults results) {

            if ((new[] { "sqlserver", "mysql" }).Any(p => p.Equals(Provider, IC))) {
                if (string.IsNullOrEmpty(ConnectionString) && string.IsNullOrEmpty(Database)) {
                    var message = string.Format("The {0} provider requires either the ConnectionString or Database property setting.", Provider);
                    results.AddResult(new ValidationResult(message, this, null, null, null));
                } else {
                    if (String.IsNullOrEmpty(Database)) {
                        var user = ConnectionStringParser.GetUsername(ConnectionString);
                        var pw = ConnectionStringParser.GetPassword(ConnectionString);
                        var trusted = ConnectionStringParser.GetTrustedConnection(ConnectionString);
                        if (!trusted && (String.IsNullOrEmpty(user) || String.IsNullOrEmpty(pw))) {
                            var message = string.Format("An untrusted connection string like '{0}' must include credentials (i.e. username, password) or be set to trusted (i.e. trusted_connection=true).", ConnectionString);
                            results.AddResult(new ValidationResult(message, this, null, null, null));
                        }
                    }
                }
            } else if (Provider.Equals("file", IC)) {
                if (string.IsNullOrEmpty(File)) {
                    var message = string.Format("The {0} provider requires the File property setting.", Provider);
                    results.AddResult(new ValidationResult(message, this, null, null, null));
                } else {
                    if (!new FileInfo(File).Exists) {
                        var message = string.Format("The file '{0}' doesn't exist.", File);
                        results.AddResult(new ValidationResult(message, this, null, null, null));
                    }
                }
            }
        }
    }
}