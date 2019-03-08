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
using Cfg.Net.Contracts;
using Cfg.Net.Shorthand;
using JavaScriptEngineSwitcher.ChakraCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Transformalize.Configuration;
using Transformalize.Context;
using Transformalize.Contracts;
using Transformalize.Providers.File.Transforms;
using Transformalize.Providers.Web;
using Transformalize.Transforms;
using Transformalize.Transforms.Compression;
using Transformalize.Transforms.DateMath;
using Transformalize.Transforms.Geography;
using Transformalize.Transforms.Globalization;
using Transformalize.Transforms.Html;
using Transformalize.Transforms.JavaScript;
using Transformalize.Transforms.Json;
using Transformalize.Transforms.Velocity;
using Transformalize.Transforms.Xml;
using Module = Autofac.Module;
using Parameter = Cfg.Net.Shorthand.Parameter;

namespace Transformalize.Ioc.Autofac.Modules {
    public class TransformModule : Module {

        public const string FieldsName = "shorthand-t";
        public const string ParametersName = "shorthand-p";
        private readonly HashSet<string> _methods = new HashSet<string>();
        private readonly ShorthandRoot _shortHand = new ShorthandRoot();
        private readonly Process _process;
        private readonly IPipelineLogger _logger;
        private IContext _context;

        public TransformModule(Process process, IPipelineLogger logger) {
            _process = process;
            _logger = logger;
        }

