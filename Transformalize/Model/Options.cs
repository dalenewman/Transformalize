using System;
using System.Collections.Generic;
using System.Linq;
using Transformalize.Libs.NLog;
using Transformalize.Libs.fastJSON;

namespace Transformalize.Model
{
    public class Options
    {
        public List<string> Problems = new List<string>(); 

        public Modes Mode { get; set; }
        public bool UseBeginVersion { get; set; }
        public bool WriteEndVersion { get; set; }
        public bool RenderTemplates { get; set; }
        public bool PerformTemplateActions { get; set; }

        private readonly Logger _log = LogManager.GetCurrentClassLogger();

        public Options(string settings)
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
        }

    }
}