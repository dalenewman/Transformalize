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

using System.ComponentModel;
using Transformalize.Libs.NLog.Config;
using Transformalize.Libs.NLog.Layouts;

#if !SILVERLIGHT

namespace Transformalize.Libs.NLog.Targets
{
    /// <summary>
    ///     Represents a parameter to a Database target.
    /// </summary>
    [NLogConfigurationItem]
    public class DatabaseParameterInfo
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="DatabaseParameterInfo" /> class.
        /// </summary>
        public DatabaseParameterInfo()
            : this(null, null)
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="DatabaseParameterInfo" /> class.
        /// </summary>
        /// <param name="parameterName">Name of the parameter.</param>
        /// <param name="parameterLayout">The parameter layout.</param>
        public DatabaseParameterInfo(string parameterName, Layout parameterLayout)
        {
            Name = parameterName;
            Layout = parameterLayout;
        }

        /// <summary>
        ///     Gets or sets the database parameter name.
        /// </summary>
        /// <docgen category='Parameter Options' order='10' />
        [RequiredParameter]
        public string Name { get; set; }

        /// <summary>
        ///     Gets or sets the layout that should be use to calcuate the value for the parameter.
        /// </summary>
        /// <docgen category='Parameter Options' order='10' />
        [RequiredParameter]
        public Layout Layout { get; set; }

        /// <summary>
        ///     Gets or sets the database parameter size.
        /// </summary>
        /// <docgen category='Parameter Options' order='10' />
        [DefaultValue(0)]
        public int Size { get; set; }

        /// <summary>
        ///     Gets or sets the database parameter precision.
        /// </summary>
        /// <docgen category='Parameter Options' order='10' />
        [DefaultValue(0)]
        public byte Precision { get; set; }

        /// <summary>
        ///     Gets or sets the database parameter scale.
        /// </summary>
        /// <docgen category='Parameter Options' order='10' />
        [DefaultValue(0)]
        public byte Scale { get; set; }
    }
}

#endif