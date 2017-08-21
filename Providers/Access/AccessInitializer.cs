using System;
using System.IO;
using Transformalize.Actions;
using Transformalize.Context;
using Transformalize.Contracts;

namespace Transformalize.Providers.Access {
    public class AccessInitializer : IInitializer {
        private readonly OutputContext _output;
        private readonly IAction _adoInitializer;

        public AccessInitializer(IAction adoInitializer, OutputContext output) {
            _adoInitializer = adoInitializer;
            _output = output;
        }
        public ActionResponse Execute() {

            try {
                var fileInfo = new FileInfo(_output.Connection.File);
                if (fileInfo.DirectoryName != null) {
                    Directory.CreateDirectory(fileInfo.DirectoryName);
                }
                if (fileInfo.Exists)
                    return _adoInitializer.Execute();

                var sourceFile = new FileInfo(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Files\\access", "empty.mdb"));
                File.Copy(sourceFile.FullName, fileInfo.FullName, true);

                return _adoInitializer.Execute();
            } catch (Exception e) {
                return new ActionResponse(500, e.Message) { Action = new Configuration.Action { Type = "internal", ErrorMode = "abort" } };
            }

        }
    }
}
