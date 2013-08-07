/*
Transformalize - Replicate, Transform, and Denormalize Your Data...
Copyright (C) 2013 Dale Newman

This program is free software: you can redistribute it and/or modify
it under the terms of the GNU General Public License as published by
the Free Software Foundation, either version 3 of the License, or
(at your option) any later version.

This program is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU General Public License for more details.

You should have received a copy of the GNU General Public License
along with this program.  If not, see <http://www.gnu.org/licenses/>.
*/

using System;
using System.Collections.Generic;
using System.Linq;
using Transformalize.Libs.NLog;
using Transformalize.Libs.fastJSON;

namespace Transformalize.Core
{
    public class Options
    {
        public List<string> Problems = new List<string>(); 

        public Modes Mode { get; set; }
        public bool UseBeginVersion { get; set; }
        public bool WriteEndVersion { get; set; }
        public bool RenderTemplates { get; set; }
        public bool PerformTemplateActions { get; set; }
        public int TestCount { get; set; }

        private readonly Logger _log = LogManager.GetCurrentClassLogger();

        public Options(string settings = "")
        {
            SetDefaults();
            if (!string.IsNullOrEmpty(settings))
            {
                try
                {
                    if (settings.Contains("'"))
                    {
                        settings = settings.Replace('\'','"');
                    }
                    var options = JSON.Instance.ToObject<Dictionary<string, object>>(settings);

                    foreach (var option in options)
                    {
                        var key = option.Key.ToLower();
                        var value = option.Value.ToString().ToLower();
                        bool input;
                        switch (key)
                        {
                            case "mode":
                                if (value.StartsWith("init"))
                                {
                                    Mode = Modes.Initialize;
                                }
                                break;
                            case "rendertemplates":
                                if (bool.TryParse(value, out input))
                                {
                                    RenderTemplates = input;
                                }
                                else
                                {
                                    RecordBadValue(option);
                                }
                                break;
                            case "usebeginversion":
                                if (bool.TryParse(value, out input))
                                {
                                    UseBeginVersion = input;
                                }
                                else
                                {
                                    RecordBadValue(option);
                                }

                                break;
                            case "writeendversion":
                                if (bool.TryParse(value, out input))
                                {
                                    WriteEndVersion = input;
                                }
                                else
                                {
                                    RecordBadValue(option);
                                }
                                break;

                            case "performtemplateactions":
                                if (bool.TryParse(value, out input))
                                {
                                    PerformTemplateActions = input;
                                }
                                else
                                {
                                    RecordBadValue(option);
                                }
                                break;

                            default:
                                RecordBadProperty(option);
                                break;
                        }
                    }

                }
                catch (Exception e)
                {
                    var message = string.Format("Couldn't parse options: {0}.", settings);
                    _log.DebugException(message + " " + e.Message, e);
                    Problems.Add(message);
                }

            }
        }

        public bool IsValid()
        {
            return !Problems.Any();
        }

        private void RecordBadValue(KeyValuePair<string, object> option)
        {
            Problems.Add(string.Format("The {0} option value of {1} must evaluate to True or False.", option.Key, option.Value));
        }

        private void RecordBadProperty(KeyValuePair<string, object> option)
        {
            Problems.Add(string.Format("The {0} property is invalid.", option.Key));
        }

        private void SetDefaults()
        {
            UseBeginVersion = true;
            WriteEndVersion = true;
            RenderTemplates = true;
            PerformTemplateActions = true;
            Mode = Modes.Normal;
            TestCount = 20;
        }

    }
}