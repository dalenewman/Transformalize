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

using System.Collections.Generic;
using System.Text;
using Noesis.Javascript;
using Transformalize.Libs.NLog;
using Transformalize.Libs.Rhino.Etl;

namespace Transformalize.Main
{
    public class JavascriptTransform : AbstractTransform
    {
        private readonly JavascriptContext _context = new JavascriptContext();
        private readonly string _field;
        private readonly Logger _log = LogManager.GetCurrentClassLogger();
        private readonly string _script;

        public JavascriptTransform(string script, string field, Dictionary<string, Script> scripts)
        {
            _script = script;
            _field = field;
            foreach (var pair in scripts)
            {
                _log.Debug("Running script {0}.", pair.Value.File);
                _context.Run(pair.Value.Content);
            }
        }

        public JavascriptTransform(string script, IParameters parameters, Dictionary<string, Script> scripts)
            : base(parameters)
        {
            Name = "Javascript";
            _script = script;
            foreach (var pair in scripts)
            {
                _context.Run(pair.Value.Content);
            }
        }

        public override void Transform(ref StringBuilder sb)
        {
            _context.SetParameter(_field, sb.ToString());
            sb.Clear();
            sb.Append(Run());
        }

        public override object Transform(object value)
        {
            _context.SetParameter(_field, value);
            return Run();
        }

        public override void Transform(ref Row row, string resultKey)
        {
            foreach (var pair in Parameters)
            {
                _context.SetParameter(pair.Value.Name, pair.Value.Value ?? row[pair.Key]);
            }
            row[resultKey] = Run();
        }

        private object Run()
        {
            return _context.Run(_script);
        }

        public new void Dispose()
        {
            _context.Dispose();
        }
    }
}