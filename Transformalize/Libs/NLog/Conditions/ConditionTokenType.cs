#region License
// /*
// See license included in this library folder.
// */
#endregion
namespace Transformalize.Libs.NLog.Conditions
{
    /// <summary>
    ///     Token types for condition expressions.
    /// </summary>
    internal enum ConditionTokenType
    {
        EndOfInput,

        BeginningOfInput,

        Number,

        String,

        Keyword,

        Whitespace,

        FirstPunct,

        LessThan,

        GreaterThan,

        LessThanOrEqualTo,

        GreaterThanOrEqualTo,

        EqualTo,

        NotEqual,

        LeftParen,

        RightParen,

        Dot,

        Comma,

        Not,

        And,

        Or,

        Minus,

        LastPunct,

        Invalid,

        ClosingCurlyBrace,

        Colon,

        Exclamation,

        Ampersand,

        Pipe,
    }
}