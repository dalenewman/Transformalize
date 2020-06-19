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
using System.Collections.Generic;
using System.Linq;
using Cfg.Net;

namespace Transformalize.ConfigurationFacade {

   public class Connection : CfgNode {

      [Cfg]
      public string Name { get; set; }

      [Cfg]
      public string Buffer { get; set; }

      [Cfg]
      public string ConnectionString { get; set; }

      [Cfg]
      public string ContentType { get; set; }

      [Cfg]
      public string Database { get; set; }

      [Cfg]
      public string Core { get; set; }

      [Cfg]
      public string Cube { get; set; }

      [Cfg]
      public string Index { get; set; }

      [Cfg]
      public string DateFormat { get; set; }

      [Cfg]
      public string Delimiter { get; set; }

      [Cfg]
      public string Encoding { get; set; }

      [Cfg]
      public string End { get; set; }

      [Cfg]
      public string ErrorMode { get; set; }

      [Cfg]
      public string Folder { get; set; }

      [Cfg]
      public string File { get; set; }

      [Cfg]
      public string Footer { get; set; }

      [Cfg]
      public string Header { get; set; }

      [Cfg]
      public string Password { get; set; }

      [Cfg]
      public string Path { get; set; }

      [Cfg]
      public string Port { get; set; }

      [Cfg]
      public string Provider { get; set; }

      [Cfg]
      public string SearchOption { get; set; }

      [Cfg]
      public string SearchPattern { get; set; }

      [Cfg]
      public string Server { get; set; }

      [Cfg]
      public string Start { get; set; }

      [Cfg]
      public string Url { get; set; }

      [Cfg]
      public string User { get; set; }

      [Cfg]
      public string Version { get; set; }

      [Cfg]
      public string WebMethod { get; set; }

      [Cfg]
      public string RequestTimeout { get; set; }

      [Cfg]
      public string Timeout { get; set; }

      [Cfg]
      public string TextQualifier { get; set; }

      [Cfg]
      public string Schema { get; set; }

      [Cfg]
      public string Table { get; set; }

      [Cfg]
      public string MaxLength { get; set; }

      [Cfg]
      public string MinLength { get; set; }

      [Cfg]
      public string Sample { get; set; }

      [Cfg]
      public List<TflType> Types { get; set; }

      [Cfg]
      public List<Delimiter> Delimiters { get; set; }

      [Cfg]
      public List<Server> Servers { get; set; }

      [Cfg]
      public string SchemaFileName { get; set; }

      [Cfg]
      public string DropControl { get; set; }

      [Cfg]
      public string OpenWith { get; set; }

      [Cfg]
      public string Format { get; set; }

      [Cfg]
      public string ErrorLimit { get; set; }

      [Cfg]
      public string ModelType { get; set; }

      [Cfg]
      public string Command { get; set; }

      [Cfg]
      public string Arguments { get; set; }

      [Cfg]
      public string Template { get; set; }

      [Cfg]
      public string Stream { get; set; }

      [Cfg]
      public string Seed { get; set; }

      [Cfg]
      public string UseSsl { get; set; }

      [Cfg]
      public string ScrollWindow { get; set; }

      [Cfg]
      public string LinePattern { get; set; }

      [Cfg]
      public string Shards { get; set; }

      [Cfg]
      public string Replicas { get; set; }

      [Cfg]
      public string MaxDegreeOfParallism { get; set; }

      public Configuration.Connection ToConnection() {
         var c = new Configuration.Connection {
            Arguments = this.Arguments,
            Command = this.Command,
            ConnectionString = this.ConnectionString,
            ContentType = this.ContentType,
            Core = this.Core,
            Cube = this.Cube,
            Database = this.Database,
            DateFormat = this.DateFormat,
            Delimiter = this.Delimiter,
            Delimiters = this.Delimiters.Select(d=>d.ToDelimiter()).ToList(),
            Encoding = this.Encoding,
            ErrorMode = this.ErrorMode,
            File = this.File,
            Folder = this.Folder,
            Footer = this.Footer,
            Format = this.Format,
            Header = this.Header,
            Index = this.Index,
            LinePattern = this.LinePattern,
            ModelType = this.ModelType,
            Name = this.Name,
            OpenWith = this.OpenWith,
            Password = this.Password,
            Path = this.Path,
            Provider = this.Provider,
            Schema = this.Schema,
            SchemaFileName = this.SchemaFileName,
            SearchOption = this.SearchOption,
            SearchPattern = this.SearchPattern,
            Server = this.Server,
            Servers = this.Servers.Select(s=> s.ToServer()).ToList(),
            Table = this.Table,
            Template = this.Template,
            TextQualifier = this.TextQualifier,
            Types = this.Types.Select(t=>t.ToType()).ToList(),
            Url = this.Url,
            User = this.User,
            Version = this.Version,
            WebMethod = this.WebMethod
         };

         bool.TryParse(this.Buffer, out bool buffer);
         c.Buffer = buffer;

         bool.TryParse(this.DropControl, out var dropControl);
         c.DropControl = dropControl;

         int.TryParse(this.End, out var end);
         c.End = end;

         int.TryParse(this.ErrorLimit, out var errorLimit);
         c.ErrorLimit = errorLimit;

         int.TryParse(this.MaxDegreeOfParallism, out int mdop);
         c.MaxDegreeOfParallelism = mdop;

         short.TryParse(this.MaxLength, out short maxLength);
         c.MaxLength = maxLength;

         short.TryParse(this.MinLength, out short minLength);
         c.MinLength = minLength;

         int.TryParse(this.Port, out int port);
         c.Port = port;

         short.TryParse(this.Replicas, out short replicas);
         c.Replicas = replicas;

         int.TryParse(this.RequestTimeout, out int requestTimeout);
         c.RequestTimeout = requestTimeout;

         short.TryParse(this.Sample, out short sample);
         c.Sample = sample;

         double.TryParse(this.ScrollWindow, out double scrollWindow);
         c.ScrollWindow = scrollWindow;

         int.TryParse(this.Seed, out int seed);
         c.Seed = seed;

         short.TryParse(this.Shards, out short shards);
         c.Shards = shards;

         int.TryParse(this.Start, out int start);
         c.Start = start;

         bool.TryParse(this.Stream, out bool stream);
         c.Stream = stream;

         int.TryParse(this.Timeout, out int timeOut);
         c.Timeout = timeOut;

         bool.TryParse(this.UseSsl, out bool useSsl);
         c.UseSsl = useSsl;

         return c;
      }

   }
}