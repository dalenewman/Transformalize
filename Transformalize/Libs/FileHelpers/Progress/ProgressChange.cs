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

using Transformalize.Libs.FileHelpers.Enums;

namespace Transformalize.Libs.FileHelpers.Progress
{
#if ! MINI

    /// <summary>Class used to notify the current progress position and other context info.</summary>
    public class ProgressEventArgs
    {
        private readonly int mProgressCurrent;
        private readonly ProgressMode mProgressMode = ProgressMode.DontNotify;
        private readonly int mProgressTotal;

        internal ProgressEventArgs(ProgressMode mode, int current, int total)
        {
            mProgressMode = mode;
            mProgressCurrent = current;
            mProgressTotal = total;
        }

        internal ProgressEventArgs()
        {
            mProgressMode = ProgressMode.DontNotify;
        }

        /// <summary>The current progress position. Check also the ProgressMode property.</summary>
        public int ProgressCurrent
        {
            get { return mProgressCurrent; }
        }

        /// <summary>
        ///     The total when the progress finish. (<b>-1 means undefined</b>)
        /// </summary>
        public int ProgressTotal
        {
            get { return mProgressTotal; }
        }

        /// <summary>The ProgressMode used.</summary>
        public ProgressMode ProgressMode
        {
            get { return mProgressMode; }
        }
    }

    /// <summary>Delegate used to notify progress to the user.</summary>
    /// <param name="e">The Event args with information about the progress.</param>
    public delegate void ProgressChangeHandler(ProgressEventArgs e);

#endif
}