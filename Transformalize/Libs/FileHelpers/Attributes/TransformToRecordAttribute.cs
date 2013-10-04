#region License
// /*
// See license included in this library folder.
// */
#endregion

using System;

namespace Transformalize.Libs.FileHelpers.Attributes
{
    /// <summary>With this attribute you can mark a method in the RecordClass that is the responsable of convert it to the specified.</summary>
    [AttributeUsage(AttributeTargets.Method)]
    public sealed class TransformToRecordAttribute : Attribute
    {
        internal Type TargetType;

        /// <summary>With this attribute you can mark a method in the RecordClass that is the responsable of convert it to the specified.</summary>
        /// <param name="targetType">The target of the convertion.</param>
        public TransformToRecordAttribute(Type targetType)
        {
            //throw new NotImplementedException("This feature is not ready yet. In the next release maybe work =)");
            TargetType = targetType;
        }
    }
}