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
using System.Linq;
using Autofac;
using Cfg.Net.Shorthand;
using Transformalize.Configuration;
using Transformalize.Context;
using Transformalize.Contracts;
using Transformalize.Transforms;
using Transformalize.Transforms.Dates;
using Parameter = Cfg.Net.Shorthand.Parameter;

namespace Transformalize.Containers.Autofac {

   public class TransformBuilder {

      private readonly List<TransformHolder> _transforms;
      private readonly HashSet<string> _methods;
      private readonly ShorthandRoot _shortHand;
      private readonly Process _process;
      private readonly IPipelineLogger _logger;
      private readonly ContainerBuilder _builder;
      private IContext _context;

      public TransformBuilder(Process process, ContainerBuilder builder, IPipelineLogger logger) {
         _process = process;
         _builder = builder;
         _logger = logger;
         _methods = new HashSet<string>();
         _shortHand = new ShorthandRoot();
         _transforms = new List<TransformHolder>();
      }

      public TransformBuilder(Process process, ContainerBuilder builder, HashSet<string> methods, ShorthandRoot shortHand, List<TransformHolder> transforms, IPipelineLogger logger) {
         _process = process;
         _builder = builder;
         _logger = logger;
         _methods = methods;
         _shortHand = shortHand;
         _transforms = transforms;
      }

