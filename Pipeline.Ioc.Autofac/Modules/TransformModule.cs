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

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Autofac;
using Cfg.Net.Contracts;
using Cfg.Net.Shorthand;
using JavaScriptEngineSwitcher.ChakraCore;
using Newtonsoft.Json;
using Transformalize.Contracts;
using Transformalize.Providers.File.Transforms;
using Transformalize.Providers.Razor;
using Transformalize.Providers.Web;
using Transformalize.Transforms;
using Transformalize.Transforms.Compression;
using Transformalize.Transforms.DateMath;
using Transformalize.Transforms.Geography;
using Transformalize.Transforms.Html;
using Transformalize.Transforms.JavaScript;
using Transformalize.Transforms.Json;
using Transformalize.Transforms.Velocity;
using Transformalize.Transforms.Xml;
using Transformalize.Transforms.Globalization;
using Parameter = Cfg.Net.Shorthand.Parameter;
using Transformalize.Configuration;
using Transformalize.Context;
using Module = Autofac.Module;

namespace Transformalize.Ioc.Autofac.Modules
{
    public class TransformModule : Module
    {

        public const string Name = "shorthand-t";
        private readonly HashSet<string> _methods = new HashSet<string>();
        private readonly ShorthandRoot _shortHand = new ShorthandRoot();
        private readonly Process _process;
        private readonly IPipelineLogger _logger;

        public TransformModule(Process process, IPipelineLogger logger)
        {
            _process = process;
            _logger = logger;
        }

