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
using Autofac;
using Cfg.Net.Shorthand;
using Transformalize.Actions;
using Transformalize.Configuration;
using Transformalize.Context;
using Transformalize.Contracts;
using Transformalize.Nulls;
using Transformalize.Providers.File.Actions;
using Transformalize.Providers.File.Transforms;

namespace Transformalize.Providers.File.Autofac {
   public class FileModule : Module {

      private Process _process;
      private readonly HashSet<string> _fileActions = new HashSet<string> { "copy", "move", "archive", "replace" };
      private HashSet<string> _methods;
      private ShorthandRoot _shortHand;
      private readonly HashSet<string> _conflicts = new HashSet<string>();

      public FileModule() { }

      public FileModule(Process process) {
         _process = process;
      }

      protected override void Load(ContainerBuilder builder) {

         // get methods and shorthand from builder
         _methods = builder.Properties.ContainsKey("Methods") ? (HashSet<string>)builder.Properties["Methods"] : new HashSet<string>();
         _shortHand = builder.Properties.ContainsKey("ShortHand") ? (ShorthandRoot)builder.Properties["ShortHand"] : new ShorthandRoot();

         RegisterTransform(builder, (ctx, c) => new FileExtTransform(c), new FileExtTransform().GetSignatures());
         RegisterTransform(builder, (ctx, c) => new FileNameTransform(c), new FileNameTransform().GetSignatures());
         RegisterTransform(builder, (ctx, c) => new FilePathTransform(c), new FilePathTransform().GetSignatures());
         RegisterTransform(builder, (ctx, c) => new FileReadAllBytesTransform(c), new FileReadAllBytesTransform().GetSignatures());

         if (builder.Properties.ContainsKey("Process")) {
            _process = (Process)builder.Properties["Process"];
         }

         if (_process == null)
            return;

         foreach (var action in _process.Templates.Where(t => t.Enabled).SelectMany(t => t.Actions).Where(a => a.GetModes().Any(m => m == _process.Mode || m == "*"))) {
            if (_fileActions.Contains(action.Type)) {

               builder.Register(ctx => {
                  return SwitchAction(ctx, action);
               }).Named<IAction>(action.Key);
            }
         }

         foreach (var action in _process.Actions.Where(a => a.GetModes().Any(m => m == _process.Mode || m == "*"))) {
            if (_fileActions.Contains(action.Type)) {
               builder.Register(ctx => {
                  return SwitchAction(ctx, action);
               }).Named<IAction>(action.Key);
            }
         }
      }

      private IAction SwitchAction(IComponentContext ctx, Configuration.Action action) {
         var context = new PipelineContext(ctx.Resolve<IPipelineLogger>(), _process);
         switch (action.Type) {
            case "copy":
               return action.InTemplate ? (IAction)
                   new ContentToFileAction(context, action) :
                   new FileCopyAction(context, action);
            case "move":
               return new FileMoveAction(context, action);
            case "archive":
               return new FileArchiveAction(context, action);
            case "replace":
               return new FileReplaceAction(context, action);
            default:
               context.Error($"Attempting to register unsupported file action: {action.Type}");
               return new NullAction();
         }
      }

      private void RegisterTransform(ContainerBuilder builder, Func<IComponentContext, IContext, ITransform> getTransform, IEnumerable<OperationSignature> signatures) {

         foreach (var s in signatures) {
            if (_methods.Add(s.Method)) {

               var method = new Method { Name = s.Method, Signature = s.Method, Ignore = s.Ignore };
               _shortHand.Methods.Add(method);

               var signature = new Signature {
                  Name = s.Method,
                  NamedParameterIndicator = s.NamedParameterIndicator
               };

               foreach (var parameter in s.Parameters) {
                  signature.Parameters.Add(new Cfg.Net.Shorthand.Parameter {
                     Name = parameter.Name,
                     Value = parameter.Value
                  });
               }
               _shortHand.Signatures.Add(signature);
            } else {
               var existingParameters = _shortHand.Signatures.First(sg => sg.Name == s.Method).Parameters;
               if (existingParameters.Count == s.Parameters.Count) {
                  for (int i = 0; i < existingParameters.Count; i++) {
                     if (existingParameters[i].Name != s.Parameters[i].Name) {
                        _conflicts.Add(s.Method);
                        break;
                     }
                  }
               }
            }

            builder.Register((ctx, p) => {
               var context = p.Positional<IContext>(0);
               var transform = getTransform(ctx, context);
               if (transform.GetSignatures().Any(sg => _conflicts.Contains(sg.Method))) {
                  var method = transform.GetSignatures().First(sg=>_conflicts.Contains(sg.Method));
                  context.Error($"Conflicting signatures for {method}");
                  return new NullTransform(context);
               } else {
                  return transform;
               }
            }).Named<ITransform>(s.Method);
         }

      }
   }
}