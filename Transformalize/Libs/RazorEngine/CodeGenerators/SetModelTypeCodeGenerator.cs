#region License

// /*
// Transformalize - Replicate, Transform, and Denormalize Your Data...
// Copyright (C) 2013 Dale Newman
// 
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with this program.  If not, see <http://www.gnu.org/licenses/>.
// */

#endregion

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