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

using Transformalize.Libs.NLog.Config;
using Transformalize.Libs.NLog.Layouts;

namespace Transformalize.Libs.NLog.Targets
{
    /// <summary>
    ///     Represents a parameter to a NLogViewer target.
    /// </summary>
    [NLogConfigurationItem]
    public class NLogViewerParameterInfo
    {
        /// <summary>
        ///     Gets or sets viewer parameter name.
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
    }
}