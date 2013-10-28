using System;
using System.IO;
using Transformalize.Libs.NLog;

namespace Transformalize.Runner
{
    public class ContentsFileReader : IContentsReader {
        private readonly Logger _log = LogManager.GetCurrentClassLogger();

        public Contents Read(string file) {

            var xmlFileInfo = new FileInfo(file);

            if (!xmlFileInfo.Exists) {
                _log.Error("Sorry. I can't find file {0}.", xmlFileInfo.FullName);
                Environment.Exit(1);
            }

            return new Contents {
                Name = Path.GetFileNameWithoutExtension(xmlFileInfo.FullName),
                Content = File.ReadAllText(xmlFileInfo.FullName)
            };

        }
    }
}