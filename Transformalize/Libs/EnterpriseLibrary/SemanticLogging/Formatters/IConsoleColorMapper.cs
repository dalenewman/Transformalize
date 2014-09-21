#region license
// ==============================================================================
// Microsoft patterns & practices Enterprise Library
// Semantic Logging Application Block
// ==============================================================================
// Copyright © Microsoft Corporation.  All rights reserved.
// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY
// OF ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT
// LIMITED TO THE IMPLIED WARRANTIES OF MERCHANTABILITY AND
// FITNESS FOR A PARTICULAR PURPOSE.
// ==============================================================================
#endregion

using System;
using System.Diagnostics.Tracing;

namespace Transformalize.Libs.EnterpriseLibrary.SemanticLogging.Formatters
{
    /// <summary>
    /// Provides mapping between an <see cref="EventLevel"/> and a console foreground color.
    /// </summary>
    public interface IConsoleColorMapper
    {
        /// <summary>
        /// Maps the specified <see cref="System.Diagnostics.Tracing.EventLevel"/> to a <see cref="System.ConsoleColor"/>
        /// </summary>
        /// <param name="eventLevel">The <see cref="System.Diagnostics.Tracing.EventLevel"/>.</param>
        /// <returns>The <see cref="System.ConsoleColor"/>.</returns>
        ConsoleColor? Map(EventLevel eventLevel);
    }
}
