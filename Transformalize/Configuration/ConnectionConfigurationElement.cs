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
using System.Linq;
using Transformalize.Libs.EnterpriseLibrary.Validation;
using Transformalize.Libs.EnterpriseLibrary.Validation.Validators;
using Transformalize.Libs.FileHelpers.Enums;
using Transformalize.Main;

namespace Transformalize.Configuration {
    [HasSelfValidation]
    public class ConnectionConfigurationElement : ConfigurationElement {
        private readonly char[] _slash = { '/' };

        private const string NAME = "name";
        private const string PROVIDER = "provider";
        private const string BATCH_SIZE = "batch-size";
        private const string ENABLED = "enabled";
        private const string DELIMITER = "delimiter";
        private const string DATABASE = "database";
        private const string SERVER = "server";
        private const string PORT = "port";
        private const string FILE = "file";
        private const string FOLDER = "folder";
        private const string SEARCH_PATTERN = "search-pattern";
        private const string SEARCH_OPTION = "search-option";
        private const string USER = "user";
        private const string PASSWORD = "password";
        private const string CONNECTION_STRING = "connection-string";
        private const string START = "start";
        private const string END = "end";
        private const string ERROR_MODE = "error-mode";
        private const string DATE_FORMAT = "date-format";
        private const string HEADER = "header";
        private const string FOOTER = "footer";
        private const string PATH = "path";
        private const string ENABLE_SSL = "enable-ssl";
        private const string SCHEMA = "schema";
        private const string ENCODING = "encoding";
        private const string VERSION = "version";
        private const string DIRECT = "direct";

        private const string WEB_METHOD = "web-method";
        private const string URL = "url";
        private const string DATA = "data";
        private const string CONTENT_TYPE = "content-type";

        private const StringComparison IC = StringComparison.OrdinalIgnoreCase;

        [ConfigurationProperty(NAME, IsRequired = true)]
        public string Name {
            get { return this[NAME] as string; }
            set { this[NAME] = value; }
        }

        [ConfigurationProperty(WEB_METHOD, IsRequired = false, DefaultValue = "GET")]
        public string WebMethod {
            get { return this[WEB_METHOD] as string; }
            set { this[WEB_METHOD] = value; }
        }

        [ConfigurationProperty(DATA, IsRequired = false, DefaultValue = Common.DefaultValue)]
        public string Data {
            get { return this[DATA] as string; }
            set { this[DATA] = value; }
        }

        [ConfigurationProperty(CONTENT_TYPE, IsRequired = false, DefaultValue = "")]
        public string ContentType {
            get { return this[CONTENT_TYPE] as string; }
            set { this[CONTENT_TYPE] = value; }
        }

        [ConfigurationProperty(URL, IsRequired = false, DefaultValue = "")]
        public string Url {
            get { return this[URL] as string; }
            set { this[URL] = value; }
        }

        [ConfigurationProperty(ENCODING, IsRequired = false, DefaultValue = "utf-8")]
        public string Encoding {
            get { return this[ENCODING] as string; }
            set { this[ENCODING] = value; }
        }

        [ConfigurationProperty(USER, IsRequired = false, DefaultValue = "")]
        public string User {
            get { return this[USER] as string; }
            set { this[USER] = value; }
        }

        [ConfigurationProperty(SCHEMA, IsRequired = false, DefaultValue = "")]
        public string Schema {
            get { return this[SCHEMA] as string; }
            set { this[SCHEMA] = value; }
        }

        [ConfigurationProperty(PATH, IsRequired = false, DefaultValue = "")]
        public string Path {
            get { return this[PATH] as string; }
            set { this[PATH] = value; }
        }

        [EnumConversionValidator(typeof(ErrorMode), MessageTemplate = "{1} must be a valid ErrorMode. (e.g. ThrowException, SaveAndContinue, IgnoreAndContinue)")]
        [ConfigurationProperty(ERROR_MODE, IsRequired = false, DefaultValue = "SaveAndContinue")]
        public string ErrorMode {
            get { return this[ERROR_MODE] as string; }
            set { this[ERROR_MODE] = value; }
        }

        [ConfigurationProperty(VERSION, IsRequired = false, DefaultValue = Common.DefaultValue)]
        public string Version {
            get { return this[VERSION] as string; }
            set { this[VERSION] = value; }
        }

