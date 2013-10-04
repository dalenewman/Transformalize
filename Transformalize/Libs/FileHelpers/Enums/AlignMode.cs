#region License
// /*
// See license included in this library folder.
// */
#endregion
namespace Transformalize.Libs.FileHelpers.Enums
{
    /// <summary>
    ///     Indicates the align of the field when the <see cref="T:Transformalize.Libs.FileHelpers.Engines.FileHelperEngine" /> <b>writes</b> the record.
    /// </summary>
    public enum AlignMode
    {
        /// <summary>Aligns the field to the left.</summary>
        Left,

        /// <summary>Aligns the field to the center.</summary>
        Center,

        /// <summary>Aligns the field to the right.</summary>
        Right
    }
}