        protected override void Load(ContainerBuilder builder)
        {

            var loadContext = new PipelineContext(_logger, _process);
            builder.Properties["ShortHand"] = _shortHand;
            builder.Properties["Methods"] = _methods;
            builder.Properties["Process"] = _process;

            // new method so transform author can define shorthand signature(s)
            RegisterTransform(builder, (ctx, c) => new AbsTransform(c), new AbsTransform().GetSignatures());
            RegisterTransform(builder, (ctx, c) => new SliceTransform(c), new SliceTransform().GetSignatures());
            RegisterTransform(builder, (ctx, c) => new FormatTransform(c), new FormatTransform().GetSignatures());
            RegisterTransform(builder, (ctx, c) => new AddTransform(c), new AddTransform().GetSignatures());
            RegisterTransform(builder, (ctx, c) => new EqualsTransform(c), new EqualsTransform().GetSignatures());
            RegisterTransform(builder, (ctx, c) => new CeilingTransform(c), new CeilingTransform().GetSignatures());
            RegisterTransform(builder, (ctx, c) => new CoalesceTransform(c), new CoalesceTransform().GetSignatures());
            RegisterTransform(builder, (ctx, c) => new ConcatTransform(c), new ConcatTransform().GetSignatures());
            RegisterTransform(builder, (ctx, c) => new ConnectionTransform(c), new ConnectionTransform().GetSignatures());
            RegisterTransform(builder, (ctx, c) => new ConvertTransform(c), new ConvertTransform().GetSignatures());
            RegisterTransform(builder, (ctx, c) => new CopyTransform(c), new CopyTransform().GetSignatures());
            RegisterTransform(builder, (ctx, c) => new DateDiffTransform(c), new DateDiffTransform().GetSignatures());
            RegisterTransform(builder, (ctx, c) => new DatePartTransform(c), new DatePartTransform().GetSignatures());
            RegisterTransform(builder, (ctx, c) => new DecompressTransform(c), new DecompressTransform().GetSignatures());
            RegisterTransform(builder, (ctx, c) => new CompressTransform(c), new CompressTransform().GetSignatures());
            RegisterTransform(builder, (ctx, c) => new FileExtTransform(c), new FileExtTransform().GetSignatures());
            RegisterTransform(builder, (ctx, c) => new FileNameTransform(c), new FileNameTransform().GetSignatures());
            RegisterTransform(builder, (ctx, c) => new FilePathTransform(c), new FilePathTransform().GetSignatures());
            RegisterTransform(builder, (ctx, c) => new SplitTransform(c), new SplitTransform().GetSignatures());
            RegisterTransform(builder, (ctx, c) => new LastDayTransform(c), new LastDayTransform().GetSignatures());
            RegisterTransform(builder, (ctx, c) => new LastTransform(c), new LastTransform().GetSignatures());
            RegisterTransform(builder, (ctx, c) => new FirstTransform(c), new FirstTransform().GetSignatures());
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
            RegisterTransform(builder, (ctx, c) => new RazorTransform(c), new RazorTransform().GetSignatures());
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

            // row filtering
            RegisterTransform(builder, (ctx, c) => new FilterTransform(FilterType.Include, c), new FilterTransform(FilterType.Include).GetSignatures());
            RegisterTransform(builder, (ctx, c) => new FilterTransform(FilterType.Exclude, c), new FilterTransform(FilterType.Exclude).GetSignatures());

            // field producing
            RegisterTransform(builder, (ctx, c) => new FromSplitTransform(c), new FromSplitTransform().GetSignatures());
            RegisterTransform(builder, (ctx, c) => new FromRegexTransform(c), new FromRegexTransform().GetSignatures());
            RegisterTransform(builder, (ctx, c) => new FromLengthsTransform(c), new FromLengthsTransform().GetSignatures());
            RegisterTransform(builder, (ctx, c) => new FromJsonTransform(c, o => JsonConvert.SerializeObject(o, Formatting.None)), new FromJsonTransform().GetSignatures());

            var pluginsFolder = Path.Combine(AssemblyDirectory, "plugins");
            if (Directory.Exists(pluginsFolder))
            {

                var assemblies = new List<Assembly>();
                foreach (var file in Directory.GetFiles(pluginsFolder, "Transformalize.Transform.*.Autofac.dll", SearchOption.TopDirectoryOnly))
                {
                    var info = new FileInfo(file);
                    var name = info.Name.ToLower().Split('.').FirstOrDefault(f => f != "dll" && f != "transformalize" && f != "transform" && f != "autofac");
                    loadContext.Debug(() => $"Loading {name} transform");
                    var assembly = Assembly.LoadFile(new FileInfo(file).FullName);
                    assemblies.Add(assembly);
                }
                if (assemblies.Any())
                {
                    builder.RegisterAssemblyModules(assemblies.ToArray());
                }
            }

            // register the short hand
            builder.Register((c, p) => _shortHand).Named<ShorthandRoot>(Name).InstancePerLifetimeScope();

            // old method

            // return true or false transforms
            builder.Register((c, p) => new AnyTransform(p.Positional<IContext>(0))).Named<ITransform>("any");
            builder.Register((c, p) => new StartsWithTransform(p.Positional<IContext>(0))).Named<ITransform>("startswith");
            builder.Register((c, p) => new EndsWithTransform(p.Positional<IContext>(0))).Named<ITransform>("endswith");
            builder.Register((c, p) => new InTransform(p.Positional<IContext>(0))).Named<ITransform>("in");
            builder.Register((c, p) => new ContainsTransform(p.Positional<IContext>(0))).Named<ITransform>("contains");
            builder.Register((c, p) => new IsTransform(p.Positional<IContext>(0))).Named<ITransform>("is");
            builder.Register((c, p) => new EqualsTransform(p.Positional<IContext>(0))).Named<ITransform>("equal");
            builder.Register((c, p) => new EqualsTransform(p.Positional<IContext>(0))).Named<ITransform>("equals");
            builder.Register((c, p) => new IsEmptyTransform(p.Positional<IContext>(0))).Named<ITransform>("isempty");
            builder.Register((c, p) => new IsDefaultTransform(p.Positional<IContext>(0))).Named<ITransform>("isdefault");
            builder.Register((c, p) => new IsNumericTransform(p.Positional<IContext>(0))).Named<ITransform>("isnumeric");
            builder.Register((c, p) => new RegexIsMatchTransform(p.Positional<IContext>(0))).Named<ITransform>("ismatch");

            builder.Register((c, p) => new GeocodeTransform(p.Positional<IContext>(0))).Named<ITransform>("fromaddress");
            builder.Register((c, p) => new DateMathTransform(p.Positional<IContext>(0))).Named<ITransform>("datemath");
            builder.Register((c, p) => new IsDaylightSavingsTransform(p.Positional<IContext>(0))).Named<ITransform>("isdaylightsavings");
            

            builder.Register((c, p) => new WebTransform(p.Positional<IContext>(0))).Named<ITransform>("web");
            builder.Register((c, p) => new UrlEncodeTransform(p.Positional<IContext>(0))).Named<ITransform>("urlencode");

            builder.Register((c, p) => new DistinctTransform(p.Positional<IContext>(0))).Named<ITransform>("distinct");
            builder.Register((c, p) => new RegexMatchCountTransform(p.Positional<IContext>(0))).Named<ITransform>("matchcount");

            builder.Register((c, p) => {
                var context = p.Positional<IContext>(0);
                return context.Operation.Mode == "all" || context.Field.Engine != "auto" ?
                    new Transforms.Xml.FromXmlTransform(context, c.ResolveNamed<IRowFactory>(context.Entity.Key, new NamedParameter("capacity", context.GetAllEntityFields().Count()))) :
                    new Transforms.FromXmlTransform(context) as ITransform;
            }).Named<ITransform>("fromxml");

            builder.Register((c, p) => new JavascriptTransform(new ChakraCoreJsEngineFactory(), p.Positional<IContext>(0), c.Resolve<IReader>())).Named<ITransform>("js");
            builder.Register((c, p) => new JavascriptTransform(new ChakraCoreJsEngineFactory(), p.Positional<IContext>(0), c.Resolve<IReader>())).Named<ITransform>("javascript");
            builder.Register((c, p) => new JavascriptTransform(new ChakraCoreJsEngineFactory(), p.Positional<IContext>(0), c.Resolve<IReader>())).Named<ITransform>("chakra");

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

                    foreach (var parameter in s.Parameters)
                    {
                        signature.Parameters.Add(new Parameter
                        {
                            Name = parameter.Name,
                            Value = parameter.Value
                        });
                    }
                    _shortHand.Signatures.Add(signature);
                }

                builder.Register((ctx, p) => getTransform(ctx, p.Positional<IContext>(0))).Named<ITransform>(s.Method);
            }

        }

        public static string AssemblyDirectory  {
            get {
                var codeBase = typeof(Process).Assembly.CodeBase;
                var uri = new UriBuilder(codeBase);
                var path = Uri.UnescapeDataString(uri.Path);
                return Path.GetDirectoryName(path);
            }
        }
    }
}