        [ConfigurationProperty(PORT, IsRequired = false, DefaultValue = 0)]
        public int Port {
            get { return (int)this[PORT]; }
            set { this[PORT] = value; }
        }

        [ConfigurationProperty(START, IsRequired = false, DefaultValue = 1)]
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

        [ConfigurationProperty(FOLDER, IsRequired = false, DefaultValue = "")]
        public string Folder {
            get { return this[FOLDER] as string; }
            set { this[FOLDER] = value; }
        }

        [ConfigurationProperty(SEARCH_PATTERN, IsRequired = false, DefaultValue = "*.*")]
        public string SearchPattern {
            get { return this[SEARCH_PATTERN] as string; }
            set { this[SEARCH_PATTERN] = value; }
        }

        [EnumConversionValidator(typeof(SearchOption), MessageTemplate = "{1} must be a valid SearchOption. (e.g. AllDirectories, TopDirectoryOnly)")]
        [ConfigurationProperty(SEARCH_OPTION, IsRequired = false, DefaultValue = "TopDirectoryOnly")]
        public string SearchOption {
            get { return this[SEARCH_OPTION] as string; }
            set { this[SEARCH_OPTION] = value; }
        }

        [ConfigurationProperty(DELIMITER, IsRequired = false, DefaultValue = ",")]
        public string Delimiter {
            get { return this[DELIMITER] as string; }
            set { this[DELIMITER] = value; }
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

        [ConfigurationProperty(ENABLE_SSL, IsRequired = false, DefaultValue = false)]
        public bool EnableSsl {
            get { return (bool)this[ENABLE_SSL]; }
            set { this[ENABLE_SSL] = value; }
        }

        [ConfigurationProperty(DIRECT, IsRequired = false, DefaultValue = false)]
        public bool Direct {
            get { return (bool)this[DIRECT]; }
            set { this[DIRECT] = value; }
        }

        [ConfigurationProperty(DATE_FORMAT, IsRequired = false, DefaultValue = "MM/dd/yyyy h:mm:ss tt")]
        public string DateFormat {
            get { return this[DATE_FORMAT] as string; }
            set { this[DATE_FORMAT] = value; }
        }

        [ConfigurationProperty(HEADER, IsRequired = false, DefaultValue = Common.DefaultValue)]
        public string Header {
            get { return this[HEADER] as string; }
            set { this[HEADER] = value; }
        }

        [ConfigurationProperty(FOOTER, IsRequired = false, DefaultValue = "")]
        public string Footer {
            get { return this[FOOTER] as string; }
            set { this[FOOTER] = value; }
        }

        public override bool IsReadOnly() {
            return false;
        }

        public string NormalizeUrl(int defaultPort) {
            var builder = new UriBuilder(Server);
            if (Port > 0) {
                builder.Port = Port;
            }
            if (builder.Port == 0) {
                builder.Port = defaultPort;
            }
            if (!Path.Equals(string.Empty) && Path != builder.Path) {
                builder.Path = builder.Path.TrimEnd(_slash) + "/" + Path.TrimStart(_slash);
            } else if (!Folder.Equals(string.Empty) && Folder != builder.Path) {
                builder.Path = builder.Path.TrimEnd(_slash) + "/" + Folder.TrimStart(_slash);
            }
            return builder.ToString();
        }


        [SelfValidation]
        public void Validate(ValidationResults results) {

            var provider = Provider.ToLower();
            var providers = new[] {
                "sqlserver",
                "mysql",
                "postgresql",
                "sqlce",
                "analysisservices",
                "file",
                "folder",
                "internal",
                "console",
                "log",
                "mail",
                "html",
                "elasticsearch",
                "solr",
                "lucene",
                "web"
            };

            if (!providers.Any(p => p.Equals(provider))) {
                results.AddResult(new ValidationResult("The provider " + provider + " is not yet implemented.", this, null, null, null));
            }

            if (Provider.Equals("file", IC)) {
                if (string.IsNullOrEmpty(File)) {
                    var message = string.Format("The {0} provider requires the File property setting.", Provider);
                    results.AddResult(new ValidationResult(message, this, null, null, null));
                }
            } else if (Provider.Equals("folder", IC)) {
                if (string.IsNullOrEmpty(Folder)) {
                    var message = string.Format("The {0} provider requires the Folder property setting.", Provider);
                    results.AddResult(new ValidationResult(message, this, null, null, null));
                }
            }
        }
    }
}