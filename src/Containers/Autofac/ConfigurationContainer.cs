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

using Autofac;
using Autofac.Core;
using Cfg.Net.Contracts;
using Cfg.Net.Environment;
using Cfg.Net.Reader;
using Cfg.Net.Shorthand;
using System;
using System.Collections.Generic;
using System.Linq;
using Transformalize.Configuration;
using Transformalize.Containers.Autofac.Modules;
using Transformalize.Context;
using Transformalize.Contracts;
using Process = Transformalize.Configuration.Process;

namespace Transformalize.Containers.Autofac {

   /// <summary>
   /// This container deals with the arrangement and how it becomes a process.
   /// Transforms and Validators are registered here as well because their 
   /// short-hand is expanded in the arrangement by customizers before it becomes a process.
   /// </summary>
   public class ConfigurationContainer {

      private readonly List<IModule> _modules = new List<IModule>();
      private readonly List<TransformHolder> _transforms = new List<TransformHolder>();
      private readonly List<ValidatorHolder> _validators = new List<ValidatorHolder>();
      private readonly List<IDependency> _dependencies = new List<IDependency>();

      /// <summary>
      /// Initializes a new instance of the <see cref="ConfigurationContainer"/> class.
      /// </summary>
      public ConfigurationContainer() { }

      /// <summary>
      /// Initializes a new instance of the <see cref="ConfigurationContainer"/> class with modules.
      /// These modules are used to register shorthand (e.g., transforms and validators) that are not in the plugins folder.
      /// </summary>
      /// <param name="args">The modules to register.</param>
      public ConfigurationContainer(params IModule[] args) {
         _modules.AddRange(args);
      }

      /// <summary>
      /// Initializes a new instance of the <see cref="ConfigurationContainer"/> class with custom transforms.
      /// </summary>
      /// <param name="transforms">The transforms to register.</param>
      public ConfigurationContainer(params TransformHolder[] transforms) {
         _transforms.AddRange(transforms);
      }

      /// <summary>
      /// Initializes a new instance of the <see cref="ConfigurationContainer"/> class with custom validators.
      /// </summary>
      /// <param name="validators">The validators to register.</param>
      public ConfigurationContainer(params ValidatorHolder[] validators) {
         _validators.AddRange(validators);
      }

      private readonly HashSet<string> _methods = new HashSet<string>();
      private readonly ShorthandRoot _shortHand = new ShorthandRoot();

