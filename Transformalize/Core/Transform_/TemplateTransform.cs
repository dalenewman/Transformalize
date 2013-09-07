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
using Transformalize.Core.Parameters_;
using Transformalize.Core.Template_;
using Transformalize.Libs.NLog;
using Transformalize.Libs.RazorEngine.Core;
using Transformalize.Libs.RazorEngine.Core.Templating;
using Transformalize.Libs.Rhino.Etl.Core;

namespace Transformalize.Core.Transform_
{
    public class TemplateTransform : AbstractTransform
    {
        private readonly Logger _log = LogManager.GetCurrentClassLogger();
        private readonly string _templateModelType;
        private readonly string _key;
        private Dictionary<string, object> _dictionaryContext = new Dictionary<string, object>();
        private DynamicViewBag _dynamicViewBagContext = new DynamicViewBag();
        private object _value;
        private readonly StringBuilder _builder = new StringBuilder();

        public TemplateTransform(string template, string key, IEnumerable<KeyValuePair<string, Template>> templates)
        {
            Name = "Template";
            CombineTemplates(templates, ref _builder);
            _builder.Append("@{ var ");
            _builder.Append(key);
            _builder.Append(" = Model; }");
            _builder.Append(template);

            _key = key;

            Razor.Compile(_builder.ToString(), typeof(object), key);
            _log.Debug("Compiled template with key {0}.", key);
        }

        public TemplateTransform(string template, string key, string templateModelType, IParameters parameters, IEnumerable<KeyValuePair<string, Template>> templates)
            : base(parameters)
        {
            Name = "Template";
            _templateModelType = templateModelType;
            
            CombineTemplates(templates, ref _builder);
            _builder.Append(template);

            _key = key;

            var type = templateModelType == "dynamic" ? typeof(DynamicViewBag) : typeof(Dictionary<string, object>);

            Razor.Compile(_builder.ToString(), type, _key);
            _log.Debug("Compiled {0} template with key {1}.", templateModelType, _key);

        }

        public override void Transform(ref StringBuilder sb)
        {
            _value = sb.ToString();
            sb.Clear();
            sb.Append(Razor.Run(_key, _value));
        }

        public override object Transform(object value)
        {
            _value = value;
            return Razor.Run(_key, _value);
        }

        public override void Transform(ref Row row, string resultKey)
        {
            if (_templateModelType == "dynamic")
                RunWithDynamic(ref row, resultKey);
            else
                RunWithDictionary(ref row, resultKey);
        }

        private void RunWithDictionary(ref Row row, string resultKey)
        {
            foreach (var pair in Parameters)
            {
                _dictionaryContext[pair.Value.Name] = pair.Value.Value ?? row[pair.Key];
            }
            row[resultKey] = Razor.Run(_key, _dictionaryContext);
        }

        private void RunWithDynamic(ref Row row, string resultKey)
        {
            foreach (var pair in Parameters)
            {
                _dynamicViewBagContext.AddValue(pair.Value.Name, pair.Value.Value ?? row[pair.Key]);
            }
            row[resultKey] = Razor.Run(_key, _dynamicViewBagContext);
        }

        private static void CombineTemplates(IEnumerable<KeyValuePair<string, Template>> templates, ref StringBuilder builder)
        {
            foreach (var pair in templates)
            {
                builder.AppendLine(pair.Value.Content);
            }
        }

        public new void Dispose()
        {
            _dictionaryContext = null;
            _dynamicViewBagContext = null;
            base.Dispose();
        }
    }
}