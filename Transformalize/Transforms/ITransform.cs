using System;
using System.Collections.Generic;
using System.Text;
using Transformalize.Model;
using Transformalize.Rhino.Etl.Core;

namespace Transformalize.Transforms {

    public interface ITransform : IDisposable {
        void Transform(ref StringBuilder sb);
        void Transform(ref Object value);
        void Transform(ref Row row);
        bool HasParameters { get; }
        bool HasResults { get; }
        Dictionary<string, Field> Parameters { get; }
        Dictionary<string, Field> Results { get; } 
    }

}