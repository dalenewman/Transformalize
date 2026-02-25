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
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Cfg.Net.Contracts;
using RazorEngineCore;
using Transformalize.Contracts;

namespace Transformalize.Providers.Razor {

   public class RazorWriter : IWrite {

      private readonly IConnectionContext _output;
      private readonly IReader _templateReader;

      public RazorWriter(IConnectionContext output, IReader templateReader) {
         _templateReader = templateReader;
         _output = output;
      }

      public void Write(IEnumerable<IRow> rows) {
         var l = new Cfg.Net.Loggers.MemoryLogger();
         _output.Debug(() => $"Loading template {_output.Connection.Template}");
         var template = _templateReader.Read(_output.Connection.Template, new Dictionary<string, string>(), l);

         template = Regex.Replace(template, "^@model .+$", string.Empty, RegexOptions.Multiline);

         if (l.Errors().Any()) {
            foreach (var error in l.Errors()) {
               _output.Error(error);
            }
         } else {

            var engine = new RazorEngine();
            IRazorEngineCompiledTemplate<RazorEngineTemplateBase<RazorModel>> compiledTemplate;

            try {
               compiledTemplate = engine.Compile<RazorEngineTemplateBase<RazorModel>>(template, builder => {
                  builder.AddAssemblyReference(typeof(Configuration.Process));
                  builder.AddAssemblyReference(typeof(Cfg.Net.CfgNode));
                  builder.AddAssemblyReferenceByName("System.Collections");
               } );
               // doesn't appear to be a way to stream output yet (in this library), so will just write to string and then file
               var output = compiledTemplate.Run(instance => {
                  instance.Model = new RazorModel() {
                     Process = _output.Process,
                     Entity = _output.Entity,
                     Rows = rows
                  };
               });
               File.WriteAllText(_output.Connection.File, output);
            } catch (RazorEngineCompilationException ex) {
               foreach (var error in ex.Errors) {
                  var line = error.Location.GetLineSpan();
                  _output.Error($"C# error on line {line.StartLinePosition.Line}, column {line.StartLinePosition.Character}.");
                  _output.Error(error.GetMessage());
               }
               _output.Error(ex.Message.Replace("{", "{{").Replace("}", "}}"));
               Utility.CodeToError(_output, template);
            } catch(System.AggregateException ex) {
               _output.Error(ex.Message.Replace("{", "{{").Replace("}", "}}"));
               foreach(var error in ex.InnerExceptions) {
                  _output.Error(error.Message.Replace("{", "{{").Replace("}", "}}"));
               }
               Utility.CodeToError(_output, template);
            }

            // the template must set Model.Entity.Inserts

         }
      }

   }
}
