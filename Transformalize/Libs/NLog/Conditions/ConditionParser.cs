#region License
// /*
// See license included in this library folder.
// */
#endregion

using System;
using System.Collections.Generic;
using System.Globalization;
using Transformalize.Libs.NLog.Config;
using Transformalize.Libs.NLog.Internal;
using Transformalize.Libs.NLog.Layouts;

namespace Transformalize.Libs.NLog.Conditions
{
    /// <summary>
    ///     Condition parser. Turns a string representation of condition expression
    ///     into an expression tree.
    /// </summary>
    public class ConditionParser
    {
        private readonly ConfigurationItemFactory configurationItemFactory;
        private readonly ConditionTokenizer tokenizer;

        /// <summary>
        ///     Initializes a new instance of the <see cref="ConditionParser" /> class.
        /// </summary>
        /// <param name="stringReader">The string reader.</param>
        /// <param name="configurationItemFactory">
        ///     Instance of <see cref="ConfigurationItemFactory" /> used to resolve references to condition methods and layout renderers.
        /// </param>
        private ConditionParser(SimpleStringReader stringReader, ConfigurationItemFactory configurationItemFactory)
        {
            this.configurationItemFactory = configurationItemFactory;
            tokenizer = new ConditionTokenizer(stringReader);
        }

        /// <summary>
        ///     Parses the specified condition string and turns it into
        ///     <see cref="ConditionExpression" /> tree.
        /// </summary>
        /// <param name="expressionText">The expression to be parsed.</param>
        /// <returns>The root of the expression syntax tree which can be used to get the value of the condition in a specified context.</returns>
        public static ConditionExpression ParseExpression(string expressionText)
        {
            return ParseExpression(expressionText, ConfigurationItemFactory.Default);
        }

        /// <summary>
        ///     Parses the specified condition string and turns it into
        ///     <see cref="ConditionExpression" /> tree.
        /// </summary>
        /// <param name="expressionText">The expression to be parsed.</param>
        /// <param name="configurationItemFactories">
        ///     Instance of <see cref="ConfigurationItemFactory" /> used to resolve references to condition methods and layout renderers.
        /// </param>
        /// <returns>The root of the expression syntax tree which can be used to get the value of the condition in a specified context.</returns>
        public static ConditionExpression ParseExpression(string expressionText, ConfigurationItemFactory configurationItemFactories)
        {
            if (expressionText == null)
            {
                return null;
            }

            var parser = new ConditionParser(new SimpleStringReader(expressionText), configurationItemFactories);
            var expression = parser.ParseExpression();
            if (!parser.tokenizer.IsEOF())
            {
                throw new ConditionParseException("Unexpected token: " + parser.tokenizer.TokenValue);
            }

            return expression;
        }

        /// <summary>
        ///     Parses the specified condition string and turns it into
        ///     <see cref="ConditionExpression" /> tree.
        /// </summary>
        /// <param name="stringReader">The string reader.</param>
        /// <param name="configurationItemFactories">
        ///     Instance of <see cref="ConfigurationItemFactory" /> used to resolve references to condition methods and layout renderers.
        /// </param>
        /// <returns>
        ///     The root of the expression syntax tree which can be used to get the value of the condition in a specified context.
        /// </returns>
        internal static ConditionExpression ParseExpression(SimpleStringReader stringReader, ConfigurationItemFactory configurationItemFactories)
        {
            var parser = new ConditionParser(stringReader, configurationItemFactories);
            var expression = parser.ParseExpression();

            return expression;
        }

        private ConditionMethodExpression ParsePredicate(string functionName)
        {
            var par = new List<ConditionExpression>();

            while (!tokenizer.IsEOF() && tokenizer.TokenType != ConditionTokenType.RightParen)
            {
                par.Add(ParseExpression());
                if (tokenizer.TokenType != ConditionTokenType.Comma)
                {
                    break;
                }

                tokenizer.GetNextToken();
            }

            tokenizer.Expect(ConditionTokenType.RightParen);

            try
            {
                var methodInfo = configurationItemFactory.ConditionMethods.CreateInstance(functionName);
                return new ConditionMethodExpression(functionName, methodInfo, par);
            }
            catch (Exception exception)
            {
                if (exception.MustBeRethrown())
                {
                    throw;
                }

                throw new ConditionParseException("Cannot resolve function '" + functionName + "'", exception);
            }
        }

