// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved. See License.txt in the project root for license information.

using Transformalize.Libs.Microsoft.System.Web.Razor.Text;

namespace Transformalize.Libs.Microsoft.System.Web.Razor.Tokenizer.Symbols
{
    public interface ISymbol
    {
        SourceLocation Start { get; }
        string Content { get; }

        void OffsetStart(SourceLocation documentStart);
        void ChangeStart(SourceLocation newStart);
    }
}
