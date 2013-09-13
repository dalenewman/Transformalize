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
using Transformalize.Libs.FileHelpers.Progress;

namespace Transformalize.Libs.FileHelpers.Helpers
{
    internal sealed class ProgressHelper
    {
        private ProgressHelper()
        {
        }

        public static void Notify(ProgressChangeHandler handler, ProgressMode mode, int current, int total)
        {
            if (handler == null)
                return;

            if (mode == ProgressMode.DontNotify)
                return;

            switch (mode)
            {
                case ProgressMode.NotifyBytes:
                    handler(new ProgressEventArgs(mode, current, total));
                    break;

                case ProgressMode.NotifyRecords:
                    handler(new ProgressEventArgs(mode, current, total));
                    break;

                case ProgressMode.NotifyPercent:
                    if (total == -1)
                        return;
                    handler(new ProgressEventArgs(mode, (current*100/total), 100));
                    break;
            }
        }
    }
}