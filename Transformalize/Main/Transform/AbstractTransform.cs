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

using System;
using System.Collections.Generic;
using System.Text;
using Transformalize.Libs.NLog;
using Transformalize.Libs.Rhino.Etl;

namespace Transformalize.Main
{
    public abstract class AbstractTransform : IDisposable
    {
        private readonly Logger _log = LogManager.GetCurrentClassLogger();

        public Dictionary<string, string> Scripts = new Dictionary<string, string>();
        private IParameters _parameters;

        /// <summary>
        ///     Used for field level transformations, there are no parameters and the result is inline
        /// </summary>
        protected AbstractTransform()
        {
            HasParameters = false;
            Parameters = new Parameters();
        }

        /// <summary>
        ///     Used for entity and process level transformations, requires parameters
        /// </summary>
        /// <param name="parameters"></param>
        protected AbstractTransform(IParameters parameters)
        {
            Parameters = parameters;
        }

        public IParameters Parameters
        {
            get { return _parameters; }
            set
            {
                _parameters = value;
                PrepareParameters(value);
            }
        }

        protected KeyValuePair<string, IParameter> FirstParameter { get; set; }
        public bool HasParameters { get; private set; }
        protected object[] ParameterValues { get; private set; }
        public bool RequiresRow { get; set; }
        public bool RequiresParameters { get; set; }
        public string Name { get; set; }

        public void Dispose()
        {
            Parameters = null;
        }

        private void PrepareParameters(IParameters parameters)
        {
            HasParameters = parameters != null && parameters.Count > 0;
            if (!HasParameters) return;
            ParameterValues = new object[Parameters.Count];
            FirstParameter = Parameters.First();
        }

        public virtual void Transform(ref StringBuilder sb)
        {
            _log.Error("Transform with StringBuilder is not implemented in {0}!", Name);
        }

        public virtual object Transform(Object value)
        {
            _log.Error("Transform with object value is not implemented in {0}!", Name);
            return null;
        }

        public virtual void Transform(ref Row row, string resultKey)
        {
            _log.Error(
                "Transform with row is not implemented in {0}!  It must be implemented at the field level, or pushed down based on a matching type attribute.",
                Name);
        }

        public override string ToString()
        {
            return Name;
        }
    }
}