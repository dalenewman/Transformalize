#region license
// Transformalize
// Configurable Extract, Transform, and Load
// Copyright 2013-2026 Dale Newman
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
using Autofac;
using Lucene.Net.Analysis;
using Lucene.Net.Analysis.Core;
using Lucene.Net.Analysis.Miscellaneous;
using Lucene.Net.Analysis.Standard;
using Lucene.Net.Util;
using Transformalize.Configuration;
using Transformalize.Context;
using Transformalize.Contracts;
using Transformalize.Impl;
using Transformalize.Nulls;
using Transformalize.Transforms.System;

namespace Transformalize.Providers.Lucene.Autofac {

   public class LuceneModule : Module {

      private const string DefaultAnalyzer = "keyword";

      protected override void Load(ContainerBuilder builder) {

         if (!builder.Properties.ContainsKey("Process")) {
            return;
         }

         var process = (Process)builder.Properties["Process"];

         // Connections
         foreach (var c in process.Connections.Where(cn => cn.Provider == "lucene")) {

            switch (c.Provider) {
               case "lucene":
                  // Analyzers
                  builder.Register<Analyzer>(ctx => new KeywordAnalyzer()).Named<Analyzer>(DefaultAnalyzer);
                  foreach (var analyzer in process.SearchTypes.Where(st => st.Analyzer != string.Empty && st.Analyzer != DefaultAnalyzer).Select(st => st.Analyzer).Distinct()) {
                     switch (analyzer) {
                        case "simple":
                           builder.Register<Analyzer>(ctx => new SimpleAnalyzer(LuceneVersion.LUCENE_48)).Named<Analyzer>(analyzer);
                           break;
                        case "whitespace":
                           builder.Register<Analyzer>(ctx => new WhitespaceAnalyzer(LuceneVersion.LUCENE_48)).Named<Analyzer>(analyzer);
                           break;
                        case "standard":
                           builder.Register<Analyzer>(ctx => new StandardAnalyzer(LuceneVersion.LUCENE_48)).Named<Analyzer>(analyzer);
                           break;
                        default:
                           builder.Register<Analyzer>(ctx => new KeywordAnalyzer()).Named<Analyzer>(analyzer);
                           break;
                     }
                  }

                  // entity index writers
                  foreach (var e in process.Entities) {

                     // Directory
                     builder.Register(ctx => new DirectoryFactory(Path.Combine(c.Folder, e.Alias))).Named<DirectoryFactory>(e.Key);

                     // Per Field Analyzer
                     builder.Register<Analyzer>(ctx => {
                        var fieldAnalyzers = new Dictionary<string, Analyzer>();
                        var context = ctx.ResolveNamed<OutputContext>(e.Key);
                        foreach (var field in new FieldSearchTypes(context.Process, context.OutputFields)) {
                           if (field.SearchType.Name != "none") {
                              fieldAnalyzers[field.Field.Alias] = ctx.ResolveNamed<Analyzer>(field.SearchType.Analyzer == string.Empty ? DefaultAnalyzer : field.SearchType.Analyzer);
                           }
                        }
                        return new PerFieldAnalyzerWrapper(ctx.ResolveNamed<Analyzer>(DefaultAnalyzer), fieldAnalyzers);
                     }).Named<Analyzer>(e.Key + ReadFrom.Output);

                     builder.Register<Analyzer>(ctx => {
                        var fieldAnalyzers = new Dictionary<string, Analyzer>();
                        var context = ctx.ResolveNamed<InputContext>(e.Key);
                        foreach (var field in new FieldSearchTypes(context.Process, context.InputFields)) {
                           if (field.SearchType.Name != "none") {
                              fieldAnalyzers[field.Field.Name] = ctx.ResolveNamed<Analyzer>(field.SearchType.Analyzer == string.Empty ? DefaultAnalyzer : field.SearchType.Analyzer);
                           }
                        }
                        return new PerFieldAnalyzerWrapper(ctx.ResolveNamed<Analyzer>(DefaultAnalyzer));
                     }).Named<Analyzer>(e.Key + ReadFrom.Input);

                     // Index Writer Factory
                     builder.Register(ctx => new IndexWriterFactory(ctx.ResolveNamed<DirectoryFactory>(e.Key), ctx.ResolveNamed<Analyzer>(e.Key + ReadFrom.Output))).Named<IndexWriterFactory>(e.Key);

                     // Index Reader Factory
                     builder.Register(ctx => new IndexReaderFactory(ctx.ResolveNamed<DirectoryFactory>(e.Key), ctx.ResolveNamed<IndexWriterFactory>(e.Key))).Named<IndexReaderFactory>(e.Key);

                     // Index Searcher Factory
                     builder.Register(ctx => new SearcherFactory(ctx.ResolveNamed<IndexReaderFactory>(e.Key))).Named<SearcherFactory>(e.Key);

                  }

                  break;
            }
         }

         // entity input
         foreach (var entity in process.Entities.Where(e => process.Connections.First(c => c.Name == e.Input).Provider == "lucene")) {

            // INPUT VERSION DETECTOR
            builder.Register<IInputProvider>(ctx => {
               var input = ctx.ResolveNamed<InputContext>(entity.Key);
               switch (input.Connection.Provider) {
                  case "lucene":
                     return new LuceneInputProvider(input, ctx.ResolveNamed<SearcherFactory>(entity.Key));
                  default:
                     return new NullInputProvider();
               }
            }).Named<IInputProvider>(entity.Key);

            // INPUT READER
            builder.Register<IRead>(ctx => {
               var input = ctx.ResolveNamed<InputContext>(entity.Key);
               var rowFactory = ctx.ResolveNamed<IRowFactory>(entity.Key, new NamedParameter("capacity", input.RowCapacity));

               switch (input.Connection.Provider) {
                  case "lucene":
                     return new LuceneReader(input, input.InputFields, ctx.ResolveNamed<SearcherFactory>(entity.Key), ctx.ResolveNamed<Analyzer>(entity.Key + ReadFrom.Input), ctx.ResolveNamed<IndexReaderFactory>(entity.Key), rowFactory, ReadFrom.Input);
                  default:
                     return new NullReader(input, false);
               }
            }).Named<IRead>(entity.Key);
         }

         // entity output
         if (process.GetOutputConnection().Provider == "lucene") {

            // PROCESS OUTPUT CONTROLLER
            builder.Register<IOutputController>(ctx => new NullOutputController()).As<IOutputController>();

            // PROCESS INITIALIZER
            builder.Register<IInitializer>(ctx => new NullInitializer()).As<IInitializer>();

            foreach (var entity in process.Entities) {

               // UPDATER
               builder.Register<IUpdate>(ctx => {
                  var output = ctx.ResolveNamed<OutputContext>(entity.Key);
                  output.Debug(() => $"{output.Connection.Provider} does not denormalize.");
                  return new NullMasterUpdater();
               }).Named<IUpdate>(entity.Key);

               // OUTPUT
               builder.Register<IOutputController>(ctx => {

                  var output = ctx.ResolveNamed<OutputContext>(entity.Key);

                  switch (output.Connection.Provider) {
                     case "lucene":
                        return new LuceneOutputController(
                            output,
                            new LuceneInitializer(output),
                            ctx.ResolveNamed<IInputProvider>(entity.Key),
                            new LuceneOutputProvider(output, ctx.ResolveNamed<SearcherFactory>(entity.Key))
                        );
                     default:
                        return new NullOutputController();
                  }

               }).Named<IOutputController>(entity.Key);

               // WRITER
               builder.Register<IWrite>(ctx => {
                  var output = ctx.ResolveNamed<OutputContext>(entity.Key);

                  switch (output.Connection.Provider) {
                     case "lucene":
                        return new LuceneWriter(output, ctx.ResolveNamed<IndexWriterFactory>(entity.Key), ctx.ResolveNamed<SearcherFactory>(entity.Key));
                     default:
                        return new NullWriter(output);
                  }
               }).Named<IWrite>(entity.Key);

               // DELETE HANDLER
               if (entity.Delete) {
                  builder.Register<IEntityDeleteHandler>(ctx => {

                     var context = ctx.ResolveNamed<IContext>(entity.Key);
                     var inputContext = ctx.ResolveNamed<InputContext>(entity.Key);
                     var rowFactory = ctx.ResolveNamed<IRowFactory>(entity.Key, new NamedParameter("capacity", inputContext.RowCapacity));
                     IRead input = new NullReader(context);
                     var primaryKey = entity.GetPrimaryKey();

                     switch (inputContext.Connection.Provider) {
                        case "lucene":
                           input = new LuceneReader(
                               inputContext,
                               primaryKey,
                               ctx.ResolveNamed<SearcherFactory>(entity.Key),
                               ctx.ResolveNamed<Analyzer>(entity.Key + ReadFrom.Input),
                               ctx.ResolveNamed<IndexReaderFactory>(entity.Key),
                               rowFactory,
                               ReadFrom.Input);
                           break;
                     }

                     IRead output = new NullReader(context);
                     IDelete deleter = new NullDeleter(context);
                     var outputConnection = process.GetOutputConnection();
                     var outputContext = ctx.ResolveNamed<OutputContext>(entity.Key);

                     switch (outputConnection.Provider) {
                        case "lucene":
                           output = new LuceneReader(
                               outputContext,
                               primaryKey,
                               ctx.ResolveNamed<SearcherFactory>(entity.Key),
                               ctx.ResolveNamed<Analyzer>(entity.Key + ReadFrom.Output),
                               ctx.ResolveNamed<IndexReaderFactory>(entity.Key),
                               rowFactory,
                               ReadFrom.Output
                           );
                           //TODO: need LuceneUpdater (update TflDeleted to true)
                           break;
                     }

                     var handler = new DefaultDeleteHandler(context, input, output, deleter);

                     // since the primary keys from the input may have been transformed into the output, you have to transform before comparing
                     // feels a lot like entity pipeline on just the primary keys... may look at consolidating
                     handler.Register(new DefaultTransform(context, entity.GetPrimaryKey().ToArray()));
                     handler.Register(new StringTruncateTransform(context, primaryKey));

                     return handler;
                  }).Named<IEntityDeleteHandler>(entity.Key);
               }


            }
         }
      }
   }
}