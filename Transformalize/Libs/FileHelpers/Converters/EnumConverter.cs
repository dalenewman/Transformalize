#region License
// /*
// See license included in this library folder.
// */
#endregion

using System;
using Transformalize.Libs.FileHelpers.ErrorHandling;

namespace Transformalize.Libs.FileHelpers.Converters
{
    internal class EnumConverter : ConverterBase
    {
        private readonly Type mEnumType;

        public EnumConverter(Type sourceEnum)
        {
            if (sourceEnum.IsEnum == false)
                throw new BadUsageException("The sourceType must be an Enum and is of type " + sourceEnum.Name);

            mEnumType = sourceEnum;
        }

        public override object StringToField(string from)
        {
            try
            {
                return Enum.Parse(mEnumType, from.Trim(), true);
            }
            catch (ArgumentException)
            {
                throw new ConvertException(from, mEnumType, "The value don't is on the Enum.");
            }
        }
    }
}