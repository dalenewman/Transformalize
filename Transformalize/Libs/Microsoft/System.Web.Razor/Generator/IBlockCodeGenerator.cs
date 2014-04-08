// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved. See License.txt in the project root for license information.

using Transformalize.Libs.Microsoft.System.Web.Razor.Parser.SyntaxTree;

namespace Transformalize.Libs.Microsoft.System.Web.Razor.Generator
{
    public interface IBlockCodeGenerator
    {
        void GenerateStartBlockCode(Block target, CodeGeneratorContext context);
        void GenerateEndBlockCode(Block target, CodeGeneratorContext context);
    }
}
