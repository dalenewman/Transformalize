using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Transformalize.Core;
using Transformalize.Core.Entity_;
using Transformalize.Core.Field_;
using Transformalize.Core.Process_;
using Transformalize.Core.Template_;
using Transformalize.Libs.NLog;
using Transformalize.Libs.RazorEngine.Core;
using Transformalize.Libs.RazorEngine.Core.Configuration.Fluent;
using Transformalize.Libs.RazorEngine.Core.Templating;
using Transformalize.Libs.Rhino.Etl.Core.Pipelines;
using Transformalize.Processes;
using Transformalize.Providers.SqlServer;
using Encoding = System.Text.Encoding;

namespace Transformalize.Runner
{
    public class ProcessRunner
    {
        private Process _process;
        private Logger _log = LogManager.GetCurrentClassLogger();

        public ProcessRunner(Process process)
        {
            _process = process;
        }

        public void Run()
        {
            if (!_process.IsReady()) return;

            switch (_process.Options.Mode)
            {
                case Modes.Initialize:
                    new InitializationProcess(_process).Execute();
                    break;
                case Modes.Metadata:
                    var fileName = new FileInfo(Path.Combine(Common.GetTemporaryFolder(_process.Name), "MetaData.xml")).FullName;
                    File.WriteAllText(fileName, GetMetaData(), Encoding.UTF8);
                    System.Diagnostics.Process.Start(fileName);
                    break;
                default:
                    new EntityRecordsExist(ref _process).Check();

                    SetupRazorTemplateService();
                    ProcessEntities();
                    ProcessMaster();
                    ProcessTransforms();
                    RenderTemplates();

                    break;
            }
        }

        private void SetupRazorTemplateService()
        {
            var config = new FluentTemplateServiceConfiguration(c => c.WithEncoding(_process.TemplateContentType));
            var templateService = new TemplateService(config);
            Razor.SetTemplateService(templateService);
            _log.Debug("Set RazorEngine to {0} content type.", _process.TemplateContentType);
        }

        private string GetMetaData()
        {
            var content = new StringBuilder();
            var count = 1;

            content.AppendLine("<?xml version=\"1.0\" encoding=\"utf-8\" ?>");
            content.AppendLine("<process>");
            content.AppendLine("  <entities>");

            foreach (var entity in _process.Entities)
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

        private void RenderTemplates()
        {
            if (_process.Options.RenderTemplates)
                new TemplateManager(_process).Manage();
        }

        private void ProcessTransforms()
        {
            if (_process.CalculatedFields.Count <= 0) return;

            var transformProcess = new TransformProcess(_process);

            if (_process.Options.Mode == Modes.Test)
                transformProcess.PipelineExecuter = new SingleThreadedNonCachedPipelineExecuter();

            transformProcess.Execute();
        }

        private void ProcessMaster()
        {
            var updateMasterProcess = new UpdateMasterProcess(ref _process);
            if (_process.Options.Mode == Modes.Test)
                updateMasterProcess.PipelineExecuter = new SingleThreadedNonCachedPipelineExecuter();

            updateMasterProcess.Execute();
        }

        private void ProcessEntities()
        {
            foreach (var entityKeysProcess in _process.Entities.Select(entity => new EntityKeysProcess(_process, entity, new SqlServerEntityBatchReader())))
            {
                if (_process.Options.Mode == Modes.Test)
                    entityKeysProcess.PipelineExecuter = new SingleThreadedNonCachedPipelineExecuter();

                entityKeysProcess.Execute();
            }

            foreach (var entityProcess in _process.Entities.Select(entity => new EntityProcess(_process, entity)))
            {
                if (_process.Options.Mode == Modes.Test)
                    entityProcess.PipelineExecuter = new SingleThreadedNonCachedPipelineExecuter();

                entityProcess.Execute();
            }
        }
    }
}