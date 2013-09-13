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

using System.Net;

namespace Transformalize.Main.Template_
{
    public class TemplateActionWeb : TemplateActionHandler
    {
        public override void Handle(TemplateAction action)
        {
            string method = action.Method.ToLower();
            if (!string.IsNullOrEmpty(action.Url))
            {
                WebResponse response = method == "post" ? Web.Post(action.Url, string.Empty) : Web.Get(action.Url);
                if (response.Code == HttpStatusCode.OK)
                {
                    Log.Info("Made web request to {0}.", action.Url);
                }
                else
                {
                    Log.Warn("Web request to {0} returned {1}.", action.Url, response.Code);
                }
            }
            else
            {
                Log.Warn("Missing url for web action.");
            }
        }
    }
}