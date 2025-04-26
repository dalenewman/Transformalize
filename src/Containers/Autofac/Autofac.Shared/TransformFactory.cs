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

using Autofac;
using System.Collections.Generic;
using System.Linq;
using Transformalize.Configuration;
using Transformalize.Context;
using Transformalize.Contracts;
using Transformalize.Transforms;

namespace Transformalize.Containers.Autofac {

   public static class TransformFactory {

      public static IEnumerable<ITransform> GetTransforms(IComponentContext ctx, IContext context, IEnumerable<Field> fields) {
         var transforms = new List<ITransform>();

         foreach (var f in fields.Where(f => f.Transforms.Any() || f.Validators.Any())) {
            var field = f;
            var fieldTransforms = new List<ITransform>();

            foreach (var t in field.Transforms) {

               if (t.Method == "convert" && t.Type == "string" && t.Format != string.Empty) {
                  t.Method = "tostring";
               }

               var transformContext = new PipelineContext(ctx.Resolve<IPipelineLogger>(), context.Process, context.Entity, field, t);
               if (TryTransform(ctx, transformContext, out var add)) {
                  fieldTransforms.Add(add);
                  transforms.Add(add);
               }
            }

            // add conversion if necessary
            if (fieldTransforms.Any()) {
               var lastType = fieldTransforms.Last().Returns;
               if (lastType != null && lastType != "object" && field.Type != lastType) {
                  context.Warn($"The output field {field.Alias} is not setup to receive a {lastType} type. It expects a {field.Type}.  Adding conversion.");
                  transforms.Add(new ConvertTransform(new PipelineContext(ctx.Resolve<IPipelineLogger>(), context.Process, context.Entity, field, new Operation { Method = "convert" })));
               }
            }

         }

         return transforms;
      }

      public static bool TryTransform(IComponentContext ctx, IContext context, out ITransform transform) {
         transform = null;
         var success = true;

         if (ctx.IsRegisteredWithName<ITransform>(context.Operation.Method)) {

            var t = ctx.ResolveNamed<ITransform>(context.Operation.Method, new PositionalParameter(0, context));

            foreach (var warning in t.Warnings()) {
               context.Warn(warning);
            }

            if (t.Errors().Any()) {
               foreach (var error in t.Errors()) {
                  context.Error(error);
               }
               success = false;
            } else {
               transform = t;
            }
         } else {

            if (ctx.IsRegisteredWithName<ITransform>(context.Operation.Key)) {
               var t = ctx.ResolveNamed<ITransform>(context.Operation.Key, new PositionalParameter(0, context));

               foreach (var warning in t.Warnings()) {
                  context.Warn(warning);
               }

               if (t.Errors().Any()) {
                  foreach (var error in t.Errors()) {
                     context.Error(error);
                  }
                  success = false;
               } else {
                  transform = t;
               }
            } else {
               context.Error($"The {context.Operation.Method} method used in the {context.Field.Alias} field is not registered.");
               success = false;
            }

         }
         return success;
      }

   }
}