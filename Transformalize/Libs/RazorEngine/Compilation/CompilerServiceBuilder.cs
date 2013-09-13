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

using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Contracts;
using Transformalize.Libs.RazorEngine.Configuration;

namespace Transformalize.Libs.RazorEngine.Compilation
{
    /// <summary>
    ///     Manages creation of <see cref="ICompilerService" /> instances.
    /// </summary>
    public static class CompilerServiceBuilder
    {
        #region Fields

        private static ICompilerServiceFactory _factory = new DefaultCompilerServiceFactory();
        private static readonly object sync = new object();

        #endregion

        #region Methods

        /// <summary>
        ///     Sets the <see cref="ICompilerServiceFactory" /> used to create compiler service instances.
        /// </summary>
        /// <param name="factory">The compiler service factory to use.</param>
        public static void SetCompilerServiceFactory(ICompilerServiceFactory factory)
        {
            Contract.Requires(factory != null);

            lock (sync)
            {
                _factory = factory;
            }
        }

        /// <summary>
        ///     Gets the <see cref="ICompilerService" /> for the specfied language.
        /// </summary>
        /// <param name="language">The code language.</param>
        /// <returns>The compiler service instance.</returns>
        public static ICompilerService GetCompilerService(Language language)
        {
            lock (sync)
            {
                return _factory.CreateCompilerService(language);
            }
        }

        /// <summary>
        ///     Gets the <see cref="ICompilerService" /> for the default <see cref="Language" />.
        /// </summary>
        /// <returns>The compiler service instance.</returns>
        [SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate")]
        public static ICompilerService GetDefaultCompilerService()
        {
            var config = RazorEngineConfigurationSection.GetConfiguration();
            if (config == null)
                return GetCompilerService(Language.CSharp);

            return GetCompilerService(config.DefaultLanguage);
        }

        #endregion
    }
}