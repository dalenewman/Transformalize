
using Cfg.Net.Environment;
using Cfg.Net.Parsers;
using Cfg.Net.Serializers;
using Transformalize.Configuration;
using Pipeline.Web.Orchard.Models;
using Transformalize.Transforms.DateMath;
using Orchard;
using Orchard.Logging;
using Pipeline.Web.Orchard.Modules;
using Transformalize.Impl;
using ILogger = Orchard.Logging.ILogger;
using OrchardDependency = Orchard.IDependency;

namespace Pipeline.Web.Orchard.Services {

    public interface IProcessService : OrchardDependency {
        Process Resolve(PipelineConfigurationPart part, string input, string output);
        Process Resolve(PipelineConfigurationPart part);
    }

    public class ProcessService : IProcessService {

        private readonly IOrchardServices _orchard;

        public ILogger Logger { get; set; }

        public ProcessService(IOrchardServices orchard) {
            _orchard = orchard;
            Logger = NullLogger.Instance;
        }

        public Process Resolve(PipelineConfigurationPart part, string input, string output) {
            var marker = part.PlaceHolderStyle[0];
            var prefix = part.PlaceHolderStyle[1];
            var suffix = part.PlaceHolderStyle[2];
            var modifier = new EnvironmentModifier(new PlaceHolderReplacer(marker, prefix, suffix), "environments", "environment", "name", "parameters", "name", "value");

            switch (input) {
                case "json":
                    switch (output) {
                        case "json":
                            return new Process(
                                new FormParameterModifier(new DateMathModifier()),
                                new FastJsonParser(),
                                new JsonSerializer(),
                                _orchard.WorkContext.Resolve<TransformShorthandCustomizer>(),
                                _orchard.WorkContext.Resolve<ValidateShorthandCustomizer>(),
                                modifier
                            );
                        default:  // xml
                            return new Process(
                                new FormParameterModifier(new DateMathModifier()),
                                new FastJsonParser(),
                                new XmlSerializer(),
                                _orchard.WorkContext.Resolve<TransformShorthandCustomizer>(),
                                _orchard.WorkContext.Resolve<ValidateShorthandCustomizer>(),
                                modifier
                            );
                    }
                default:
                    switch (output) {
                        case "json":
                            return new Process(
                                new FormParameterModifier(new DateMathModifier()),
                                new NanoXmlParser(),
                                new JsonSerializer(),
                                _orchard.WorkContext.Resolve<TransformShorthandCustomizer>(),
                                _orchard.WorkContext.Resolve<ValidateShorthandCustomizer>(),
                                modifier
                            );
                        default: // xml
                            return new Process(
                                new FormParameterModifier(new DateMathModifier()),
                                new NanoXmlParser(),
                                new XmlSerializer(),
                                _orchard.WorkContext.Resolve<TransformShorthandCustomizer>(),
                                _orchard.WorkContext.Resolve<ValidateShorthandCustomizer>(),
                                modifier
                            );
                    }
            }
        }

        public Process Resolve(PipelineConfigurationPart part) {
            return Resolve(part, part.EditorMode, part.EditorMode);
        }

    }
}
