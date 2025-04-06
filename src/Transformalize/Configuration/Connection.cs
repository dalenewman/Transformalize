#region license
// Transformalize
// Configurable Extract, Transform, and Load
// Copyright 2013-2019 Dale Newman
//  
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//   
//       http://www.apache.org/licenses/LICENSE-2.0
//   
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
#endregion
using Cfg.Net;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Transformalize.Configuration {
   public class Connection : CfgNode {

      [Cfg(required = true, unique = true, toLower = true)]
      public string Name { get; set; }

      [Cfg(value = "")]
      public string ConnectionString { get; set; }

      [Cfg(value = "")]
      public string ContentType { get; set; }

      [Cfg(value = "")]
      public string Database { get; set; }

      [Cfg(value = "")]
      public string Core { get; set; }

      [Cfg(value = "")]
      public string Cube { get; set; }

      [Cfg(value = "", toLower = true)]
      public string Index { get; set; }

      [Cfg(value = "MM/dd/yyyy h:mm:ss tt")]
      public string DateFormat { get; set; }

      [Cfg(value = "")]
      public string Delimiter { get; set; }

      [Cfg(value = 1)]
      public int MaxDegreeOfParallelism { get; set; }

      // "ASCII,BigEndianUnicode,Default,UTF32,UTF7,UTF8,Unicode"
      [Cfg(value = "utf-8", domain = "UTF-16LE,utf-16,ucs-2,unicode,ISO-10646-UCS-2,UTF-16BE,unicodeFFFE,windows-1252,utf-7,csUnicode11UTF7,unicode-1-1-utf-7,unicode-2-0-utf-7,x-unicode-1-1-utf-7,x-unicode-2-0-utf-7,utf-8,unicode-1-1-utf-8,unicode-2-0-utf-8,x-unicode-1-1-utf-8,x-unicode-2-0-utf-8,us-ascii,us,ascii,ANSI_X3.4-1968,ANSI_X3.4-1986,cp367,csASCII,IBM367,iso-ir-6,ISO646-US,ISO_646.irv:1991,GB18030")]
      public string Encoding { get; set; }

      [Cfg(value = 0)]
      public int End { get; set; }

      [Cfg(value = "SaveAndContinue", domain = "ThrowException,SaveAndContinue,IgnoreAndContinue", ignoreCase = true)]
      public string ErrorMode { get; set; }

      [Cfg(value = "")]
      public string Folder { get; set; }

      [Cfg(value = "")]
      public string File { get; set; }

      [Cfg(value = "")]
      public string Footer { get; set; }

      [Cfg(value = Constants.DefaultSetting)]
      public string Header { get; set; }

      [Cfg(value = "")]
      public string Password { get; set; }

      [Cfg(value = "")]
      public string Path { get; set; }

      [Cfg(value = 0, minValue = 0, maxValue = 65535)]
      public int Port { get; set; }

      [Cfg(value = Constants.DefaultSetting, domain = Constants.ProviderDomain, toLower = true)]
      public string Provider { get; set; }

      [Cfg(value ="", toLower = true)]
      public string Service { get; set; }

      [Cfg(value = "TopDirectoryOnly", domain = "AllDirectories,TopDirectoryOnly", ignoreCase = true)]
      public string SearchOption { get; set; }

      [Cfg(value = "*.*")]
      public string SearchPattern { get; set; }

      [Cfg(value = "localhost")]
      public string Server { get; set; }

      [Cfg(value = 1)]
      public int Start { get; set; }

      [Cfg(value = "")]
      public string Url { get; set; }

      [Cfg(value = "")]
      public string User { get; set; }

      [Cfg(value = Constants.DefaultSetting)]
      public string Version { get; set; }

      [Cfg(value = "GET")]
      public string WebMethod { get; set; }

      /// <summary>
      /// Timeout for each request (SQL or DML)
      /// </summary>
      [Cfg(value = 60)]
      public int RequestTimeout { get; set; }

      // Timeout for the connection
      [Cfg(value = 30)]
      public int Timeout { get; set; }

      [Cfg(value = "\"", maxLength = 1)]
      public string TextQualifier { get; set; }

      [Cfg(value = Constants.DefaultSetting)]
      public string Schema { get; set; }

      [Cfg(value = Constants.DefaultSetting)]
      public string Table { get; set; }

      /* Start File Inspection Properties */

      [Cfg(value = (short)0)]
      public short MaxLength { get; set; }

      [Cfg(value = (short)1)]
      public short MinLength { get; set; }

      [Cfg(value = (short)100)]
      public short Sample { get; set; }

      [Cfg(required = false)]
      public List<TflType> Types { get; set; }

      [Cfg(required = false)]
      public List<Delimiter> Delimiters { get; set; }

      [Cfg]
      public List<Server> Servers { get; set; }

      /* End File Inspection Properties */

      protected override void PreValidate() {
         ModifyProvider();

         if (string.IsNullOrEmpty(File))
            return;

         if (Provider != "file")
            return;

         var file = File.ToLower();
         if (file.EndsWith(".xls") || file.EndsWith(".xlsx")) {
            Provider = "excel";
         }

         //if (Delimiter == "" && file.EndsWith(".csv")) {
         //    Delimiter = ",";
         //}

      }

      private void ModifyProvider() {
         //backwards compatibility, default provider used to be sqlserver
         if (Provider == Constants.DefaultSetting && (Database != string.Empty || ConnectionString != string.Empty)) {
            Provider = "sqlserver";
         }
         if (Provider == "elasticsearch" && Port == 0) {
            Port = 9200;
         }
      }

      protected override void Validate() {

         if (Provider == "folder" && string.IsNullOrEmpty(Folder)) {
            Error("The folder provider requires a folder.");
         }

         if (Provider == "file" && string.IsNullOrEmpty(File)) {
            Error("The file provider requires a file.");
         }

         if (Provider == "web" && string.IsNullOrEmpty(Url)) {
            Error("The file provider requires a url.");
         }

         if (Provider == "sqlite" && string.IsNullOrEmpty(File) && string.IsNullOrEmpty(Database)) {
            Error("The sqlite provider requires a file.");
         }

         if (Provider == "access" && string.IsNullOrEmpty(File) && string.IsNullOrEmpty(Database)) {
            Error("The access provider requires a .mdb file.");
         }

         if (Provider == "lucene" && string.IsNullOrEmpty(Folder)) {
            Error("The lucene provider requires a folder.");
         }

         if (Provider == "solr") {
            if (Url == string.Empty) {
               if (Server == string.Empty || Core == string.Empty) {
                  Error("The server and core are required for the solr provider. The path is sometimes necessary. (e.g. <add provider='solr' server='localhost' port='8983' path='solr' core='collection1' />)");
               }
            } else {
               ValidateUrl();
            }
         }

         if (Provider == "elasticsearch") {
            if (Servers.Any()) {
               if (Index == string.Empty) {
                  Error("An index is required for elasticsearch connections.");
               }
               foreach (var server in Servers) {
                  if (server.Port == 0) {
                     server.Port = 9200;
                  }
               }
            } else {
               if (Url == string.Empty) {
                  if (Server == string.Empty || Index == string.Empty) {
                     Error("The server and index are required for the elastic provider. (e.g. <add provider='elastic' server='localhost' port='9200' index='twitter' />)");
                  }
               } else {
                  ValidateUrl();
               }
            }
         }

         if (Delimiter.Length > 1) {
            Error($"Invalid delimiter defined for connection '{Name}'.  The delimiter '{Delimiter}' is too long.  It can only be zero or one character.");
         }

         if (Provider == "sqlce" && RequestTimeout != 0) {
            RequestTimeout = 0;
         }

         if (Provider == "aws" && Service == string.Empty) {
            Error("The aws provider requires a service setting (e.g. provider='aws' service='logs').");
         }
      }

      private void ValidateUrl() {
         if (!Uri.TryCreate(Url, UriKind.Absolute, out var uriResult)) {
            Error($"The url {Url} is invalid for the {Name} connection.");
         }
      }

      public override string ToString() {
         switch (Provider) {
            case "activedirectory":
               return $"{Name} {Provider}";
            case "mysql":
            case "sqlserver":
            case "postgresql":
               return $"{Name} {Provider}:{Server}.{Database}";
            case "lucene":
            case "folder":
            case "filesystem":
               return $"{Name} {Provider}:{Folder}";
            case "elasticsearch":
            case "solr":
               return $"{Name} {Provider}:{Url}";
            case "file":
            case "excel":
            case "sqlite":
               return $"{Name} {Provider}:{File}";
            default:
               return Name + " " + Provider;
         }
      }

      public string Key { get; set; }

      [Cfg(value = "schema.xml")]
      public string SchemaFileName { get; set; }

      /// <summary>
      /// In init mode, when drop control is set to true, the control table is dropped.  If drop control 
      /// is false, the records are deleted, but the table remains intact.
      /// </summary>
      [Cfg(value = true)]
      public bool DropControl { get; set; }

      /// <summary>
      /// An option full path to a tool you would use to open a database or query with
      /// </summary>
      [Cfg(value = "")]
      public string OpenWith { get; set; }

      [Cfg(value = "csv", domain = "csv,json")]
      public string Format { get; set; }

      [Cfg(value = 5)]
      public int ErrorLimit { get; set; }

      [Cfg(value = Constants.DefaultSetting, domain = Constants.ModelDomain, toLower = true)]
      public string ModelType { get; set; }

      [Cfg(value = "")]
      public string Command { get; set; }

      [Cfg(value = "")]
      public string Arguments { get; set; }

      [Cfg(value = "")]
      public string Template { get; set; }

      [Cfg(value = false)]
      public bool Stream { get; set; }

      /// <summary>
      /// Seed for bogus provider
      /// </summary>
      [Cfg(value = 0)]
      public int Seed { get; set; }

      /// <summary>
      /// Added for mail provider
      /// </summary>
      [Cfg(value = false)]
      public bool UseSsl { get; set; }

      /// <summary>
      /// Added for mail provider
      /// </summary>
      [Cfg(value = false)]
      public bool StartTls { get; set; }

      // for elasticsearch, a time unit of long to maintain a consistent view of the data for scrolling (exporting large result sets)
      [Cfg(value = "1m")]
      public string Scroll { get; set; }

      [Cfg(value = "")]
      public string LinePattern { get; set; }

      [Cfg(value = 5)]
      public short Shards { get; set; }

      [Cfg(value = 1)]
      public short Replicas { get; set; }

      /// <summary>
      /// If you want to read data as fast as you can consume it, leave Buffer false. 
      /// If you want to read all the data into memory, set Buffer true.
      /// </summary>
      [Cfg(value = false)]
      public bool Buffer { get; set; }

      [Cfg(value = false)]
      public bool Synchronous { get; set; }

      /// <summary>
      /// Mars = Multiple Active Result Sets.  This is a SQL Server specific connection string option.
      /// </summary>
      [Cfg(value = false)]
      public bool Mars { get; set; }

      /// <summary>
      /// Used to optionally tell postgresql to enclose all identifiers (instead of just keywords).  Sometimes needed when table has been created with double quoted identifiers.
      /// </summary>
      [Cfg(value =false)]
      public bool Enclose { get; set; }

      public Dictionary<int, string> Lines { get; set; } = new Dictionary<int, string>();

      protected override void PostValidate() {
         if (Command.Contains(" ")) {
            if (!Command.Contains("\"")) {
               Command = string.Concat("\"", Command, "\"");
            }
         }
      }

      /// <summary>
      /// matches sql server connection string property with same name
      /// </summary>
      [Cfg(value = false)]
      public bool Encrypt { get; set; }

      /// <summary>
      /// matches sql server connection string property with same name
      /// </summary>
      [Cfg(value = false)]
      public bool TrustServerCertificate { get; set; }

      /// <summary>
      /// matches elasticsearch client property with same name
      /// </summary>
      [Cfg(value = "")]
      public string CertificateFingerprint { get; set; }

      /// <summary>
      /// browse the connection (intended for orchardcore transformalize module feature)
      /// </summary>
      [Cfg(value = false)]
      public bool Browse { get; set; }
   }
}