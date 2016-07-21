#region license
// Transformalize
// A Configurable ETL Solution Specializing in Incremental Denormalization.
// Copyright 2013 Dale Newman
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
using System.Linq;
using Cfg.Net.Environment;
using Cfg.Net.Shorthand;
using Pipeline.Configuration;
using Pipeline.Context;
using Pipeline.Contracts;
using Pipeline.DotNetFiddle.Impl.Transforms;
using Pipeline.Nulls;
using Pipeline.Transforms;
using Pipeline.Transforms.System;
using Pipeline.Validators;

namespace Pipeline.DotNetFiddle.Impl {

    public static class Factory {

        public static Process CreateProcess(string cfg, Dictionary<string, string> parameters = null) {
            var shortHandRoot = new ShorthandRoot(Settings.ShortHand);

            return new Process(
                cfg,
                new ShorthandValidator(shortHandRoot, "sh"),
                new ShorthandModifier(shortHandRoot, "sh"),
                new EnvironmentModifier(new PlaceHolderModifier(), new ParameterModifier()),
                new PlaceHolderModifier(),
                new PlaceHolderValidator()
            );

        }

        public static IProcessController CreateController(Process process, LogLevel logLevel = LogLevel.None) {

            var logger = new ConsoleLogger(logLevel);
            var pipelines = new List<IPipeline>();

            // entity-level pipelines
            foreach (var entity in process.Entities) {
                var entityContext = new PipelineContext(logger, process, entity);
                var entityPipeline = new DefaultPipeline(new NullOutputController(), entityContext);
                var entityInputContext = new InputContext(entityContext, new Incrementer(entityContext));
                var entityRowFactory = new RowFactory(entityInputContext.RowCapacity, entity.IsMaster, false);
                var entityOutputContext = new OutputContext(entityContext, new Incrementer(entityContext));

                switch (entityInputContext.Connection.Provider) {
                    case "web":
                        entityPipeline.Register(new WebCsvReader(entityInputContext, entityRowFactory));
                        break;
                    default:
                        entityPipeline.Register(new InternalReader(entityInputContext, entityRowFactory));
                        break;
                }

                entityPipeline.Register(new SetSystemFields(entityContext));
                entityPipeline.Register(new DefaultTransform(entityContext, entityContext.GetAllEntityFields()));

                // js and razor not active (for now)
                var js = new NullTransform(entityContext);
                var razor = new NullTransform(entityContext);

                entityPipeline.Register(GetTransforms(process, entity, entity.GetAllFields().Where(f => f.Transforms.Any()), logger, js, razor));
                entityPipeline.Register(new StringTruncateTransfom(entityContext));

                entityPipeline.Register(new NullUpdater(entityContext, false));
                entityPipeline.Register(new ConsoleWriter(new CsvSerializer(entityOutputContext)));

                pipelines.Add(entityPipeline);
            }

            var calculatedProcess = process.ToCalculatedFieldsProcess();
            var calculatedEntity = calculatedProcess.Entities.First();
            var calculatedContext = new PipelineContext(logger, calculatedProcess, calculatedEntity);
            var calculatedPipeline = new DefaultPipeline(new NullOutputController(), calculatedContext);

            calculatedPipeline.Register(new NullUpdater(calculatedContext, false));
            calculatedPipeline.Register(new NullWriter(calculatedContext, false));  // future home of console writer that joins entities together for process output
            calculatedPipeline.Register(new NullReader(calculatedContext, false));

            calculatedPipeline.Register(new DefaultTransform(calculatedContext, calculatedEntity.CalculatedFields));
            calculatedPipeline.Register(GetTransforms(calculatedProcess, calculatedEntity, calculatedEntity.CalculatedFields, logger, new NullTransform(calculatedContext), new NullTransform(calculatedContext)));
            calculatedPipeline.Register(new StringTruncateTransfom(calculatedContext));

            pipelines.Add(calculatedPipeline);
            return new ProcessController(pipelines, new PipelineContext(logger, process));
        }

