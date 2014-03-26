using System;

namespace Transformalize.Libs.Jint.Runtime.Interop
{
    public interface ITypeConverter
    {
        object Convert(object value, Type type, IFormatProvider formatProvider);
    }
}
