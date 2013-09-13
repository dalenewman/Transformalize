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
using System.Collections.Generic;
using Transformalize.Libs.RazorEngine.Compilation;
using Transformalize.Libs.RazorEngine.Compilation.Inspectors;
using Transformalize.Libs.RazorEngine.Templating;
using Transformalize.Libs.RazorEngine.Text;

namespace Transformalize.Libs.RazorEngine.Configuration
{
    /// <summary>
    ///     Defines the required contract for implementing template service configuration.
    /// </summary>
    public interface ITemplateServiceConfiguration
    {
        #region Properties

        /// <summary>
        ///     Gets the activator.
        /// </summary>
        IActivator Activator { get; }

        /// <summary>
        ///     Gets the base template type.
        /// </summary>
        Type BaseTemplateType { get; }

        /// <summary>
        ///     Gets the code inspectors.
        /// </summary>
        IEnumerable<ICodeInspector> CodeInspectors { get; }

        /// <summary>
        ///     Gets the compiler service factory.
        /// </summary>
        ICompilerServiceFactory CompilerServiceFactory { get; }

        /// <summary>
        ///     Gets whether the template service is operating in debug mode.
        /// </summary>
        bool Debug { get; }

        /// <summary>
        ///     Gets the encoded string factory.
        /// </summary>
        IEncodedStringFactory EncodedStringFactory { get; }

        /// <summary>
        ///     Gets the language.
        /// </summary>
        Language Language { get; }

        /// <summary>
        ///     Gets the namespaces.
        /// </summary>
        ISet<string> Namespaces { get; }

        /// <summary>
        ///     Gets the template resolver.
        /// </summary>
        ITemplateResolver Resolver { get; }

        #endregion
    }
}