        public static IEnumerable<ITransform> GetTransforms(Process process, Entity entity, IEnumerable<Field> fields, IPipelineLogger logger, ITransform js, ITransform razor) {
            var transforms = new List<ITransform>();
            foreach (var f in fields.Where(f => f.Transforms.Any())) {
                var field = f;
                if (field.RequiresCompositeValidator()) {
                    transforms.Add(new CompositeValidator(
                        new PipelineContext(logger, process, entity, field),
                        field.Transforms.Select(t => ShouldRunTransform(new PipelineContext(logger, process, entity, field, t), js, razor))
                        ));
                } else {
                    transforms.AddRange(field.Transforms.Select(t => ShouldRunTransform(new PipelineContext(logger, process, entity, field, t), js, razor)));
                }
            }
            return transforms;
        }

        private static ITransform ShouldRunTransform(PipelineContext context, ITransform js, ITransform razor) {
            return context.Transform.ShouldRun == null ? SwitchTransform(context, js, razor) : new ShouldRunTransform(context, SwitchTransform(context, js, razor));
        }

        private static ITransform SwitchTransform(PipelineContext context, ITransform js, ITransform razor) {

            switch (context.Transform.Method) {
                case "add":
                case "sum":
                    return new AddTransform(context);
                case "multiply": return new MultiplyTransform(context);
                case "convert": return new ConvertTransform(context);
                case "toyesno": return new ToYesNoTransform(context);
                case "regexreplace": return new CompiledRegexReplaceTransform(context);
                case "match": return new CompiledRegexMatchTransform(context);
                case "replace": return new ReplaceTransform(context);
                case "now": return new UtcNowTransform(context);
                case "timeago": return new RelativeTimeTransform(context, true);
                case "timeahead": return new RelativeTimeTransform(context, false);
                case "format": return new FormatTransform(context);
                case "substring": return new SubStringTransform(context);
                case "left": return new LeftTransform(context);
                case "right": return new RightTransform(context);
                case "copy": return new CopyTransform(context);
                case "concat": return new ConcatTransform(context);
                case "htmldecode": return new DecodeTransform(context);
                case "xmldecode": return new DecodeTransform(context);
                case "hashcode": return new HashcodeTransform(context);
                case "padleft": return new PadLeftTransform(context);
                case "padright": return new PadRightTransform(context);
                case "splitlength": return new SplitLengthTransform(context);
                case "timezone": return new TimeZoneTransform(context);
                case "trim": return new TrimTransform(context);
                case "trimstart": return new TrimStartTransform(context);
                case "trimend": return new TrimEndTransform(context);
                case "insert": return new InsertTransform(context);
                case "remove": return new RemoveTransform(context);
                case "js": return js;
                case "cs":
                case "csharp": return new CsharpTransform(context);
                case "tostring": return new ToStringTransform(context);
                case "upper":
                case "toupper": return new ToUpperTransform(context);
                case "lower":
                case "tolower": return new ToLowerTransform(context);
                case "join": return new JoinTransform(context);
                case "decompress": return new DecompressTransform(context);
                case "next": return new NextTransform(context);
                case "last": return new LastTransform(context);
                case "datepart": return new DatePartTransform(context);
                case "datediff": return new DateDiffTransform(context);
                case "totime": return new ToTimeTransform(context);
                case "razor": return razor;
                case "connection": return new ConnectionTransform(context);
                case "filename": return new FileNameTransform(context);
                case "fileext": return new FileExtTransform(context);
                case "filepath": return new FilePathTransform(context);
                case "xpath": return new XPathTransform(context);
                case "coalesce": return new CoalesceTransform(context);
                case "invert": return new InvertTransform(context);
                case "tag": return new TagTransform(context);

                case "fromxml": return new FromXmlTransform(context);
                case "fromsplit": return new FromSplitTransform(context);
                case "fromlengths": return new FromLengthsTranform(context);

                // return true or false, validators
                case "any": return new AnyValidator(context);
                case "startswith": return new StartsWithValidator(context);
                case "endswith": return new EndsWithValidator(context);
                case "in": return new InValidator(context);
                case "contains": return new ContainsValidator(context);
                case "is": return new IsValidator(context);
                case "equal":
                case "equals": return new EqualsValidator(context);
                case "isempty": return new IsEmptyValidator(context);
                case "isdefault": return new IsDefaultValidator(context);

                default:
                    context.Warn("The {0} method is undefined.", context.Transform.Method);
                    return new NullTransformer(context);
            }
        }

    }

}