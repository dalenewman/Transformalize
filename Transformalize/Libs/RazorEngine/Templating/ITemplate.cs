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

namespace Transformalize.Libs.RazorEngine.Templating
{
    /// <summary>
    ///     Defines the required contract for implementing a template.
    /// </summary>
    public interface ITemplate
    {
        #region Properties

        /// <summary>
        ///     Sets the template service.
        /// </summary>
        ITemplateService TemplateService { set; }

        #endregion

        #region Methods

        /// <summary>
        ///     Executes the compiled template.
        /// </summary>
        void Execute();

        /// <summary>
        ///     Runs the template and returns the result.
        /// </summary>
        /// <param name="context">The current execution context.</param>
        /// <returns>The merged result of the template.</returns>
        string Run(ExecuteContext context);

        /// <summary>
        ///     Writes the specified object to the result.
        /// </summary>
        /// <param name="value">The value to write.</param>
        void Write(object value);

        /// <summary>
        ///     Writes the specified string to the result.
        /// </summary>
        /// <param name="literal">The literal to write.</param>
        void WriteLiteral(string literal);

        #endregion
    }
}