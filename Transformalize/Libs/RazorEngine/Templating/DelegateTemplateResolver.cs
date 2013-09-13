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
using System.Diagnostics.Contracts;

namespace Transformalize.Libs.RazorEngine.Templating
{
    /// <summary>
    ///     Provides an <see cref="ITemplateResolver" /> that supports delegated template resolution.
    /// </summary>
    public class DelegateTemplateResolver : ITemplateResolver
    {
        #region Fields

        private readonly Func<string, string> _resolver;

        #endregion

        #region Constructor

        /// <summary>
        ///     Initialises a new instance of <see cref="DelegateTemplateResolver" />.
        /// </summary>
        /// <param name="resolver">The resolver delegate.</param>
        public DelegateTemplateResolver(Func<string, string> resolver)
        {
            Contract.Requires(resolver != null);

            _resolver = resolver;
        }

        #endregion

        #region Methods

        /// <summary>
        ///     Resolves the template content with the specified name.
        /// </summary>
        /// <param name="name">The name of the template to resolve.</param>
        /// <returns>The template content.</returns>
        public string Resolve(string name)
        {
            return _resolver(name);
        }

        #endregion
    }
}