using System.Collections.Generic;
using System.Text;
using Transformalize.Model;
using Transformalize.Rhino.Etl.Core;

namespace Transformalize.Transforms {
    public class RightTransform : ITransform {
        private readonly int _length;
        private readonly Dictionary<string, Field> _parameters;
        private readonly Dictionary<string, Field> _results;

        public RightTransform(int length) {
            _length = length;
        }

        public RightTransform(int length, Dictionary<string, Field> parameters, Dictionary<string, Field> results) {
            _length = length;
            _parameters = parameters;
            _results = results;
            HasParameters = parameters != null && parameters.Count > 0;
            HasResults = results != null && results.Count > 0;
        }

        public void Transform(ref StringBuilder sb) {
            sb.Right(_length);
        }

        public void Transform(ref object value) {
            value = value.ToString().Right(_length);
        }

        public void Transform(ref Row row) {

        }

        public bool HasParameters { get; private set; }
        public bool HasResults { get; private set; }

        public void Dispose() { }
    }
}