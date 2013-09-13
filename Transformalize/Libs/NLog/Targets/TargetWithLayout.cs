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
using System.Diagnostics.CodeAnalysis;
using Transformalize.Libs.NLog.Config;
using Transformalize.Libs.NLog.Layouts;

namespace Transformalize.Libs.NLog.Targets
{
    /// <summary>
    ///     Represents target that supports string formatting using layouts.
    /// </summary>
    public abstract class TargetWithLayout : Target
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="TargetWithLayout" /> class.
        /// </summary>
        /// <remarks>
        ///     The default value of the layout is: <code>${longdate}|${level:uppercase=true}|${logger}|${message}</code>
        /// </remarks>
        [SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors", Justification = "This one is safe.")]
        protected TargetWithLayout()
        {
            Layout = "${longdate}|${level:uppercase=true}|${logger}|${message}";
        }

        /// <summary>
        ///     Gets or sets the layout used to format log messages.
        /// </summary>
        /// <docgen category='Layout Options' order='1' />
        [RequiredParameter]
        [DefaultValue("${longdate}|${level:uppercase=true}|${logger}|${message}")]
        public virtual Layout Layout { get; set; }
    }
}