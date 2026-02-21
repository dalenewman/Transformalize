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

using System;
using System.Collections.Generic;
using System.Linq;
using Cfg.Net.Contracts;
using Jint;
using Jint.Native;
using Transformalize.Configuration;
using Transformalize.Contracts;

namespace Transformalize.Validators.Jint {

   /// <summary>
   /// Must return a true or false.  May set the message field.
   /// </summary>
   public class JintValidator : BaseValidate {

      private readonly Field[] _input;
      private readonly Engine _jint = new Engine();
      private readonly ParameterMatcher _parameterMatcher = new ParameterMatcher();
      private readonly bool _hasHelp;

      public JintValidator(IReader reader = null, IContext context = null) : base(context) {

         if (context == null) {
            return;
         }

         if (IsMissing(Context.Operation.Script)) {
            return;
         }

         _hasHelp = Context.Field.Help != string.Empty;

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
         var input = MultipleInput().Union(new[] { Context.Field }).Distinct().ToList();

         //add the message field so it can be modified
         if (input.All(f => f.Alias != MessageField.Alias)) {
            input.Add(Context.Entity.GetAllFields().First(f => f.Alias == MessageField.Alias));
         }
         _input = input.ToArray();

         if (Context.Process.Scripts.Any(s => s.Global && (s.Language == "js" || s.Language == Constants.DefaultSetting && s.File != null && s.File.EndsWith(".js", StringComparison.OrdinalIgnoreCase)))) {
            // load any global scripts
            foreach (var sc in Context.Process.Scripts.Where(s => s.Global && (s.Language == "js" || s.Language == Constants.DefaultSetting && s.File != null && s.File.EndsWith(".js", StringComparison.OrdinalIgnoreCase)))) {
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

         // add maps (as objects)
         foreach (var map in Context.Process.Maps) {
            if (Context.Operation.Script.Contains(map.Name)) {
               var obj = new JsObject(_jint);
               foreach (var item in map.Items) {
                  obj.FastSetDataProperty(item.From.ToString(), JsValue.FromObject(_jint, item.To));
               }
               _jint.SetValue(map.Name, obj);
            }
         }

         Context.Debug(() => $"Script in {Context.Field.Alias} : {Context.Operation.Script.Replace("{", "{{").Replace("}", "}}")}");
      }

      public override IRow Operate(IRow row) {
         foreach (var field in _input) {
            _jint.SetValue(field.Alias, row[field]);
         }
         try {
            var value = _jint.Evaluate(Context.Operation.Script).ToObject();
            if (value == null) {
               Context.Error($"Jint transform in {Context.Field.Alias} returns null!");
            } else {
               switch (value) {
                  case double resultDouble:
                     if (resultDouble > 0.0 || resultDouble < 0.0) {
                        AppendResult(row, true);
                     } else {
                        AppendMessage(row, _hasHelp ? Context.Field.Help : _jint.GetValue(MessageField.Alias).ToString());
                        AppendResult(row, false);
                     }
                     break;
                  case bool resultBool:
                     if (resultBool) {
                        AppendResult(row, true);
                     } else {
                        AppendMessage(row, _hasHelp ? Context.Field.Help : _jint.GetValue(MessageField.Alias).ToString());
                        AppendResult(row, false);
                     }
                     break;
                  case int resultInt:
                     if (resultInt != 0) {
                        AppendResult(row, true);
                     } else {
                        AppendMessage(row, _hasHelp ? Context.Field.Help : _jint.GetValue(MessageField.Alias).ToString());
                        AppendResult(row, false);
                     }
                     break;
                  case string resultString:
                     if (string.IsNullOrEmpty(resultString)) {
                        AppendMessage(row, _hasHelp ? Context.Field.Help : _jint.GetValue(MessageField.Alias).ToString());
                        AppendResult(row, false);
                     } else {
                        var lower = resultString.ToLower();
                        if (lower == "false" || lower == "0") {
                           AppendMessage(row, _hasHelp ? Context.Field.Help : _jint.GetValue(MessageField.Alias).ToString());
                           AppendResult(row, false);
                        } else {
                           AppendResult(row, true);
                        }
                     }
                     break;
                  default:
                     AppendMessage(row, _hasHelp ? Context.Field.Help : _jint.GetValue(MessageField.Alias).ToString());
                     AppendResult(row, false);
                     break;
               }

               return row;
            }
         } catch (global::Jint.Runtime.JavaScriptException jse) {
            Utility.CodeToError(Context, Context.Operation.Script);
            Context.Error(jse, "Error Message: " + jse.Message);
            Context.Error("Variables:");
            foreach (var field in _input) {
               Context.Error($"{field.Alias}:{row[field]}");
            }
         }

         return row;
      }

      public override IEnumerable<OperationSignature> GetSignatures() {
         yield return new OperationSignature("jint") {
            Parameters = new List<OperationParameter> { new OperationParameter("script") }
         };
         yield return new OperationSignature("js") {
            Parameters = new List<OperationParameter> { new OperationParameter("script") }
         };
      }

   }
}
