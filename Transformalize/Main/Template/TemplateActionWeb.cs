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

namespace Transformalize.Main {
    public class TemplateActionWeb : TemplateActionHandler {
        public override void Handle(TemplateAction action) {
            var method = action.Method.ToLower();
            if (!string.IsNullOrEmpty(action.Url)) {
                var response = method == "post" ? Web.Post(action.Url, string.Empty) : Web.Get(action.Url);
                if (response.Code == HttpStatusCode.OK) {
                    TflLogger.Info(action.ProcessName, string.Empty, "Made web request to {0}.", action.Url);
                } else {
                    TflLogger.Warn(action.ProcessName, string.Empty, "Web request to {0} returned {1}.", action.Url, response.Code);
                }
            } else {
                TflLogger.Warn(action.ProcessName, string.Empty, "Missing url for web action.");
            }
        }
    }
}