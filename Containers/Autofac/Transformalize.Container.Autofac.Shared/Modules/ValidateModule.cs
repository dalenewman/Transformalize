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

using Autofac;
using Cfg.Net.Shorthand;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Transformalize.Configuration;
using Transformalize.Context;
using Transformalize.Contracts;
using Module = Autofac.Module;

namespace Transformalize.Containers.Autofac.Modules {

   /// <inheritdoc />
   /// <summary>
   /// Registers all the built-in validators
   /// </summary>
   public class ValidateModule : Module {

      private readonly List<ValidatorHolder> _validators = new List<ValidatorHolder>();

      public const string FieldsName = "shorthand-vf";
      public const string ParametersName = "shorthand-vp";
      private readonly HashSet<string> _methods;
      private readonly ShorthandRoot _shortHand;
      private readonly Process _process;
      private readonly IPipelineLogger _logger;

      public ValidateModule(Process process, IPipelineLogger logger) {
         _process = process;
         _logger = logger;
         _methods = new HashSet<string>();
         _shortHand = new ShorthandRoot();
      }

      public ValidateModule(Process process, HashSet<string> methods, ShorthandRoot shortHand, IPipelineLogger logger) {
         _process = process;
         _logger = logger;
         _methods = methods;
         _shortHand = shortHand;
      }

      protected override void Load(ContainerBuilder builder) {

         new ValidateBuilder(_process, builder, _methods, _shortHand, _validators, _logger).Build();

#if PLUGINS
         var loadContext = new PipelineContext(_logger, _process);

         builder.Properties["ShortHand"] = _shortHand;
         builder.Properties["Methods"] = _methods;
         builder.Properties["Process"] = _process;

         var pluginsFolder = Path.Combine(AssemblyDirectory, "plugins");
         if (Directory.Exists(pluginsFolder)) {

            var assemblies = new List<Assembly>();

            foreach (var file in Directory.GetFiles(pluginsFolder, "Transformalize.Validate.*.Autofac.dll", SearchOption.TopDirectoryOnly)) {
               var info = new FileInfo(file);
               var name = info.Name.ToLower().Split('.').FirstOrDefault(f => f != "dll" && f != "transformalize" && f != "validate" && f != "autofac");
               loadContext.Debug(() => $"Loading {name} validator(s)");
               var assembly = Assembly.LoadFile(new FileInfo(file).FullName);
               assemblies.Add(assembly);
            }

            if (assemblies.Any()) {
               builder.RegisterAssemblyModules(assemblies.ToArray());
            }
         }
#endif
      }

      public void AddValidator(ValidatorHolder v) {
         _validators.Add(v);
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