using System.Text;

namespace Transformalize.Transforms {

    public interface ITransform {
        void Transform(StringBuilder sb);
    }
}