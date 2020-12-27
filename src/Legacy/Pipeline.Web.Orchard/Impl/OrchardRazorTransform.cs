#region license
// Transformalize
// Configurable Extract, Transform, and Load
// Copyright 2013-2017 Dale Newman
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//  
//      http://www.apache.org/licenses/LICENSE-2.0
//  
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
#endregion

using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using Orchard.Templates.Services;
using Transformalize;
using Transformalize.Configuration;
using Transformalize.Contracts;
using Transformalize.Transforms;

namespace Pipeline.Web.Orchard.Impl {
   public class OrchardRazorTransform : BaseTransform {
      private readonly ITemplateProcessor _processor;

      private readonly Field[] _input;
  
      public OrchardRazorTransform(ITemplateProcessor processor, IContext context = null) : base(context, null) {

         if (IsMissingContext()) {
            return;
         }

         Returns = Context.Field.Type;

         _processor = processor;

         var fields = MultipleInput().ToList();
         _input = fields.Union(Context.Entity.GetFieldMatches(Context.Operation.Template)).ToArray();

         // test it out
         var testObject = new ExpandoObject();
         var dict = (IDictionary<string, object>)testObject;
         foreach (var field in _input) {
            dict[field.Alias] = field.DefaultValue();
         }
         try {
            _processor.Process(Context.Operation.Template, Context.Key, null, testObject);
         } catch (System.Exception ex) {
            Context.Error("Razor error parsing script in {0} field.", Context.Field.Alias);
            Context.Error(ex.Message);
            Utility.CodeToError(Context, Context.Operation.Template);
            Run = false;
         }

      }

      public override IRow Operate(IRow row) {
         row[Context.Field] = Context.Field.Convert(_processor.Process(Context.Operation.Template, Context.Key, null, row.ToFriendlyExpandoObject(_input)).Trim(' ', '\n', '\r'));
         return row;
      }

      public override IEnumerable<OperationSignature> GetSignatures() {
         yield return new OperationSignature("razor") { Parameters = new List<OperationParameter>(1) { new OperationParameter("template") } };
      }
   }
}