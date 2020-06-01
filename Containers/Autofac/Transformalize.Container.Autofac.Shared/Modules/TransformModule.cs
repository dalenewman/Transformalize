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
using System.IO;
using System.Linq;
using System.Reflection;
using Autofac;
using Cfg.Net.Shorthand;
using Transformalize.Configuration;
using Transformalize.Context;
using Transformalize.Contracts;
using Module = Autofac.Module;

namespace Transformalize.Containers.Autofac.Modules {

   /// <inheritdoc />
   /// <summary>
   /// Registers all the built-in transforms
   /// </summary>
   public class TransformModule : Module {

      private readonly List<TransformHolder> _transforms = new List<TransformHolder>();

      public const string FieldsName = "shorthand-tf";
      public const string ParametersName = "shorthand-tp";
      private readonly HashSet<string> _methods;
      private readonly ShorthandRoot _shortHand;
      private readonly Process _process;
      private readonly IPipelineLogger _logger;

      public TransformModule(Process process, HashSet<string> methods, ShorthandRoot shortHand, IPipelineLogger logger) {
         _process = process;
         _logger = logger;
         _methods = methods;
         _shortHand = shortHand;
      }

      public TransformModule(Process process, IPipelineLogger logger) {
         _process = process;
         _logger = logger;
         _methods = new HashSet<string>();
         _shortHand = new ShorthandRoot();
      }

      protected override void Load(ContainerBuilder builder) {

         new TransformBuilder(_process, builder, _methods, _shortHand, _transforms, _logger).Build();

#if PLUGINS

         var context = new PipelineContext(_logger, _process);

         builder.Properties["ShortHand"] = _shortHand;
         builder.Properties["Methods"] = _methods;
         builder.Properties["Process"] = _process;

         var pluginsFolder = Path.Combine(AssemblyDirectory, "plugins");
         if (Directory.Exists(pluginsFolder)) {

            var assemblies = new List<Assembly>();
            foreach (var file in Directory.GetFiles(pluginsFolder, "Transformalize.Transform.*.Autofac.dll", SearchOption.TopDirectoryOnly)) {
               var info = new FileInfo(file);
               var name = info.Name.ToLower().Split('.').FirstOrDefault(f => f != "dll" && f != "transformalize" && f != "transform" && f != "autofac");
               context.Debug(() => $"Loading {name} transform(s)");
               var assembly = Assembly.LoadFile(new FileInfo(file).FullName);
               assemblies.Add(assembly);
            }
            if (assemblies.Any()) {
               builder.RegisterAssemblyModules(assemblies.ToArray());
            }
         }
#endif

      }

      public void AddTransform(TransformHolder t) {
         _transforms.Add(t);
      }

      public static string AssemblyDirectory {
         get {
            var codeBase = typeof(Process).Assembly.CodeBase;
            var uri = new UriBuilder(codeBase);
            var path = Uri.UnescapeDataString(uri.Path);
            return Path.GetDirectoryName(path);
         }
      }
   }
}
