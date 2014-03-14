using System.Linq;
using Transformalize.Libs.FileHelpers.RunTime;
using Transformalize.Main;
using Transformalize.Main.Providers;

namespace Transformalize.Operations.Load {

    public class HtmlLoadOperation : FileLoadOperation {
        private readonly string _htmlField;

        public HtmlLoadOperation(AbstractConnection connection, Entity entity, string htmlField)
            : base(connection, entity) {
            _htmlField = htmlField;
        }

        protected override void PrepareHeader(Entity entity) {
            foreach (var pair in entity.Fields.Where(f => f.Value.FileOutput)) {
                Headers.Add(pair.Value.Alias);
            }
            foreach (var pair in entity.CalculatedFields.Where(f => f.Value.FileOutput)) {
                Headers.Add(pair.Value.Alias);
            }
            HeaderText = HeaderText = string.Format(@"<!DOCTYPE html PUBLIC ""-//W3C//DTD HTML 3.2//EN"">
<html>
  <head>
    <title>{0}</title>
    <link rel=""stylesheet"" href=""http://netdna.bootstrapcdn.com/bootswatch/3.1.1/united/bootstrap.min.css"">
    <link rel=""stylesheet"" href=""http://netdna.bootstrapcdn.com/font-awesome/4.0.3/css/font-awesome.min.css"">
    <script src=""http://netdna.bootstrapcdn.com/bootstrap/3.1.1/js/bootstrap.min.js""></script>
  </head>
  <body>
    <table class=""table table-striped table-condensed table-hover"">
		<thead>
            <th>{1}</th>
		</thead>
		<tbody>
", entity.OutputName(), string.Join("</th><th>", Headers));
        }

        protected override void PrepareFooter(Entity entity) {
            FooterText = @"      </tbody>
	</table>
  </body>
</html>
";
        }

        protected override void PrepareType(Entity entity) {
            var builder = new DelimitedClassBuilder("Tfl" + entity.OutputName()) { IgnoreEmptyLines = true, Delimiter = " ", IgnoreFirstLines = 0 };
            builder.AddField(_htmlField, typeof(string));
            Type = builder.CreateRecordClass();
        }
    }
}