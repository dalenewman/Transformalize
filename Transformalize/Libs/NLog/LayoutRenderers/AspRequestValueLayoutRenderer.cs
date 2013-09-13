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

using System;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Text;
using Transformalize.Libs.NLog.Config;
using Transformalize.Libs.NLog.Internal;

#if !NET_CF && !SILVERLIGHT

namespace Transformalize.Libs.NLog.LayoutRenderers
{
    /// <summary>
    ///     ASP Request variable.
    /// </summary>
    [LayoutRenderer("asp-request")]
    public class AspRequestValueLayoutRenderer : LayoutRenderer
    {
        /// <summary>
        ///     Gets or sets the item name. The QueryString, Form, Cookies, or ServerVariables collection variables having the specified name are rendered.
        /// </summary>
        /// <docgen category='Rendering Options' order='10' />
        [DefaultParameter]
        public string Item { get; set; }

        /// <summary>
        ///     Gets or sets the QueryString variable to be rendered.
        /// </summary>
        /// <docgen category='Rendering Options' order='10' />
        public string QueryString { get; set; }

        /// <summary>
        ///     Gets or sets the form variable to be rendered.
        /// </summary>
        /// <docgen category='Rendering Options' order='10' />
        public string Form { get; set; }

        /// <summary>
        ///     Gets or sets the cookie to be rendered.
        /// </summary>
        /// <docgen category='Rendering Options' order='10' />
        public string Cookie { get; set; }

        /// <summary>
        ///     Gets or sets the ServerVariables item to be rendered.
        /// </summary>
        /// <docgen category='Rendering Options' order='10' />
        public string ServerVariable { get; set; }

        /// <summary>
        ///     Renders the specified ASP Request variable and appends it to the specified <see cref="StringBuilder" />.
        /// </summary>
        /// <param name="builder">
        ///     The <see cref="StringBuilder" /> to append the rendered data to.
        /// </param>
        /// <param name="logEvent">Logging event.</param>
        protected override void Append(StringBuilder builder, LogEventInfo logEvent)
        {
            var request = AspHelper.GetRequestObject();
            if (request != null)
            {
                if (QueryString != null)
                {
                    builder.Append(GetItem(request.GetQueryString(), QueryString));
                }
                else if (Form != null)
                {
                    builder.Append(GetItem(request.GetForm(), Form));
                }
                else if (Cookie != null)
                {
                    var cookie = request.GetCookies().GetItem(Cookie);
                    builder.Append(Convert.ToString(AspHelper.GetComDefaultProperty(cookie), CultureInfo.InvariantCulture));
                }
                else if (ServerVariable != null)
                {
                    builder.Append(GetItem(request.GetServerVariables(), ServerVariable));
                }
                else if (Item != null)
                {
                    var o = request.GetItem(Item);
                    var sl = o as AspHelper.IStringList;
                    if (sl != null)
                    {
                        if (sl.GetCount() > 0)
                        {
                            builder.Append(sl.GetItem(1));
                        }

                        Marshal.ReleaseComObject(sl);
                    }
                }

                Marshal.ReleaseComObject(request);
            }
        }

        private static string GetItem(AspHelper.IRequestDictionary dict, string key)
        {
            object retVal = null;
            var o = dict.GetItem(key);
            var sl = o as AspHelper.IStringList;
            if (sl != null)
            {
                if (sl.GetCount() > 0)
                {
                    retVal = sl.GetItem(1);
                }

                Marshal.ReleaseComObject(sl);
            }
            else
            {
                return o.GetType().ToString();
            }

            return Convert.ToString(retVal, CultureInfo.InvariantCulture);
        }
    }
}

#endif