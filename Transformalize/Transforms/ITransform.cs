using System;
using System.Text;

namespace Transformalize.Transforms {

    public interface ITransform : IDisposable {
        void Transform(StringBuilder sb);
        object Transform(object value);
    }

}