//===============================================================================
// Microsoft patterns & practices Enterprise Library
// Core
//===============================================================================
// Copyright © Microsoft Corporation.  All rights reserved.
// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY
// OF ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT
// LIMITED TO THE IMPLIED WARRANTIES OF MERCHANTABILITY AND
// FITNESS FOR A PARTICULAR PURPOSE.
//===============================================================================

using System;

namespace Transformalize.Libs.EnterpriseLibrary.Common.Utility
{
    /// <summary>
    /// Resolves strings by invoking a delegate and returning the resulting value.
    /// </summary>
    public sealed class DelegateStringResolver : IStringResolver
    {
        /// <summary>
        /// Initializes a new instance of <see cref="ConstantStringResolver"/> with a delegate.
        /// </summary>
        /// <param name="resolverDelegate">The delegate to invoke when resolving a string.</param>
        public DelegateStringResolver(Func<string> resolverDelegate)
        {
            this.resolverDelegate = resolverDelegate;
        }

        private readonly Func<string> resolverDelegate;

        string IStringResolver.GetString()
        {
            return this.resolverDelegate();
        }
    }
}
