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

using System.CodeDom;

namespace Transformalize.Libs.RazorEngine.Compilation.Inspectors
{
    /// <summary>
    ///     Defines the required contract for implementing a code inspector.
    /// </summary>
    public interface ICodeInspector
    {
        #region Methods

        /// <summary>
        ///     Inspects the specified code unit.
        /// </summary>
        /// <param name="unit">The code unit.</param>
        /// <param name="ns">The code namespace declaration.</param>
        /// <param name="type">The code type declaration.</param>
        /// <param name="executeMethod">The code method declaration for the Execute method.</param>
        void Inspect(CodeCompileUnit unit, CodeNamespace ns, CodeTypeDeclaration type, CodeMemberMethod executeMethod);

        #endregion
    }
}