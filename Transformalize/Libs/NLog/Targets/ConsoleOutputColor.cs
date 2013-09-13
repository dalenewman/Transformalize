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


#if !NET_CF && !SILVERLIGHT

namespace Transformalize.Libs.NLog.Targets
{
    /// <summary>
    ///     Colored console output color.
    /// </summary>
    /// <remarks>
    ///     Note that this enumeration is defined to be binary compatible with
    ///     .NET 2.0 System.ConsoleColor + some additions
    /// </remarks>
    public enum ConsoleOutputColor
    {
        /// <summary>
        ///     Black Color (#000000).
        /// </summary>
        Black = 0,

        /// <summary>
        ///     Dark blue Color (#000080).
        /// </summary>
        DarkBlue = 1,

        /// <summary>
        ///     Dark green Color (#008000).
        /// </summary>
        DarkGreen = 2,

        /// <summary>
        ///     Dark Cyan Color (#008080).
        /// </summary>
        DarkCyan = 3,

        /// <summary>
        ///     Dark Red Color (#800000).
        /// </summary>
        DarkRed = 4,

        /// <summary>
        ///     Dark Magenta Color (#800080).
        /// </summary>
        DarkMagenta = 5,

        /// <summary>
        ///     Dark Yellow Color (#808000).
        /// </summary>
        DarkYellow = 6,

        /// <summary>
        ///     Gray Color (#C0C0C0).
        /// </summary>
        Gray = 7,

        /// <summary>
        ///     Dark Gray Color (#808080).
        /// </summary>
        DarkGray = 8,

        /// <summary>
        ///     Blue Color (#0000FF).
        /// </summary>
        Blue = 9,

        /// <summary>
        ///     Green Color (#00FF00).
        /// </summary>
        Green = 10,

        /// <summary>
        ///     Cyan Color (#00FFFF).
        /// </summary>
        Cyan = 11,

        /// <summary>
        ///     Red Color (#FF0000).
        /// </summary>
        Red = 12,

        /// <summary>
        ///     Magenta Color (#FF00FF).
        /// </summary>
        Magenta = 13,

        /// <summary>
        ///     Yellow Color (#FFFF00).
        /// </summary>
        Yellow = 14,

        /// <summary>
        ///     White Color (#FFFFFF).
        /// </summary>
        White = 15,

        /// <summary>
        ///     Don't change the color.
        /// </summary>
        NoChange = 16,
    }
}

#endif