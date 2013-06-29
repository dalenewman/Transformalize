using System;
using System.Text;
using Transformalize.Rhino.Etl.Core;

namespace Transformalize.Transforms {

    public interface ITransform : IDisposable {
        void Transform(ref StringBuilder sb);
        void Transform(ref Object value);
        void Transform(ref Row row);
        bool HasParameters { get; }
        bool HasResults { get; }
    }

}