#region license
// Transformalize
// Configurable Extract, Transform, and Load
// Copyright 2013-2017 Dale Newman
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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Cfg.Net;
using Cfg.Net.Contracts;
using Cfg.Net.Ext;
using Cfg.Net.Serializers;
using Transformalize.Configuration.Ext;
using Transformalize.Context;
using Transformalize.Impl;
using Transformalize.Logging;

namespace Transformalize.ConfigurationFacade {

    [Cfg]
    public class Process : CfgNode {

        [Cfg]
        public string Shorthand { get; set; }

        [Cfg]
        public string Request { get; set; }

        [Cfg]
        public short Status { get; set; }

        [Cfg]
        public string Message { get; set; }

        [Cfg]
        public long Time { get; set; }

        [Cfg]
        public List<LogEntry> Log { get; set; }

        [Cfg]
        public string Environment { get; set; }

        [Cfg]
        public List<Environment> Environments { get; set; }

        public Process(
            string cfg,
            IDictionary<string, string> parameters,
            params IDependency[] dependencies)
        : base(dependencies) {
            Load(cfg, parameters);
        }

        public Process(string cfg, params IDependency[] dependencies) : this(cfg, null, dependencies) { }

        public Process(params IDependency[] dependencies) : base(dependencies) { }

        public Process() : base(new XmlSerializer()) { }

        [Cfg]
        public string Name { get; set; }

        [Cfg]
        public string Version { get; set; }

        [Cfg]
        public string Enabled { get; set; }

        [Cfg]
        public string Mode { get; set; }

        [Cfg]
        public string Pipeline { get; set; }

        [Cfg]
        public string StarSuffix { get; set; }

        [Cfg]
        public string Flatten { get; set; }

        [Cfg]
        public string FlatSuffix { get; set; }

        [Cfg]
        public string TemplateContentType { get; set; }

        [Cfg]
        public string TimeZone { get; set; }

        [Cfg]
        public List<Action> Actions { get; set; }

        [Cfg]
        public List<Field> CalculatedFields { get; set; }

        [Cfg]
        public List<Connection> Connections { get; set; }

        [Cfg]
        public List<Entity> Entities { get; set; }

        [Cfg]
        public List<Map> Maps { get; set; }

        [Cfg]
        public List<Relationship> Relationships { get; set; }

        [Cfg]
        public List<Script> Scripts { get; set; }

        [Cfg]
        public List<SearchType> SearchTypes { get; set; }

        [Cfg]
        public List<Template> Templates { get; set; }

        [Cfg]
        public List<Parameter> Parameters { get; set; }

        [Cfg]
        public string Description { get; set; }

        [Cfg]
        public string MaxMemory { get; set; }

        [Cfg]
        public List<Schedule> Schedule { get; set; }

        [Cfg]
        public List<CfgRow> Rows { get; set; }

        [Cfg]
        public string ReadOnly { get; set; }

        [Cfg]
        public string InternalProvider { get; set; }

        [Cfg]
        public string Buffer { get; set; }

        [Cfg]
        public string Id { get; set; }

    }
}