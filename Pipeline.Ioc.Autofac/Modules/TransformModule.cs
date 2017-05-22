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
using System.Linq;
using Autofac;
using Cfg.Net.Contracts;
using JavaScriptEngineSwitcher.ChakraCore;
using Newtonsoft.Json;
using Transformalize.Configuration;
using Transformalize.Contracts;
using Transformalize.Desktop.Transforms;
using Transformalize.Nulls;
using Transformalize.Transform.CSharp;
using Transformalize.Transform.DateMath;
using Transformalize.Transform.Dates;
using Transformalize.Transform.Geocode;
using Transformalize.Transform.GeoCoordinate;
using Transformalize.Transform.Geohash;
using Transformalize.Transform.Html;
using Transformalize.Transform.Humanizer;
using Transformalize.Transform.JavaScriptEngineSwitcher;
using Transformalize.Transform.Jint;
using Transformalize.Transform.LamdaParser;
using Transformalize.Transform.Razor;
using Transformalize.Transform.Velocity;
using Transformalize.Transform.Vin;
using Transformalize.Transforms;
using Transformalize.Validators;
using Transformalize.Provider.File.Transforms;

namespace Transformalize.Ioc.Autofac.Modules {
    public class TransformModule : Module {
        private readonly Process _process;

        public TransformModule() { }

        public TransformModule(Process process) {
            _process = process;
        }

