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

using Transformalize.Libs.RazorEngine.Configuration;

namespace Transformalize.Libs.RazorEngine.Templating
{
    /// <summary>
    ///     Provides factory methods for creating <see cref="ITemplateService" /> instances.
    /// </summary>
    public static class TemplateServiceFactory
    {
        #region Fields

        private static readonly RazorEngineConfigurationSection Configuration;

        #endregion

        #region Constructor

        /// <summary>
        ///     Initialises the <see cref="TemplateServiceFactory" /> type.
        /// </summary>
        static TemplateServiceFactory()
        {
            Configuration = RazorEngineConfigurationSection.GetConfiguration();
        }

        #endregion

        #region Methods

        /// <summary>
        ///     Creates an instance of a template service.
        /// </summary>
        /// <param name="language">The language to use in this service.</param>
        /// <param name="encoding">The type of encoding to use in this service.</param>
        /// <returns></returns>
        public static ITemplateService CreateTemplateService(Language language, Encoding encoding)
        {
            return null;
        }

        #endregion
    }
}