      /// <summary>
      /// Creates an Autofac lifetime scope for a configuration string, resolving shorthand and parameters.
      /// </summary>
      /// <param name="cfg">The raw configuration string (XML, JSON, etc.).</param>
      /// <param name="logger">The pipeline logger.</param>
      /// <param name="parameters">Optional parameters for environment/placeholder replacement.</param>
      /// <param name="placeHolderStyle">The style of placeholders, default is @[]. Must be three characters.</param>
      /// <returns>An Autofac <see cref="ILifetimeScope"/> containing the processed <see cref="Process"/>.</returns>
      public ILifetimeScope CreateScope(string cfg, IPipelineLogger logger, IDictionary<string, string> parameters = null, string placeHolderStyle = "@[]") {

         if (placeHolderStyle == null || placeHolderStyle.Length != 3) {
            throw new ArgumentException("The placeHolderStyle parameter must be three characters (e.g. @[], or @())");
         }

         if (parameters == null) {
            parameters = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
         }

         var builder = new ContainerBuilder();

         builder.Register((ctx) => logger).As<IPipelineLogger>();
         builder.Register<IReader>(c => new DefaultReader(new FileReader(), new WebReader())).As<IReader>();

         // register shorthand for the "t" (transform) attribute. 
         // This expands shorthand like t="copy" into the full Transform object.
         var transformModule = new TransformModule(new Process { Name = "TransformShorthand" }, _methods, _shortHand, logger);
         foreach (var t in _transforms) {
            transformModule.AddTransform(t);
         }
         builder.RegisterModule(transformModule);

         // register shorthand for the "v" (validate) attribute.
         // This expands shorthand like v="required" into the full Validate object.
         var validateModule = new ValidateModule(new Process { Name = "ValidateShorthand" }, _methods, _shortHand, logger);
         foreach (var v in _validators) {
            validateModule.AddValidator(v);
         }
         builder.RegisterModule(validateModule);

         // Shorthand customizers are responsible for scanning the configuration and 
         // expanding shorthand attributes before the configuration is fully loaded by Cfg-Net.
         
         // register the validator shorthand
         builder.Register((c, p) => _shortHand).Named<ShorthandRoot>(ValidateModule.FieldsName).InstancePerLifetimeScope();
         builder.Register((c, p) => new ShorthandCustomizer(c.ResolveNamed<ShorthandRoot>(ValidateModule.FieldsName), new[] { "fields", "calculated-fields", "calculatedfields" }, "v", "validators", "method")).Named<IDependency>(ValidateModule.FieldsName).InstancePerLifetimeScope();

         // register the transform shorthand for both fields and parameters
         builder.Register((c, p) => _shortHand).Named<ShorthandRoot>(TransformModule.FieldsName).InstancePerLifetimeScope();
         builder.Register((c, p) => _shortHand).Named<ShorthandRoot>(TransformModule.ParametersName).InstancePerLifetimeScope();
         builder.Register((c, p) => new ShorthandCustomizer(c.ResolveNamed<ShorthandRoot>(TransformModule.FieldsName), new[] { "fields", "calculated-fields", "calculatedfields" }, "t", "transforms", "method")).Named<IDependency>(TransformModule.FieldsName).InstancePerLifetimeScope();
         builder.Register((c, p) => new ShorthandCustomizer(c.ResolveNamed<ShorthandRoot>(TransformModule.ParametersName), new[] { "parameters" }, "t", "transforms", "method")).Named<IDependency>(TransformModule.ParametersName).InstancePerLifetimeScope();

         // store shorthand objects so additional registered modules can add their signatures to them
         builder.Properties["ShortHand"] = _shortHand;
         builder.Properties["Methods"] = _methods;

         foreach (var module in _modules) {
            builder.RegisterModule(module);
         }

         builder.Register((c, p) => _methods).Named<HashSet<string>>("Methods").InstancePerLifetimeScope();
         builder.Register((c, p) => _shortHand).As<ShorthandRoot>().InstancePerLifetimeScope();

         builder.Register(ctx => {

            // If any parameters have transforms, they must be resolved before the main process is built.
            // This also injects environment variable values into the parameters dictionary.
            var transformed = TransformParameters(ctx, cfg, parameters);

            // Dependencies passed to the Cfg-Net loader. 
            // These include the expanded shorthand and parameter modifiers.
            var dependancies = new List<IDependency> {
                 ctx.Resolve<IReader>(),
                 new ParameterModifier(new PlaceHolderReplacer(placeHolderStyle[0], placeHolderStyle[1], placeHolderStyle[2]), "parameters", "name", "value"),
                 ctx.ResolveNamed<IDependency>(TransformModule.FieldsName),
                 ctx.ResolveNamed<IDependency>(TransformModule.ParametersName),
                 ctx.ResolveNamed<IDependency>(ValidateModule.FieldsName)
             };
            dependancies.AddRange(_dependencies);

            return new Process(transformed ?? cfg, parameters, dependancies.ToArray());

         }).As<Process>().InstancePerDependency();
         return builder.Build().BeginLifetimeScope();
      }