        private ConditionExpression ParseLiteralExpression()
        {
            if (tokenizer.IsToken(ConditionTokenType.LeftParen))
            {
                tokenizer.GetNextToken();
                var e = ParseExpression();
                tokenizer.Expect(ConditionTokenType.RightParen);
                return e;
            }

            if (tokenizer.IsToken(ConditionTokenType.Minus))
            {
                tokenizer.GetNextToken();
                if (!tokenizer.IsNumber())
                {
                    throw new ConditionParseException("Number expected, got " + tokenizer.TokenType);
                }

                var numberString = tokenizer.TokenValue;
                tokenizer.GetNextToken();
                if (numberString.IndexOf('.') >= 0)
                {
                    return new ConditionLiteralExpression(-double.Parse(numberString, CultureInfo.InvariantCulture));
                }

                return new ConditionLiteralExpression(-int.Parse(numberString, CultureInfo.InvariantCulture));
            }

            if (tokenizer.IsNumber())
            {
                var numberString = tokenizer.TokenValue;
                tokenizer.GetNextToken();
                if (numberString.IndexOf('.') >= 0)
                {
                    return new ConditionLiteralExpression(double.Parse(numberString, CultureInfo.InvariantCulture));
                }

                return new ConditionLiteralExpression(int.Parse(numberString, CultureInfo.InvariantCulture));
            }

            if (tokenizer.TokenType == ConditionTokenType.String)
            {
                ConditionExpression e = new ConditionLayoutExpression(Layout.FromString(tokenizer.StringTokenValue, configurationItemFactory));
                tokenizer.GetNextToken();
                return e;
            }

            if (tokenizer.TokenType == ConditionTokenType.Keyword)
            {
                var keyword = tokenizer.EatKeyword();

                if (0 == string.Compare(keyword, "level", StringComparison.OrdinalIgnoreCase))
                {
                    return new ConditionLevelExpression();
                }

                if (0 == string.Compare(keyword, "logger", StringComparison.OrdinalIgnoreCase))
                {
                    return new ConditionLoggerNameExpression();
                }

                if (0 == string.Compare(keyword, "message", StringComparison.OrdinalIgnoreCase))
                {
                    return new ConditionMessageExpression();
                }

                if (0 == string.Compare(keyword, "loglevel", StringComparison.OrdinalIgnoreCase))
                {
                    tokenizer.Expect(ConditionTokenType.Dot);
                    return new ConditionLiteralExpression(LogLevel.FromString(tokenizer.EatKeyword()));
                }

                if (0 == string.Compare(keyword, "true", StringComparison.OrdinalIgnoreCase))
                {
                    return new ConditionLiteralExpression(true);
                }

                if (0 == string.Compare(keyword, "false", StringComparison.OrdinalIgnoreCase))
                {
                    return new ConditionLiteralExpression(false);
                }

                if (0 == string.Compare(keyword, "null", StringComparison.OrdinalIgnoreCase))
                {
                    return new ConditionLiteralExpression(null);
                }

                if (tokenizer.TokenType == ConditionTokenType.LeftParen)
                {
                    tokenizer.GetNextToken();

                    var predicateExpression = ParsePredicate(keyword);
                    return predicateExpression;
                }
            }

            throw new ConditionParseException("Unexpected token: " + tokenizer.TokenValue);
        }

        private ConditionExpression ParseBooleanRelation()
        {
            var e = ParseLiteralExpression();

            if (tokenizer.IsToken(ConditionTokenType.EqualTo))
            {
                tokenizer.GetNextToken();
                return new ConditionRelationalExpression(e, ParseLiteralExpression(), ConditionRelationalOperator.Equal);
            }

            if (tokenizer.IsToken(ConditionTokenType.NotEqual))
            {
                tokenizer.GetNextToken();
                return new ConditionRelationalExpression(e, ParseLiteralExpression(), ConditionRelationalOperator.NotEqual);
            }

            if (tokenizer.IsToken(ConditionTokenType.LessThan))
            {
                tokenizer.GetNextToken();
                return new ConditionRelationalExpression(e, ParseLiteralExpression(), ConditionRelationalOperator.Less);
            }

            if (tokenizer.IsToken(ConditionTokenType.GreaterThan))
            {
                tokenizer.GetNextToken();
                return new ConditionRelationalExpression(e, ParseLiteralExpression(), ConditionRelationalOperator.Greater);
            }

            if (tokenizer.IsToken(ConditionTokenType.LessThanOrEqualTo))
            {
                tokenizer.GetNextToken();
                return new ConditionRelationalExpression(e, ParseLiteralExpression(), ConditionRelationalOperator.LessOrEqual);
            }

            if (tokenizer.IsToken(ConditionTokenType.GreaterThanOrEqualTo))
            {
                tokenizer.GetNextToken();
                return new ConditionRelationalExpression(e, ParseLiteralExpression(), ConditionRelationalOperator.GreaterOrEqual);
            }

            return e;
        }

        private ConditionExpression ParseBooleanPredicate()
        {
            if (tokenizer.IsKeyword("not") || tokenizer.IsToken(ConditionTokenType.Not))
            {
                tokenizer.GetNextToken();
                return new ConditionNotExpression(ParseBooleanPredicate());
            }

            return ParseBooleanRelation();
        }

        private ConditionExpression ParseBooleanAnd()
        {
            var expression = ParseBooleanPredicate();

            while (tokenizer.IsKeyword("and") || tokenizer.IsToken(ConditionTokenType.And))
            {
                tokenizer.GetNextToken();
                expression = new ConditionAndExpression(expression, ParseBooleanPredicate());
            }

            return expression;
        }

        private ConditionExpression ParseBooleanOr()
        {
            var expression = ParseBooleanAnd();

            while (tokenizer.IsKeyword("or") || tokenizer.IsToken(ConditionTokenType.Or))
            {
                tokenizer.GetNextToken();
                expression = new ConditionOrExpression(expression, ParseBooleanAnd());
            }

            return expression;
        }

        private ConditionExpression ParseBooleanExpression()
        {
            return ParseBooleanOr();
        }

        private ConditionExpression ParseExpression()
        {
            return ParseBooleanExpression();
        }
    }
}