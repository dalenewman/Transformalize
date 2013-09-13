using System;
using System.Globalization;
using System.Web.Razor.Generator;
using Transformalize.Libs.RazorEngine.Common;

namespace Transformalize.Libs.RazorEngine.CodeGenerators
{
    internal class SetModelTypeCodeGenerator : SetBaseTypeCodeGenerator
    {
        private readonly string _genericTypeFormat;

        public SetModelTypeCodeGenerator(string modelType, string genericTypeFormat)
            : base(modelType)
        {
            _genericTypeFormat = genericTypeFormat;
        }

        protected override string ResolveType(CodeGeneratorContext context, string baseType)
        {
            return String.Format(
                CultureInfo.InvariantCulture,
                _genericTypeFormat,
                context.Host.DefaultBaseClass,
                baseType);
        }

        public override bool Equals(object obj)
        {
            var other = obj as SetModelTypeCodeGenerator;
            return other != null &&
                   base.Equals(obj) &&
                   String.Equals(_genericTypeFormat, other._genericTypeFormat, StringComparison.Ordinal);
        }

        public override int GetHashCode()
        {
            return HashCodeCombiner.Start()
                                   .Add(base.GetHashCode())
                                   .Add(_genericTypeFormat)
                                   .CombinedHash;
        }

        public override string ToString()
        {
            return "Model:" + BaseType;
        }
    }
}