        protected override void Load(ContainerBuilder builder) {

            if (_process == null)
                return;

            builder.Register((c, p) => new AbsTransform(p.Positional<IContext>(0))).Named<ITransform>("abs");
            builder.Register((c, p) => new AddTransform(p.Positional<IContext>(0))).Named<ITransform>("add");
            builder.Register((c, p) => new AddTransform(p.Positional<IContext>(0))).Named<ITransform>("sum");
            builder.Register((c, p) => new CeilingTransform(p.Positional<IContext>(0))).Named<ITransform>("ceiling");
            builder.Register((c, p) => new CoalesceTransform(p.Positional<IContext>(0))).Named<ITransform>("coalesce");
            builder.Register((c, p) => new ConcatTransform(p.Positional<IContext>(0))).Named<ITransform>("concat");
            builder.Register((c, p) => new ConnectionTransform(p.Positional<IContext>(0))).Named<ITransform>("connection");
            builder.Register((c, p) => new ConvertTransform(p.Positional<IContext>(0))).Named<ITransform>("convert");
            builder.Register((c, p) => new CopyTransform(p.Positional<IContext>(0))).Named<ITransform>("copy");
            builder.Register((c, p) => new DateDiffTransform(p.Positional<IContext>(0))).Named<ITransform>("datediff");
            builder.Register((c, p) => new DatePartTransform(p.Positional<IContext>(0))).Named<ITransform>("datepart");
            builder.Register((c, p) => new DecompressTransform(p.Positional<IContext>(0))).Named<ITransform>("decompress");
            builder.Register((c, p) => new CompressTransform(p.Positional<IContext>(0))).Named<ITransform>("compress");
            builder.Register((c, p) => new FileExtTransform(p.Positional<IContext>(0))).Named<ITransform>("fileext");
            builder.Register((c, p) => new FileNameTransform(p.Positional<IContext>(0))).Named<ITransform>("filename");
            builder.Register((c, p) => new FilePathTransform(p.Positional<IContext>(0))).Named<ITransform>("filepath");
            builder.Register((c, p) => new FloorTransform(p.Positional<IContext>(0))).Named<ITransform>("floor");
            builder.Register((c, p) => new BetterFormatTransform(p.Positional<IContext>(0))).Named<ITransform>("format");
            builder.Register((c, p) => new FormatXmlTransfrom(p.Positional<IContext>(0))).Named<ITransform>("formatxml");
            builder.Register((c, p) => new FormatPhoneTransform(p.Positional<IContext>(0))).Named<ITransform>("formatphone");
            builder.Register((c, p) => new HashcodeTransform(p.Positional<IContext>(0))).Named<ITransform>("hashcode");
            builder.Register((c, p) => new DecodeTransform(p.Positional<IContext>(0))).Named<ITransform>("htmldecode");
            builder.Register((c, p) => new HtmlEncodeTransform(p.Positional<IContext>(0))).Named<ITransform>("htmlencode");
            builder.Register((c, p) => new InsertTransform(p.Positional<IContext>(0))).Named<ITransform>("insert");
            builder.Register((c, p) => new InvertTransform(p.Positional<IContext>(0))).Named<ITransform>("invert");
            builder.Register((c, p) => new JoinTransform(p.Positional<IContext>(0))).Named<ITransform>("join");
            builder.Register((c, p) => new LastTransform(p.Positional<IContext>(0))).Named<ITransform>("last");
            builder.Register((c, p) => new LeftTransform(p.Positional<IContext>(0))).Named<ITransform>("left");
            builder.Register((c, p) => new ToLowerTransform(p.Positional<IContext>(0))).Named<ITransform>("lower");
            builder.Register((c, p) => new ToLowerTransform(p.Positional<IContext>(0))).Named<ITransform>("tolower");
            builder.Register((c, p) => new MapTransform(p.Positional<IContext>(0))).Named<ITransform>("map");
            builder.Register((c, p) => new RegexMatchTransform(p.Positional<IContext>(0))).Named<ITransform>("match");
            builder.Register((c, p) => new MultiplyTransform(p.Positional<IContext>(0))).Named<ITransform>("multiply");
            builder.Register((c, p) => new NextTransform(p.Positional<IContext>(0))).Named<ITransform>("next");
            builder.Register((c, p) => new UtcNowTransform(p.Positional<IContext>(0))).Named<ITransform>("now");
            builder.Register((c, p) => new PadLeftTransform(p.Positional<IContext>(0))).Named<ITransform>("padleft");
            builder.Register((c, p) => new PadRightTransform(p.Positional<IContext>(0))).Named<ITransform>("padright");
            builder.Register((c, p) => new RazorTransform(p.Positional<IContext>(0))).Named<ITransform>("razor");
            builder.Register((c, p) => new TimeZoneTransform(p.Positional<IContext>(0))).Named<ITransform>("timezone");

            builder.Register((c, p) => new RegexReplaceTransform(p.Positional<IContext>(0))).Named<ITransform>("regexreplace");
            builder.Register((c, p) => new RemoveTransform(p.Positional<IContext>(0))).Named<ITransform>("remove");
            builder.Register((c, p) => new ReplaceTransform(p.Positional<IContext>(0))).Named<ITransform>("replace");
            builder.Register((c, p) => new RightTransform(p.Positional<IContext>(0))).Named<ITransform>("right");
            builder.Register((c, p) => new RoundTransform(p.Positional<IContext>(0))).Named<ITransform>("round");
            builder.Register((c, p) => new SplitLengthTransform(p.Positional<IContext>(0))).Named<ITransform>("splitlength");
            builder.Register((c, p) => new SubStringTransform(p.Positional<IContext>(0))).Named<ITransform>("substring");
            builder.Register((c, p) => new TagTransform(p.Positional<IContext>(0))).Named<ITransform>("tag");
            builder.Register((c, p) => new RelativeTimeTransform(p.Positional<IContext>(0), true)).Named<ITransform>("timeago");
            builder.Register((c, p) => new RelativeTimeTransform(p.Positional<IContext>(0), false)).Named<ITransform>("timeahead");
            builder.Register((c, p) => new ToStringTransform(p.Positional<IContext>(0))).Named<ITransform>("tostring");
            builder.Register((c, p) => new ToTimeTransform(p.Positional<IContext>(0))).Named<ITransform>("totime");
            builder.Register((c, p) => new ToYesNoTransform(p.Positional<IContext>(0))).Named<ITransform>("toyesno");
            builder.Register((c, p) => new TrimTransform(p.Positional<IContext>(0))).Named<ITransform>("trim");
            builder.Register((c, p) => new TrimEndTransform(p.Positional<IContext>(0))).Named<ITransform>("trimend");
            builder.Register((c, p) => new TrimStartTransform(p.Positional<IContext>(0))).Named<ITransform>("trimstart");
            builder.Register((c, p) => new VelocityTransform(p.Positional<IContext>(0), c.Resolve<IReader>())).Named<ITransform>("velocity");
            builder.Register((c, p) => new ToUpperTransform(p.Positional<IContext>(0))).Named<ITransform>("upper");
            builder.Register((c, p) => new ToUpperTransform(p.Positional<IContext>(0))).Named<ITransform>("toupper");

            builder.Register((c, p) => new DecodeTransform(p.Positional<IContext>(0))).Named<ITransform>("xmldecode");
            builder.Register((c, p) => new XPathTransform(p.Positional<IContext>(0))).Named<ITransform>("xpath");
            builder.Register((c, p) => new IIfTransform(p.Positional<IContext>(0))).Named<ITransform>("iif");
            builder.Register((c, p) => new GeohashEncodeTransform(p.Positional<IContext>(0))).Named<ITransform>("geohashencode");
            builder.Register((c, p) => new GeohashNeighborTransform(p.Positional<IContext>(0))).Named<ITransform>("geohashneighbor");
            builder.Register((c, p) => new CommonPrefixTransform(p.Positional<IContext>(0))).Named<ITransform>("commonprefix");
            builder.Register((c, p) => new CommonPrefixesTransform(p.Positional<IContext>(0))).Named<ITransform>("commonprefixes");
            builder.Register((c, p) => new DistanceTransform(p.Positional<IContext>(0))).Named<ITransform>("distance");

            builder.Register((c, p) => new FilterTransform(p.Positional<IContext>(0), FilterType.Include)).Named<ITransform>("include");
            builder.Register((c, p) => new FilterTransform(p.Positional<IContext>(0), FilterType.Exclude)).Named<ITransform>("exclude");

            // Humanizer
            builder.Register((c, p) => new CamelizeTransform(p.Positional<IContext>(0))).Named<ITransform>("camelize");
            builder.Register((c, p) => new FromMetricTransform(p.Positional<IContext>(0))).Named<ITransform>("frommetric");
            builder.Register((c, p) => new FromRomanTransform(p.Positional<IContext>(0))).Named<ITransform>("fromroman");
            builder.Register((c, p) => new HumanizeTransform(p.Positional<IContext>(0))).Named<ITransform>("humanize");
            builder.Register((c, p) => new DehumanizeTransform(p.Positional<IContext>(0))).Named<ITransform>("dehumanize");
            builder.Register((c, p) => new HyphenateTransform(p.Positional<IContext>(0))).Named<ITransform>("hyphenate");
            builder.Register((c, p) => new HyphenateTransform(p.Positional<IContext>(0))).Named<ITransform>("dasherize");
            builder.Register((c, p) => new OrdinalizeTransform(p.Positional<IContext>(0))).Named<ITransform>("ordinalize");
            builder.Register((c, p) => new PascalizeTransform(p.Positional<IContext>(0))).Named<ITransform>("pascalize");
            builder.Register((c, p) => new PluralizeTransform(p.Positional<IContext>(0))).Named<ITransform>("pluralize");
            builder.Register((c, p) => new SingularizeTransform(p.Positional<IContext>(0))).Named<ITransform>("singularize");
            builder.Register((c, p) => new TitleizeTransform(p.Positional<IContext>(0))).Named<ITransform>("titleize");
            builder.Register((c, p) => new ToMetricTransform(p.Positional<IContext>(0))).Named<ITransform>("tometric");
            builder.Register((c, p) => new ToOrdinalWordsTransform(p.Positional<IContext>(0))).Named<ITransform>("toordinalwords");
            builder.Register((c, p) => new ToRomanTransform(p.Positional<IContext>(0))).Named<ITransform>("toroman");
            builder.Register((c, p) => new ToWordsTransform(p.Positional<IContext>(0))).Named<ITransform>("towords");
            builder.Register((c, p) => new UnderscoreTransform(p.Positional<IContext>(0))).Named<ITransform>("underscore");
            builder.Register((c, p) => new BytesTransform(p.Positional<IContext>(0))).Named<ITransform>("bytes");

            builder.Register((c, p) => new DateAddTransform(p.Positional<IContext>(0), "ticks")).Named<ITransform>("addticks");
            builder.Register((c, p) => new DateAddTransform(p.Positional<IContext>(0), "milliseconds")).Named<ITransform>("addmilliseconds");
            builder.Register((c, p) => new DateAddTransform(p.Positional<IContext>(0), "seconds")).Named<ITransform>("addseconds");
            builder.Register((c, p) => new DateAddTransform(p.Positional<IContext>(0), "minutes")).Named<ITransform>("addminutes");
            builder.Register((c, p) => new DateAddTransform(p.Positional<IContext>(0), "hours")).Named<ITransform>("addhours");
            builder.Register((c, p) => new DateAddTransform(p.Positional<IContext>(0), "days")).Named<ITransform>("adddays");

            builder.Register((c, p) => new FromSplitTransform(p.Positional<IContext>(0))).Named<ITransform>("fromsplit");
            builder.Register((c, p) => new FromLengthsTranform(p.Positional<IContext>(0))).Named<ITransform>("fromlengths");

            // return true or false, validators
            builder.Register((c, p) => new AnyValidator(p.Positional<IContext>(0))).Named<ITransform>("any");
            builder.Register((c, p) => new StartsWithValidator(p.Positional<IContext>(0))).Named<ITransform>("startswith");
            builder.Register((c, p) => new EndsWithValidator(p.Positional<IContext>(0))).Named<ITransform>("endswith");
            builder.Register((c, p) => new InValidator(p.Positional<IContext>(0))).Named<ITransform>("in");
            builder.Register((c, p) => new ContainsValidator(p.Positional<IContext>(0))).Named<ITransform>("contains");
            builder.Register((c, p) => new IsValidator(p.Positional<IContext>(0))).Named<ITransform>("is");
            builder.Register((c, p) => new EqualsValidator(p.Positional<IContext>(0))).Named<ITransform>("equal");
            builder.Register((c, p) => new EqualsValidator(p.Positional<IContext>(0))).Named<ITransform>("equals");
            builder.Register((c, p) => new IsEmptyValidator(p.Positional<IContext>(0))).Named<ITransform>("isempty");
            builder.Register((c, p) => new IsDefaultValidator(p.Positional<IContext>(0))).Named<ITransform>("isdefault");
            builder.Register((c, p) => new IsNumericValidator(p.Positional<IContext>(0))).Named<ITransform>("isnumeric");
            builder.Register((c, p) => new RegexIsMatchTransform(p.Positional<IContext>(0))).Named<ITransform>("ismatch");

            builder.Register((c, p) => new GeocodeTransform(p.Positional<IContext>(0))).Named<ITransform>("fromaddress");
            builder.Register((c, p) => new DateMathTransform(p.Positional<IContext>(0))).Named<ITransform>("datemath");
            builder.Register((c, p) => new IsDaylightSavings(p.Positional<IContext>(0))).Named<ITransform>("isdaylightsavings");
            builder.Register((c, p) => new SlugifyTransform(p.Positional<IContext>(0))).Named<ITransform>("slugify");

            /* VIN, Vehicle Identification Number, note: you get red intellisense here because vin library is portable */
            builder.Register((c, p) => new VinValidateTransform(p.Positional<IContext>(0))).Named<ITransform>("isvin");
            builder.Register((c, p) => new VinValidateTransform(p.Positional<IContext>(0))).Named<ITransform>("vinisvalid");
            builder.Register((c, p) => new VinGetWorldManufacturerTransform(p.Positional<IContext>(0))).Named<ITransform>("vingetworldmanufacturer");
            builder.Register((c, p) => new VinGetModelYearTransform(p.Positional<IContext>(0))).Named<ITransform>("vingetmodelyear");

            // wip
            builder.Register((c, p) => new WebTransform(p.Positional<IContext>(0))).Named<ITransform>("web");
            builder.Register((c, p) => new UrlEncodeTransform(p.Positional<IContext>(0))).Named<ITransform>("urlencode");
            builder.Register((c, p) => new FromJsonTransform(p.Positional<IContext>(0), o => JsonConvert.SerializeObject(o, Formatting.None))).Named<ITransform>("fromjson");

            builder.Register((c, p) => new LamdaParserEvalTransform(p.Positional<IContext>(0))).Named<ITransform>("eval");
            builder.Register((c, p) => new DistinctTransform(p.Positional<IContext>(0))).Named<ITransform>("distinct");
            builder.Register((c, p) => new RegexMatchCountTransform(p.Positional<IContext>(0))).Named<ITransform>("matchcount");

            builder.Register((c, p) => {
                var context = p.Positional<IContext>(0);
                return context.Transform.XmlMode == "all" ?
                    new Desktop.Transforms.FromXmlTransform(context, c.ResolveNamed<IRowFactory>(context.Entity.Key, new NamedParameter("capacity", context.GetAllEntityFields().Count()))) :
                    new Transforms.FromXmlTransform(context) as ITransform;
            }).Named<ITransform>("fromxml");

            builder.Register<ITransform>((c, p) => {
                var context = p.Positional<IContext>(0);
                if (c.ResolveNamed<IHost>("cs").Start()) {
                    return new CsharpTransform(context);
                }
                context.Error("Unable to register csharp transform");
                return new NullTransform(context);
            }).Named<ITransform>("cs");
            builder.Register((c, p) => c.ResolveNamed<ITransform>("cs", p)).Named<ITransform>("csharp");


            builder.Register<ITransform>((c, p) => {
                var context = p.Positional<IContext>(0);
                switch (context.Field.Engine) {
                    case "jint":
                        return new JintTransform(context, c.Resolve<IReader>());
                    default:
                        return new JavascriptTransform(new ChakraCoreJsEngineFactory(), context, c.Resolve<IReader>());
                }
            }).Named<ITransform>("js");
            builder.Register((c, p) => c.ResolveNamed<ITransform>("js", p)).Named<ITransform>("javascript");

        }

    }
}
