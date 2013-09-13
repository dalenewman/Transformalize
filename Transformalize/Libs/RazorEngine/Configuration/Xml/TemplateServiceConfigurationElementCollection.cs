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

using System.Configuration;

namespace Transformalize.Libs.RazorEngine.Configuration.Xml
{
    /// <summary>
    ///     Defines a collection of <see cref="TemplateServiceConfigurationElement" /> instances.
    /// </summary>
    [ConfigurationCollection(typeof (TemplateServiceConfigurationElement), AddItemName = "service")]
    public class TemplateServiceConfigurationElementCollection : ConfigurationElementCollection
    {
        #region Methods

        /// <summary>
        ///     Creates a new <see cref="ConfigurationElement" /> for use with the collection.
        /// </summary>
        /// <returns>
        ///     The <see cref="ConfigurationElement" /> instance.
        /// </returns>
        protected override ConfigurationElement CreateNewElement()
        {
            return new TemplateServiceConfigurationElement();
        }

        /// <summary>
        ///     Gets a unique key for the specified element.
        /// </summary>
        /// <param name="element">The configuration element.</param>
        /// <returns>The key for the element.</returns>
        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((TemplateServiceConfigurationElement) element).Name;
        }

        #endregion
    }
}