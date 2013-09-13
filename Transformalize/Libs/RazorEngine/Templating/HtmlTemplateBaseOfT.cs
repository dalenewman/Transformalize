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
    ///     Provides a base implementation of an html template with a model.
    /// </summary>
    /// <remarks>
    ///     This type does not currently serve a purpose, and the WriteAttribute* API has been migrated to the TemplateBase type. This type is not deprecated, as it
    ///     may form the basis for a future template that supports MVC like @Html syntax.
    /// </remarks>
    /// <typeparam name="T">The model type.</typeparam>
    public class HtmlTemplateBase<T> : TemplateBase<T>
    {
    }
}