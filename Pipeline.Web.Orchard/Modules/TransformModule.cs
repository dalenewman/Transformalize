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
using Autofac;
using Cfg.Net.Contracts;
using Cfg.Net.Shorthand;
using Newtonsoft.Json;
using Orchard.Templates.Services;
using Pipeline.Web.Orchard.Impl;
using Transformalize;
using Transformalize.Contracts;
using Transformalize.Transforms.DateMath;
using Transformalize.Transforms.Geography;
using Transformalize.Transforms.Html;
using Transformalize.Transforms.Humanizer;
using Transformalize.Transforms.Jint;
using Transformalize.Transforms;
using Transformalize.Providers.File.Transforms;
using Transformalize.Providers.Web;
using Transformalize.Transforms.Compression;
using Transformalize.Transforms.Globalization;
using Transformalize.Transforms.Json;
using Transformalize.Transforms.LambdaParser;
using Transformalize.Transforms.Xml;

namespace Pipeline.Web.Orchard.Modules {

    public class TransformShorthandCustomizer : ShorthandCustomizer {
        public TransformShorthandCustomizer(ShorthandRoot root, IEnumerable<string> shortHandCollections, string shortHandProperty, string longHandCollection, string longHandProperty) : base(root, shortHandCollections, shortHandProperty, longHandCollection, longHandProperty) { }
    }

    public class TransformModule : Module {

        private readonly HashSet<string> _methods = new HashSet<string>();
        private readonly ShorthandRoot _shortHand = new ShorthandRoot();

        protected override void Load(ContainerBuilder builder) {

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

            // row filtering
            RegisterTransform(builder, (ctx, c) => new FilterTransform(FilterType.Include, c), new FilterTransform(FilterType.Include).GetSignatures());
            RegisterTransform(builder, (ctx, c) => new FilterTransform(FilterType.Exclude, c), new FilterTransform(FilterType.Exclude).GetSignatures());

            // field producing
            RegisterTransform(builder, (ctx, c) => new FromSplitTransform(c), new FromSplitTransform().GetSignatures());
            RegisterTransform(builder, (ctx, c) => new FromRegexTransform(c), new FromRegexTransform().GetSignatures());
            RegisterTransform(builder, (ctx, c) => new FromLengthsTransform(c), new FromLengthsTransform().GetSignatures());
            RegisterTransform(builder, (ctx, c) => new FromJsonTransform(c, o => JsonConvert.SerializeObject(o, Formatting.None)), new FromJsonTransform().GetSignatures());
            RegisterTransform(builder, (ctx, c) => new GeocodeTransform(c), new GeocodeTransform().GetSignatures());

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

            // uncategorized
            RegisterTransform(builder, (ctx, c) => new LogTransform(c), new LogTransform().GetSignatures());

            // Humanizer Transforms
            RegisterTransform(builder, (ctx, c) => new CamelizeTransform(c), new CamelizeTransform().GetSignatures());
            RegisterTransform(builder, (ctx, c) => new HumanizeTransform(c), new HumanizeTransform().GetSignatures());
            RegisterTransform(builder, (ctx, c) => new FromMetricTransform(c), new FromMetricTransform().GetSignatures());
            RegisterTransform(builder, (ctx, c) => new FromRomanTransform(c), new FromRomanTransform().GetSignatures());
            RegisterTransform(builder, (ctx, c) => new DehumanizeTransform(c), new DehumanizeTransform().GetSignatures());
            RegisterTransform(builder, (ctx, c) => new HyphenateTransform(c), new HyphenateTransform().GetSignatures());
            RegisterTransform(builder, (ctx, c) => new OrdinalizeTransform(c), new OrdinalizeTransform().GetSignatures());
            RegisterTransform(builder, (ctx, c) => new PascalizeTransform(c), new PascalizeTransform().GetSignatures());
            RegisterTransform(builder, (ctx, c) => new PluralizeTransform(c), new PluralizeTransform().GetSignatures());
            RegisterTransform(builder, (ctx, c) => new SingularizeTransform(c), new SingularizeTransform().GetSignatures());
            RegisterTransform(builder, (ctx, c) => new TitleizeTransform(c), new TitleizeTransform().GetSignatures());
            RegisterTransform(builder, (ctx, c) => new ToMetricTransform(c), new ToMetricTransform().GetSignatures());
            RegisterTransform(builder, (ctx, c) => new ToOrdinalWordsTransform(c), new ToOrdinalWordsTransform().GetSignatures());
            RegisterTransform(builder, (ctx, c) => new ToRomanTransform(c), new ToRomanTransform().GetSignatures());
            RegisterTransform(builder, (ctx, c) => new ToWordsTransform(c), new ToWordsTransform().GetSignatures());
            RegisterTransform(builder, (ctx, c) => new UnderscoreTransform(c), new UnderscoreTransform().GetSignatures());
            RegisterTransform(builder, (ctx, c) => new BytesTransform(c), new BytesTransform().GetSignatures());
            RegisterTransform(builder, (ctx, c) => new ByteSizeTransform(c), new ByteSizeTransform().GetSignatures());

            RegisterTransform(builder, (ctx, c) => new LambdaParserEvalTransform(c), new LambdaParserEvalTransform().GetSignatures());

            // javascript implementation is jint only in Orchard CMS
            RegisterTransform(builder, (ctx, c) => new JintTransform(ctx.Resolve<IReader>(), c), new JintTransform().GetSignatures());

            // razor implementation uses Orchard CMS implementation
            RegisterTransform(builder, (ctx, c) => new OrchardRazorTransform(ctx.Resolve<ITemplateProcessor>(), c), new OrchardRazorTransform(null).GetSignatures());

            // register the short hand
            builder.Register((c, p) => new TransformShorthandCustomizer(_shortHand, new[] {"fields", "calculated-fields"}, "t", "transforms", "method")).As<TransformShorthandCustomizer>().SingleInstance();

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
                }

                builder.Register((ctx, p) => getTransform(ctx, p.Positional<IContext>(0))).Named<ITransform>(s.Method);
            }

        }

    }
}
