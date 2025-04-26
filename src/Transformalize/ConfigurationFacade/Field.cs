#region license
// Transformalize
// Configurable Extract, Transform, and Load
// Copyright 2013-2025 Dale Newman
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
using Cfg.Net;

namespace Transformalize.ConfigurationFacade {
   public class Field : CfgNode {

      [Cfg]
      public string Name { get; set; }

      [Cfg]
      public string Type { get; set; }

      [Cfg]
      public string InputType { get; set; }

      [Cfg]
      public string InputAccept { get; set; }

      [Cfg]
      public string InputCapture { get; set; }

      [Cfg]
      public string DefaultWhiteSpace { get; set; }

      [Cfg]
      public string DefaultEmpty { get; set; }

      [Cfg]
      public string Distinct { get; set; }

      [Cfg]
      public string Input { get; set; }

      [Cfg]
      public string Optional { get; set; }

      [Cfg]
      public string Output { get; set; }

      [Cfg]
      public string PrimaryKey { get; set; }

      [Cfg]
      public string Raw { get; set; }

      [Cfg]
      public string ReadInnerXml { get; set; }

      [Cfg]
      public short Index { get; set; }

      [Cfg]
      public string Precision { get; set; }

      [Cfg]
      public string Scale { get; set; }

      [Cfg]
      public string Aggregate { get; set; }

      [Cfg]
      public string Alias { get; set; }

      [Cfg]
      public string Default { get; set; }

      [Cfg]
      public string Delimiter { get; set; }

      [Cfg]
      public string Label { get; set; }

      [Cfg]
      public string Length { get; set; }

      [Cfg]
      public string NodeType { get; set; }

      [Cfg]
      public string SearchType { get; set; }

      [Cfg]
      public string T { get; set; }

      [Cfg]
      public string V { get; set; }

      [Cfg]
      public string ValidField { get; set; }

      [Cfg]
      public string MessageField { get; set; }

      [Cfg]
      public string Unicode { get; set; }

      [Cfg]
      public string VariableLength { get; set; }

      [Cfg]
      public List<Operation> Transforms { get; set; }

      [Cfg]
      public List<Operation> Validators { get; set; }

      [Cfg]
      public List<string> Domain { get; set; }

      [Cfg]
      public short EntityIndex { get; internal set; }

      [Cfg]
      public string System { get; set; }

      [Cfg]
      public string SortField { get; set; }


      [Cfg]
      public string Export { get; set; }


      [Cfg]
      public string Sortable { get; set; }

      [Cfg]
      public string Class { get; set; }
      [Cfg]
      public string Style { get; set; }
      [Cfg]
      public string Role { get; set; }
      [Cfg]
      public string HRef { get; set; }
      [Cfg]
      public string Target { get; set; }

      [Cfg]
      public string Body { get; set; }

      [Cfg]
      public string Src { get; set; }

      [Cfg]
      public string Engine { get; set; }

      [Cfg]
      public string ClassMap { get; set; }

      [Cfg]
      public string Format { get; set; }

      [Cfg]
      public string Facet { get; set; }

      [Cfg]
      public string Learn { get; set; }

      [Cfg]
      public string RunField { get; set; }

      [Cfg]
      public string Dimension { get; set; }

      [Cfg]
      public string Measure { get; set; }

      [Cfg]
      public string AggregateFunction { get; set; }

      [Cfg]
      public string Expression { get; set; }

      [Cfg]
      public string RunOperator { get; set; }

      [Cfg]
      public string RunValue { get; set; }

      [Cfg]
      public string Width { get; set; }

      [Cfg]
      public string Height { get; set; }

      [Cfg]
      public string Help { get; set; }

      [Cfg]
      public string Section { get; set; }

      [Cfg]
      public string Hint { get; set; }

      [Cfg]
      public string Map { get; set; }

      [Cfg]
      public string PostBack { get; set; }

      [Cfg]
      public string Min { get; set; }

      [Cfg]
      public string Max { get; set; }

      [Cfg]
      public string Remote { get; set; }

      [Cfg]
      public string Connection { get; set; }

      [Cfg]
      public string Parameter { get; set; }

      [Cfg]
      public string Property { get; set; }
   }
}