      /// <summary>
      /// Resolves transforms applied directly to parameters by running a "mini-pipeline".
      /// This is necessary before the main process is instantiated because parameters may affect global settings.
      /// Also injects environment variable values for parameters that declare an <c>env</c> attribute,
      /// provided the caller has not already supplied a value for that parameter name.
      /// </summary>
      /// <param name="ctx">The Autofac component context.</param>
      /// <param name="cfg">The raw configuration string.</param>
      /// <param name="callerParameters">The caller-supplied parameters (CLI / query-string). Env-var values are added here when not already present.</param>
      /// <returns>A serialized configuration string with resolved parameter values, or null if no parameter transforms were found.</returns>
      private string TransformParameters(IComponentContext ctx, string cfg, IDictionary<string, string> callerParameters) {

         var parameters = new Dictionary<string, string>();
         var dependencies = new List<IDependency>() {
            ctx.Resolve<IReader>(),
            new DateMathModifier(),
            new ParameterModifier(new NullPlaceHolderReplacer()),
            ctx.ResolveNamed<IDependency>(TransformModule.ParametersName)
         };
         dependencies.AddRange(_dependencies);

         var preProcess = new ConfigurationFacade.Process(cfg, parameters, dependencies.ToArray());

         // Inject environment variable values for parameters that declare env="VAR_NAME",
         // but only when the caller has not already provided a value for that parameter.
         foreach (var p in preProcess.Parameters) {
            if (!string.IsNullOrEmpty(p.Env) && !callerParameters.ContainsKey(p.Name)) {
               var envValue = Environment.GetEnvironmentVariable(p.Env);
               if (envValue != null) {
                  callerParameters[p.Name] = envValue;
               }
            }
         }

         if (!preProcess.Parameters.Any(pr => pr.Transforms.Any()))
            return null;

         var fields = preProcess.Parameters.Select(pr => new Field {
            Name = pr.Name,
            Alias = pr.Name,
            Default = pr.Value,
            Type = pr.Type,
            Transforms = pr.Transforms.Select(o => o.ToOperation()).ToList()
         }).ToList();
         var len = fields.Count;
         var entity = new Entity { Name = "Parameters", Alias = "Parameters", Fields = fields };
         var mini = new Process {
            Name = "ParameterTransform",
            ReadOnly = true,
            Entities = new List<Entity> { entity },
            Maps = preProcess.Maps.Select(m => m.ToMap()).ToList(), // for map transforms
            Scripts = preProcess.Scripts.Select(s => s.ToScript()).ToList() // for transforms that use scripts (e.g. js)
         };

         mini.Load(); // very important to check after creating, as it runs validation and even modifies!

         if (!mini.Errors().Any()) {

            // modification in Load() do not make it out to local variables so overwrite them
            fields = mini.Entities.First().Fields;
            entity = mini.Entities.First();

            var c = new PipelineContext(ctx.Resolve<IPipelineLogger>(), mini, entity);
            var transforms = new List<ITransform> {
               new Transforms.System.DefaultTransform(c, fields)
            };
            transforms.AddRange(TransformFactory.GetTransforms(ctx, c, fields));

            // make an input out of the parameters
            var input = new List<IRow>();
            var row = new MasterRow(len);
            for (var i = 0; i < len; i++) {
               row[fields[i]] = preProcess.Parameters[i].Value;
            }

            input.Add(row);

            var output = transforms.Aggregate(input.AsEnumerable(), (rows, t) => t.Operate(rows)).ToList().First();

            for (var i = 0; i < len; i++) {
               var parameter = preProcess.Parameters[i];
               parameter.Value = output[fields[i]].ToString();
               parameter.T = string.Empty;
               parameter.Transforms.Clear();
            }

            return preProcess.Serialize();
         }

         var context = new PipelineContext(ctx.Resolve<IPipelineLogger>(), mini, entity);
         foreach (var error in mini.Errors()) {
            context.Error(error);
         }

         return null;
      }

      /// <summary>
      /// Manually adds a validator and its shorthand signatures to the container.
      /// </summary>
      /// <param name="getValidator">A factory function to create the validator.</param>
      /// <param name="signatures">The shorthand signatures for the validator.</param>
      public void AddValidator(Func<IContext, IValidate> getValidator, IEnumerable<OperationSignature> signatures) {
         _validators.Add(new ValidatorHolder(getValidator, signatures));
      }

      /// <summary>
      /// Manually adds a transform and its shorthand signatures to the container.
      /// </summary>
      /// <param name="getTransform">A factory function to create the transform.</param>
      /// <param name="signatures">The shorthand signatures for the transform.</param>
      public void AddTransform(Func<IContext, ITransform> getTransform, IEnumerable<OperationSignature> signatures) {
         _transforms.Add(new TransformHolder(getTransform, signatures));
      }

      /// <summary>
      /// Adds a custom dependency that can modify the configuration before it becomes a process.
      /// </summary>
      /// <param name="customizer">The customizer to add.</param>
      [Obsolete("This method is obsolete.  Use AddDependency instead.")]
      public void AddCustomizer(ICustomizer customizer) {
         _dependencies.Add(customizer);
      }

      /// <summary>
      /// Adds a dependency used during the configuration parsing and expansion phase.
      /// </summary>
      /// <param name="dependency">The dependency to add.</param>
      public void AddDependency(IDependency dependency) {
         _dependencies.Add(dependency);
      }

      /// <summary>
      /// Registers an Autofac module that will be included in the configuration scope.
      /// </summary>
      /// <param name="module">The module to add.</param>
      public void AddModule(IModule module) {
         _modules.Add(module);
      }

   }

}
