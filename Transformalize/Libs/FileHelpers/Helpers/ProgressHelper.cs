#region License
// /*
// See license included in this library folder.
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