#region license
// Transformalize
// Configurable Extract, Transform, and Load
// Copyright © 2013-2023 Dale Newman
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
using Cfg.Net.Contracts;
using Jint;
using System;
using System.Collections.Generic;
using System.Linq;
using Transformalize.Configuration;
using Transformalize.Contracts;

namespace Transformalize.Transforms.Jint {

   public class JintTransform : BaseTransform {

      private readonly Field[] _input;
      private readonly Engine _jint = new Engine();
      private readonly ParameterMatcher _parameterMatcher = new ParameterMatcher();

      public JintTransform(IReader reader = null, IContext context = null) : base(context, null) {

         if (IsMissingContext()) {
            return;
         }

         Returns = Context.Field.Type;

         if (IsMissing(Context.Operation.Script)) {
            return;
         }

         var scriptReader = new ScriptReader(context, reader);

         // to support shorthand script (e.g. t="js(scriptName)")
         if (Context.Operation.Scripts.Count == 0) {
            var script = Context.Process.Scripts.FirstOrDefault(s => s.Name == Context.Operation.Script);
            if (script != null) {
               Context.Operation.Script = scriptReader.Read(script);
            }
         }

         var tester = new ScriptTester(context);

         if (tester.Passes(Context.Operation.Script)) {
            // automatic parameter binding
            if (!Context.Operation.Parameters.Any()) {
               var parameters = _parameterMatcher.Match(Context.Operation.Script, Context.GetAllEntityFields());
               foreach (var parameter in parameters) {
                  Context.Operation.Parameters.Add(new Parameter { Field = parameter, Entity = Context.Entity.Alias });
               }
            }
         } else {
            Run = false;
            return;
         }

         // for js, always add the input parameter
         _input = MultipleInput().Union(new[] { Context.Field }).Distinct().ToArray();

         if (Context.Process.Scripts.Any(s => s.Global && (s.Language == "js" || s.Language == Constants.DefaultSetting && s.File.EndsWith(".js", StringComparison.OrdinalIgnoreCase)))) {
            // load any global scripts
            foreach (var sc in Context.Process.Scripts.Where(s => s.Global && (s.Language == "js" || s.Language == Constants.DefaultSetting && s.File.EndsWith(".js", StringComparison.OrdinalIgnoreCase)))) {
               var content = scriptReader.Read(Context.Process.Scripts.First(s => s.Name == sc.Name));
               if (tester.Passes(content)) {
                  _jint.Execute(content);
               } else {
                  Run = false;
                  return;
               }
            }
         }

         // load any specified scripts
         if (Context.Operation.Scripts.Any()) {
            foreach (var sc in Context.Operation.Scripts) {
               var content = scriptReader.Read(Context.Process.Scripts.First(s => s.Name == sc.Name));
               if (tester.Passes(content)) {
                  _jint.Execute(content);
               } else {
                  Run = false;
                  return;
               }
            }
         }

         Context.Debug(() => $"Script in {Context.Field.Alias} : {Context.Operation.Script.Replace("{", "{{").Replace("}", "}}")}");
      }

      public override IRow Operate(IRow row) {
         throw new NotImplementedException();
      }

      public override IEnumerable<IRow> Operate(IEnumerable<IRow> rows) {

         if (!Run) {
            foreach (var row in rows) {
               yield return row;
            }
            yield break;
         }

         bool tryFirst = true;
         foreach (var row in rows) {
            foreach (var field in _input) {
               _jint.SetValue(field.Alias, row[field]);
            }
            if (tryFirst) {
               try {
                  tryFirst = false;
                  var obj = _jint.Evaluate(Context.Operation.Script).ToObject();
                  var value = obj == null ? null : Context.Field.Convert(obj);
                  if (value == null) {
                     Context.Error($"Jint transform in {Context.Field.Alias} returns null!");
                  } else {
                     row[Context.Field] = value;
                  }
               } catch (global::Jint.Runtime.JavaScriptException jse) {
                  Utility.CodeToError(Context, Context.Operation.Script);
                  Context.Error(jse, "Error Message: " + jse.Message);
                  Context.Error("Variables:");
                  foreach (var field in _input) {
                     Context.Error($"{field.Alias}:{row[field]}");
                  }
               }
            } else {
               row[Context.Field] = Context.Field.Convert(_jint.Evaluate(Context.Operation.Script).ToObject());
            }

            yield return row;
         }
      }

      public override IEnumerable<OperationSignature> GetSignatures() {
         yield return new OperationSignature("jint") {
            Parameters = new List<OperationParameter> { new OperationParameter("script") }
         };
         yield return new OperationSignature("js") {
            Parameters = new List<OperationParameter> { new OperationParameter("script") }
         };
         yield return new OperationSignature("javascript") {
            Parameters = new List<OperationParameter> { new OperationParameter("script") }
         };
      }

   }
}
