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

using System;
using System.Collections.Generic;
using System.Text;
using Transformalize.Core.Field_;
using Transformalize.Core.Fields_;
using Transformalize.Core.Parameter_;
using Transformalize.Core.Parameters_;
using Transformalize.Libs.NLog;
using Transformalize.Libs.Rhino.Etl.Core;

namespace Transformalize.Core.Transform_
{

    public abstract class AbstractTransform : IDisposable
    {

        private readonly Logger _log = LogManager.GetCurrentClassLogger();
        protected abstract string Name { get; }
        public IParameters Parameters { get; set; }
        public IFields Results { get; set; }
        public Dictionary<string, string> Scripts = new Dictionary<string, string>();
        protected KeyValuePair<string, Field> FirstResult { get; set; }
        protected KeyValuePair<string, IParameter> FirstParameter { get; set; }
        public bool HasParameters { get; private set; }
        public bool HasResults { get; private set; }
        protected object[] ParameterValues { get; private set; }
        public bool RequiresRow { get; set; }

        /// <summary>
        /// Used for field level transformations, there are no parameters and the result is inline
        /// </summary>
        protected AbstractTransform()
        {
            HasParameters = false;
            Parameters = new Parameters();

            HasResults = false;
            Results = new Fields();
        }

        /// <summary>
        /// Used for entity and process level transformations, requires parameters, and result
        /// </summary>
        /// <param name="parameters"></param>
        /// <param name="results"></param>
        protected AbstractTransform(IParameters parameters, IFields results)
        {
            Parameters = parameters;
            Results = results;
            HasParameters = parameters != null && parameters.Count > 0;
            HasResults = results != null && results.Count > 0;
            if (HasResults)
            {
                FirstResult = Results.First();
            }
            if (HasParameters)
            {
                ParameterValues = new object[Parameters.Count];
                FirstParameter = Parameters.First();
            }
        }

        public virtual void Transform(ref StringBuilder sb)
        {
            _log.Error("Transform with StringBuilder is not implemented in {0}!", Name);
        }
        public virtual void Transform(ref Object value)
        {
            _log.Error("Transform with object value is not implemented in {0}!", Name);
        }
        public virtual void Transform(ref Row row)
        {
            _log.Error("Transform with row is not implemented in {0}!  It must be implemented at the field level, or pushed down based on a matching type attribute.", Name);
        }

        public void TransformResult(Field field, ref object value)
        {
            if (!field.HasTransforms)
                return;

            if (field.UseStringBuilder)
            {
                field.StringBuilder.Clear();
                field.StringBuilder.Append(value);
                field.Transform();
                value = field.StringBuilder.ToString();
            }
            else
            {
                field.Transform(ref value);
            }
        }

        public void TransformResult(Field field, ref string value)
        {
            if (!field.HasTransforms)
                return;

            field.StringBuilder.Clear();
            field.StringBuilder.Append(value);
            field.Transform();
            value = field.StringBuilder.ToString();
        }

        public virtual void Dispose()
        {
            Parameters = null;
            Results = null;
        }
    }

}