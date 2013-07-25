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
using RazorEngine.Templating;
using Transformalize.Model;
using Transformalize.Rhino.Etl.Core;

namespace Transformalize.Transforms {
    public class TemplateTransform : AbstractTransform {
        private readonly string _key;
        private DynamicViewBag _context = new DynamicViewBag();

        public TemplateTransform(string template, string key) {
            _key = key;
            RazorEngine.Razor.Compile(template, key);
        }

        public TemplateTransform(string template, IParameters parameters, Dictionary<string, Field> results)
            : base(parameters, results) {
            _key = FirstResult.Key;
            RazorEngine.Razor.Compile(template, _key);
        }

        protected override string Name {
            get { return "Template Transform"; }
        }

        public override void Transform(ref StringBuilder sb) {
            _context.AddValue("field", sb.ToString());
            sb.Clear();
            sb.Append(Run());
        }

        public override void Transform(ref object value) {
            _context.AddValue("field", value);
            value = Run();
        }

        public override void Transform(ref Row row) {
            foreach (var pair in Parameters) {
                _context.AddValue(pair.Value.Name, pair.Value.Value ?? row[pair.Key]);
            }
            row[FirstResult.Key] = Run();
        }

        private string Run() {
            return RazorEngine.Razor.Run(_key, _context);
        }

        public new void Dispose() {
            _context = null;
            base.Dispose();
        }
    }
}