      public void Build() {

         _context = new PipelineContext(_logger, _process);

         // new method so transform author can define shorthand signature(s)
         RegisterTransform(_builder, (ctx, c) => new AbsTransform(c), new AbsTransform().GetSignatures());
         RegisterTransform(_builder, (ctx, c) => new SliceTransform(c), new SliceTransform().GetSignatures());
         RegisterTransform(_builder, (ctx, c) => new FormatTransform(c), new FormatTransform().GetSignatures());
         RegisterTransform(_builder, (ctx, c) => new AddTransform(c), new AddTransform().GetSignatures());
         RegisterTransform(_builder, (ctx, c) => new CeilingTransform(c), new CeilingTransform().GetSignatures());
         RegisterTransform(_builder, (ctx, c) => new CoalesceTransform(c), new CoalesceTransform().GetSignatures());
         RegisterTransform(_builder, (ctx, c) => new ConcatTransform(c), new ConcatTransform().GetSignatures());
         RegisterTransform(_builder, (ctx, c) => new ConvertTransform(c), new ConvertTransform().GetSignatures());
         RegisterTransform(_builder, (ctx, c) => new CopyTransform(c), new CopyTransform().GetSignatures());

         RegisterTransform(_builder, (ctx, c) => new FloorTransform(c), new FloorTransform().GetSignatures());
         RegisterTransform(_builder, (ctx, c) => new FormatXmlTransform(c), new FormatXmlTransform().GetSignatures());
         RegisterTransform(_builder, (ctx, c) => new FormatPhoneTransform(c), new FormatPhoneTransform().GetSignatures());
         RegisterTransform(_builder, (ctx, c) => new HashcodeTransform(c), new HashcodeTransform().GetSignatures());
         RegisterTransform(_builder, (ctx, c) => new DecodeTransform(c), new DecodeTransform().GetSignatures());

         RegisterTransform(_builder, (ctx, c) => new InsertTransform(c), new InsertTransform().GetSignatures());
         RegisterTransform(_builder, (ctx, c) => new InvertTransform(c), new InvertTransform().GetSignatures());
         RegisterTransform(_builder, (ctx, c) => new LeftTransform(c), new LeftTransform().GetSignatures());
         RegisterTransform(_builder, (ctx, c) => new RightTransform(c), new RightTransform().GetSignatures());
         RegisterTransform(_builder, (ctx, c) => new ToLowerTransform(c), new ToLowerTransform().GetSignatures());
         RegisterTransform(_builder, (ctx, c) => new MapTransform(c), new MapTransform().GetSignatures());
         RegisterTransform(_builder, (ctx, c) => new RegexMatchTransform(c), new RegexMatchTransform().GetSignatures());
         RegisterTransform(_builder, (ctx, c) => new RegexMatchingTransform(c), new RegexMatchingTransform().GetSignatures());
         RegisterTransform(_builder, (ctx, c) => new MultiplyTransform(c), new MultiplyTransform().GetSignatures());
         RegisterTransform(_builder, (ctx, c) => new RowNumberTransform(c), new RowNumberTransform().GetSignatures());

         RegisterTransform(_builder, (ctx, c) => new OppositeTransform(c), new OppositeTransform().GetSignatures());

         RegisterTransform(_builder, (ctx, c) => new PadLeftTransform(c), new PadLeftTransform().GetSignatures());
         RegisterTransform(_builder, (ctx, c) => new PadRightTransform(c), new PadRightTransform().GetSignatures());
         RegisterTransform(_builder, (ctx, c) => new RegexReplaceTransform(c), new RegexReplaceTransform().GetSignatures());
         RegisterTransform(_builder, (ctx, c) => new RemoveTransform(c), new RemoveTransform().GetSignatures());
         RegisterTransform(_builder, (ctx, c) => new ReplaceTransform(c), new ReplaceTransform().GetSignatures());
         RegisterTransform(_builder, (ctx, c) => new RoundTransform(c), new RoundTransform().GetSignatures());
         RegisterTransform(_builder, (ctx, c) => new RoundToTransform(c), new RoundToTransform().GetSignatures());
         RegisterTransform(_builder, (ctx, c) => new RoundUpToTransform(c), new RoundUpToTransform().GetSignatures());
         RegisterTransform(_builder, (ctx, c) => new RoundDownToTransform(c), new RoundDownToTransform().GetSignatures());
         RegisterTransform(_builder, (ctx, c) => new SplitLengthTransform(c), new SplitLengthTransform().GetSignatures());
         RegisterTransform(_builder, (ctx, c) => new SubStringTransform(c), new SubStringTransform().GetSignatures());
         RegisterTransform(_builder, (ctx, c) => new TagTransform(c), new TagTransform().GetSignatures());
         RegisterTransform(_builder, (ctx, c) => new ToStringTransform(c), new ToStringTransform().GetSignatures());
         RegisterTransform(_builder, (ctx, c) => new ToTimeTransform(c), new ToTimeTransform().GetSignatures());
         RegisterTransform(_builder, (ctx, c) => new ToYesNoTransform(c), new ToYesNoTransform().GetSignatures());
         RegisterTransform(_builder, (ctx, c) => new TrimTransform(c), new TrimTransform().GetSignatures());
         RegisterTransform(_builder, (ctx, c) => new TrimStartTransform(c), new TrimStartTransform().GetSignatures());
         RegisterTransform(_builder, (ctx, c) => new TrimEndTransform(c), new TrimEndTransform().GetSignatures());
         RegisterTransform(_builder, (ctx, c) => new ToUpperTransform(c), new ToUpperTransform().GetSignatures());

         RegisterTransform(_builder, (ctx, c) => new IIfTransform(c), new IIfTransform().GetSignatures());
         RegisterTransform(_builder, (ctx, c) => new AppendTransform(c), new AppendTransform().GetSignatures());
         RegisterTransform(_builder, (ctx, c) => new PrependTransform(c), new PrependTransform().GetSignatures());
         
         RegisterTransform(_builder, (ctx, c) => new CommonPrefixTransform(c), new CommonPrefixTransform().GetSignatures());
         RegisterTransform(_builder, (ctx, c) => new CommonPrefixesTransform(c), new CommonPrefixesTransform().GetSignatures());

         RegisterTransform(_builder, (ctx, c) => new RegexMatchCountTransform(c), new RegexMatchCountTransform().GetSignatures());
         RegisterTransform(_builder, (ctx, c) => new CondenseTransform(c), new CondenseTransform().GetSignatures());
         RegisterTransform(_builder, (ctx, c) => new RandomTransform(c), new RandomTransform().GetSignatures());
         RegisterTransform(_builder, (ctx, c) => new IdentityTransform(c), new IdentityTransform().GetSignatures());
         RegisterTransform(_builder, (ctx, c) => new GuidTransform(c), new GuidTransform().GetSignatures());
         RegisterTransform(_builder, (ctx, c) => new HexTransform(c), new HexTransform().GetSignatures());

         // dates
         RegisterTransform(_builder, (ctx, c) => new DatePartTransform(c), new DatePartTransform().GetSignatures());
         RegisterTransform(_builder, (ctx, c) => new UtcNowTransform(c), new UtcNowTransform().GetSignatures());
         RegisterTransform(_builder, (ctx, c) => new DateDiffTransform(c), new DateDiffTransform().GetSignatures());
         RegisterTransform(_builder, (ctx, c) => new TimeZoneTransform(c), new TimeZoneTransform().GetSignatures());
         RegisterTransform(_builder, (ctx, c) => new TimeZoneOffsetTransform(c), new TimeZoneOffsetTransform().GetSignatures());
         RegisterTransform(_builder, (ctx, c) => new TimeAheadTransform(c), new TimeAheadTransform().GetSignatures());
         RegisterTransform(_builder, (ctx, c) => new TimeAgoTransform(c), new TimeAgoTransform().GetSignatures());
         RegisterTransform(_builder, (ctx, c) => new DateMathTransform(c), new DateMathTransform().GetSignatures());
         RegisterTransform(_builder, (ctx, c) => new IsDaylightSavingsTransform(c), new IsDaylightSavingsTransform().GetSignatures());
         RegisterTransform(_builder, (ctx, c) => new DateAddTransform(c), new DateAddTransform().GetSignatures());
         RegisterTransform(_builder, (ctx, c) => new SpecifyKindTransform(c), new SpecifyKindTransform().GetSignatures());
         RegisterTransform(_builder, (ctx, c) => new StartOfWeekTransform(c), new StartOfWeekTransform().GetSignatures());
         RegisterTransform(_builder, (ctx, c) => new NextDayTransform(c), new NextDayTransform().GetSignatures());
         RegisterTransform(_builder, (ctx, c) => new LastDayTransform(c), new LastDayTransform().GetSignatures());
         RegisterTransform(_builder, (ctx, c) => new ToUnixTimeTransform(c), new ToUnixTimeTransform().GetSignatures());
         RegisterTransform(_builder, (ctx, c) => new CountTransform(c), new CountTransform().GetSignatures());

         // getting properties from the configuration you're running in
         RegisterTransform(_builder, (ctx, c) => new ConnectionTransform(c), new ConnectionTransform().GetSignatures());
         RegisterTransform(_builder, (ctx, c) => new ParameterTransform(c), new ParameterTransform().GetSignatures());
         RegisterTransform(_builder, (ctx, c) => new ActionTransform(c), new ActionTransform().GetSignatures());
         RegisterTransform(_builder, (ctx, c) => new ScriptTransform(c), new ScriptTransform().GetSignatures());

         // produces string array
         RegisterTransform(_builder, (ctx, c) => new SplitTransform(c), new SplitTransform().GetSignatures());
         RegisterTransform(_builder, (ctx, c) => new SortTransform(c), new SortTransform().GetSignatures());
         RegisterTransform(_builder, (ctx, c) => new ReverseTransform(c), new ReverseTransform().GetSignatures());
         RegisterTransform(_builder, (ctx, c) => new DistinctTransform(c), new DistinctTransform().GetSignatures());
         RegisterTransform(_builder, (ctx, c) => new ToArrayTransform(c), new ToArrayTransform().GetSignatures());

         // can work with string arrays
         RegisterTransform(_builder, (ctx, c) => new LastTransform(c), new LastTransform().GetSignatures());
         RegisterTransform(_builder, (ctx, c) => new FirstTransform(c), new FirstTransform().GetSignatures());
         RegisterTransform(_builder, (ctx, c) => new GetTransform(c), new GetTransform().GetSignatures());
         RegisterTransform(_builder, (ctx, c) => new LengthTransform(c), new LengthTransform().GetSignatures());
         RegisterTransform(_builder, (ctx, c) => new JoinTransform(c), new JoinTransform().GetSignatures());

         // row producing and expects string array
         RegisterTransform(_builder, (ctx, c) => new ToRowTransform(c, ctx.ResolveNamed<IRowFactory>(c.Entity.Key, new NamedParameter("capacity", c.GetAllEntityFields().Count()))), new ToRowTransform().GetSignatures());

         // row filtering
         RegisterTransform(_builder, (ctx, c) => new FilterTransform(FilterType.Include, c), new FilterTransform(FilterType.Include).GetSignatures());
         RegisterTransform(_builder, (ctx, c) => new FilterTransform(FilterType.Exclude, c), new FilterTransform(FilterType.Exclude).GetSignatures());

         // field producing
         RegisterTransform(_builder, (ctx, c) => new FromSplitTransform(c), new FromSplitTransform().GetSignatures());
         RegisterTransform(_builder, (ctx, c) => new FromRegexTransform(c), new FromRegexTransform().GetSignatures());
         RegisterTransform(_builder, (ctx, c) => new FromLengthsTransform(c), new FromLengthsTransform().GetSignatures());

         // return true or false transforms
         RegisterTransform(_builder, (ctx, c) => new AnyTransform(c), new AnyTransform().GetSignatures());
         RegisterTransform(_builder, (ctx, c) => new StartsWithTransform(c), new StartsWithTransform().GetSignatures());
         RegisterTransform(_builder, (ctx, c) => new EndsWithTransform(c), new EndsWithTransform().GetSignatures());
         RegisterTransform(_builder, (ctx, c) => new InTransform(c), new InTransform().GetSignatures());
         RegisterTransform(_builder, (ctx, c) => new ContainsTransform(c), new ContainsTransform().GetSignatures());
         RegisterTransform(_builder, (ctx, c) => new IsTransform(c), new IsTransform().GetSignatures());
         RegisterTransform(_builder, (ctx, c) => new EqualsTransform(c), new EqualsTransform().GetSignatures());
         RegisterTransform(_builder, (ctx, c) => new IsEmptyTransform(c), new IsEmptyTransform().GetSignatures());
         RegisterTransform(_builder, (ctx, c) => new IsDefaultTransform(c), new IsDefaultTransform().GetSignatures());
         RegisterTransform(_builder, (ctx, c) => new IsNumericTransform(c), new IsNumericTransform().GetSignatures());
         RegisterTransform(_builder, (ctx, c) => new RegexIsMatchTransform(c), new RegexIsMatchTransform().GetSignatures());

         RegisterTransform(_builder, (ctx, c) => new ToBoolTransform(c), new ToBoolTransform().GetSignatures());

         // uncategorized
         RegisterTransform(_builder, (ctx, c) => new LogTransform(c), new LogTransform().GetSignatures());

         // xml
         // TODO: figure out how to handle fromXml that needs to produce rows
         RegisterTransform(_builder, (ctx, c) => new FromXmlTransform(c) as ITransform, new[] { new OperationSignature("fromxml") });

         foreach (var transform in _transforms) {
            RegisterTransform(_builder, (ctx, c) => transform.GetTransform(c), transform.Signatures);
         }

         //RegisterTransform(_builder, (ctx, c) => new DecompressTransform(c), new DecompressTransform().GetSignatures());
         //RegisterTransform(_builder, (ctx, c) => new CompressTransform(c), new CompressTransform().GetSignatures());
         //RegisterTransform(_builder, (ctx, c) => new FileExtTransform(c), new FileExtTransform().GetSignatures());
         //RegisterTransform(_builder, (ctx, c) => new FileNameTransform(c), new FileNameTransform().GetSignatures());
         //RegisterTransform(_builder, (ctx, c) => new FilePathTransform(c), new FilePathTransform().GetSignatures());
         //RegisterTransform(_builder, (ctx, c) => new HtmlEncodeTransform(c), new HtmlEncodeTransform().GetSignatures());
         //RegisterTransform(_builder, (ctx, c) => new LineTransform(c), new LineTransform().GetSignatures());
         //RegisterTransform(_builder, (ctx, c) => new VelocityTransform(c, ctx.Resolve<IReader>()), new VelocityTransform().GetSignatures());
         //RegisterTransform(_builder, (ctx, c) => new XPathTransform(c), new XPathTransform().GetSignatures());
         //RegisterTransform(_builder, (ctx, c) => new GeohashEncodeTransform(c), new GeohashEncodeTransform().GetSignatures());
         //RegisterTransform(_builder, (ctx, c) => new GeohashNeighborTransform(c), new GeohashNeighborTransform().GetSignatures());
         //RegisterTransform(_builder, (ctx, c) => new DistanceTransform(c), new DistanceTransform().GetSignatures());
         //RegisterTransform(_builder, (ctx, c) => new SlugifyTransform(c), new SlugifyTransform().GetSignatures());
         //RegisterTransform(_builder, (ctx, c) => new WebTransform(c), new WebTransform().GetSignatures());
         //RegisterTransform(_builder, (ctx, c) => new UrlEncodeTransform(c), new UrlEncodeTransform().GetSignatures());
         //RegisterTransform(_builder, (ctx, c) => new FromJsonTransform(c, o => JsonConvert.SerializeObject(o, Formatting.None)), new FromJsonTransform().GetSignatures());
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
               var existingParameters = _shortHand.Signatures.First(sg => sg.Name == s.Method).Parameters;
               if(existingParameters.Count == s.Parameters.Count) {
                  for (int i = 0; i < existingParameters.Count; i++) {
                     if(existingParameters[i].Name != s.Parameters[i].Name) {
                        _context.Warn($"There are multiple {s.Method} operations with conflicting parameters trying to register.");
                        break;
                     }
                  }
               }
            }

            builder.Register((ctx, p) => getTransform(ctx, p.Positional<IContext>(0))).Named<ITransform>(s.Method);
         }

      }

   }
}
