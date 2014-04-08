// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved. See License.txt in the project root for license information.

using Transformalize.Libs.Microsoft.System.Web.Razor.Tokenizer.Symbols;

namespace Transformalize.Libs.Microsoft.System.Web.Razor.Tokenizer
{
    public interface ITokenizer
    {
        ISymbol NextSymbol();
    }
}
