using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Transformalize.Core;
using Transformalize.Core.Entity_;
using Transformalize.Core.Field_;
using Transformalize.Core.Process_;
using Transformalize.Core.Template_;
using Transformalize.Libs.Rhino.Etl.Core.Pipelines;
using Transformalize.Processes;
using Transformalize.Providers.SqlServer;

namespace Transformalize.Runner
{
    public class ProcessRunner
    {
        private Process _process;

        public ProcessRunner(Process process)
        {
            _process = process;
        }

        public void Run()
        {
            if (!_process.IsReady()) return;

            switch (Process.Options.Mode)
            {
                case Modes.Initialize:
                    new InitializationProcess(_process).Execute();
                    break;
                case Modes.Metadata:
                    var fileName = new FileInfo(Path.Combine(Common.GetTemporaryFolder(), "MetaData.xml")).FullName;
                    File.WriteAllText(fileName, GetMetaData(), Encoding.UTF8);
                    System.Diagnostics.Process.Start(fileName);
                    break;
                default:
                    new EntityRecordsExist(ref _process).Check();

                    ProcessEntities();
                    ProcessMaster();
                    ProcessTransforms();
                    RenderTemplates();

                    break;
            }
        }

        private static string GetMetaData()
        {
            var content = new StringBuilder();
            var count = 1;

            content.AppendLine("<?xml version=\"1.0\" encoding=\"utf-8\" ?>");
            content.AppendLine("<process>");
            content.AppendLine("  <entities>");

            foreach (var entity in Process.Entities)
            {
                content.AppendFormat("    <add name=\"{0}\">\r\n", entity.Name);

                var autoReader = new SqlServerEntityAutoFieldReader(entity, count); //for now
                autoReader.ReadAll();
                var fields = autoReader.ReadFields();
                var keys = autoReader.ReadPrimaryKey();

                content.AppendLine("      <primaryKey>");
                foreach (var f in keys)
                {
                    AppendField(content, f);
                }
                content.AppendLine("      </primaryKey>");

                content.AppendLine("      <fields>");
                foreach (var f in fields)
                {
                    AppendField(content, f);
                }
                content.AppendLine("      </fields>");

                content.AppendLine("    </add>");

                count++;
            }
            content.AppendLine("  </entities>");
            content.AppendLine("</process>");
            return content.ToString();
        }

        private static void AppendField(StringBuilder content, KeyValuePair<string, Field> f)
        {
            content.AppendFormat("        <add name=\"{0}\" {1}{2}{3}{4}></add>\r\n",
                f.Value.Name,
                f.Value.SimpleType.Equals("string") ? string.Empty : "type=\"" + f.Value.Type + "\" ",
                f.Value.Length != "0" ? "length=\"" + f.Value.Length + "\" " : string.Empty,
                f.Value.SimpleType == "decimal" && f.Value.Precision > 0 ? "precision=\"" + f.Value.Precision + "\" " : string.Empty,
                f.Value.SimpleType == "decimal" && f.Value.Scale > 0 ? "scale=\"" + f.Value.Scale + "\"" : string.Empty);
        }

        private static void RenderTemplates()
        {
            if (Process.Options.RenderTemplates)
                new TemplateManager().Manage();
        }

        private void ProcessTransforms()
        {
            if (Process.CalculatedFields.Count > 0)
            {
                var transformProcess = new TransformProcess(_process);

                if (Process.Options.Mode == Modes.Test)
                    transformProcess.PipelineExecuter = new SingleThreadedNonCachedPipelineExecuter();

                transformProcess.Execute();
            }
        }

        private void ProcessMaster()
        {
            var updateMasterProcess = new UpdateMasterProcess(ref _process);
            if (Process.Options.Mode == Modes.Test)
                updateMasterProcess.PipelineExecuter = new SingleThreadedNonCachedPipelineExecuter();

            updateMasterProcess.Execute();
        }

        private static void ProcessEntities()
        {
            foreach (var entityProcess in Process.Entities.Select(entity => new EntityProcess(entity)))
            {
                if (Process.Options.Mode == Modes.Test)
                    entityProcess.PipelineExecuter = new SingleThreadedNonCachedPipelineExecuter();

                entityProcess.Execute();
            }
        }
    }
}