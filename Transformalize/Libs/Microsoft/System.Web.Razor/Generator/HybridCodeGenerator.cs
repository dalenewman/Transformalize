// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved. See License.txt in the project root for license information.

using Transformalize.Libs.Microsoft.System.Web.Razor.Parser.SyntaxTree;

namespace Transformalize.Libs.Microsoft.System.Web.Razor.Generator
{
    public abstract class HybridCodeGenerator : ISpanCodeGenerator, IBlockCodeGenerator
    {
        public virtual void GenerateStartBlockCode(Block target, CodeGeneratorContext context)
        {
        }

        public virtual void GenerateEndBlockCode(Block target, CodeGeneratorContext context)
        {
        }

        public virtual void GenerateCode(Span target, CodeGeneratorContext context)
        {
        }
    }
}
