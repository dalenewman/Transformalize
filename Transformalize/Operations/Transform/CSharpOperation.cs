using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Threading;
using Microsoft.CSharp;
using Transformalize.Libs.Rhino.Etl;
using Transformalize.Main;

namespace Transformalize.Operations.Transform {

    public interface ITransformer {
        object Transform(Row row);
    }

    public class CSharpOperation : ShouldRunOperation {
        private readonly ITransformer _transformer;

        public CSharpOperation(string outKey, string outType, string script, Dictionary<string, Script> scripts, IParameters parameters)
            : base(string.Empty, outKey) {

            var csc = new CSharpCodeProvider();
            var ca = Assembly.GetExecutingAssembly();
            var cp = new CompilerParameters { GenerateInMemory = true };
            var testRow = new Row();

            cp.ReferencedAssemblies.Add("System.dll");
            cp.ReferencedAssemblies.Add("System.Core.dll");
            cp.ReferencedAssemblies.Add("mscorlib.dll");
            cp.ReferencedAssemblies.Add(ca.Location);

            var scriptBuilder = new StringBuilder(string.Empty);
            foreach (var s in scripts) {
                scriptBuilder.AppendLine(string.Format("// {0} script", s.Value.Name));
                scriptBuilder.AppendLine(s.Value.Content);
            }

            var castBuilder = new StringBuilder(string.Empty);

            if (!parameters.Any()) {
                castBuilder.AppendLine(String.Format("{1} {0} = ({1}) row[\"{0}\"];", OutKey, Common.ToSystemType(outType)));
                testRow[OutKey] = new DefaultFactory().Convert(null, outType);
            } else {
                var map = Common.GetLiteral();
                foreach (var pair in parameters) {
                    if (pair.Value.HasValue()) {
                        castBuilder.AppendLine(String.Format("{0} {1} = {2};", Common.ToSystemType(pair.Value.SimpleType), pair.Value.Name, map[pair.Value.SimpleType](pair.Value.Value)));
                    } else {
                        castBuilder.AppendLine(String.Format("{1} {0} = ({1}) row[\"{0}\"];", pair.Value.Name, Common.ToSystemType(pair.Value.SimpleType)));
                    }
                    testRow[pair.Value.Name] = new DefaultFactory().Convert(null, pair.Value.SimpleType);
                }
            }

            var code = string.Format(@"using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Transformalize.Operations.Transform;
using Transformalize.Libs.Rhino.Etl;

{0}

public class Transformer : ITransformer
{{
    public object Transform(Row row)
    {{
        {1}
        //User's script
        {2}
    }}
}}", scriptBuilder, castBuilder, script);

            Debug("Compiling this code:");
            Debug(code);

            var res = csc.CompileAssemblyFromSource(
                cp,
                code
            );

            if (res.Errors.Count == 0) {
                var type = res.CompiledAssembly.GetType("Transformer");
                _transformer = (ITransformer)Activator.CreateInstance(type);
                try {
                    var test = _transformer.Transform(testRow);
                    Debug("CSharp transform compiled and passed test. {0}", test);
                } catch (Exception e) {
                    Warn("CSharp transform compiled but failed test. {0}", e.Message);
                    Debug(e.StackTrace);
                }
            } else {
                foreach (var error in res.Errors) {
                    Error(error.ToString());
                }
                throw new TransformalizeException("Failed to compile code. {0}", code);
            }

            Name = "CSharpOperation (" + outKey + ")";
        }

        public override IEnumerable<Row> Execute(IEnumerable<Row> rows) {
            foreach (var row in rows) {
                if (ShouldRun(row)) {
                    row[OutKey] = _transformer.Transform(row);
                } else {
                    Interlocked.Increment(ref SkipCount);
                }
                yield return row;
            }
        }

    }
}