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
using System.Collections.Generic;
using System.Text;
using Noesis.Javascript;
using Transformalize.Model;
using Transformalize.Rhino.Etl.Core;

namespace Transformalize.Transforms {
    public class JavascriptTransform : AbstractTransform {
        private readonly JavascriptContext _context = new JavascriptContext();
        private readonly string _script;

        public JavascriptTransform(string script) {
            _script = script;
        }

        public JavascriptTransform(string script, IParameters parameters, Dictionary<string, Field> results)
            : base(parameters, results) {
            _script = script;
        }

        protected override string Name {
            get { return "Javascript Transform"; }
        }

        public override void Transform(ref StringBuilder sb) {
            _context.SetParameter("field", sb.ToString());
            sb.Clear();
            sb.Append(Run());
        }

        public override void Transform(ref object value) {
            _context.SetParameter("field", value);
            value = Run();
        }

        public override void Transform(ref Row row) {
            foreach (var pair in Parameters) {
                _context.SetParameter(pair.Value.Name, pair.Value.Value ?? row[pair.Key]);
            }
            var result = Run();
            foreach (var pair in Results) {
                row[pair.Key] = result;
            }
        }

        private object Run() {
            return _context.Run(_script);
        }

        public new void Dispose() {
            _context.Dispose();
            base.Dispose();
        }
    }
}