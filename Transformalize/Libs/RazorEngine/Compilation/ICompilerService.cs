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
using System.Reflection;
using Transformalize.Libs.RazorEngine.Compilation.Inspectors;

namespace Transformalize.Libs.RazorEngine.Compilation
{
    /// <summary>
    ///     Defines the required contract for implementing a compiler service.
    /// </summary>
    public interface ICompilerService
    {
        #region Properties

        /// <summary>
        ///     Gets or sets the set of code inspectors.
        /// </summary>
        IEnumerable<ICodeInspector> CodeInspectors { get; set; }

        /// <summary>
        ///     Gets or sets whether the compiler service is operating in debug mode.
        /// </summary>
        bool Debug { get; set; }

        #endregion

        #region Methods

        /// <summary>
        ///     Builds a type name for the specified template type.
        /// </summary>
        /// <param name="templateType">The template type.</param>
        /// <returns>The string type name (including namespace).</returns>
        string BuildTypeName(Type templateType);

        /// <summary>
        ///     Compiles the type defined in the specified type context.
        /// </summary>
        /// <param name="context">The type context which defines the type to compile.</param>
        /// <returns>The compiled type.</returns>
        Tuple<Type, Assembly> CompileType(TypeContext context);

        /// <summary>
        ///     Returns a set of assemblies that must be referenced by the compiled template.
        /// </summary>
        /// <returns>The set of assemblies.</returns>
        IEnumerable<string> IncludeAssemblies();

        #endregion
    }
}