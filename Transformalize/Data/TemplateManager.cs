using System;
using System.IO;
using Transformalize.Libs.Rhino.Etl.Core;
using Transformalize.Model;

namespace Transformalize.Data
{
    public class TemplateManager : WithLoggingMixin
    {
        private readonly Process _process;
        private readonly char[] _trim = new[] { '\\' };
        public TemplateManager(Process process)
        {
            _process = process;
        }

        public void Manage()
        {

            var folder = GetTemporaryFolder();
            if (!Directory.Exists(folder))
                Directory.CreateDirectory(folder);

            foreach (var pair in _process.Templates)
            {
                var result = pair.Value.Render();
                Info("{0} | Rendered {1} template.", _process.Name, pair.Value.Name);

                var renderedInfo = new FileInfo(folder.TrimEnd(_trim) + @"\" + pair.Value.Name + ".temp.txt");
                File.WriteAllText(renderedInfo.FullName, result);

                if (!_process.Options.PerformTemplateActions)
                    continue;

                foreach (var action in pair.Value.Actions)
                {
                    var actionFile = string.IsNullOrEmpty(action.File)
                                         ? string.Empty
                                         : new FileInfo(action.File).FullName;

                    switch (action.Action.ToLower())
                    {
                        case "copy":
                            if (actionFile != string.Empty)
                            {
                                File.Copy(renderedInfo.FullName, actionFile, true);
                                Info("{0} | Copied {1} template output to {2}.", _process.Name, pair.Value.Name, actionFile);
                            }
                            break;
                        case "open":
                            var openFile = actionFile == string.Empty ? renderedInfo.FullName : actionFile;
                            System.Diagnostics.Process.Start(openFile);
                            Info("{0} | Opened file {1}.", _process.Name, openFile);
                            break;
                        default:
                            Warn("{0} | The {1} action is not implemented.", _process.Name, action.Action);
                            break;

                    }
                }


            }
        }


        public string GetTemporaryFolder()
        {
            return Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData).TrimEnd(_trim) + @"\Tfl\" + _process.Name;
        }
    }
}