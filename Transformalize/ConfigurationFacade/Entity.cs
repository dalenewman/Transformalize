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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Cfg.Net;
using Transformalize.Impl;

namespace Transformalize.ConfigurationFacade {
   public class Entity : CfgNode {

      [Cfg]
      public string Alias { get; set; }

      [Cfg]
      public string Label { get; set; }

      [Cfg]
      public string Connection { get; set; }

      [Cfg]
      public string Input { get; set; }

      [Cfg]
      public string Insert { get; set; }

      [Cfg]
      public string Update { get; set; }

      [Cfg]
      public string Delete { get; set; }

      [Cfg]
      public string InsertSize { get; set; }

      [Cfg]
      public string UpdateSize { get; set; }

      [Cfg]
      public string DeleteSize { get; set; }

      [Cfg]
      public string InsertCommand { get; set; }

      [Cfg]
      public string UpdateCommand { get; set; }

      [Cfg]
      public string DeleteCommand { get; set; }

      [Cfg]
      public string CreateCommand { get; set; }

      [Cfg]
      public string Group { get; set; }

      [Cfg]
      public string Name { get; set; }

      [Cfg]
      public List<CfgRow> Rows { get; set; }

      [Cfg]
      public string Pipeline { get; set; }

      [Cfg]
      public string Prefix { get; set; }

      [Cfg]
      public string PrependProcessNameToOutputName { get; set; }

      [Cfg]
      public string Query { get; set; }

      [Cfg]
      public string QueryKeys { get; set; }

      [Cfg]
      public string Sample { get; set; }

      [Cfg]
      public string Schema { get; set; }

      [Cfg]
      public string Script { get; set; }

      [Cfg]
      public string ScriptKeys { get; set; }

      [Cfg]
      public string TrimAll { get; set; }

      [Cfg]
      public string Unicode { get; set; }

      [Cfg]
      public string VariableLength { get; set; }

      [Cfg]
      public string Version { get; set; }

      [Cfg]
      public List<Filter> Filter { get; set; }

      [Cfg]
      public List<Field> Fields { get; set; }

      [Cfg]
      public List<Field> CalculatedFields { get; set; }

      [Cfg]
      public List<Order> Order { get; set; }

      [Cfg]
      public string LogInterval { get; set; }

      [Cfg]
      public string Overlap { get; set; }

      [Cfg]
      public string ValidField { get; set; }

      [Cfg]
      public string ReadSize { get; set; }

      [Cfg]
      public string Page { get; set; }

      [Cfg]
      public string Size { get; set; }

      [Cfg]
      public string OrderBy { get; set; }

      [Cfg]
      public string NoLock { get; set; }

      [Cfg]
      public string Hits { get; set; }

      [Cfg]
      public string DataTypeWarnings { get; set; }

      [Cfg]
      public string SearchType { get; set; }

      [Cfg]
      public string Sortable { get; set; }

      [Cfg]
      public string IgnoreDuplicateKey { get; set; }

      [Cfg]
      public string Locale { get; set; }
   }
}