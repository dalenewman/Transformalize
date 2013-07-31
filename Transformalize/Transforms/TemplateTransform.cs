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
using Transformalize.Libs.RazorEngine.Core;
using Transformalize.Libs.RazorEngine.Core.Templating;
using Transformalize.Libs.Rhino.Etl.Core;
using Transformalize.Model;

namespace Transformalize.Transforms
{
    public class TemplateTransform : AbstractTransform
    {
        private readonly string _templateModelType;
        private readonly string _key;
        private Dictionary<string, object> _dictionaryContext = new Dictionary<string, object>();
        private DynamicViewBag _dynamicViewBagContext = new DynamicViewBag();
        private object _value;

        public TemplateTransform(string template, string key)
        {
            _key = key;
            Razor.Compile("@{ var " + key + " = Model; }" + template, typeof(object), key);
            Debug("Compiled template with hashcode {0} and key {1}.", template.GetHashCode(), _key);
        }

        public TemplateTransform(string template, string templateModelType, IParameters parameters, Dictionary<string, Field> results)
            : base(parameters, results)
        {
            _templateModelType = templateModelType;
            _key = FirstResult.Key;
            if (templateModelType == "dynamic")
            {
                Razor.Compile(template, typeof(DynamicViewBag), _key);
            }
            else
            {
                Razor.Compile(template, typeof(Dictionary<string, object>), _key);
            } 
            Debug("Compiled {0} template with hashcode {1} and key {2}.", templateModelType, template.GetHashCode(), _key);

        }

        protected override string Name
        {
            get { return "Template Transform"; }
        }

        public override void Transform(ref StringBuilder sb)
        {
            _value = sb.ToString();
            sb.Clear();
            sb.Append(Razor.Run(_key, _value));
        }

        public override void Transform(ref object value)
        {
            _value = value;
            value = Razor.Run(_key, _value);
        }

        public override void Transform(ref Row row)
        {
            if (_templateModelType == "dynamic")
                RunWithDynamic(ref row);
            else
                RunWithDictionary(ref row);
        }

        private void RunWithDictionary(ref Row row)
        {
            foreach (var pair in Parameters)
            {
                _dictionaryContext[pair.Value.Name] = pair.Value.Value ?? row[pair.Key];
            }
            row[FirstResult.Key] = Razor.Run(_key, _dictionaryContext);
        }

        private void RunWithDynamic(ref Row row)
        {
            foreach (var pair in Parameters)
            {
                 _dynamicViewBagContext.AddValue(pair.Value.Name, pair.Value.Value ?? row[pair.Key]);
            }
            row[FirstResult.Key] = Razor.Run(_key, _dynamicViewBagContext);
        }

        public new void Dispose()
        {
            _dictionaryContext = null;
            _dynamicViewBagContext = null;
            base.Dispose();
        }
    }
}