        protected override void Load(ContainerBuilder builder) {

            _context = new PipelineContext(_logger, _process);

            // set properties for plugins
            builder.Properties["ShortHand"] = _shortHand;
            builder.Properties["Methods"] = _methods;
            builder.Properties["Process"] = _process;

            // new method so transform author can define shorthand signature(s)
            RegisterTransform(builder, (ctx, c) => new AbsTransform(c), new AbsTransform().GetSignatures());
            RegisterTransform(builder, (ctx, c) => new SliceTransform(c), new SliceTransform().GetSignatures());
            RegisterTransform(builder, (ctx, c) => new FormatTransform(c), new FormatTransform().GetSignatures());
            RegisterTransform(builder, (ctx, c) => new AddTransform(c), new AddTransform().GetSignatures());
            RegisterTransform(builder, (ctx, c) => new CeilingTransform(c), new CeilingTransform().GetSignatures());
            RegisterTransform(builder, (ctx, c) => new CoalesceTransform(c), new CoalesceTransform().GetSignatures());
            RegisterTransform(builder, (ctx, c) => new ConcatTransform(c), new ConcatTransform().GetSignatures());
            RegisterTransform(builder, (ctx, c) => new ConvertTransform(c), new ConvertTransform().GetSignatures());
            RegisterTransform(builder, (ctx, c) => new CopyTransform(c), new CopyTransform().GetSignatures());
            RegisterTransform(builder, (ctx, c) => new DateDiffTransform(c), new DateDiffTransform().GetSignatures());
            RegisterTransform(builder, (ctx, c) => new DatePartTransform(c), new DatePartTransform().GetSignatures());
            RegisterTransform(builder, (ctx, c) => new DecompressTransform(c), new DecompressTransform().GetSignatures());
            RegisterTransform(builder, (ctx, c) => new CompressTransform(c), new CompressTransform().GetSignatures());
            RegisterTransform(builder, (ctx, c) => new FileExtTransform(c), new FileExtTransform().GetSignatures());
            RegisterTransform(builder, (ctx, c) => new FileNameTransform(c), new FileNameTransform().GetSignatures());
            RegisterTransform(builder, (ctx, c) => new FilePathTransform(c), new FilePathTransform().GetSignatures());
            RegisterTransform(builder, (ctx, c) => new LastDayTransform(c), new LastDayTransform().GetSignatures());
            RegisterTransform(builder, (ctx, c) => new FloorTransform(c), new FloorTransform().GetSignatures());
            RegisterTransform(builder, (ctx, c) => new FormatXmlTransform(c), new FormatXmlTransform().GetSignatures());
            RegisterTransform(builder, (ctx, c) => new FormatPhoneTransform(c), new FormatPhoneTransform().GetSignatures());
            RegisterTransform(builder, (ctx, c) => new HashcodeTransform(c), new HashcodeTransform().GetSignatures());
            RegisterTransform(builder, (ctx, c) => new DecodeTransform(c), new DecodeTransform().GetSignatures());
            RegisterTransform(builder, (ctx, c) => new HtmlEncodeTransform(c), new HtmlEncodeTransform().GetSignatures());
            RegisterTransform(builder, (ctx, c) => new InsertTransform(c), new InsertTransform().GetSignatures());
            RegisterTransform(builder, (ctx, c) => new InvertTransform(c), new InvertTransform().GetSignatures());
            RegisterTransform(builder, (ctx, c) => new JoinTransform(c), new JoinTransform().GetSignatures());
            RegisterTransform(builder, (ctx, c) => new LeftTransform(c), new LeftTransform().GetSignatures());
            RegisterTransform(builder, (ctx, c) => new RightTransform(c), new RightTransform().GetSignatures());
            RegisterTransform(builder, (ctx, c) => new ToLowerTransform(c), new ToLowerTransform().GetSignatures());
            RegisterTransform(builder, (ctx, c) => new MapTransform(c), new MapTransform().GetSignatures());
            RegisterTransform(builder, (ctx, c) => new RegexMatchTransform(c), new RegexMatchTransform().GetSignatures());
            RegisterTransform(builder, (ctx, c) => new RegexMatchingTransform(c), new RegexMatchingTransform().GetSignatures());
            RegisterTransform(builder, (ctx, c) => new MultiplyTransform(c), new MultiplyTransform().GetSignatures());
            RegisterTransform(builder, (ctx, c) => new NextDayTransform(c), new NextDayTransform().GetSignatures());
            RegisterTransform(builder, (ctx, c) => new UtcNowTransform(c), new UtcNowTransform().GetSignatures());
            RegisterTransform(builder, (ctx, c) => new RowNumberTransform(c), new RowNumberTransform().GetSignatures());
            RegisterTransform(builder, (ctx, c) => new TimeZoneTransform(c), new TimeZoneTransform().GetSignatures());
            RegisterTransform(builder, (ctx, c) => new TimeZoneOffsetTransform(c), new TimeZoneOffsetTransform().GetSignatures());
            RegisterTransform(builder, (ctx, c) => new OppositeTransform(c), new OppositeTransform().GetSignatures());
            RegisterTransform(builder, (ctx, c) => new DateAddTransform(c), new DateAddTransform().GetSignatures());
            RegisterTransform(builder, (ctx, c) => new TimeAheadTransform(c), new TimeAheadTransform().GetSignatures());
            RegisterTransform(builder, (ctx, c) => new TimeAgoTransform(c), new TimeAgoTransform().GetSignatures());
            RegisterTransform(builder, (ctx, c) => new PadLeftTransform(c), new PadLeftTransform().GetSignatures());
            RegisterTransform(builder, (ctx, c) => new PadRightTransform(c), new PadRightTransform().GetSignatures());
            RegisterTransform(builder, (ctx, c) => new RegexReplaceTransform(c), new RegexReplaceTransform().GetSignatures());
            RegisterTransform(builder, (ctx, c) => new RemoveTransform(c), new RemoveTransform().GetSignatures());
            RegisterTransform(builder, (ctx, c) => new ReplaceTransform(c), new ReplaceTransform().GetSignatures());
            RegisterTransform(builder, (ctx, c) => new RoundTransform(c), new RoundTransform().GetSignatures());
            RegisterTransform(builder, (ctx, c) => new RoundToTransform(c), new RoundToTransform().GetSignatures());
            RegisterTransform(builder, (ctx, c) => new RoundUpToTransform(c), new RoundUpToTransform().GetSignatures());
            RegisterTransform(builder, (ctx, c) => new RoundDownToTransform(c), new RoundDownToTransform().GetSignatures());
            RegisterTransform(builder, (ctx, c) => new SplitLengthTransform(c), new SplitLengthTransform().GetSignatures());
            RegisterTransform(builder, (ctx, c) => new SubStringTransform(c), new SubStringTransform().GetSignatures());
            RegisterTransform(builder, (ctx, c) => new TagTransform(c), new TagTransform().GetSignatures());
            RegisterTransform(builder, (ctx, c) => new ToStringTransform(c), new ToStringTransform().GetSignatures());
            RegisterTransform(builder, (ctx, c) => new ToTimeTransform(c), new ToTimeTransform().GetSignatures());
            RegisterTransform(builder, (ctx, c) => new ToYesNoTransform(c), new ToYesNoTransform().GetSignatures());
            RegisterTransform(builder, (ctx, c) => new TrimTransform(c), new TrimTransform().GetSignatures());
            RegisterTransform(builder, (ctx, c) => new TrimStartTransform(c), new TrimStartTransform().GetSignatures());
            RegisterTransform(builder, (ctx, c) => new TrimEndTransform(c), new TrimEndTransform().GetSignatures());
            RegisterTransform(builder, (ctx, c) => new LineTransform(c), new LineTransform().GetSignatures());
            RegisterTransform(builder, (ctx, c) => new VelocityTransform(c, ctx.Resolve<IReader>()), new VelocityTransform().GetSignatures());
            RegisterTransform(builder, (ctx, c) => new ToUpperTransform(c), new ToUpperTransform().GetSignatures());
            RegisterTransform(builder, (ctx, c) => new XPathTransform(c), new XPathTransform().GetSignatures());
            RegisterTransform(builder, (ctx, c) => new IIfTransform(c), new IIfTransform().GetSignatures());
            RegisterTransform(builder, (ctx, c) => new AppendTransform(c), new AppendTransform().GetSignatures());
            RegisterTransform(builder, (ctx, c) => new PrependTransform(c), new PrependTransform().GetSignatures());
            RegisterTransform(builder, (ctx, c) => new GeohashEncodeTransform(c), new GeohashEncodeTransform().GetSignatures());
            RegisterTransform(builder, (ctx, c) => new GeohashNeighborTransform(c), new GeohashNeighborTransform().GetSignatures());
            RegisterTransform(builder, (ctx, c) => new CommonPrefixTransform(c), new CommonPrefixTransform().GetSignatures());
            RegisterTransform(builder, (ctx, c) => new CommonPrefixesTransform(c), new CommonPrefixesTransform().GetSignatures());
            RegisterTransform(builder, (ctx, c) => new DistanceTransform(c), new DistanceTransform().GetSignatures());
            RegisterTransform(builder, (ctx, c) => new SlugifyTransform(c), new SlugifyTransform().GetSignatures());
            RegisterTransform(builder, (ctx, c) => new DateMathTransform(c), new DateMathTransform().GetSignatures());
            RegisterTransform(builder, (ctx, c) => new WebTransform(c), new WebTransform().GetSignatures());
            RegisterTransform(builder, (ctx, c) => new UrlEncodeTransform(c), new UrlEncodeTransform().GetSignatures());
            RegisterTransform(builder, (ctx, c) => new DistinctTransform(c), new DistinctTransform().GetSignatures());
            RegisterTransform(builder, (ctx, c) => new RegexMatchCountTransform(c), new RegexMatchCountTransform().GetSignatures());
            RegisterTransform(builder, (ctx, c) => new CondenseTransform(c), new CondenseTransform().GetSignatures());
            RegisterTransform(builder, (ctx, c) => new RandomTransform(c), new RandomTransform().GetSignatures());
            RegisterTransform(builder, (ctx, c) => new IdentityTransform(c), new IdentityTransform().GetSignatures());
            RegisterTransform(builder, (ctx, c) => new GuidTransform(c), new GuidTransform().GetSignatures());

            // getting properties from the configuration you're running in
            RegisterTransform(builder, (ctx, c) => new ConnectionTransform(c), new ConnectionTransform().GetSignatures());
            RegisterTransform(builder, (ctx, c) => new ParameterTransform(c), new ParameterTransform().GetSignatures());
            RegisterTransform(builder, (ctx, c) => new ActionTransform(c), new ActionTransform().GetSignatures());
            RegisterTransform(builder, (ctx, c) => new ScriptTransform(c), new ScriptTransform().GetSignatures());

            // split
            RegisterTransform(builder, (ctx, c) => new SplitTransform(c), new SplitTransform().GetSignatures());
            RegisterTransform(builder, (ctx, c) => new LastTransform(c), new LastTransform().GetSignatures());
            RegisterTransform(builder, (ctx, c) => new FirstTransform(c), new FirstTransform().GetSignatures());
            RegisterTransform(builder, (ctx, c) => new SortTransform(c), new SortTransform().GetSignatures());
            RegisterTransform(builder, (ctx, c) => new ReverseTransform(c), new ReverseTransform().GetSignatures());
            RegisterTransform(builder, (ctx, c) => new GetTransform(c), new GetTransform().GetSignatures());

            // row producing
            RegisterTransform(builder, (ctx, c) => new ToRowTransform(c, ctx.ResolveNamed<IRowFactory>(c.Entity.Key, new NamedParameter("capacity", c.GetAllEntityFields().Count()))), new ToRowTransform().GetSignatures());

            // row filtering
            RegisterTransform(builder, (ctx, c) => new FilterTransform(FilterType.Include, c), new FilterTransform(FilterType.Include).GetSignatures());
            RegisterTransform(builder, (ctx, c) => new FilterTransform(FilterType.Exclude, c), new FilterTransform(FilterType.Exclude).GetSignatures());

            // field producing
            RegisterTransform(builder, (ctx, c) => new FromSplitTransform(c), new FromSplitTransform().GetSignatures());
            RegisterTransform(builder, (ctx, c) => new FromRegexTransform(c), new FromRegexTransform().GetSignatures());
            RegisterTransform(builder, (ctx, c) => new FromLengthsTransform(c), new FromLengthsTransform().GetSignatures());
            RegisterTransform(builder, (ctx, c) => new FromJsonTransform(c, o => JsonConvert.SerializeObject(o, Formatting.None)), new FromJsonTransform().GetSignatures());

            // return true or false transforms
            RegisterTransform(builder, (ctx, c) => new AnyTransform(c), new AnyTransform().GetSignatures());
            RegisterTransform(builder, (ctx, c) => new StartsWithTransform(c), new StartsWithTransform().GetSignatures());
            RegisterTransform(builder, (ctx, c) => new EndsWithTransform(c), new EndsWithTransform().GetSignatures());
            RegisterTransform(builder, (ctx, c) => new InTransform(c), new InTransform().GetSignatures());
            RegisterTransform(builder, (ctx, c) => new ContainsTransform(c), new ContainsTransform().GetSignatures());
            RegisterTransform(builder, (ctx, c) => new IsTransform(c), new IsTransform().GetSignatures());
            RegisterTransform(builder, (ctx, c) => new EqualsTransform(c), new EqualsTransform().GetSignatures());
            RegisterTransform(builder, (ctx, c) => new IsEmptyTransform(c), new IsEmptyTransform().GetSignatures());
            RegisterTransform(builder, (ctx, c) => new IsDefaultTransform(c), new IsDefaultTransform().GetSignatures());
            RegisterTransform(builder, (ctx, c) => new IsNumericTransform(c), new IsNumericTransform().GetSignatures());
            RegisterTransform(builder, (ctx, c) => new RegexIsMatchTransform(c), new RegexIsMatchTransform().GetSignatures());
            RegisterTransform(builder, (ctx, c) => new IsDaylightSavingsTransform(c), new IsDaylightSavingsTransform().GetSignatures());
            RegisterTransform(builder, (ctx, c) => new ToBoolTransform(c), new ToBoolTransform().GetSignatures());

            // uncategorized
            RegisterTransform(builder, (ctx, c) => new LogTransform(c), new LogTransform().GetSignatures());

            // new
            RegisterTransform(builder, (ctx, c) => new CountTransform(c), new CountTransform().GetSignatures());

            // xml
            RegisterTransform(builder, (ctx, c) => c.Operation.Mode == "all" || c.Field.Engine != "auto" ?
                    new Transforms.Xml.FromXmlTransform(ctx.ResolveNamed<IRowFactory>(c.Entity.Key, new NamedParameter("capacity", c.GetAllEntityFields().Count())), c) :
                    new Transforms.FromXmlTransform(c) as ITransform, new[] { new OperationSignature("fromxml") }
            );

            RegisterTransform(builder, (ctx, c) => new JavascriptTransform(new ChakraCoreJsEngineFactory(), ctx.Resolve<IReader>(), c), new JavascriptTransform(null, null).GetSignatures());

            var pluginsFolder = Path.Combine(AssemblyDirectory, "plugins");
            if (Directory.Exists(pluginsFolder)) {

                var assemblies = new List<Assembly>();
                foreach (var file in Directory.GetFiles(pluginsFolder, "Transformalize.Transform.*.Autofac.dll", SearchOption.TopDirectoryOnly)) {
                    var info = new FileInfo(file);
                    var name = info.Name.ToLower().Split('.').FirstOrDefault(f => f != "dll" && f != "transformalize" && f != "transform" && f != "autofac");
                    _context.Debug(() => $"Loading {name} transform(s)");
                    var assembly = Assembly.LoadFile(new FileInfo(file).FullName);
                    assemblies.Add(assembly);
                }
                if (assemblies.Any()) {
                    builder.RegisterAssemblyModules(assemblies.ToArray());
                }
            }

            // register the short hand
            builder.Register((c, p) => _shortHand).Named<ShorthandRoot>(FieldsName).InstancePerLifetimeScope();
            builder.Register((c, p) => _shortHand).Named<ShorthandRoot>(ParametersName).InstancePerLifetimeScope();
            builder.Register((c, p) => new ShorthandCustomizer(c.ResolveNamed<ShorthandRoot>(FieldsName), new[] { "fields", "calculated-fields", "calculatedfields" }, "t", "transforms", "method")).Named<IDependency>(FieldsName).InstancePerLifetimeScope();
            builder.Register((c, p) => new ShorthandCustomizer(c.ResolveNamed<ShorthandRoot>(ParametersName), new[] { "parameters" }, "t", "transforms", "method")).Named<IDependency>(ParametersName).InstancePerLifetimeScope();

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
                        signature.Parameters.Add(new Parameter {
                            Name = parameter.Name,
                            Value = parameter.Value
                        });
                    }
                    _shortHand.Signatures.Add(signature);
                } else {
                    _context.Warn($"There are multiple {s.Method} transforms trying to register.");
                }

                builder.Register((ctx, p) => getTransform(ctx, p.Positional<IContext>(0))).Named<ITransform>(s.Method);
            }

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
