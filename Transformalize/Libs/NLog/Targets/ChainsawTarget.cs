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

namespace Transformalize.Libs.NLog.Targets
{
    /// <summary>
    ///     Sends log messages to the remote instance of Chainsaw application from log4j.
    /// </summary>
    /// <seealso href="http://nlog-project.org/wiki/Chainsaw_target">Documentation on NLog Wiki</seealso>
    /// <example>
    ///     <p>
    ///         To set up the target in the <a href="config.html">configuration file</a>,
    ///         use the following syntax:
    ///     </p>
    ///     <code lang="XML" source="examples/targets/Configuration File/Chainsaw/NLog.config" />
    ///     <p>
    ///         This assumes just one target and a single rule. More configuration
    ///         options are described <a href="config.html">here</a>.
    ///     </p>
    ///     <p>
    ///         To set up the log target programmatically use code like this:
    ///     </p>
    ///     <code lang="C#" source="examples/targets/Configuration API/Chainsaw/Simple/Example.cs" />
    ///     <p>
    ///         NOTE: If your receiver application is ever likely to be off-line, don't use TCP protocol
    ///         or you'll get TCP timeouts and your application will crawl.
    ///         Either switch to UDP transport or use <a href="target.AsyncWrapper.html">AsyncWrapper</a> target
    ///         so that your application threads will not be blocked by the timing-out connection attempts.
    ///     </p>
    /// </example>
    [Target("Chainsaw")]
    public class ChainsawTarget : NLogViewerTarget
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="ChainsawTarget" /> class.
        /// </summary>
        public ChainsawTarget()
        {
            IncludeNLogData = false;
        }
    }
}