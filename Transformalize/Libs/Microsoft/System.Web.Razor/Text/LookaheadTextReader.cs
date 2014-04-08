// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved. See License.txt in the project root for license information.

using System;
using System.IO;

namespace Transformalize.Libs.Microsoft.System.Web.Razor.Text
{
    public abstract class LookaheadTextReader : TextReader
    {
        public abstract SourceLocation CurrentLocation { get; }
        public abstract IDisposable BeginLookahead();
        public abstract void CancelBacktrack();
    }
}
