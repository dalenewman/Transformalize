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
using System.Linq;
using System.Text;
using Transformalize.Rhino.Etl.Core;

namespace Transformalize.Model {

    public abstract class AbstractTransform : WithLoggingMixin, IDisposable {

        protected abstract string Name { get; }
        public IParameters Parameters { get; set; }
        public Dictionary<string, Field> Results { get; set; }
        protected KeyValuePair<string, Field> FirstResult { get; set; }
        protected KeyValuePair<string, IParameter> FirstParameter { get; set; } 
        protected bool HasParameters { get; private set; }
        protected bool HasResults { get; private set; }
        protected object[] ParameterValues { get; private set; }

        /// <summary>
        /// Used for field level transformations, there are no parameters and the result is inline
        /// </summary>
        protected AbstractTransform() {
            HasParameters = false;
            HasResults = false;
        }

        /// <summary>
        /// Used for entity and process level transformations, requires parameters, and result
        /// </summary>
        /// <param name="parameters"></param>
        /// <param name="results"></param>
        protected AbstractTransform(IParameters parameters, Dictionary<string, Field> results) {
            Parameters = parameters;
            Results = results;
            HasParameters = parameters != null && parameters.Count > 0;
            HasResults = results != null && results.Count > 0;
            if (HasResults) {
                FirstResult = Results.First();
            }
            if (HasParameters) {
                ParameterValues = new object[Parameters.Count];
                FirstParameter = Parameters.First();
            }
        }

        public virtual void Transform(ref StringBuilder sb) {
            Error("Field level transformation is not implemented for {0} transform!", Name);
        }
        public virtual void Transform(ref Object value) {
            Error("Field level transformation is not implemented for {0} transform!", Name);
        }
        public virtual void Transform(ref Row row) {
            Error("Entity or Process level transformation is not implemented for {0} transform!", Name);
        }

        public virtual void Dispose() {
            Parameters = null;
            Results = null;
        }
    }

}