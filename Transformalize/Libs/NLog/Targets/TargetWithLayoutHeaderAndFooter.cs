#region License
// /*
// See license included in this library folder.
// */
#endregion

using Transformalize.Libs.NLog.Config;
using Transformalize.Libs.NLog.Layouts;

namespace Transformalize.Libs.NLog.Targets
{
    /// <summary>
    ///     Represents target that supports string formatting using layouts.
    /// </summary>
    public abstract class TargetWithLayoutHeaderAndFooter : TargetWithLayout
    {
        /// <summary>
        ///     Gets or sets the text to be rendered.
        /// </summary>
        /// <docgen category='Layout Options' order='1' />
        [RequiredParameter]
        public override Layout Layout
        {
            get { return LHF.Layout; }

            set
            {
                if (value is LayoutWithHeaderAndFooter)
                {
                    base.Layout = value;
                }
                else if (LHF == null)
                {
                    LHF = new LayoutWithHeaderAndFooter
                              {
                                  Layout = value
                              };
                }
                else
                {
                    LHF.Layout = value;
                }
            }
        }

        /// <summary>
        ///     Gets or sets the footer.
        /// </summary>
        /// <docgen category='Layout Options' order='3' />
        public Layout Footer
        {
            get { return LHF.Footer; }
            set { LHF.Footer = value; }
        }

        /// <summary>
        ///     Gets or sets the header.
        /// </summary>
        /// <docgen category='Layout Options' order='2' />
        public Layout Header
        {
            get { return LHF.Header; }
            set { LHF.Header = value; }
        }

        /// <summary>
        ///     Gets or sets the layout with header and footer.
        /// </summary>
        /// <value>The layout with header and footer.</value>
        private LayoutWithHeaderAndFooter LHF
        {
            get { return (LayoutWithHeaderAndFooter) base.Layout; }
            set { base.Layout = value; }
        }
    }
}